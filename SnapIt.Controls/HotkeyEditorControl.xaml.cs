using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnapIt.Controls;

/// <summary>
/// Interaction logic for HotkeyEditorControl.xaml
/// </summary>
public partial class HotkeyEditorControl : UserControl
{
    public static readonly DependencyProperty HotkeyProperty =
      DependencyProperty.Register(nameof(Text), typeof(string), typeof(HotkeyEditorControl),
          new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Text
    {
        get => (string)GetValue(HotkeyProperty);
        set => SetValue(HotkeyProperty, value);
    }

    //public static readonly DependencyProperty IsFocusProperty =
    // DependencyProperty.Register(nameof(IsFocused), typeof(bool), typeof(HotkeyEditorControl),
    //     new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    //public bool IsFocused
    //{
    //    get => (bool)GetValue(IsFocusProperty);
    //    set => SetValue(IsFocusProperty, value);
    //}

    public HotkeyEditorControl()
    {
        InitializeComponent();

        //HotkeyTextBox.GotFocus += HotkeyTextBox_GotFocus;
        //HotkeyTextBox.LostFocus += HotkeyTextBox_LostFocus;
    }

    //private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
    //{
    //    IsFocused = false;
    //}

    //private void HotkeyTextBox_GotFocus(object sender, RoutedEventArgs e)
    //{
    //    IsFocused = true;
    //}

    private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        // Get modifiers and key data
        var modifiers = Keyboard.Modifiers;
        var key = e.Key;

        // When Alt is pressed, SystemKey is used instead
        if (key == Key.System)
        {
            key = e.SystemKey;
        }

        // Pressing delete, backspace or escape without modifiers clears the current value
        if (modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back || key == Key.Escape))
        {
            Text = null;
            return;
        }

        // If no actual key was pressed - return
        if (key == Key.LeftCtrl
            || key == Key.RightCtrl
            || key == Key.LeftAlt
            || key == Key.RightAlt
            || key == Key.LeftShift
            || key == Key.RightShift
            //|| key == Key.LWin
            //|| key == Key.RWin
            || key == Key.Clear
            || key == Key.OemClear
            || key == Key.Apps)
        {
            return;
        }

        if (modifiers != ModifierKeys.None)
        {
            var str = new StringBuilder();

            if (modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Control + ");
            if (modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(key);

            Text = str.ToString();
        }
    }
}