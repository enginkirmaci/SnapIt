using System;

namespace SnapIt.Library.Entities
{
    public class ApplicationGroup : Bindable
    {
        private string name;
        private string activateHotkey;

        public ApplicationGroup()
        {
            Guid = Guid.NewGuid();
            ApplicationAreas = new ObservableCollectionWithItemNotify<ApplicationArea>();
        }

        public Guid Guid { get; set; }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string ActivateHotkey { get => activateHotkey; set => SetProperty(ref activateHotkey, value); }
        public ObservableCollectionWithItemNotify<ApplicationArea> ApplicationAreas { get; set; }
    }
}