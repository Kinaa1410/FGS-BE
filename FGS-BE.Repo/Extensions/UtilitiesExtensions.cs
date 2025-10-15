namespace FGS_BE.Repo.Extensions;

public static class UtilitiesExtensions
{
    public static string Format(this string format, params object[] args)
    {
        return string.Format(format, args);
    }
}
