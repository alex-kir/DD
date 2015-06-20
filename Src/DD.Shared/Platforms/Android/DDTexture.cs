//
//  DDTexture_Android.cs
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
using System.Text;
using Android.Graphics;

using OpenTK.Graphics.ES11;
using Android.Opengl;
using System.IO;

public partial class DDTexture
{
    int _textureId = -1;
    public int TextureId { get { return _textureId; } }
    //float[] _vertexFloats;
    //float[] _textureFloats;

    public DDTexture()
    {
        Name = null;
        _textureId = -1;
        Size = new DDVector(4, 4);
    }

    public DDTexture(string name)
    {
        try
        {
            byte[] bytes = DDFile.GetBytes(name);
            //GL.CompressedTexImage2D(All.Texture2D, 0, All., );

            var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            //var bitmap = BitmapFactory.DecodeResource(DDDirector.Instance.Activity.Resources, id);
            Size = new DDVector(bitmap.Width, bitmap.Height);

            GL.GenTextures(1, out _textureId);
            GL.BindTexture(All.Texture2D, _textureId);

            GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.Linear);//All.Nearest);
            GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(All.Texture2D, All.TextureWrapS, (int)All.ClampToEdge);//Repeat
            GL.TexParameter(All.Texture2D, All.TextureWrapT, (int)All.ClampToEdge);//ClampToEdge

            GLUtils.TexImage2D((int)All.Texture2D, 0, bitmap, 0);

            //var pixels = new byte[bitmap.Width * bitmap.Height * 4];
            //for (int x = 0; x < bitmap.Width; x++)
            //{
            //    for (int y = 0; y < bitmap.Height; y++)
            //    {
            //        pixels[x * 4 + y * bitmap.Width * 4 + 0] = (byte)(x % 255);
            //        pixels[x * 4 + y * bitmap.Width * 4 + 1] = (byte)(x % 255);
            //        pixels[x * 4 + y * bitmap.Width * 4 + 2] = (byte)(x % 255);
            //        pixels[x * 4 + y * bitmap.Width * 4 + 3] = (byte)(255);
            //    }
            //}
            //GL.TexImage2D(All.Texture2D, 0, (int)All.Rgba, bitmap.Width, bitmap.Height, 0, All.Rgba, All.UnsignedByte, pixels); 

            bitmap.Recycle();
            bitmap = null;

            //_vertexFloats = new float[] {
            //    0,          0,           0,
            //    0,          Size.Height, 0,
            //    Size.Width, Size.Height, 0,
            //    Size.Width, 0,           0,
            //};
            
            //float pixw = 1 / Size.Width;
            //float pixh = 1 / Size.Height;

            //_textureFloats = new float[] {
            //    0 + pixw, 1 - pixh,
            //    0 + pixw, 0 + pixh,
            //    1 - pixw, 0 + pixh,
            //    1 - pixw, 1 - pixh,
            //};
        }
        catch (Exception ex)
        {
            DDDebug.Log(ex.ToString());
        }
    }

    public DDTexture(string name, byte [] bytes)
    {
    }

    //public void Draw(DDColor color)
    //{
    //    if (_vertexFloats == null || _textureFloats == null)
    //        return;

    //    //if (!GL.IsTexture(_textureId))
    //    //{
    //    //    reload texture;
    //    //}

    //    GL.BindTexture(All.Texture2D, _textureId);
    //    GL.CullFace(All.FrontAndBack);
    //    GL.FrontFace(All.Cw);

    //    GL.VertexPointer(3, All.Float, 0, _vertexFloats);
    //    GL.EnableClientState(All.VertexArray);

    //    GL.TexCoordPointer(2, All.Float, 0, _textureFloats);
    //    GL.EnableClientState(All.TextureCoordArray);

    //    if (color != DDColor.White)
    //    {
    //        GL.EnableClientState(All.ColorArray);
    //        GL.ColorPointer(4, All.Float, 0, new float[]{
    //            color.R, color.G, color.B, color.A,
    //            color.R, color.G, color.B, color.A,
    //            color.R, color.G, color.B, color.A,
    //            color.R, color.G, color.B, color.A,
    //        });
    //    }

    //    //GL.BlendFunc(All.One, All.OneMinusSrcAlpha);
    //    //GL.Enable(All.Blend);

    //    //GL.EnableClientState(All.ColorArray);
    //    //GL.ColorPointer(4, All.Byte, 0, new byte[] {
    //    //    128, 255, 255, 128,
    //    //    128, 255, 255, 255,
    //    //    255, 128, 255, 128,
    //    //    255, 255, 128, 255
    //    //});

    //    GL.DrawArrays(All.TriangleFan, 0, _vertexFloats.Length / 3);

    //    GL.DisableClientState(All.VertexArray);
    //    GL.DisableClientState(All.TextureCoordArray);
    //    GL.DisableClientState(All.ColorArray);
    //}

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
}



#endif