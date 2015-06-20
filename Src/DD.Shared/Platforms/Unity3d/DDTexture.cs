//
//  DDTexture_Unity3d.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013-2014 - Alexander Kirienko
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public partial class DDTexture
{
    public Texture2D Texture { get; set; }
	private bool isAsset;	
	
	protected DDTexture()
    {
		isAsset = false;
		Name = null;
		this.Texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
		this.Texture.SetPixels(Enumerable.Range(0, 16).Select(it => Color.white).ToArray());
		this.Texture.Apply();
		this.Size = new DDVector(4, 4);
	}
	
    protected DDTexture(string name)
    {
		isAsset = true;
		Name = name;
		this.Load();
	}

	protected DDTexture(string name, byte [] bytes)
    {
		isAsset = false;
		Name = name;
		this.Create();
		this.SetData(bytes);
	}
	
	private void Create()
	{
		this.Texture = new Texture2D(4, 4, TextureFormat.DXT5, false);
		this.Size = new DDVector(4, 4);
	}
	
	private void Load()
	{
		if (isAsset)
		{
			try
	        {
	            var n = Path.GetFileNameWithoutExtension(Name);
	            Texture = UnityEngine.Resources.Load(n, typeof(Texture2D)) as Texture2D;
	            Size = new DDVector(Texture.width, Texture.height);
	        }
	        catch
	        {
	            DDDebug.Log("DDTexture ", Name);
	            throw;
	        }
		}
    }
	
	public void Unload()
	{
		if (isAsset)
			UnityEngine.Resources.UnloadAsset(this.Texture);
		else
			GameObject.Destroy(this.Texture);
	}
	
	public void SetData(byte [] bytes)
	{
		this.Texture.LoadImage(bytes);
		this.Size = new DDVector(this.Texture.width, this.Texture.height);
	}
		
}

#endif