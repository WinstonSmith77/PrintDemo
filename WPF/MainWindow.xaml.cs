using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PrintDemo;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            PrinterNames = Win32Stuff.GetPrinterNames();

            InitializeComponent();
        }

        public string[] PrinterNames { get; set; }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var name = (string)((Button) sender).Content;
            Win32Stuff.SetDefaultPrinter(name);
        }
    }
}
