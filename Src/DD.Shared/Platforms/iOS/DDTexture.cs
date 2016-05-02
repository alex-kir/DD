//
//  DDTexture_iOS.cs
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


#if DD_PLATFORM_IOS

using System;
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;

using All1 = OpenTK.Graphics.ES11.All;
using GL1 = OpenTK.Graphics.ES11.GL;
//using GL2 = OpenTK.Graphics.ES20.GL;
//using MonoTouch.OpenGLES;
//using OpenTK.Graphics.ES20;
//using OpenTK;

public partial class DDTexture
{
	int _textureId = -1;
    public int TextureId { get { return _textureId; } }
	float[] _vertexFloats2;
//	float[] _vertexFloats3;
	float[] _textureFloats;

	public DDTexture(string name)
	{
        var data = NSData.FromArray(DDFile.GetBytes(name));

		UIImage image1 = UIImage.LoadFromData(data);
		if (image1 == null)
			DDDebug.Log(@"Do real error checking here");

		Size = image1.Size;

		GL1.GenTextures(1, out _textureId);
		GL1.BindTexture(All1.Texture2D, _textureId);

		GL1.TexParameter(All1.Texture2D, All1.TextureMinFilter, (int)All1.Linear);//All.Nearest);
		GL1.TexParameter(All1.Texture2D, All1.TextureMagFilter, (int)All1.Linear);
		GL1.TexParameter(All1.Texture2D, All1.TextureWrapS, (int)All1.ClampToEdge);//Repeat
		GL1.TexParameter(All1.Texture2D, All1.TextureWrapT, (int)All1.ClampToEdge);//ClampToEdge


		CGImage image = image1.CGImage;
		byte[] pixels = new byte[image.Width * image.Height * 4];
		var context = new CGBitmapContext(pixels, image.Width, image.Height, 8, image.Width * 4, image.ColorSpace, CGImageAlphaInfo.PremultipliedLast);
		context.DrawImage(new RectangleF(0, 0, image.Width, image.Height), image);
        GL1.TexImage2D(All1.Texture2D, 0, (int)All1.Rgba,
            (int)image.Width, (int)image.Height, 0,
            All1.Rgba, All1.UnsignedByte, pixels);

//        _vertexFloats2 = new float[] {
//			0,          0,          
//			0,          Size.Height,
//			Size.Width, Size.Height,
//			Size.Width, 0,          
//		};
//
//		float pixw = 1 / Size.Width;
//		float pixh = 1 / Size.Height;
//		
//		_textureFloats = new float[] {
//			0 + pixw, 1 - pixh,
//			0 + pixw, 0 + pixh,
//			1 - pixw, 0 + pixh,
//			1 - pixw, 1 - pixh,
//		};
	}

    public DDTexture(string name, byte [] bytes)
    {
    }

    public void Unload()
    {
        throw new NotImplementedException();
    }

    public void SetData(byte [] bytes)
    {
        throw new NotImplementedException();
        //        this.Texture.LoadImage(bytes);
        //        this.Size = new DDVector(this.Texture.width, this.Texture.height);
    }
//
//	public void Draw (DDColor color)
//	{
//		if (_vertexFloats2 == null || _textureFloats == null)
//			return;
//
//		var _colorVertex = new float[] {
//			color.R, color.G, color.B, color.A,
//			color.R, color.G, color.B, color.A,
//			color.R, color.G, color.B, color.A,
//			color.R, color.G, color.B, color.A,
//		};
//
//		//if (!GL.IsTexture(_textureId))
//		//{
//		//    reload texture;
//		//}
//		
//		GL1.BindTexture(All1.Texture2D, _textureId);
//		GL1.CullFace(All1.FrontAndBack);
//		GL1.FrontFace(All1.Cw);
//
//		GL1.Disable(All1.DepthTest);
//		GL1.Disable(All1.Lighting);
//		GL1.Disable(All1.AlphaTest);
//		
//		GL1.BlendFunc(All1.One, All1.OneMinusSrcAlpha);
//		GL1.Enable(All1.Blend);
//
//
//		GL1.EnableClientState (All1.VertexArray);
//		GL1.EnableClientState (All1.ColorArray);
//		GL1.EnableClientState(All1.TextureCoordArray);
//
//		GL1.VertexPointer (2, All1.Float, 0, _vertexFloats2);
//		GL1.ColorPointer (4, All1.Float, 0, _colorVertex);
//		GL1.TexCoordPointer(2, All1.Float, 0, _textureFloats);
//
//
//		GL1.DrawArrays(All1.TriangleFan, 0, _vertexFloats2.Length / 2);
//		
//		GL1.DisableClientState(All1.VertexArray);
//		GL1.DisableClientState(All1.ColorArray);
//		GL1.DisableClientState(All1.TextureCoordArray);
//	}
}

#endif