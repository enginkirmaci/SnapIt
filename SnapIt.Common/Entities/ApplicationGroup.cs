using SnapIt.Common.Mvvm;

namespace SnapIt.Common.Entities;

public class ApplicationGroup : Bindable
{
    private string name;
    private string activateHotkey;

    public ApplicationGroup()
    {
        Guid = Guid.NewGuid();
        ApplicationAreas = [];
    }

    public Guid Guid { get; set; }
    public string Name { get => name; set => SetProperty(ref name, value); }
    public string ActivateHotkey { get => activateHotkey; set => SetProperty(ref activateHotkey, value); }
    public ObservableCollectionWithItemNotify<ApplicationArea> ApplicationAreas { get; set; }
}