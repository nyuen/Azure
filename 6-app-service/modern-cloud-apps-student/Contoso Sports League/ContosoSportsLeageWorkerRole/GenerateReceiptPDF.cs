using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using iTextSharp;
using Contoso.Apps.SportsLeague.Data.Models;
using Contoso.Apps.SportsLeague.Data.Logic;
using System.Reflection;
using iTextSharp.text;
using System.IO;
using Contoso.Apps.SportsLeague.WorkerRole.Models;

namespace Contoso.Apps.SportsLeague.WorkerRole
{
    public class GenerateReceiptPDF
    {
        public byte[] CreatePdfReport(OrderViewModel order, string fileName)
        {
            var pdfReportData = new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Contoso Sports League", Application = "Contoso.Apps.SportsLeague", Subject = "Contoso Sports League Store Receipt", Title = "Receipt" });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\arial.ttf"),
                           System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.ImagePath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images\\logo.png"));
                    defaultHeader.Message("Contoso Sports League - Purchase Receipt");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ClassicTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(order.OrderDetails);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Summary");
                summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                summarySettings.PageSummarySettings("Page Summary");
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName<OrderDetailViewModel>(o => o.ProductName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(4);
                    column.HeaderCell("Product");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<OrderDetailViewModel>(o => o.Quantity);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("Quantity");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<OrderDetailViewModel>(o => o.UnitPrice);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Unit Price");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:c}", obj));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<OrderDetailViewModel>(o => o.Cost);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Total");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:c}", obj));
                    });
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There are no purchased items to display.");

                events.MainTableAdded(args =>
                {
                    var total = order.Total;

                    var summaryTable = new PdfGrid(args.Table.RelativeWidths); // Create a clone of the MainTable's structure                   
                    summaryTable.WidthPercentage = args.Table.WidthPercentage;
                    summaryTable.SpacingBefore = args.Table.SpacingBefore;

                    summaryTable.AddSimpleRow(
                        null, null,
                        (data, cellProperties) =>
                        {
                            data.Value = "Total";
                            cellProperties.PdfFont = args.PdfFont;
                            cellProperties.HorizontalAlignment = HorizontalAlignment.Right;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = string.Format("{0:c}", total);
                            cellProperties.PdfFont = args.PdfFont;
                            cellProperties.BorderColor = BaseColor.LIGHT_GRAY;
                            cellProperties.ShowBorder = true;
                        });

                    args.PdfDoc.Add(summaryTable);
                });
            })
            .Export(export =>
            {
                export.ToExcel();
            })
            .Generate(data => data.AsPdfStream(new MemoryStream()));

            return ((MemoryStream)pdfReportData.PdfStreamOutput).ToArray();

        }
    }
}