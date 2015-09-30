//
//  DDSprite.cs
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

public class DDSprite : DDNode
{
    protected DDTextureFrame _frame { get; private set; }
    public DDTextureFrame Frame { get { return _frame; } }
    
	private DDRenderer.Quad quad;
	

    public DDSprite()
	{
		SetTextureFrame(DDTextureManager.Instance.NullTextureFrame);
	}

 	public DDSprite(string texName)
	{
		SetTextureFrame(DDTextureManager.Instance.GetNamedTextureFrame(texName));
	}

    public DDSprite(DDTextureFrame frame)
	{
		SetTextureFrame(frame);
	}

    public DDSprite(byte[] bytes)
	{
        SetTextureFrame(new DDTextureFrame(DDTextureManager.Instance.CreateTexture(bytes)));
	}

	public void SetTexture(string texName)
	{
		SetTextureFrame(DDTextureManager.Instance.GetNamedTextureFrame(texName));
	}
	
	public void SetTextureFrame(DDTextureFrame frame)
	{
		_frame = frame;
		Size = _frame.Size;
		quad = _frame.BuildQuad();
	}

    public override DDVector Size
    {
        get
        {
            return base.Size;
        }
        set
        {
            if (base.Size != value) {
                base.Size = value;
                if (_frame != null)
                    quad = _frame.BuildQuad(value);
            }
        }
    }
	
    public override void Draw(DDRenderer renderer)
    {
		quad.Matrix = this.NodeToWorldTransform();
		quad.white_color1 = quad.white_color2 = quad.white_color3 = quad.white_color4 = this.CombinedColor;
		quad.black_color1 = quad.black_color2 = quad.black_color3 = quad.black_color4 = this.CombinedColorBlack;
		renderer.DrawQuad(_frame.Texture, quad);
    }

	protected override DDTexture GetUsedTexture()
	{
		return _frame.Texture;
	}
}