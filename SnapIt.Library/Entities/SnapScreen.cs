﻿using System.Collections.Generic;
using System.Windows;

namespace SnapIt.Library.Entities
{
    public class SnapScreen : Bindable
    {
        private Layout layout;
        private bool isActive = true;
        private string primary;
        private bool isPrimary = false;
        private string deviceNumber;
        private string resolution;
        private List<ApplicationGroup> applicationGroups;

        public string DeviceName { get; set; }
        public Rect WorkingArea { get; set; }
        public double ScaleFactor { get; set; }
        public Rect Bounds { get; set; }

        public bool IsActive
        { get => isActive; set { SetProperty(ref isActive, value); } }

        public bool IsPrimary
        { get => isPrimary; set { SetProperty(ref isPrimary, value); } }

        public string Primary
        { get => primary; set { SetProperty(ref primary, value); } }

        public string DeviceNumber
        { get => deviceNumber; set { SetProperty(ref deviceNumber, value); } }

        public string Resolution
        { get => resolution; set { SetProperty(ref resolution, value); } }

        public Layout Layout
        { get => layout; set { SetProperty(ref layout, value); } }

        public List<ApplicationGroup> ApplicationGroups
        { get => applicationGroups; set { SetProperty(ref applicationGroups, value); } }

        public SnapScreen()
        { }

        public SnapScreen(WpfScreenHelper.Screen screen, string devicePath)
        {
            IsPrimary = screen.Primary;
            Primary = IsPrimary ? "Primary" : "";
            DeviceNumber = screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty);
            Resolution = $"{screen.Bounds.Width} X {screen.Bounds.Height}";

            WorkingArea = screen.WorkingArea;
            ScaleFactor = screen.ScaleFactor;

            Bounds = screen.WpfBounds;
            if (!string.IsNullOrEmpty(devicePath))
            {
                DeviceName = devicePath;
            }
            else
            {
                DeviceName = screen.DeviceName;
            }
        }
    }
}