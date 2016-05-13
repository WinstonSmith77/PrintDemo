using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.WebUI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace PrintDemo
{
    public class PrintStuff
    {
        private PrintDocument _printDocument;
        private IPrintDocumentSource _printDocumentSource;

        public async void Print()
        {
            var manager = PrintManager.GetForCurrentView();

            _printDocument = new PrintDocument();
            _printDocumentSource = _printDocument.DocumentSource;
            _printDocument.Paginate += CreatePrintPreviewPages;
            _printDocument.GetPreviewPage += GetPrintPreviewPage;
            _printDocument.AddPages += AddPrintPages;

            manager.PrintTaskRequested += Manager_PrintTaskRequested;
            try
            {
                try
                {
                    var result = await PrintManager.ShowPrintUIAsync();

                    await ShowInfoDialog(result);
                }
                catch
                {
                    await ShowInfoDialog(false);
                }
            }
            finally
            {
                manager.PrintTaskRequested -= Manager_PrintTaskRequested;
            }
        }

        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {

        }

        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var doc = GetPrintDoc(sender);

            doc.SetPreviewPage(e.PageNumber, new TextBlock { Text = e.PageNumber.ToString() });
        }

        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            var doc = GetPrintDoc(sender);
            doc.SetPreviewPageCount(10, PreviewPageCountType.Final);
        }

        private static PrintDocument GetPrintDoc(object sender)
        {
            return (PrintDocument)sender;
        }

        private static async Task ShowInfoDialog(bool result)
        {
            // Printing cannot proceed at this time
            ContentDialog noPrintingDialog = new ContentDialog
            {
                Title = "Printing",
                Content = result ? "Print Successful!" : "\nSorry, printing can' t proceed at this time.",
                PrimaryButtonText = "OK"
            };
            await noPrintingDialog.ShowAsync();
        }

        private void Manager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            var printTask = e.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
            {
                sourceRequested.SetSource(_printDocumentSource);

            });
        }
    }
}
