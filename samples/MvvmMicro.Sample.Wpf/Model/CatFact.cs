using System;
using System.Windows.Media;

namespace MvvmMicro.Sample.Wpf.Model;

public class CatFact
{
    public string Fact { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ImageSource Picture { get; set; }
}
