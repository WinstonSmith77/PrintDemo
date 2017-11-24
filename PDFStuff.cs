using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Controls;
using PrintHelper;

namespace PrintDemo
{
    public static class PDFStuff
    {
        public static void Export(List<Control> pages)
        {

            var index = 0;
            var path = Windows.Storage.ApplicationData.Current.LocalFolder;
            pages.ForEach(ui =>
            {
                var file = path.CreateFileAsync($"{index++}.pdf",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();



                ToPDF.XAMLtoPDF(ui, file);
            });
        }
    }
}
