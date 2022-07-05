using System;

namespace SnapIt.Library.Entities
{
    public class ApplicationItem : Bindable
    {
        private string path;
        private string title;
        private string arguments;
        private string startIn;
        private int delayAfterOpen;

        public ApplicationItem()
        {
        }

        public ApplicationItem(ApplicationItem selectedApplicationItem)
        {
            Guid = selectedApplicationItem.Guid;
            AreaNumber = selectedApplicationItem.AreaNumber;
            Path = selectedApplicationItem.Path;
            Title = selectedApplicationItem.Title;
            Arguments = selectedApplicationItem.Arguments;
            DelayAfterOpen = selectedApplicationItem.DelayAfterOpen;
        }

        public Guid Guid { get; set; }
        public int AreaNumber { get; set; }
        public string Path { get => path; set => SetProperty(ref path, value); }
        public string Title { get => title; set => SetProperty(ref title, value); }
        public string Arguments { get => arguments; set => SetProperty(ref arguments, value); }
        public string StartIn { get => startIn; set => SetProperty(ref startIn, value); }
        public int DelayAfterOpen { get => delayAfterOpen; set => SetProperty(ref delayAfterOpen, value); }
    }
}