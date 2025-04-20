using System.Collections.Generic;

namespace MvvmMicro.Sample.Wpf.Model;

public class CatFactList
{
    public List<CatFact> Data { get; set; }

    public int CurrentPage { get; set; }

    public int LastPage { get; set; }
}
