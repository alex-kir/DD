
#if DD_PLATFORM_IOS && false //|| DD_PLATFORM_ANDROID
using System;

//namespace DD
//{
public partial class DDRenderer
{
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

    private DD.Graphics.DDGraphicsMesh GetMesh(Quad q)
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
            q.uv1.X, 1 - q.uv1.Y,
            q.uv2.X, 1 - q.uv2.Y,
            q.uv3.X, 1 - q.uv3.Y,
                     
            q.uv1.X, 1 - q.uv1.Y,
            q.uv4.X, 1 - q.uv3.Y,
            q.uv4.X, 1 - q.uv4.Y,
            }, q.white_color1, q.black_color1);
    }
}
//}

#endif
