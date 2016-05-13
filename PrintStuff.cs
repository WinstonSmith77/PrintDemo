using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace PrintDemo
{
    public class PrintStuff
    {

        private IPrintDocumentSource _printDocumentSource;
        private PrintTaskOptions _printingOptions;

        public async void Print()
        {
            var manager = PrintManager.GetForCurrentView();

            var printDocument = new PrintDocument();
            _printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += CreatePrintPreviewPages;
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;

            manager.PrintTaskRequested += Manager_PrintTaskRequested;
            try
            {
                try
                {
                    await PrintManager.ShowPrintUIAsync();
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
            var doc = GetPrintDoc(sender);

            for (int i = 0; i < _numberOfPages; i++)
            {
                doc.AddPage(CreatePreviewPage(i));
            }

            // Indicate that all of the print pages have been provided
            doc.AddPagesComplete();
        }

        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var doc = GetPrintDoc(sender);
            doc.SetPreviewPage(e.PageNumber, CreatePreviewPage(e.PageNumber));
        }

        private UIElement CreatePreviewPage(int index)
        {
            var page = _printingOptions.GetPageDescription(0);
            var textblock = new TextBlock { Text = "Page" + index };

            return new Viewbox { Child = textblock, Width = page.PageSize.Width, Height = page.PageSize.Height };
        }

        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            _printingOptions = e.PrintTaskOptions;
            var doc = GetPrintDoc(sender);
            doc.SetPreviewPageCount(_numberOfPages, PreviewPageCountType.Final);
        }

        private const int _numberOfPages = 10;

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
            e.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
            {
                sourceRequested.SetSource(_printDocumentSource);
            });
        }
    }
}
