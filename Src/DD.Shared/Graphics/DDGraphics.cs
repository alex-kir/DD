
#if DD_PLATFORM_IOS || true

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace DD.Graphics
{
    public class DDGraphics
    {
        static DDGraphicsProgram defaultProgram;

        public static void SetupGL()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public static void Draw(DDGraphicsMesh mesh, DDGraphicsProgram program, DDMatrix matrix, DDTexture texture)
        {
            if (program == null)
            {
                if (defaultProgram == null)
                    defaultProgram = new DDGraphicsProgram(null, null);
                program = defaultProgram;
            }

            program.UseProgram();

            program.SetAttrib("position", mesh.positions, mesh.positionSize, mesh.positionSize * sizeof(float));
            program.SetAttrib("uv", mesh.uvs, mesh.uvSize, mesh.uvSize * sizeof(float));
            program.SetAttrib("color_white", mesh.colors_white, 4, 4 * sizeof(float));
            program.SetAttrib("color_black", mesh.colors_black, 4, 4 * sizeof(float));

            program.SetUniform("matrix", matrix);
            program.SetUniform("texture_0", texture, 0);

            GL.DrawElements(BeginMode.Triangles, mesh.indices.Length, DrawElementsType.UnsignedShort, mesh.indices);

        }
    }
}

#endif