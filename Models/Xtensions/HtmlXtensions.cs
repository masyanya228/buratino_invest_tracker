namespace Buratino.Models.Xtensions
{
    public static class HtmlXtensions
    {
        public static string AsHtmlDateTime(this DateTime? source)
        {
            return source?.ToString("s") ?? "";
        }
    }
}