using SnapIt.Common.Mvvm;

namespace SnapIt.Common.Entities;

public class ApplicationArea : Bindable
{
    public ApplicationArea(int number)
    {
        Applications = [];
        Number = number;
    }

    public string Name
    { get { return $"Area {Number}"; } }

    public int Number { get; set; }
    public ObservableCollectionWithItemNotify<ApplicationItem> Applications { get; set; }
}