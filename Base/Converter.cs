using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static BaseUI.BaseEnum;
using Binding = System.Windows.Data.Binding;

namespace BaseUI
{
    /// <summary>
    /// Null 또는 Empty 값을 Visibility 로 변환하는 변환기
    /// </summary>
    public class NullOrEmptyToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Null 값을 Empty string 으로 변환하는 변환기
    /// </summary>
    public class NullToEmptyString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? null : value;
        }
    }

    /// <summary>
    /// Null 값을 False로 변환하는 변환기
    /// </summary>
    public class NullToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : true;
        }
    }

    /// <summary>
    /// UI의 이미지 Enabled 상태에 따른 이미지 전환용 변환기
    /// </summary>
    public class EnabledImageSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Function.GetEnabledImageSource(values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Bool 값을 Visibility로 변환하는 변환기
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                return Function.BoolToVisibility(b, Invert);
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    

    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }

    public sealed class ScaleToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is double d && !double.IsNaN(d))
            {
                var k = 0.5; // 기본 1/2
                if(parameter is string s && double.TryParse(s, out var p))
                    k = p;
                var r = Math.Max(0, d * k);
                return new CornerRadius(r);
            }
            return new CornerRadius(0);
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
    }
    public sealed class ScaleToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            /// null 또는 double 아닌 것 방지.
            if(!(value is double d) || double.IsNaN(d)) return 0d;

            var k = 1.0;
            /// 파라미터 string 인 경우
            if(parameter is string s)
            {
                s = s.Trim();
                /// 파라미터에 % 들어갔을 경우 대비해서 string pareser
                if(s.EndsWith("%"))
                {
                    if(double.TryParse(s.TrimEnd('%'), NumberStyles.Float, cInfo, out var p))
                    {
                        k = (p / 100.0);
                    }
                }
                /// 파라미터 값이 소숫점 텍스트일 경우 대비해서.
                else if(double.TryParse(s, NumberStyles.Float, cInfo, out var p))
                {
                    k = p;
                }
            }
            /// 파라미터 double 인 경우
            else if(parameter is double pd)
            {
                k = pd;
            }

            var result = d * k;
            return double.IsNaN(result) ? 0d : Math.Max(0d, result);
        }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }

    /// <summary>
    /// UI 내부에서 사용하는 날짜(yyyy-MM-dd HH:mm:ss) 텍스트들의 바인딩 변수를
    /// DateTime -> string 으로 해당 날짜 포맷으로 자동변환 해주는 컨버터 클래스.
    /// </summary>
    public class DateTimeFormatConverter : IValueConverter
    {
        public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public object Convert(object value, Type targetType, object parameter, CultureInfo cInfo)
        {
            if(value is DateTime dt)
                return dt.ToString(Format);

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }

    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is double d)
                return d.ToString();

            return string.Empty;
        }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }

    public class DoubleToPercentageStringConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is double d)
            {
                decimal dd = (decimal)d;
                return $"{dd} %";
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }
    public class EnumPassThroughConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture) => value;
        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture) => value;
    }

    public class ButtonStatusToIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is ComponentStatus status)
            {
                switch(status)
                {
                    case ComponentStatus.Enable:
                        return true;

                    case ComponentStatus.Disable:
                        return false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }

    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double d)
                return new GridLength(d);

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is GridLength g)
                return g.Value;

            return 0d;
        }
    }

    public class PopupTypeToButtonStateConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is PopupType pt)
            {
                switch(pt)
                {
                    case PopupType.Info:
                    case PopupType.InfoSingle:
                        return Function.GetResourceObject("Button.Info");
                    case PopupType.Warning:
                    case PopupType.WarningSingle:
                        return Function.GetResourceObject("Button.Warn");
                    case PopupType.Error:
                    case PopupType.ErrorSingle:
                        return Function.GetResourceObject("Button.Error");
                }
            }
            return Function.GetResourceObject("Button.Idle");
        }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }

    public class PopupTypeToAlarmConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo cInfo)
        {
            if(value is PopupType pt)
            {
                switch(pt)
                {
                    case PopupType.Info:
                    case PopupType.InfoSingle:
                        return Function.GetResourceObject("Alarm.Info");
                    case PopupType.Warning:
                    case PopupType.WarningSingle:
                        return Function.GetResourceObject("Alarm.Warn");
                    case PopupType.Error:
                    case PopupType.ErrorSingle:
                        return Function.GetResourceObject("Alarm.Error");
                }
            }
            return Function.GetResourceObject("Alarm.Info");
        }
        public object ConvertBack(object value, Type type, object parameter, CultureInfo cInfo) => Binding.DoNothing;
    }
}
