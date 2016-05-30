using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI.Core;
using Windows.UI.Text;
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

            _pages.ForEach(doc.AddPage);

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
            return _pages[index - 1];
        }

        private List<UIElement> _pages = new List<UIElement>();

        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            _printingOptions = e.PrintTaskOptions;
            var doc = GetPrintDoc(sender);

            _pages = CreatePages();

            doc.SetPreviewPageCount(_pages.Count, PreviewPageCountType.Final);

            _invalidatePreview =
                () => doc.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => doc.InvalidatePreview());
        }

        private List<UIElement> CreatePages()
        {
            var pageInfo = _printingOptions.GetPageDescription(0);
            var items = Enumerable.Range(6, 31).ToList();

            var result = new List<UIElement>();

            var page = CreateEmptyPage(1);
            result.Add(page);

            foreach (int item in items)
            {
                GetChildrenContainer(page).Children.Add(new ContentControl { Content = item, FontSize = 50 });
                page = CheckForNewPage(page, pageInfo, result);
            }

            return result;
        }

        private static Panel GetChildrenContainer(Page page)
        {
            return (Panel)(page.FindName("PrintArea"));
        }

        private PrintPage CheckForNewPage(PrintPage page, PrintPageDescription pageInfo, List<UIElement> result)
        {
            var height = CalcUsedHeight(page);

            if (height > pageInfo.PageSize.Height)
            {
                var oldItemList = GetChildrenContainer(page).Children;
                var lastItemOldPage = oldItemList.Last();
                oldItemList.Remove(lastItemOldPage);

                page = CreateEmptyPage(page.Page + 1);
                result.Add(page);
                GetChildrenContainer(page).Children.Add(lastItemOldPage);
            }
            return page;
        }

        private static double CalcUsedHeight(Page page)
        {
            page.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return page.DesiredSize.Height;
        }

        private PrintPage CreateEmptyPage(int page)
        {

            return new PrintPage(page, () => _pages.Count);
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
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
             {
                 var printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);
                 var allOptions = printDetailedOptions.Options.ToList();

                 printDetailedOptions.DisplayedOptions.Clear();

                 var bal = allOptions.Select(item => item.Key).Aggregate("", (result, item) => result + (item + Environment.NewLine));

                 foreach (KeyValuePair<string, IPrintOptionDetails> option in allOptions)
                 {
                     // if (option.Value.OptionType == PrintOptionType.ItemList)
                     {
                         printDetailedOptions.DisplayedOptions.Add(option.Key);
                     }
                 }

                 AddStyleParameter(printDetailedOptions);

                 sourceRequested.SetSource(_printDocumentSource);
             });
        }

        private void AddStyleParameter(PrintTaskOptionDetails printDetailedOptions)
        {
            var pageFormat = printDetailedOptions.CreateItemListOption(StyleParameter,
                StyleParameter);
            pageFormat.AddItem(StyleNormal, StyleNormal);
            pageFormat.AddItem(BoldStyle, BoldStyle);
            printDetailedOptions.DisplayedOptions.Add(StyleParameter);

            printDetailedOptions.OptionChanged += PrintDetailedOptions_OptionChanged;
        }

        private void PrintDetailedOptions_OptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            if (args.OptionId != null && args.OptionId.Equals(StyleParameter))
            {
                _isBold = sender.Options[StyleParameter].Value.Equals(BoldStyle);
                _invalidatePreview?.Invoke();
            }
        }

        private Action _invalidatePreview;
        private bool _isBold;
        private const string StyleParameter = "Style";
        private const string StyleNormal = "Normal";
        private const string BoldStyle = "Bold";
    }
}
