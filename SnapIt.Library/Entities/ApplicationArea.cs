namespace SnapIt.Library.Entities
{
    public class ApplicationArea : Bindable
    {
        public ApplicationArea(int number)
        {
            Applications = new ObservableCollectionWithItemNotify<ApplicationItem>();
            Number = number;
        }

        public string Name
        { get { return $"Area {Number}"; } }

        public int Number { get; set; }
        public ObservableCollectionWithItemNotify<ApplicationItem> Applications { get; set; }
    }
}