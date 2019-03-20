public static class BooleanExtensions
{
    public static string ToYesNoString(this bool value)
    {
        if (value)
        {
            return "Yes";
        }
        else
        {
            return "No";
        }
    }
    public static string ToYesNoString(this bool? value)
    {
        if (value.HasValue)
        {
            if (value.Value)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }
        else
        {
            return string.Empty;
        }
    }
}
