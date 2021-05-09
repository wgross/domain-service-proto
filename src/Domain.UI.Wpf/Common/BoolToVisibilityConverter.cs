using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Domain.UI.Wpf.Common
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool TrueIsVisible { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return this.TrueIsVisible
                    ? (booleanValue ? Visibility.Visible : Visibility.Collapsed)
                    : (booleanValue ? Visibility.Collapsed : Visibility.Visible);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}