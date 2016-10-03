
namespace Fluffle.AndroidApp
{
    public static class LocalizationExtensions
    {
        public static string Localize(this int resId)
        {
            return MainActivity.instance.Resources.GetString(resId);
        }
    }

}