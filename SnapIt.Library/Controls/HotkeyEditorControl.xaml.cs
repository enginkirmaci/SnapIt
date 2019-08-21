using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SnapIt.Library.Controls
{
    /// <summary>
    /// Interaction logic for HotkeyEditorControl.xaml
    /// </summary>
    public partial class HotkeyEditorControl : UserControl
    {
        public static readonly DependencyProperty HotkeyProperty =
          DependencyProperty.Register(nameof(Hotkey), typeof(string), typeof(HotkeyEditorControl),
              new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Hotkey
        {
            get => (string)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public HotkeyEditorControl()
        {
            InitializeComponent();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
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

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (Keyboard.Modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                Hotkey = null;
                return;
            }

            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                // Set values
                var str = new StringBuilder();

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    str.Append("Control + ");
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    str.Append("Shift + ");
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
                    str.Append("Alt + ");
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows))
                    str.Append("Win + ");

                str.Append(key);

                Hotkey = str.ToString();
            }
        }
    }
}