using System.Collections.Generic;

public class GoBagResult
{
    public bool IsComplete { get; private set; }
    public List<SelectChoice> MissingItems { get; private set; }

    public GoBagResult(List<SelectChoice> missingItems)
    {
        MissingItems = missingItems;
        IsComplete = missingItems.Count == 0;
    }

    public string Describe()
    {
        return (IsComplete ? "COMPLETE" : "INCOMPLETE") + ". Missing: " + GetNames(MissingItems);
    }

    private static string GetNames(List<SelectChoice> list)
    {
        if (list.Count == 0) return "none";

        string names = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0) names += ", ";
            names += list[i].label;
        }
        return names;
    }
}
