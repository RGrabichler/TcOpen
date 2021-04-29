﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Vortex.Presentation.Wpf.Converters;

namespace TcOpen
{
    public class ActuatorToBrushConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes.LightGray;
            try
            {
                return (bool)value ? Vortex.Presentation.Styling.Wpf.VortexResources.Accent : Vortex.Presentation.Styling.Wpf.VortexResources.Secondary;
            }
            catch (Exception)
            {
                return Vortex.Presentation.Styling.Wpf.VortexResources.Alert;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class VisibleOnConverter : BaseConverter
    {
        public override object ToConvert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.Equals("True") && (bool)value)
                return Visibility.Visible;
            if (parameter.Equals("False") && !((bool)value))
                return Visibility.Collapsed;
            throw new ArgumentException("Always use paramater True or False to display the value on value");
        }
    }
}
