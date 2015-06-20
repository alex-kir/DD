//
//  DDFile_Unity3d.cs
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
#if DD_PLATFORM_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public partial class DDFile
{
    public static byte[] GetBytes(string name)
    {
        string n = Path.GetFileNameWithoutExtension(name);
        TextAsset asset = Resources.Load(n, typeof(TextAsset)) as TextAsset;
        return asset.bytes;
    }

    public static string GetString(string name)
    {
#if UNITY_EDITOR
		var folder = Path.Combine(Application.dataPath, "Resources");
		var filepath = Path.Combine(folder, Path.GetFileNameWithoutExtension(name) + ".txt");
		return File.ReadAllText(filepath);
#else
        string n = Path.GetFileNameWithoutExtension(name);
        TextAsset asset = Resources.Load(n, typeof(TextAsset)) as TextAsset;
		return asset == null ? null : asset.text;
#endif
    }

	public static void PutString(string name, string text)
	{
#if UNITY_EDITOR
		var folder = Path.Combine(Application.dataPath, "Resources");
		var filepath = Path.Combine(folder, Path.GetFileNameWithoutExtension(name) + ".txt");
		File.WriteAllText(filepath, text);
#endif
	}
}

#endif