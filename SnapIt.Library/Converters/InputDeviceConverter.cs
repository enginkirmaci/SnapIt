﻿using SnapIt.Library.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SnapIt.Library.Converters
{
    public class InputDeviceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is InputDevice)
            {
                switch ((InputDevice)value)
                {
                    case InputDevice.Both:
                        return "Keyboard, Mouse";

                    case InputDevice.Keyboard:
                        return "Keyboard";

                    case InputDevice.Mouse:
                        return "Mouse";
                }
            }

            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}