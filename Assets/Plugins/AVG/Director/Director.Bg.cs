using System.Linq;

public partial class Director
{
    private void SetBackground(object[] args = null)
    {
        string key = args?.ElementAtOrDefault(0)?.ToString() ?? "defaultKey";
        string effectType = args?.ElementAtOrDefault(1)?.ToString() ?? "fade";
        float duration = float.TryParse(args?.ElementAtOrDefault(2)?.ToString(), out float parsedDuration) ? parsedDuration : 2f;

        Avg.Background.GoTo(key, effectType, duration);
    }
}
