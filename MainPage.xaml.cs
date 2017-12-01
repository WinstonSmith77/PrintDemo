using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PrintDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            _pages = PrintStuff.CreatePages(2000);

            printStuff.Children.Clear();

            _pages.ForEach(ui => printStuff.Children.Add(ui));
        }

        private readonly List<Control> _pages;

        private  void PrintClick(object sender, RoutedEventArgs e)
        {/*
            var printerNames = Win32Stuff.GetPrinterNames();
            var result = Win32Stuff.SetDefaultPrinter(printerNames[2]);*/
            new PrintStuff().Print();
        }

        private void PDFClick(object sender, RoutedEventArgs e)
        {
            PDFStuff.Export(_pages);
        }

        private void PNGClick(object sender, RoutedEventArgs e)
        {
           DumpToPNG.DumpToFile(_pages.Cast<UIElement>().ToList());
        }
    }
}
