﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFUI.Controls
{
    /// <summary>
    /// Notifies page about being navigated.
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Navigation service, from which the navigation was made.</param>
        /// <param name="current">Current page.</param>
        void OnNavigationRequest(INavigation sender, object current);
    }
}
