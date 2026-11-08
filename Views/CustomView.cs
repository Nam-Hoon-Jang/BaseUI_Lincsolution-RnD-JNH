using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BaseUI
{
    public static class EllipseProgressBar
    {
        public static double GetValue(Ellipse ellipse) => (double)ellipse.GetValue(ValueProperty);
        public static void SetValue(Ellipse obj, double value) => obj.SetValue(ValueProperty, value);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(EllipseProgressBar), new PropertyMetadata(0d, OnPropertyChanged));

        private static readonly DependencyProperty EllipseProgressBindingProperty =
            DependencyProperty.RegisterAttached("EllipseProgressBinding", typeof(MultiBinding), typeof(EllipseProgressBar), new PropertyMetadata(null));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is Ellipse ellipse && !(ellipse.GetValue(EllipseProgressBindingProperty) is MultiBinding))
            {
                var multiBinding = new MultiBinding { Converter = new Converter() };
                multiBinding.Bindings.Add(new System.Windows.Data.Binding { Path = new PropertyPath(ValueProperty), Source = ellipse });
                multiBinding.Bindings.Add(new System.Windows.Data.Binding(nameof(ellipse.ActualWidth)) { Source = ellipse });
                multiBinding.Bindings.Add(new System.Windows.Data.Binding(nameof(ellipse.ActualHeight)) { Source = ellipse });
                multiBinding.Bindings.Add(new System.Windows.Data.Binding(nameof(ellipse.StrokeThickness)) { Source = ellipse });
                BindingOperations.SetBinding(ellipse, Shape.StrokeDashArrayProperty, multiBinding);
                ellipse.SetValue(EllipseProgressBindingProperty, multiBinding);
            }
        }

        class Converter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if(values.Length >= 4
                    && values[0] != DependencyProperty.UnsetValue
                    && values[1] is double width
                    && values[2] is double height
                    && values[3] is double thickness)
                {
                    width -= thickness;
                    height -= thickness;
                    var total = Math.PI * Math.Sqrt((width * width + height * height) / 2) / thickness;
                    var value = Math.Min(1, Math.Max(0, System.Convert.ToDouble(values[0]) / 100)) * total;
                    var quarter = total * 0.25;
                    return new DoubleCollection() { Math.Max(0, value - quarter), Math.Min(total - quarter, total - value), value, total };
                }
                return null;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    }
}
