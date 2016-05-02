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

//    public static T Assign<T>(this T self, ref T variable)
//    {
//        variable = self;
//        return self;
//    }

    public static void OpenMarketForApp(string iosId, string androidId)
    {
#if DD_PLATFORM_ANDROID
        
        var uri = string.Format("market://details?id={0}", androidId ?? DDDirector.Instance.Activity.PackageName);
#elif DD_PLATFORM_IOS
        var uri = string.Format("https://itunes.apple.com/us/app/xxxx/id{0}?ls=1&mt=8", iosId);
#endif        
        OpenUri(uri);
    }

    public static void OpenUri(string uri)
    {
        DDDebug.Trace(uri);
#if DD_PLATFORM_ANDROID
        DDDirector.Instance.Activity.StartActivity(
            new global::Android.Content.Intent(global::Android.Content.Intent.ActionView)
            .SetData(global::Android.Net.Uri.Parse(uri))
        );
#elif DD_PLATFORM_IOS

#endif        
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

    #elif DD_PLATFORM_IOS

    public static string LoadString(string key, string defolt)
    {
        return Foundation.NSUserDefaults.StandardUserDefaults.StringForKey(key) ?? defolt;
    }

    public static void SaveString(string key, string value)
    {
        Foundation.NSUserDefaults.StandardUserDefaults.SetString(value, key);
    }

    #endif

    public static bool LoadBool(string key, bool defolt)
    {
        var s = LoadString(key, null);
        if (s == null)
            return defolt;
        return s == "true";
    }

    public static void SaveBool(string key, bool value)
    {
        SaveString(key, value ? "true" : "false");
    }

    public static int LoadInt(string key, int defolt)
    {
        int ret;
        if (int.TryParse(LoadString(key, ""), out ret))
            return ret;
        return defolt;
    }

    public static void SaveInt(string key, int value)
    {
        SaveString(key, value.ToString());
    }

    public static void CatchAllTaps(this DDNode self)
    {
        self.AddTouchHandler(DDTouchPhase.Began | DDTouchPhase.Moved | DDTouchPhase.Ended, args => args.Processed = true);
    }
}

