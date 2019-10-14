namespace SnapIt.Library.Entities
{
    public class ExcludedApplication
    {
        public string Keyword { get; set; }
        public MatchRule MatchRule { get; set; } = MatchRule.Contains;
        public InputDevice AppliedFor
        {
            get
            {
                if (Mouse && Keyboard)
                    return InputDevice.Both;
                else if (Mouse)
                    return InputDevice.Mouse;
                else if (Keyboard)
                    return InputDevice.Keyboard;

                return InputDevice.None;
            }
        }

        public bool Mouse { get; set; } = true;
        public bool Keyboard { get; set; } = true;
    }
}