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

}

