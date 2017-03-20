using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.SmtpApi;
using System.Net;
using System.Net.Mail;
using System.IO;
using Contoso.Apps.SportsLeague.WorkerRole.Models;

namespace Contoso.Apps.SportsLeague.WorkerRole
{
    public class EmailMethods
    {
        public void EmailReceipt(byte[] file, string fileName, OrderViewModel order)
        {
            // Set our SendGrid API User and Key values for authenticating our transport.
            var sendGridApiKey = "";
            var sendGridApiUser = "";

            // Create the email object first, and then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("SenderEmailAddress@test.com");

            // Add customer's email addresses to the To field.
            myMessage.AddTo(order.Email);

            myMessage.Subject = "Contoso Sports League order received";

            // Add the HTML and Text bodies.
            myMessage.Html = "";
            myMessage.Text = "";

            // Add our generated PDF receipt as an attachment.
            using (var stream = new MemoryStream(file))
            {
                myMessage.AddAttachment(stream, fileName);
            }

            var credentials = new NetworkCredential(sendGridApiUser, sendGridApiKey);
            // Create a Web transport using our SendGrid API user and key.
            var transportWeb = new Web(credentials);

            // Send the email.
            transportWeb.DeliverAsync(myMessage);
        }
    }
}
