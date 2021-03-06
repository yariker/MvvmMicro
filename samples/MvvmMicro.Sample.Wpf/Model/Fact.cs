using System;
using System.Windows.Media;

namespace MvvmMicro.Sample.Wpf.Model
{
    public class Fact
    {
        public string Text { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ImageSource Picture { get; set; }
    }
}