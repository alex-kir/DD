using System;

public static class DDUtils
{
    public static float GetDPI()
    {
        #if DD_PLATFORM_ANDROID
        var density = DDDirector.Instance.Activity.Resources.DisplayMetrics.Density;
        return density * 160f;
        #else
        return 160;
        #endif

    }

    public static string Format(this string self, params object [] args)
    {
        return string.Format(self, args);
    }

    public static T Assign<T>(this T self, ref T variable)
    {
        variable = self;
        return self;
    }
}

