using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.WebUI;
using Windows.UI.Xaml.Controls;

namespace PrintDemo
{
    public class PrintStuff
    {
        public async void Print()
        {
            var manager = PrintManager.GetForCurrentView();

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


            });
        }
    }
}
