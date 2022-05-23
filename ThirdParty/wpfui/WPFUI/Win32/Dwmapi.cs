﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Runtime.InteropServices;

namespace WPFUI.Win32
{
    /// <summary>
    /// Used by Desktop Window Manager (DWM)
    /// </summary>
    internal static class Dwmapi
    {
        /// <summary>
        /// DWMWINDOWATTRIBUTE enumeration. (dwmapi.h)
        /// <para><see href="https://github.com/electron/electron/issues/29937"/></para>
        /// </summary>
        [Flags]
        public enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// Enables content rendered in the non-client area to be visible on the frame drawn by DWM.
            /// </summary>
            DWMWA_ALLOW_NCPAINT = 4,

            /// <summary>
            /// Retrieves the bounds of the caption button area in the window-relative space.
            /// </summary>
            DWMWA_CAPTION_BUTTON_BOUNDS = 5,

            /// <summary>
            /// Forces the window to display an iconic thumbnail or peek representation (a static bitmap), even if a live or snapshot representation of the window is available.
            /// </summary>
            DWMWA_FORCE_ICONIC_REPRESENTATION = 7,

            /// <summary>
            /// Cloaks the window such that it is not visible to the user.
            /// </summary>
            DWMWA_CLOAK = 13,

            /// <summary>
            /// If the window is cloaked, provides one of the following values explaining why.
            /// </summary>
            DWMWA_CLOAKED = 14,

            /// <summary>
            /// Freeze the window's thumbnail image with its current visuals. Do no further live updates on the thumbnail image to match the window's contents.
            /// </summary>
            DWMWA_FREEZE_REPRESENTATION = 15,

            /// <summary>
            /// Allows a window to either use the accent color, or dark, according to the user Color Mode preferences.
            /// </summary>
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,

            /// <summary>
            /// Controls the policy that rounds top-level window corners.
            /// <para>Windows 11 and above.</para>
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,

            /// <summary>
            /// The color of the thin border around a top-level window.
            /// </summary>
            DWMWA_BORDER_COLOR = 34,

            /// <summary>
            /// The color of the caption.
            /// <para>Windows 11 and above.</para>
            /// </summary>
            DWMWA_CAPTION_COLOR = 35,

            /// <summary>
            /// The color of the caption text.
            /// <para>Windows 11 and above.</para>
            /// </summary>
            DWMWA_TEXT_COLOR = 36,

            /// <summary>
            /// Width of the visible border around a thick frame window.
            /// <para>Windows 11 and above.</para>
            /// </summary>
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS = 37,

            /// <summary>
            /// Allows to enter a value from 0 to 4 deciding on the imposed backdrop effect.
            /// </summary>
            DWMWA_SYSTEMBACKDROP_TYPE = 38,

            /// <summary>
            /// Indicates whether the window should use the Mica effect.
            /// <para>Windows 11 and above.</para>
            /// </summary>
            DWMWA_MICA_EFFECT = 1029
        }

        /// <summary>
        /// Abstraction of pointer to an object containing the attribute value to set. The type of the value set depends on the value of the dwAttribute parameter.
        /// The DWMWINDOWATTRIBUTE enumeration topic indicates, in the row for each flag, what type of value you should pass a pointer to in the pvAttribute parameter.
        /// </summary>
        public enum PvAttribute
        {
            /// <summary>
            /// Object containing the <see langowrd="false"/> attribute value to set in dwmapi.h. 
            /// </summary>
            Disable = 0x00,

            /// <summary>
            /// Object containing the <see langowrd="true"/> attribute value to set in dwmapi.h. 
            /// </summary>
            Enable = 0x01
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute,
            int cbAttribute);
    }
}
