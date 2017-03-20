using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Contoso.Apps.SportsLeague.Data.Models;
using Contoso.Apps.SportsLeague.Data.Logic;

namespace Contoso.Apps.SportsLeague.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("ContosoSportsLeagueWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ContosoSportsLeagueWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ContosoSportsLeagueWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ContosoSportsLeagueWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // Generate a new PDF-based receipt, save it to Azure Blob storage, and update
            // the customer's order record with the path to the PDF.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Checking Order Status");

                // Grab order id from the Azure Queue.
                var azureStorageMethods = new AzureStorageMethods();
                int orderId = await azureStorageMethods.GetOrderIdFromQueue();

                if (orderId > 0)
                {
                    // We have received a new Order Id in the queue, requesting a generated Pdf receipt.
                    var order = new Order();
                    using (var orderActions = new OrderActions())
                    {
                        order = orderActions.GetOrder(orderId);

                        if (order != null && order.OrderId > 0)
                        {
                            var orderVm = DataMethods.MapOrderToViewModel(order);

                            var fileName = string.Format("ContosoSportsLeague-Store-Receipt-{0}.pdf", order.OrderId);
                            var receipt = new GenerateReceiptPDF().CreatePdfReport(orderVm, fileName);
                            Trace.TraceInformation("PDF generated. Saving to blob storage...");
                            var receiptUri = azureStorageMethods.UploadPdfToBlob(receipt, fileName);

                            // Update the generated receipt Uri on the order:
                            orderActions.UpdateReceiptUri(order.OrderId, receiptUri);
                        }
                        else
                        {
                            // We couldn't find the order for the passed in Order Id. We should log this somewhere.
                            Trace.TraceError("Could not find the order record based off of the passed in Id: " + orderId.ToString() + ". Receipt PDF not generated.");
                        }
                    }
                }
                Trace.TraceInformation("Pausing for 5 seconds before next check.");
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
