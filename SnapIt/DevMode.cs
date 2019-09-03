namespace SnapIt
{
    public class DevMode
    {
#if DEBUG
        public const bool IsActive = false;
        public const bool ShowSnapWindowOnStartup = false;
# else
        public const bool IsActive = false;
        public const bool ShowSnapWindowOnStartup = false;
#endif
    }
}