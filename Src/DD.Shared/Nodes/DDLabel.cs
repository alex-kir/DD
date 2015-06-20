//
//  DDLabel.cs
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
using System;
using System.Linq;

public class DDLabel : DDNode
{
    private DDFont _font;
    public DDFont Font { get { return _font; } }
    private string _text = null;
    public string Text { get { return _text; } set { SetText(value); } }
	private DDRenderer.Quad[] _quads = null;

    public DDLabel(string text)
        : this(DDFont.Default, text)
    { }

    public DDLabel(DDFont font, string text)
    {
        _font = font;
        AnchorPoint = DDVector.CenterMiddle;
		SetText(text);
	}
	
	public void SetText(string text)
	{
        if (_text != text)
        {
            _text = text;
            Size = _font.MeasureString(_text);
			_quads = _font.BuildQuads(_text);
        }
	}

    public override void Draw(DDRenderer renderer)
    {
		var m = NodeToWorldTransform();
		var wc = CombinedColor;
		var bc = CombinedColorBlack;
		foreach (var quad in _quads)
		{
			quad.Matrix = m;
			quad.white_color1 = wc;
			quad.white_color2 = wc;
			quad.white_color3 = wc;
			quad.white_color4 = wc;
			
			quad.black_color1 = bc;
			quad.black_color2 = bc;
			quad.black_color3 = bc;
			quad.black_color4 = bc;

            renderer.DrawQuad(_font.Texture, quad);
		}
    }
	
	protected override DDTexture GetUsedTexture()
	{
		return _font.Texture;
	}
}