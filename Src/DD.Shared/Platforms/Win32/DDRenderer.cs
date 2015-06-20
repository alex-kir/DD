//
//  DDRenderer_Win32.cs
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
#if DD_PLATFORM_WIN32

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;

public partial class DDRenderer
{
    private Graphics g;

    public DDRenderer(Graphics G)
    {
        this.g = G;
    }

    internal void BeginScene()
    {
        
    }

    internal void EndScene()
    {
        
    }

    internal void DrawQuad(DDTexture texture, DDRectangle frame, DDMatrix matrix, DDColor color)
    {
        var gstate = g.Save();
        float[] ff = matrix;
        var m = new Matrix(
            ff[00], ff[01],
            ff[04], ff[05],
            ff[12], ff[13]
            );
        g.MultiplyTransform(m);

        var bmp = texture.Bmp;

        try
        {
            float l = (frame.Left * bmp.Width);
            float t = (frame.Top * bmp.Height);
            float r = (frame.Right * bmp.Width);
            float b = (frame.Bottom * bmp.Height);

            float w = (frame.Size.Width * bmp.Width);
            float h = (frame.Size.Height * bmp.Height);

            var dst = new RectangleF(0, 0, w, h);
            var src = RectangleF.FromLTRB(l, t, r, b);

            if (color == DDColor.White)
            {
                g.DrawImage(bmp, dst, src, GraphicsUnit.Pixel);
            }
            else
            {
                var colorMatrix = new ColorMatrix();
                colorMatrix.Matrix00 = color.R;
                colorMatrix.Matrix11 = color.G;
                colorMatrix.Matrix22 = color.B;
                colorMatrix.Matrix33 = color.A;

                var imageAttrs = new ImageAttributes();

                imageAttrs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                var dst0 = new Rectangle((int)dst.X, (int)dst.Y, (int)dst.Width, (int)dst.Height);
                g.DrawImage(bmp, dst0, src.X, src.Y, src.Width, src.Height,
                    GraphicsUnit.Pixel, imageAttrs);
            }
        }
        catch (Exception) { }

        g.Restore(gstate);
    }
}

#endif