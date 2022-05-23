﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows;

namespace WPFUI.Controls
{
    /// <summary>
    /// Displays the rating scale with interactions.
    /// </summary>
    public class Rating : System.Windows.Controls.ContentControl
    {
        /// <summary>
        /// Property for <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value),
            typeof(double), typeof(Rating), new PropertyMetadata(3.0));

        /// <summary>
        /// Property for <see cref="Icon"/>.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
            typeof(Common.Icon), typeof(Rating),
            new PropertyMetadata(Common.Icon.Star28));

        /// <summary>
        /// User rating.
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Gets or sets displayed <see cref="Common.Icon"/>.
        /// </summary>
        public Common.Icon Icon
        {
            get => (Common.Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
