using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using C1.Xaml.Pdf;

namespace PrintHelper
{
    public class ToPDF
    {
        public class PDFAdapter
        {
            private readonly C1PdfDocument _document;
            private readonly List<string> _operations = new List<string>();

            public PDFAdapter(C1PdfDocument document)
            {
                _document = document;
            }

            private void AddToLog(params object[] items)
            {
                var trace = new StackTrace();
                var line = trace.GetFrame(1).GetMethod().Name + " ";
                line = items.Aggregate(line, (current, item) => current + item.GetType() + " " + item + "; ");
                _operations.Add(line);
            }

            public Size MeasureString(string text, Font font)
            {
                AddToLog(text, font);
                return _document.MeasureString(text, font);
            }

            public Size MeasureString(string text, Font font, double width)
            {
                AddToLog(text, font, width);
                return _document.MeasureString(text, font, width);
            }

            public int DrawString(string text, Font font, Color color, Rect rc, StringFormat sf)
            {
                AddToLog(text, font, color, rc, sf);
                return _document.DrawString(text, font, color, rc, sf);
            }

            public int DrawString(string text, Font font, Color color, Rect rc)
            {
                AddToLog(text, font, color, rc);
                return _document.DrawString(text, font, color, rc);
            }

            public void DrawLine(Pen pen, Point pt1, Point pt2)
            {
                AddToLog(pen, pt1, pt2);
                _document.DrawLine(pen, pt1, pt2);
            }

            public Size PageSize
            {
                set
                {
                    AddToLog(value);
                    _document.PageSize = value;
                }
            }

            public void Finish()
            {
               
            }
        }


        public static void XAMLtoPDF(Control control, StorageFile file)
        {
            var c1PdfDocument = new C1PdfDocument(PaperKind.Letter);
            var adapter = new PDFAdapter(c1PdfDocument);
            var list = new List<object>();
            adapter.PageSize = new Size(control.ActualWidth, control.ActualHeight);
            FindTextBlocks(control, list);
            var num1 = checked(list.Count - 1);
            var index = 0;
            while (index <= num1)
            {
                Thickness thickness;
                if (list[index] is TextBlock textBlock)
                {
                    FrameworkElement frameworkElement = textBlock;
                    var num2 = 0.0;
                    for (; frameworkElement != null; frameworkElement = (FrameworkElement)frameworkElement.Parent)
                    {
                        var renderTransform = frameworkElement.RenderTransform;
                        if (renderTransform is TransformGroup transformGroup)
                        {
                            foreach (var transform in transformGroup.Children)
                                if (transform is RotateTransform rotateTransform)
                                    num2 -= rotateTransform.Angle;
                        }
                        else if (renderTransform is RotateTransform)
                        {
                            num2 -= ((RotateTransform)renderTransform).Angle;
                        }
                    }
                    var font = new Font(textBlock.FontFamily.Source, textBlock.FontSize, PdfFontStyle.Italic);
                    var generalTransform = textBlock.TransformToVisual(control);
                    var point = generalTransform.TransformPoint(new Point(0.0, 0.0));
                    double width;
                    double height;
                    if (textBlock.TextWrapping == TextWrapping.NoWrap)
                    {
                        width = adapter.MeasureString(textBlock.Text, font).Width;
                        height = adapter.MeasureString(textBlock.Text, font).Height;
                    }
                    else
                    {
                        width = textBlock.ActualWidth + 10.0;
                        height = adapter.MeasureString(textBlock.Text, font, width).Height;
                    }
                    var rc = new Rect(point.X, point.Y, width, height);
                    if (num2 != 0.0)
                        adapter.DrawString(textBlock.Text, font, ((SolidColorBrush)textBlock.Foreground).Color,
                            rc, new StringFormat
                            {
                                Angle = num2
                            });
                    else
                        adapter.DrawString(textBlock.Text, font, ((SolidColorBrush)textBlock.Foreground).Color,
                            rc);
                }
                else if (list[index] is Border)
                {
                    var border = (Border)list[index];
                    var generalTransform = border.TransformToVisual(control);
                    var point = generalTransform.TransformPoint(new Point(0.0, 0.0));
                    var pointArray = new[]
                    {
                        new Point(point.X, point.Y),
                        new Point(point.X + border.ActualWidth, point.Y),
                        new Point(point.X + border.ActualWidth, point.Y + border.ActualHeight),
                        new Point(point.X, point.Y + border.ActualHeight)
                    };
                    var color = ((SolidColorBrush)border.BorderBrush).Color;
                    thickness = border.BorderThickness;
                    if (thickness.Top != 0.0)
                    {
                        var color1 = color;
                        thickness = border.BorderThickness;
                        var top = thickness.Top;
                        var pen = new Pen(color1, top);
                        var pt1 = pointArray[0];
                        var pt2 = pointArray[1];
                        adapter.DrawLine(pen, pt1, pt2);
                    }
                    thickness = border.BorderThickness;
                    if (thickness.Right != 0.0)
                    {
                        var color1 = color;
                        thickness = border.BorderThickness;
                        var right = thickness.Right;
                        var pen = new Pen(color1, right);
                        var pt1 = pointArray[1];
                        var pt2 = pointArray[2];
                        adapter.DrawLine(pen, pt1, pt2);
                    }
                    thickness = border.BorderThickness;
                    if (thickness.Bottom != 0.0)
                    {
                        var color1 = color;
                        thickness = border.BorderThickness;
                        var bottom = thickness.Bottom;
                        var pen = new Pen(color1, bottom);
                        var pt1 = pointArray[2];
                        var pt2 = pointArray[3];
                        adapter.DrawLine(pen, pt1, pt2);
                    }
                    thickness = border.BorderThickness;
                    if (thickness.Left != 0.0)
                    {
                        var color1 = color;
                        thickness = border.BorderThickness;
                        var left = thickness.Left;
                        var pen = new Pen(color1, left);
                        var pt1 = pointArray[3];
                        var pt2 = pointArray[0];
                        adapter.DrawLine(pen, pt1, pt2);
                    }
                }
                else if (list[index] is Rectangle)
                {
                    var rectangle = (Rectangle)list[index];
                    var generalTransform = rectangle.TransformToVisual(control);
                    var point = generalTransform.TransformPoint(new Point(0.0, 0.0));
                    var pointArray1 = new Point[4];
                    var index1 = 0;
                    var x1 = point.X;
                    thickness = rectangle.Margin;
                    var left1 = thickness.Left;
                    var x2 = x1 + left1;
                    var y1 = point.Y;
                    thickness = rectangle.Margin;
                    var top1 = thickness.Top;
                    var y2 = y1 + top1;
                    var point1 = new Point(x2, y2);
                    pointArray1[index1] = point1;
                    var index2 = 1;
                    var num2 = point.X + rectangle.ActualWidth;
                    thickness = rectangle.Margin;
                    var right1 = thickness.Right;
                    var x3 = num2 - right1;
                    var y3 = point.Y;
                    thickness = rectangle.Margin;
                    var top2 = thickness.Top;
                    var y4 = y3 + top2;
                    var point2 = new Point(x3, y4);
                    pointArray1[index2] = point2;
                    var index3 = 2;
                    var num3 = point.X + rectangle.ActualWidth;
                    thickness = rectangle.Margin;
                    var right2 = thickness.Right;
                    var x4 = num3 - right2;
                    var num4 = point.Y + rectangle.ActualHeight;
                    thickness = rectangle.Margin;
                    var bottom1 = thickness.Bottom;
                    var y5 = num4 - bottom1;
                    var point3 = new Point(x4, y5);
                    pointArray1[index3] = point3;
                    var index4 = 3;
                    var x5 = point.X;
                    thickness = rectangle.Margin;
                    var left2 = thickness.Left;
                    var x6 = x5 + left2;
                    var num5 = point.Y + rectangle.ActualHeight;
                    thickness = rectangle.Margin;
                    var bottom2 = thickness.Bottom;
                    var y6 = num5 - bottom2;
                    var point4 = new Point(x6, y6);
                    pointArray1[index4] = point4;
                    var pointArray = pointArray1;
                    var pen1 = new Pen(((SolidColorBrush)rectangle.Stroke).Color, rectangle.StrokeThickness)
                    {
                        DashStyle = DashStyle.Custom,
                        DashPattern = rectangle.StrokeDashArray.ToArray()
                    };
                    var pen2 = new Pen(((SolidColorBrush)rectangle.Stroke).Color, rectangle.StrokeThickness)
                    {
                        DashStyle = DashStyle.Custom,
                        DashPattern = rectangle.StrokeDashArray.ToArray()
                    };
                    adapter.DrawLine(pen2, pointArray[0], pointArray[1]);
                    adapter.DrawLine(pen1, pointArray[1], pointArray[2]);
                    adapter.DrawLine(pen2, pointArray[2], pointArray[3]);
                    adapter.DrawLine(pen1, pointArray[3], pointArray[0]);
                }
                checked
                {
                    ++index;
                }
            }
            adapter.Finish();
            c1PdfDocument.SaveAsync(file);
        }

        private static void FindTextBlocks(object uiElement, IList<object> foundOnes)
        {
            if (uiElement is TextBlock textBlock)
            {
                if (textBlock.Visibility != 0)
                    return;
                foundOnes.Add(textBlock);
            }
            else if (uiElement is Panel)
            {
                var panel = (Panel)uiElement;
                if (panel.Visibility != 0)
                    return;

                foreach (UIElement uiElement1 in panel.Children)
                    FindTextBlocks(uiElement1, foundOnes);
            }
            else if (uiElement is UserControl)
            {
                var userControl = (UserControl)uiElement;
                if (userControl.Visibility != 0)
                    return;
                FindTextBlocks(userControl.Content, foundOnes);
            }
            else if (uiElement is ContentControl)
            {
                var contentControl = (ContentControl)uiElement;
                if (contentControl.Visibility != 0)
                    return;
                FindTextBlocks(RuntimeHelpers.GetObjectValue(contentControl.Content), foundOnes);
            }
            else if (uiElement is Border)
            {
                var border = (Border)uiElement;
                if (border.Visibility != 0)
                    return;
                foundOnes.Add(border);
                FindTextBlocks(border.Child, foundOnes);
            }
            else
            {
                if (!(uiElement is Rectangle))
                    return;
                var rectangle = (Rectangle)uiElement;
                foundOnes.Add(rectangle);
            }
        }
    }
}