//
//  DDFile.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013 - Alexander Kirienko
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
using System.Reflection;

public partial class DDFile
{
//    private static string[] _resourcesNames;

    private static readonly Dictionary<Assembly, string []> _resourcesNames = new Dictionary<Assembly, string[]>();

    public static void LoadResourcesNames(Assembly assembly)
    {
        var names = assembly.GetManifestResourceNames();
        DDDebug.Trace(names.DDJoinToString(","));
        _resourcesNames[assembly] = names;
    }

//    static DDFile()
//    {
//        var assembly = Assembly.GetCallingAssembly();
//
////        var assembly = typeof(DDFile).GetTypeInfo().Assembly;
//        DDDebug.Trace(assembly);
//
//        DDDebug.Trace(Assembly.GetCallingAssembly());
//        DDDebug.Trace(Assembly.GetEntryAssembly());
//        DDDebug.Trace(Assembly.GetExecutingAssembly());
//
//        _resourcesNames = assembly.GetManifestResourceNames();
//        DDDebug.Trace(_resourcesNames.DDJoinToString(", "));
//    }

    private static Tuple<Assembly, string> CompleteName(string resourceName)
    {
        var resName = resourceName.ToLower();
        foreach (var kv in _resourcesNames) {
            var names = kv.Value;
            var name = names.FirstOrDefault(it => it == resName);
            if (name == null)
                name = names.FirstOrDefault(it => it.EndsWith(resName));
            if (name == null)
                name = names.FirstOrDefault(it => it.ToLower().Contains("." + resName + "."));
            if (name != null)
                return Tuple.Create(kv.Key, name);
        }
        DDDebug.Trace("RES NOT FOUND:" + resourceName);
        return null;
    }

    public static MemoryStream GetStream(string resourceName)
    {
        var bytes = GetBytes(resourceName);
        return bytes == null ? null : new MemoryStream(bytes);
    }

    public static string GetString(string name)
    {
        var bytes = GetBytes(name);
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] GetBytes(string resourceName)
    {
        var name = CompleteName(resourceName);
        if (name == null)
            return null;

//        var assembly = Assembly.GetExecutingAssembly();
//        var assembly = typeof(DDFile).GetTypeInfo().Assembly;
        using (Stream stream = name.Item1.GetManifestResourceStream(name.Item2)) {
            using (var reader = new System.IO.BinaryReader(stream)) {
                var bytes = reader.ReadBytes((int)stream.Length);
                return bytes;
            }
        }
    }

    public static string GetPath(string name)
    {
        #if DD_PLATFORM_ANDROID
        var folder = Path.Combine(DDDirector.Instance.Activity.CacheDir.AbsolutePath, "resources");
        #elif DD_PLATFORM_IOS
        var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
        var folder = Path.Combine (documents, "..", "tmp", "resources");
        #endif

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        var new_path = Path.Combine(folder, name);
        if (!File.Exists(new_path)) {
            var bytes = DDFile.GetBytes(name);
            File.WriteAllBytes(new_path, bytes);
        }
        return new_path;
    }


    //public static byte[] GetBytes(string name)
    //{
    //    byte[] bytes = null;

    //    var files = DDDirector.Instance.Activity.Assets.List("");

    //    if (!files.Contains(name))
    //    {
    //        var filenames = from f in files
    //                        where (f + ".").StartsWith(name)
    //                        select f;
    //        name = filenames.FirstOrDefault();
    //    }
    //    using (var stream = DDDirector.Instance.Activity.Assets.Open(name))
    //    {
    //        var ms = new MemoryStream();
    //        stream.CopyTo(ms);
    //        bytes = ms.ToArray();
    //    }

    //    return bytes;
    //}
}