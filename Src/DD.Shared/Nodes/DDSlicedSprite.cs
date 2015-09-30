//
//  DDSlicedSprite.cs
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

public class DDSlicedSprite : DDNode
{
	DDSprite _leftBottom;
	DDSprite _leftMiddle;
	DDSprite _leftTop;
	
	DDSprite _centerBottom;
	DDSprite _centerMiddle;
	DDSprite _centerTop;

	DDSprite _rightBottom;
	DDSprite _rightMiddle;
	DDSprite _rightTop;

    public DDSlicedSprite(DDVector size)
        : this(DDTextureManager.Instance.NullTextureFrame, size.Width, size.Height)
    {
    }

    public DDSlicedSprite(float width, float height)
        : this(DDTextureManager.Instance.NullTextureFrame, width, height)
    {
    }

    public DDSlicedSprite(string texName, DDVector size)
        : this(texName, size.Width, size.Height)
    { }
	
    public DDSlicedSprite(string texName, DDVector size, DDVector border)
        : this(texName, size.Width, size.Height, border.Width, border.Height)
    { }

	public DDSlicedSprite(string texName, float width, float height, float leftRigth = 0.4f, float bottomTop = 0.4f)
        : this(DDTextureManager.Instance.GetNamedTextureFrame(texName), width, height, leftRigth, bottomTop)
	{ }

    public DDSlicedSprite(DDTextureFrame frame, float width, float height, float leftRigth = 0.4f, float bottomTop = 0.4f)
    {
		float left = leftRigth;
		float right = 1 - leftRigth;
		float bottom = bottomTop;
		float top = 1 - bottomTop;
		
		
		var _frame = frame;

		var minSizeX = _frame.Size.Width * leftRigth * 2;
		var minSizeY = _frame.Size.Height * bottomTop * 2;
		if (minSizeX > width || minSizeY > height)
			DDDebug.Log("DDSlicedSprite " + _frame.Texture.Name + " width or height too small");
		Size = new DDVector(DDMath.Max(minSizeX, width), DDMath.Max(minSizeY, height));

		_leftBottom = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(0, 0,      left, bottom))));
		_leftMiddle = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(0, bottom, left, top))));
		_leftTop    = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(0, top,    left, 1))));

		_centerBottom = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(left, 0,      right, bottom))));
		_centerMiddle = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(left, bottom, right, top))));
		_centerTop    = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(left, top,    right, 1))));

		_rightBottom = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(right, 0,      1, bottom))));
		_rightMiddle = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(right, bottom, 1, top))));
		_rightTop    = Children.Add(new DDSprite(_frame.SubFrame(new DDRectangle(right, top,    1, 1))));
		
		_leftBottom.AnchorPoint = DDVector.LeftBottom;
		_leftBottom.Position    = DDVector.LeftBottom * Size;
		_leftMiddle.AnchorPoint = DDVector.LeftMiddle;
		_leftMiddle.Position    = DDVector.LeftMiddle * Size;
		_leftTop   .AnchorPoint = DDVector.LeftTop   ;
		_leftTop   .Position    = DDVector.LeftTop    * Size;
		
		_centerBottom.AnchorPoint = DDVector.CenterBottom;
		_centerBottom.Position    = DDVector.CenterBottom * Size;
		_centerMiddle.AnchorPoint = DDVector.CenterMiddle;
		_centerMiddle.Position    = DDVector.CenterMiddle * Size;
		_centerTop   .AnchorPoint = DDVector.CenterTop   ;
		_centerTop   .Position    = DDVector.CenterTop    * Size;

		_rightBottom.AnchorPoint = DDVector.RightBottom;
		_rightBottom.Position    = DDVector.RightBottom * Size;
		_rightMiddle.AnchorPoint = DDVector.RightMiddle;
		_rightMiddle.Position    = DDVector.RightMiddle * Size;
		_rightTop   .AnchorPoint = DDVector.RightTop   ;
		_rightTop   .Position    = DDVector.RightTop    * Size;
		
		
		_leftMiddle.ScaleY = (Size.Height - _leftTop.Size.Height  - _leftBottom.Size.Height) / _leftMiddle.Size.Height;
		_rightMiddle.ScaleY = (Size.Height - _rightTop.Size.Height  - _rightBottom.Size.Height) / _rightMiddle.Size.Height;

		_centerBottom.ScaleX = (Size.Width - _leftBottom.Size.Width  - _rightBottom.Size.Width) / _centerBottom.Size.Width;
		_centerTop.ScaleX = (Size.Width - _leftTop.Size.Width  - _rightTop.Size.Width) / _centerTop.Size.Width;
		
		_centerMiddle.ScaleXY = (Size - _leftBottom.Size - _rightTop.Size) / _centerMiddle.Size;
	}
}