﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Windows.Data;
using WPFUI.Common;

namespace WPFUI.Converters
{
    /// <summary>
    /// Converts using <see cref="Convert"/> <see cref="Icon"/> or <see cref="IconFilled"/> to <see langword="string"/>.
    /// </summary>
    internal class IconToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="Icon"/> or <see cref="IconFilled"/> to <see langword="string"/>.
        /// <para>If the given value is <see langword="char"/> or <see langword="string"/> it will simply be returned as a <see langword="string"/>.</para>
        /// </summary>
        /// <returns><see langword="string"/> representing <see cref="Icon"/> or <see cref="IconFilled"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType() == typeof(String))
            {
                return value;
            }

            if (value.GetType() == typeof(Char))
            {
                return (string)value;
            }

            if (value.GetType() == typeof(Icon))
            {
                return Glyph.ToString((Icon)value);
            }

            if (value.GetType() == typeof(IconFilled))
            {
                return Glyph.ToString((IconFilled)value);
            }

            return null;
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}