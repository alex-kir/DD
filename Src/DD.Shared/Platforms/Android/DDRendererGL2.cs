//
//  DDRenderer_Android.cs
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
using OpenTK.Graphics.ES20;


#if DD_PLATFORM_ANDROID && false
using System;
//using OpenTK.Graphics.ES11;

public class DDRendererGL2 : DDRenderer
{
    DDShader shader;

    public DDRendererGL2()
    {
        
    }

    protected override void DrawQuad2(DDTexture texture, Quad quad)
    {
        if (shader == null)
            shader = new DDShader();

//        if (currentTexture != texture)
//        {
//            EndDrawing();
//            StartDrawing(texture);
//        }

//        GL.Enable(All.Blend);
//        GL.BlendFunc(All.One, All.OneMinusSrcAlpha);

        GL.Enable(EnableCap.Blend);
//        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


        shader.SetMatrix(OpenTK.Matrix4.Identity);
        shader.SetTexture(texture);
        shader.RenderMesh(null, null, null, null, null);
        return;
//        GL.BindTexture(All.Texture2D, texture.TextureId);
//        GL.CullFace(All.FrontAndBack);
//        GL.FrontFace(All.Cw);

        var vertices = new float[] {
            quad._xy1.X, quad._xy1.Y, 0,
            quad._xy2.X, quad._xy2.Y, 0,
            quad._xy3.X, quad._xy3.Y, 0,
            quad._xy4.X, quad._xy4.Y, 0,
        };

        var uvs = new float[] {
            quad.uv1.X, quad.uv1.Y,
            quad.uv2.X, quad.uv2.Y,
            quad.uv3.X, quad.uv3.Y,
            quad.uv4.X, quad.uv4.Y,
        };

//        GL.VertexPointer(3, All.Float, 0, vertices);
//        GL.EnableClientState(All.VertexArray);
//
//        GL.TexCoordPointer(2, All.Float, 0, uvs);
//        GL.EnableClientState(All.TextureCoordArray);
//
//        GL.EnableClientState(All.ColorArray);
//        GL.ColorPointer(4, All.Float, 0, new float[]{
//            quad.white_color1.R, quad.white_color1.G, quad.white_color1.B, quad.white_color1.A,
//            quad.white_color2.R, quad.white_color2.G, quad.white_color2.B, quad.white_color2.A,
//            quad.white_color3.R, quad.white_color3.G, quad.white_color3.B, quad.white_color3.A,
//            quad.white_color4.R, quad.white_color4.G, quad.white_color4.B, quad.white_color4.A,
//        });
//
//        GL.DrawArrays(All.TriangleFan, 0, 4);
//
//        GL.DisableClientState(All.VertexArray);
//        GL.DisableClientState(All.TextureCoordArray);
//        GL.DisableClientState(All.ColorArray);



        // ----  DON'T CHANGE ORDER OF GL-COMMANDS  ---- //

//        GL.MultiTexCoord3(0, quad.uv1.X, quad.uv1.Y, 0);
//        GL.MultiTexCoord3(1, quad.black_color1.R, quad.black_color1.G, quad.black_color1.B);
//        GL.Color(quad.white_color1);
//        GL.Vertex(quad._xy1);
//
//        GL.MultiTexCoord3(0, quad.uv2.X, quad.uv2.Y, 0);
//        GL.MultiTexCoord3(1, quad.black_color2.R, quad.black_color2.G, quad.black_color2.B);
//        GL.Color(quad.white_color2);
//        GL.Vertex(quad._xy2);
//
//        GL.MultiTexCoord3(0, quad.uv3.X, quad.uv3.Y, 0);
//        GL.MultiTexCoord3(1, quad.black_color3.R, quad.black_color3.G, quad.black_color3.B);
//        GL.Color(quad.white_color3);
//        GL.Vertex(quad._xy3);
//
//        GL.MultiTexCoord3(0, quad.uv4.X, quad.uv4.Y, 0);
//        GL.MultiTexCoord3(1, quad.black_color4.R, quad.black_color4.G, quad.black_color4.B);
//        GL.Color(quad.white_color4);
//        GL.Vertex(quad._xy4);
    }

//    internal void DrawQuad(DDTexture texture, DDRectangle frame, DDMatrix matrix, DDColor color)
//    {
//        //GL.PushMatrix();
//        //GL.MultMatrix(matrix);
//
//        GL.BindTexture(All.Texture2D, texture.TextureId);
//        GL.CullFace(All.FrontAndBack);
//        GL.FrontFace(All.Cw);
//
//        float w = (frame.Size.Width * texture.Size.Width);
//        float h = (frame.Size.Height * texture.Size.Height);
//
//        DDVector v1 = matrix.TransformPoint(new DDVector(0, h));
//        DDVector v2 = matrix.TransformPoint(new DDVector(0, 0));
//        DDVector v3 = matrix.TransformPoint(new DDVector(w, 0));
//        DDVector v4 = matrix.TransformPoint(new DDVector(w, h));
//
//        var vertices = new float[] {
//            v1.X, v1.Y, 0,
//            v2.X, v2.Y, 0,
//            v3.X, v3.Y, 0,
//            v4.X, v4.Y, 0,
//        };
//
//        var uvs = new float[] {
//            frame.Left, frame.Bottom,
//            frame.Left, frame.Top,
//            frame.Right, frame.Top,
//            frame.Right, frame.Bottom
//        };
//
//        GL.VertexPointer(3, All.Float, 0, vertices);
//        GL.EnableClientState(All.VertexArray);
//
//        GL.TexCoordPointer(2, All.Float, 0, uvs);
//        GL.EnableClientState(All.TextureCoordArray);
//
//        GL.EnableClientState(All.ColorArray);
//        GL.ColorPointer(4, All.Float, 0, new float[]{
//            color.R, color.G, color.B, color.A,
//            color.R, color.G, color.B, color.A,
//            color.R, color.G, color.B, color.A,
//            color.R, color.G, color.B, color.A,
//        });
//
//        GL.DrawArrays(All.TriangleFan, 0, 4);
//
//        GL.DisableClientState(All.VertexArray);
//        GL.DisableClientState(All.TextureCoordArray);
//        GL.DisableClientState(All.ColorArray);
//
//
//        //GL.PopMatrix();
//    }
}

#endif
