using System.Windows.Controls;

namespace SnapIt.Controls;

public class ListViewEx : ListView
{
    public ListViewEx()
    {
        SelectionChanged += new SelectionChangedEventHandler(ListViewEx_SelectionChanged);
    }

    private void ListViewEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItem != null)
        {
            ScrollIntoView(SelectedItem);
        }
    }
}