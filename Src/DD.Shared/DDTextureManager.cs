//
//  DDTextureManager.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

public partial class DDTextureManager
{
	private class DDTextureFriend : DDTexture
	{
        public DDTextureFriend() : base() { }
        public DDTextureFriend(string name) : base(name) { }
        public DDTextureFriend(string name, byte [] bytes) : base(name, bytes) { }
	}
	
	private static DDTextureManager _instance = new DDTextureManager();
    public static DDTextureManager Instance { get { return _instance; } }

    List<DDTexture> _textures = new List<DDTexture>();
    Dictionary<string, DDTextureFriend> _namedTextures = new Dictionary<string, DDTextureFriend>();
    Dictionary<string, DDTextureFrame> _namedTextureFrames = new Dictionary<string, DDTextureFrame>();

	DDTexture _nullTexture = null;
	public DDTexture NullTexture { get { return _nullTexture ?? (_nullTexture = new DDTextureFriend()); } }

	DDTextureFrame _nullTextureFrame = null;
	public DDTextureFrame NullTextureFrame { get { return _nullTextureFrame ?? (_nullTextureFrame = new DDTextureFrame(NullTexture)); } }

    private DDTextureManager()
    { }
	
	public bool TestNamedTexture(string name)
	{
		if (name == null)
			return false;
		string key = System.IO.Path.GetFileNameWithoutExtension(name.ToLower());
		return _namedTextures.ContainsKey(key);
	}
	
    internal DDTexture GetNamedTexture(string name)
    {
        string key = System.IO.Path.GetFileNameWithoutExtension(name.ToLower());
        if (!_namedTextures.ContainsKey(key))
        {
            _namedTextures[key] = new DDTextureFriend(key);
        }
        return _namedTextures[key];
    }

    internal DDTextureFrame GetNamedTextureFrame(string name)
    {
        string key = System.IO.Path.GetFileNameWithoutExtension(name.ToLower());
        if (!_namedTextureFrames.ContainsKey(key))
        {
            _namedTextureFrames[key] = new DDTextureFrame(GetNamedTexture(key));
        }
        return _namedTextureFrames[key];
    }
	
	internal DDTexture CreateTexture(byte [] bytes = null)
	{
        var tex = new DDTextureFriend();
		_textures.Add(tex);
        if (bytes != null)
            tex.SetData(bytes);
		return tex;
	}
	
	internal DDTexture CreateTexture(string name, byte [] bytes)
	{
		string key = System.IO.Path.GetFileNameWithoutExtension(name.ToLower());
        var tex = new DDTextureFriend(key, bytes);
		_namedTextures[key] = tex;
		return tex;
	}
	
	public void PurgeUnusedTexture()
	{
		var nodes = new Queue<DDNode>();
		nodes.Enqueue(DDDirector.Instance.Scene);
		
		var usedTextures = new HashSet<DDTexture>();
		while (nodes.Count > 0)
		{
			var node = nodes.Dequeue();
			var tex = DDNode._GetUsedTexture(node);
			if (tex != null)
				usedTextures.Add(tex);
			foreach (var child in node.Children)
				nodes.Enqueue(child);
		}
		
		if (DDFont.Default != null)
		{
			usedTextures.Add(DDFont.Default.Texture);
		}

        var newNamedTextures = new Dictionary<string, DDTextureFriend>();
		var newNamedTextureFrames = new Dictionary<string, DDTextureFrame>();
		var newTextures = new List<DDTexture>();
		
		var unusedTextures = new HashSet<DDTexture>();
		foreach (var kv in _namedTextures)
		{
			if (usedTextures.Contains(kv.Value))
			{
				newNamedTextures[kv.Key] = kv.Value;
			}
			else
			{
				unusedTextures.Add(kv.Value);
			}
		}
		
		foreach (var kv in _namedTextureFrames)
		{
			if (usedTextures.Contains(kv.Value.Texture))
			{
				newNamedTextureFrames[kv.Key] = kv.Value;
			}
			else
			{
				unusedTextures.Add(kv.Value.Texture);
			}
		}
		
		foreach (var tex in _textures)
		{
			if (usedTextures.Contains(tex))
			{
				newTextures.Add(tex);
			}
			else
			{
				unusedTextures.Add(tex);
			}
		}
		

		
		_namedTextures = newNamedTextures;
		_namedTextureFrames = newNamedTextureFrames;
		_textures = newTextures;
		
		foreach (var tex in unusedTextures)
		{
			tex.Unload();
		}

		DDDebug.Log("Unload ", unusedTextures.Count, " textures: ", string.Join(",", unusedTextures.Where(it => !string.IsNullOrEmpty(it.Name)).Select(it => it.Name).ToArray()));
		
	}
	
    internal void PreloadNamedTexture(params string [] names)
    {
        foreach (var name in names)
            GetNamedTexture(name);
    }

    internal void PreloadAtlasCocos2d(string name)
    {
        // http://gamedev.stackexchange.com/questions/18758/in-cocos2ds-plist-output-what-are-offset-colorsourcerect-and-these-other
        var plist = DDXml.DecodePlist(DDFile.GetBytes(name));
        
        var metadata = DDXml.GetDictionary(plist, "metadata");
        var textureFile = DDXml.GetString(metadata, "realTextureFileName", null);
        var texSize = DDXml.GetPlistVector(metadata, "size");

        var texture = GetNamedTexture(textureFile);
        var frames = DDXml.GetDictionary(plist, "frames");
        foreach (string key1 in frames.Keys)
        {
            Dictionary<object, object> frame = DDXml.GetDictionary(frames, key1);

            string key = System.IO.Path.GetFileNameWithoutExtension(key1.ToLower());

            var uv = DDXml.GetPlistRectangle(frame, "frame");
            var sz = DDXml.GetPlistVector(frame, "sourceSize");
            var rt = DDXml.GetPlistRectangle(frame, "sourceColorRect");

#if DD_PLATFORM_UNITY3D
            uv = new DDRectangle(
                uv.Left / texSize.Width, 1f - uv.Bottom / texSize.Height,
                uv.Right / texSize.Width, 1f - uv.Top / texSize.Height);
            var trimed = new DDRectangle(
                rt.Left / sz.Width, 1f - rt.Bottom / sz.Height,
                rt.Right / sz.Width, 1f - rt.Top / sz.Height);
#else
            uv = new DDRectangle(
                uv.Left / texSize.Width, uv.Bottom / texSize.Height,
                uv.Right / texSize.Width, uv.Top / texSize.Height);
            var trimed = new DDRectangle(
                rt.Left / sz.Width, rt.Bottom / sz.Height,
                rt.Right / sz.Width, rt.Top / sz.Height);
#endif

            _namedTextureFrames[key] = new DDTextureFrame(texture, uv, trimed);
        }
    }
	
	internal void PreloadAtlasCocos2dFormat3(string name)
    {
        // http://gamedev.stackexchange.com/questions/18758/in-cocos2ds-plist-output-what-are-offset-colorsourcerect-and-these-other
        var plist = DDXml.DecodePlist(DDFile.GetBytes(name));
        
        var metadata = DDXml.GetDictionary(plist, "metadata");
        var textureFile = DDXml.GetString(metadata, "textureFileName", null);
        var texSize = DDXml.GetPlistVector(metadata, "size");
		
        var texture = GetNamedTexture(textureFile);
        var frames = DDXml.GetDictionary(plist, "frames");
        foreach (string key1 in frames.Keys)
        {
            Dictionary<object, object> frame = DDXml.GetDictionary(frames, key1);

            string key = System.IO.Path.GetFileNameWithoutExtension(key1.ToLower());

            var uv = DDXml.GetPlistRectangle(frame, "textureRect");
            var sz = DDXml.GetPlistVector(frame, "spriteSourceSize");
            var rt = DDXml.GetPlistRectangle(frame, "spriteColorRect");

            uv = new DDRectangle(
                uv.Left / texSize.Width, 1f - uv.Bottom / texSize.Height,
                uv.Right / texSize.Width, 1f - uv.Top / texSize.Height);
            var trimed = new DDRectangle(
                rt.Left / sz.Width, 1f - rt.Bottom / sz.Height,
                rt.Right / sz.Width, 1f - rt.Top / sz.Height);

            _namedTextureFrames[key] = new DDTextureFrame(texture, uv, trimed);
        }
    }

    internal void PreloadAtlasFlashCs6(string atlasName)
    {
        var xml = DDXml.DecodeXml(DDFile.GetBytes(atlasName));
        var imageName = Path.GetFileNameWithoutExtension(xml.AllNodes.First(it => it.Name == "TextureAtlas").GetAttribute("imagePath", "?"));
        var tex = GetNamedTexture(imageName);
        foreach (var sub in xml.AllNodes.Where(it => it.Name == "SubTexture"))
        {
            string name = Path.GetFileNameWithoutExtension(sub.GetAttribute("name", "?"));
            int x = int.Parse(sub.GetAttribute("x", "0"), NumberStyles.Any);
            int y = int.Parse(sub.GetAttribute("y", "0"), NumberStyles.Any);
            int width = int.Parse(sub.GetAttribute("width", "0"), NumberStyles.Any);
            int height = int.Parse(sub.GetAttribute("height", "0"), NumberStyles.Any);
            //tex.Size;

            var p1 = new DDVector(x, y);
            var p2 = new DDVector(x + width, y + height);

            var uv = new DDRectangle(p1 / tex.Size, p2 / tex.Size);

#if DD_PLATFORM_UNITY3D
            //uv = new DDRectangle(uv.p1.X, 1f - uv.p1.Y, uv.p2.X, 1f - uv.p2.Y);
            uv = new DDRectangle(uv.Left, 1f - uv.Bottom, uv.Right, 1f - uv.Top);
#else
            //var uv = new DDRectangle(p1 / tex.Size, p2 / tex.Size);
            //uv.p1 = uv.p1 / texSize;
            //uv.p2 = uv.p2 / texSize;
#endif

            _namedTextureFrames[name] = new DDTextureFrame(tex, uv, new DDRectangle(0, 0, 1, 1));
        }
    }
}