using System.Collections.Generic;

namespace Dimpoc;

public class FieldResultItem
{
    public string FieldName { get; set; } = string.Empty;
    public bool Empty { get; set; }
    public bool Valid { get; set; }
}

public class FieldResults
{
    public List<FieldResultItem> Items { get; } = new List<FieldResultItem>();
    public bool AllValid => Items.Count == 0 || Items.TrueForAll(i => i.Valid);
}
