//
//  DDRenderer.cs
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
using DD.Graphics;


public partial class DDRenderer
{
	public class Quad
	{
		public DDMatrix Matrix;
		
        public DDVector _xy1;
        public DDVector _xy2;
        public DDVector _xy3;
        public DDVector _xy4;
		
		public DDRectangle _rect = new DDRectangle();
		
        public DDVector xy1;
        public DDVector uv1;
        public DDColor white_color1;
        public DDColor black_color1;

        public DDVector xy2;
        public DDVector uv2;
        public DDColor white_color2;
        public DDColor black_color2;

        public DDVector xy3;
        public DDVector uv3;
        public DDColor white_color3;
        public DDColor black_color3;

        public DDVector xy4;
        public DDVector uv4;
        public DDColor white_color4;
        public DDColor black_color4;
	}


    DDRectangle screenRect;

    DDMatrix defaultMatrix;

    internal void BeginScene()
    {
        screenRect = new DDRectangle(DDVector.Zero, DDDirector.Instance.ScreenResolution);
        var sz = DDDirector.Instance.ScreenResolution * 0.5f;
        defaultMatrix = new DDMatrix(new DDVector(-1, -1), DDVector.Zero, new DDVector(1 / sz.Width, 1 / sz.Height));
    }

    internal void EndScene()
    {

    }

    internal virtual void DrawQuad(DDTexture texture, Quad quad)
    {
        //using (var stop = DDDebug.UsingMeasure("DDRenderer"))
        {
            quad._xy1 = quad.Matrix.TransformPoint(quad.xy1);
            quad._xy2 = quad.Matrix.TransformPoint(quad.xy2);
            quad._xy3 = quad.Matrix.TransformPoint(quad.xy3);
            quad._xy4 = quad.Matrix.TransformPoint(quad.xy4);

            quad._rect.SetRectangle(quad._xy1, quad._xy2, quad._xy3, quad._xy4);

            if (!quad._rect.HasIntersection(screenRect))
                return;

            DD.Graphics.DDGraphics.Draw(GetMesh(quad), null, defaultMatrix, texture);
            //            DrawQuad2(texture, quad);
        }
    }

    internal void Draw(DDGraphicsMesh mesh, DDGraphicsProgram program, DDMatrix matrix)
    {
        DD.Graphics.DDGraphics.Draw(mesh, null, defaultMatrix * matrix, mesh.Texture);
    }

    private static DD.Graphics.DDGraphicsMesh GetMesh(Quad q)
    {
        return new DD.Graphics.DDGraphicsMesh(2, new float[]
            {
                q._xy1.X, q._xy1.Y, 
                q._xy2.X, q._xy2.Y, 
                q._xy3.X, q._xy3.Y, 

                q._xy1.X, q._xy1.Y, 
                q._xy3.X, q._xy3.Y, 
                q._xy4.X, q._xy4.Y, 
            }, new float[]
            {
#if DD_PLATFORM_IOS
                q.uv1.X, 1 - q.uv1.Y,
                q.uv2.X, 1 - q.uv2.Y,
                q.uv3.X, 1 - q.uv3.Y,

                q.uv1.X, 1 - q.uv1.Y,
                q.uv4.X, 1 - q.uv3.Y,
                q.uv4.X, 1 - q.uv4.Y,
#else
                q.uv1.X, q.uv1.Y,
                q.uv2.X, q.uv2.Y,
                q.uv3.X, q.uv3.Y,

                q.uv1.X, q.uv1.Y,
                q.uv4.X, q.uv3.Y,
                q.uv4.X, q.uv4.Y,
#endif
            }, q.white_color1, q.black_color1);
    }

    public static DD.Graphics.DDGraphicsMesh GetMesh2(Quad q)
    {
        return new DD.Graphics.DDGraphicsMesh(2, new float[]
            {
                q.xy1.X, q.xy1.Y, 
                q.xy2.X, q.xy2.Y, 
                q.xy3.X, q.xy3.Y, 

                q.xy1.X, q.xy1.Y, 
                q.xy3.X, q.xy3.Y, 
                q.xy4.X, q.xy4.Y, 
            }, new float[]
            {
#if DD_PLATFORM_IOS
                q.uv1.X, 1 - q.uv1.Y,
                q.uv2.X, 1 - q.uv2.Y,
                q.uv3.X, 1 - q.uv3.Y,

                q.uv1.X, 1 - q.uv1.Y,
                q.uv4.X, 1 - q.uv3.Y,
                q.uv4.X, 1 - q.uv4.Y,
#else
                q.uv1.X, q.uv1.Y,
                q.uv2.X, q.uv2.Y,
                q.uv3.X, q.uv3.Y,

                q.uv1.X, q.uv1.Y,
                q.uv4.X, q.uv3.Y,
                q.uv4.X, q.uv4.Y,
#endif
            }, q.white_color1, q.black_color1);
    }

}