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

using OpenTK.Graphics.ES20;
using Android.Opengl;
using System.IO;

public partial class DDTexture
{
    int _textureId = -1;
    public int TextureId { get { return _textureId; } }
    //float[] _vertexFloats;
    //float[] _textureFloats;

//    public DDTexture()
//    {
//        Name = null;
//        _textureId = -1;
//        Size = new DDVector(4, 4);
//    }

    public DDTexture(string name)
    {
        using (var measure = DDDebug.Measure("DDTexture load")) {
            
            try {
                byte[] bytes = DDFile.GetBytes(name);
                //GL.CompressedTexImage2D(All.Texture2D, 0, All., );

//            try
//            {
//                var o = new BitmapFactory.Options();
//                o.InJustDecodeBounds = true;
//                var b1 = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, o);
//
//                var b2 = Bitmap.CreateBitmap(b1.Width, b1.Height, Bitmap.Config.Argb8888);
//                b2.
//
////                var b = Bitmap.CreateBitmap();
//
////                opts.InMutable = true;
////                opts.InBitmap = b;
////                opts.InBitmap.SetPremultiplied(false);
////                opts.InPremultiplied = false; // to remove dirty contour;
//            }
//            catch(Exception ex)
//            {
//                ex.LogException();
//            }
                var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, null);
//            bitmap.SetPremultiplied(false);
//            try
//            {
//                int width = bitmap.Width;
//                int height = bitmap.Height;
//
//                int [] pixels = new int[width * height];
//                int [] pixelsSave = new int[width * height];
//                bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
//
//                for (int x = 0; x < width; x++)
//                {
//                    for (int y = 0; y < height; y++)
//                    {
//                        int i = x + y * width;
//                        pixelsSave[i] = pixels[i];
//                        Color c = new Color(pixels[i]);
//                        if (c.A > 0)
//                            continue;
//
//                        int r = 0;
//                        int g = 0;
//                        int b = 0;
//                        int count = 0;
//
//                        for (int xx = x - 2; xx <= x + 2; xx++) {
//                            for (int yy = y - 2; yy <= y + 2; yy++) {
//                                if (0 <= xx && xx < width && 0 <= yy && yy < height) {
//                                    var cc = new Color(pixels[xx + yy * width]);
//                                    if (cc.A == 0)
//                                        continue;
//                                    r += cc.R;
//                                    g += cc.G;
//                                    b += cc.B;
//                                    count++;
//                                }
//                            }
//                        }
//
//                        if (count == 0)
//                            continue;
//                        //Console.WriteLine(r + ", " + g + ", " + b + " / " + count);
//                        pixelsSave[i] = new Color(r / count, g / count, b / count, 0).ToArgb();
//                        //pixelsSave[x, y] = Color.FromArgb(0, 255, 0, 0);
//
//                    }
//                }
//                bitmap.SetPixels(pixelsSave, 0, width, 0, 0, width, height);
//            }
//            catch(Exception ex)
//            {
//                ex.LogException();
//            }
//            var c = bitmap.GetPixel(0, 0);
//            DDDebug.Trace(name, c.ToString("x8"));
//
//

                //var bitmap = BitmapFactory.DecodeResource(DDDirector.Instance.Activity.Resources, id);
                Size = new DDVector(bitmap.Width, bitmap.Height);

                GL.GenTextures(1, out _textureId);
//            GL.BindTexture(All.Texture2D, _textureId);
                GL.BindTexture(TextureTarget.Texture2D, _textureId);

//            GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.Linear);//All.Nearest);
//            GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);
//            GL.TexParameter(All.Texture2D, All.TextureWrapS, (int)All.ClampToEdge);//Repeat
//            GL.TexParameter(All.Texture2D, All.TextureWrapT, (int)All.ClampToEdge);//ClampToEdge

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);//All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);//Repeat
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);//ClampToEdge

//            GL.TexImage2D(
//            GLUtils.TexImage2D((int)All.Texture2D, 0, bitmap, 0);

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
                // GL.TexImage2D(All.Texture2D, 0, (int)All.Rgba, bitmap.Width, bitmap.Height, 0, All.Rgba, All.UnsignedByte, pixels); 
                try {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    int[] pixels = new int[width * height];

//                DDDebug.Trace("start");

                    bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

//                DDDebug.Trace("1");

                    for (int i = 0, n = width * height; i < n; i++) {
                        int c = pixels[i];
                        if (c == 0)
                            continue;

                        int a = (c >> 24) & 0xff;
                        int r = (c >> 16) & 0xff;
                        int g = (c >> 8) & 0xff;
                        int b = (c) & 0xff;
                        int c1 = (a << 24) | (b << 16) | (g << 8) | (r);
                        pixels[i] = c1;
                    }

//                DDDebug.Trace("2");

                    for (int y = 0; y < height; y++) {
                        for (int x1 = 0, x2 = 1; x2 < width; x1++, x2++) {
                            int i1 = x1 + y * width;
                            int i2 = x2 + y * width;
                            int c1 = pixels[i1];
                            int c2 = pixels[i2];

                            int a1 = c1 >> 24;
                            int a2 = c2 >> 24;
                            if ((a1 == 0) && (a2 != 0)) {
                                pixels[i1] = c2 & 0x00ffffff;
                            }

                            if ((a1 != 0) && (a2 == 0)) {
                                pixels[i2] = c1 & 0x00ffffff;
                            }
                        }
                    }

                    for (int x = 0; x < width; x++) {
                        for (int y1 = 0, y2 = 1; y2 < height; y1++, y2++) {
                            int i1 = x + y1 * width;
                            int i2 = x + y2 * width;
                            int c1 = pixels[i1];
                            int c2 = pixels[i2];

                            int a1 = c1 >> 24;
                            int a2 = c2 >> 24;
                            if ((a1 == 0) && (a2 != 0)) {
                                pixels[i1] = c2 & 0x00ffffff;
                            }

                            if ((a1 != 0) && (a2 == 0)) {
                                pixels[i2] = c1 & 0x00ffffff;
                            }
                        }
                    }
//                DDDebug.Trace("3");


                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        bitmap.Width, bitmap.Height, 0,
                        OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, pixels); 

//                DDDebug.Trace("end");
                }
                catch (Exception ex) {
                    ex.LogException();
                    GLUtils.TexImage2D((int)All.Texture2D, 0, bitmap, 0);
                }

                bitmap.Recycle();
                bitmap = null;

            }
            catch (Exception ex) {
                DDDebug.Trace(name);
                ex.LogException();
            }
        }
    }

    public DDTexture(string name, byte [] bytes)
    {
        throw new NotImplementedException();
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