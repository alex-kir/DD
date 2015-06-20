//
//  DDFont.cs
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
using System.Text.RegularExpressions;
using System.IO;

// For support russian code page 1251 copy dll in Asset folder
// C:\Program Files\Unity\Editor\Data\Mono\lib\mono\unity\I18N.Other.dll
public class DDFont
{
    public static DDFont Default { get; set; }

    class DDCharInfo
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;
        public int XAdvance;

        public DDCharInfo(int x, int y, int width, int height, int xoffset, int yoffset, int xadvance)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.XOffset = xoffset;
            this.YOffset = yoffset;
            this.XAdvance = xadvance;
        }
    }

    Dictionary<char, DDCharInfo> _chars = new Dictionary<char, DDCharInfo>();
    public DDTexture Texture { get; private set; }
    string _textureName;
	private int _scaleH;
	private int _scaleW;
    private float _lineHeight;
    public float LineHeight { get { return _lineHeight; } }
	private float _upperBound;
	private float _lowerBound;
    System.Text.Encoding _encoding = null; // !!! encoding of chars in BM file !!!

    public DDFont(string fontFileName, System.Text.Encoding encoding = null, float upperBound = 1.0f, float lowerBound = 0.0f)
    {
        _encoding = encoding;
  
        _upperBound = upperBound;
		_lowerBound = lowerBound;
		
        ParseConfigFile(fontFileName);
        Texture = DDTextureManager.Instance.GetNamedTexture(_textureName);
#if DD_PLATFORM_UNITY3D
		FlipCharsInfo();
#endif
        if (Default == null)
            Default = this;
    }

    private void ParseConfigFile(string fileName)
    {
        var bytes = DDFile.GetBytes(fileName);
        var reader = new StreamReader(new MemoryStream(bytes));
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            if (line.StartsWith("char id"))
            {
                this.ParseCharacterDefinition(line);
            }
            else if (line.StartsWith("common lineHeight"))
            {
                _lineHeight = float.Parse(DDXml.StringBetween(line, "lineHeight=", " ", null)) * _upperBound;
				_scaleW = int.Parse(DDXml.StringBetween(line, "scaleW=", " ", null));
				_scaleH = int.Parse(DDXml.StringBetween(line, "scaleH=", " ", null));
            }
            else if (line.StartsWith("page id"))
            {
                _textureName = DDXml.StringBetween(line, "file=\"", "\"", null);
            }
        }
    }

    private void ParseCharacterDefinition(string line)
    {
        string id = DDXml.StringBetween(line, "char id=", " ", null);
		
        char ch = _encoding != null ? _encoding.GetChars(new byte[] { (byte)int.Parse(id) })[0] : (char)int.Parse(id);
		
        int x = int.Parse(DDXml.StringBetween(line, "x=", " ", null));
        int y = int.Parse(DDXml.StringBetween(line, "y=", " ", null));
        int width = int.Parse(DDXml.StringBetween(line, "width=", " ", null));
        int height = int.Parse(DDXml.StringBetween(line, "height=", " ", null));
        int xoffset = int.Parse(DDXml.StringBetween(line, "xoffset=", " ", null));
        int yoffset = int.Parse(DDXml.StringBetween(line, "yoffset=", " ", null));
        int xadvance = int.Parse(DDXml.StringBetween(line, "xadvance=", " ", null));

        _chars[ch] = new DDCharInfo(x, y, width, height, xoffset, yoffset, xadvance);
    }
	
	void FlipCharsInfo()
	{
//		int h = (int)Texture.Size.Height;
		int h = _scaleH;
		foreach (var info in _chars.Values)
		{
			info.Y = h - info.Y - info.Height;
		}
	}
	
    public DDVector MeasureString(string text)
    {
        float xx = 0;
        float x = 0;
        float y = _lineHeight;
        foreach (var ch in text)
        {
            if (ch == '\n')
            {
                x = 0;
                y += _lineHeight;
                continue;
            }
            if (!_chars.ContainsKey(ch))
                continue;

            DDCharInfo info = _chars[ch];

            x += info.XAdvance;
            xx = DDMath.Max(x, xx);
        }
        return new DDVector(xx, y);
    }
	
	public DDRenderer.Quad[] BuildQuads(string text)
	{
		var quads = new List<DDRenderer.Quad>();
//		var tsz = Texture.Size;
		var tsz = new DDVector (_scaleW, _scaleH);
		
        var leftTop = MeasureString(text) * DDVector.LeftTop;
        float x = leftTop.X;
        float y = leftTop.Y + _lineHeight / _upperBound * _lowerBound;
        foreach (var ch in text)
        {	
            if (ch == '\n')
            {
                x = 0;
                y -= _lineHeight;
                continue;
            }
            if (!_chars.ContainsKey(ch))
                continue;

            DDCharInfo info = _chars[ch];
            DDRectangle uv = new DDRectangle(info.X / tsz.Width, info.Y / tsz.Height,
                info.X / tsz.Width + info.Width / tsz.Width, info.Y / tsz.Height + info.Height / tsz.Height);
            var o = new DDVector(x + info.XOffset, y - info.YOffset - info.Height);
            var mm = new DDMatrix(o, DDVector.Zero, DDVector.One);
			
			var quad = new DDRenderer.Quad();
			
            quad.xy1 = mm.TransformPoint(new DDVector(0, 0));
            quad.xy2 = mm.TransformPoint(new DDVector(0, info.Height));
            quad.xy3 = mm.TransformPoint(new DDVector(info.Width, info.Height));
            quad.xy4 = mm.TransformPoint(new DDVector(info.Width, 0));

            quad.uv1 = uv.LeftBottom;
            quad.uv2 = uv.LeftTop;
            quad.uv3 = uv.RightTop;
            quad.uv4 = uv.RightBottom;
			
			quad.white_color1 = DDColor.White;
			quad.white_color2 = DDColor.White;
			quad.white_color3 = DDColor.White;
			quad.white_color4 = DDColor.White;
			
			quad.black_color1 = DDColor.Black;
			quad.black_color2 = DDColor.Black;
			quad.black_color3 = DDColor.Black;
			quad.black_color4 = DDColor.Black;
			
			quads.Add(quad);
			
            x += info.XAdvance;
        }
		return quads.ToArray();
	}
}