using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace PrintDemo
{
    public static class DumpToPNG
    {
        private static async void SaveVisualElementToFile(UIElement element, StorageFile file)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(element);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)rtb.PixelWidth,
                    (uint)rtb.PixelHeight,
                    displayInformation.RawDpiX,
                    displayInformation.RawDpiY,
                    pixels);
                await encoder.FlushAsync();
            }
        }

        public static void DumpToFile(List<UIElement> pages)
        {
            int index = 0;
            pages.ForEach(item => SaveToFile(item, index++));
        }

        private static void SaveToFile(UIElement item, int i)
        {
            var path = ApplicationData.Current.LocalFolder;
            var file = path.CreateFileAsync(
                $"printDump{i}.png",
                CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();

            SaveVisualElementToFile(item, file);
        }
    }
}
