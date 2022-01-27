using SnapIt.Library.Entities;
using System.Collections.ObjectModel;

namespace SnapIt.ViewModels.DesignTime
{
    public class MouseSettingsDesignView
    {
        public MouseButton MouseButton { get; set; }
        public ObservableCollection<MouseButton> MouseButtons { get; set; }
        public bool EnableHoldKey { get; set; }
        public int MouseDragDelay { get; set; }
        public HoldKey HoldKey { get; set; }
        public ObservableCollection<HoldKey> HoldKeys { get; set; }
        public Resource<HoldKeyBehaviour> HoldKeyBehaviour { get; set; }
        public ObservableCollection<Resource<HoldKeyBehaviour>> HoldKeyBehaviours { get; set; }

        public MouseSettingsDesignView()
        {
            MouseButtons = new ObservableCollection<MouseButton>
            {
                MouseButton.Left,
                MouseButton.Middle,
                MouseButton.Right
            };
            MouseButton = MouseButtons[0];

            EnableHoldKey = true;
            MouseDragDelay = 150;
            HoldKeys = new ObservableCollection<HoldKey> {
                HoldKey.Control,
                HoldKey.Alt,
                HoldKey.Shift,
                HoldKey.Win
            };
            HoldKey = HoldKeys[0];

            HoldKeyBehaviours = new ObservableCollection<Resource<HoldKeyBehaviour>>
            {
                new Resource<HoldKeyBehaviour>(Library.Entities.HoldKeyBehaviour.HoldToEnable, "Hold key to enable snapping"),
                new Resource<HoldKeyBehaviour>(Library.Entities.HoldKeyBehaviour.HoldToDisable, "Hold key to disable snapping")
            };
            HoldKeyBehaviour = HoldKeyBehaviours[0];
        }
    }
}