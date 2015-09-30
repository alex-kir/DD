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

    #if DD_PLATFORM_ANDROID

    private static Android.Content.ISharedPreferences _reader;
    private static Android.Content.ISharedPreferencesEditor _writer;

    private static Android.Content.ISharedPreferences GetReader()
    {
        if (_reader == null)
            _reader = DDDirector.Instance.Activity.GetSharedPreferences("DDUtils", Android.Content.FileCreationMode.Private);
        return _reader;
    }

    private static Android.Content.ISharedPreferencesEditor GetWriter()
    {
        if (_writer == null)
            _writer = GetReader().Edit();
        return _writer;
    }

    public static string LoadString(string key, string defolt)
    {
        return GetReader().GetString(key, defolt);
    }

    public static void SaveString(string key, string value)
    {
        GetWriter().PutString(key, value);
        GetWriter().Commit();
    }

    #endif
}

