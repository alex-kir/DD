//
//  DDTextureFrame.cs
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

public class DDTextureFrame
{
    public DDTexture Texture { get; private set; }
    private DDRectangle uv;
    private DDRectangle trimming;

    public DDVector Size { get; private set; }
	
    public DDTextureFrame(DDTexture texture)
        : this(texture, new DDRectangle(0, 0, 1, 1), new DDRectangle(0, 0, 1, 1))
    {

    }

    public DDTextureFrame(DDTexture texture, DDRectangle uv, DDRectangle trimming)
    {
        this.Texture = texture;
        this.uv = uv;
        this.trimming = trimming;
        this.Size = this.Texture.Size * this.uv.Size / this.trimming.Size;
	}
	
	public DDRenderer.Quad BuildQuad()
	{
		var quad = new DDRenderer.Quad();

        quad.xy1 = Size * trimming.LeftBottom;
        quad.xy2 = Size * trimming.LeftTop;
        quad.xy3 = Size * trimming.RightTop;
        quad.xy4 = Size * trimming.RightBottom;
#if DD_PLATFORM_ANDROID
        quad.uv1 = uv.LeftTop;
        quad.uv2 = uv.LeftBottom;
        quad.uv3 = uv.RightBottom;
        quad.uv4 = uv.RightTop;
#else
        quad.uv1 = uv.LeftBottom;
        quad.uv2 = uv.LeftTop;
        quad.uv3 = uv.RightTop;
        quad.uv4 = uv.RightBottom;
#endif
		return quad;
    }

    internal DDVector GetVertexXY(DDVector onQuad)
    {
        var leftBottom = Size * this.trimming.LeftBottom;
        var rightTop = Size * this.trimming.RightTop;

        return new DDVector(DDMath.Lerp(leftBottom.X, rightTop.X, onQuad.X), DDMath.Lerp(leftBottom.Y, rightTop.Y, onQuad.Y));
    }

    internal DDVector GetVertexUV(DDVector onQuad)
    {
        return new DDVector(DDMath.Lerp(uv.Left, uv.Right, onQuad.X), DDMath.Lerp(uv.Bottom, uv.Top, onQuad.Y));
    }
	
    public DDTextureFrame SubFrame(DDRectangle frame)
    {
        if (frame.Intersection(trimming).IsEmpty)
        {
            return null;
        }
        else
        {
            var frameSizeOnTexture = uv.Size / trimming.Size;

            var frameLeftBottomOnTexture = uv.LeftBottom - this.trimming.LeftBottom * frameSizeOnTexture;

            var frameOnTexture = new DDRectangle(frameLeftBottomOnTexture, frameLeftBottomOnTexture + frameSizeOnTexture);

            var frameOnTexture1 = new DDRectangle(
                frameOnTexture.LeftBottom + frameOnTexture.Size * frame.LeftBottom,
                frameOnTexture.LeftBottom + frameOnTexture.Size * frame.RightTop);

            var uv1 = uv.Intersection(frameOnTexture1);

            var trimming1 = new DDRectangle(
                (uv1.LeftBottom - frameOnTexture1.LeftBottom) / frameOnTexture1.Size,
                (uv1.RightTop - frameOnTexture1.LeftBottom) / frameOnTexture1.Size);

            return new DDTextureFrame(Texture, uv1, trimming1);
        }
    }
}