﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Domain.UI.Wpf.Common
{
    public sealed class NullToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is null) ? Visibility.Hidden : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
