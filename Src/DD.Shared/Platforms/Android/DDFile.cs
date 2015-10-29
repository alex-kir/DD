//
//  DDFile_Android.cs
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
#if DD_PLATFORM_ANDROID

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

public partial class DDFile
{
//    static string []_assersList = null;

//    public static byte[] GetBytes(string name)
//    {
//        byte[] bytes = null;
//        if (_assersList == null)
//            _assersList = DDDirector.Instance.Activity.Assets.List("");
//
//        if (!_assersList.Contains(name))
//            name = _assersList.FirstOrDefault(it => it.StartsWith(name + ".", StringComparison.InvariantCultureIgnoreCase));
//        if (name != null)
//        {
//            using (var stream = DDDirector.Instance.Activity.Assets.Open(name))
//            {
//                var ms = new MemoryStream();
//                stream.CopyTo(ms);
//                bytes = ms.ToArray();
//            }
//        }
//        return bytes;
//    }

    internal static string FindFile(string name)
    {
        string n = Path.GetFileNameWithoutExtension(name);
        var files = DDDirector.Instance.Activity.Assets.List("");
        var filenames = from f in files
                        where f.StartsWith(n + ".")
                        select f;
        return filenames.FirstOrDefault();
    }

    public static string GetString(string name)
    {
        var bytes = GetBytes(name);
        return Encoding.UTF8.GetString(bytes);
    }

    public static void PutString(string name, string value)
    {
        
    }
}

#endif