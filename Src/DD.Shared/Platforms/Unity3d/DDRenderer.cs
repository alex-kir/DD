//
//  DDRenderer_Unity3d.cs
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
#if DD_PLATFORM_UNITY3D

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public partial class DDRenderer
{
    private int debugDrawCalls = 0;

    Material material;
    protected DDTexture currentTexture;
    bool hasTexture;

    DDRectangle screenRect;

    public DDRenderer(UnityEngine.Shader shader)
    {
        this.material = new Material(shader);
    }

    internal void BeginScene()
    {
        debugDrawCalls = 0;
        currentTexture = null;
        hasTexture = false;

        screenRect = new DDRectangle(DDVector.Zero, DDDirector.Instance.WinSize);
    }

    internal virtual void EndScene()
    {
        EndDrawing();
		DDDebug.Watch(" Draw Calls", debugDrawCalls.ToString());
    }
	
    private void StartDrawing(DDTexture texure)
    {
        hasTexture = true;
        currentTexture = texure;
        material.SetTexture("_MainTex", currentTexture.Texture);
        material.SetPass(0);
        GL.Begin(GL.QUADS);
    }

    private void EndDrawing()
    {
        if (hasTexture)
        {
            currentTexture = null;
            hasTexture = false;
            GL.End();
            debugDrawCalls++;
        }
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

			DrawQuad2(texture, quad);
		}
    }
	
	protected virtual void DrawQuad2(DDTexture texture, Quad quad)
    {
        if (currentTexture != texture)
        {
            EndDrawing();
            StartDrawing(texture);
        }

        // ----  DON'T CHANGE ORDER OF GL-COMMANDS  ---- //
		
		GL.MultiTexCoord3(0, quad.uv1.X, quad.uv1.Y, 0);
        GL.MultiTexCoord3(1, quad.black_color1.R, quad.black_color1.G, quad.black_color1.B);
        GL.Color(quad.white_color1);
        GL.Vertex(quad._xy1);

        GL.MultiTexCoord3(0, quad.uv2.X, quad.uv2.Y, 0);
        GL.MultiTexCoord3(1, quad.black_color2.R, quad.black_color2.G, quad.black_color2.B);
        GL.Color(quad.white_color2);
        GL.Vertex(quad._xy2);

        GL.MultiTexCoord3(0, quad.uv3.X, quad.uv3.Y, 0);
        GL.MultiTexCoord3(1, quad.black_color3.R, quad.black_color3.G, quad.black_color3.B);
        GL.Color(quad.white_color3);
        GL.Vertex(quad._xy3);

        GL.MultiTexCoord3(0, quad.uv4.X, quad.uv4.Y, 0);
        GL.MultiTexCoord3(1, quad.black_color4.R, quad.black_color4.G, quad.black_color4.B);
        GL.Color(quad.white_color4);
        GL.Vertex(quad._xy4);
	}
}

// TODO move class to separate file when it will stable
public class DDRendererWithBuffer : DDRenderer
{
    class DC
    {
        public DDTexture texture;
        public Quad quad;
		
        internal DC(DDTexture texture_, Quad quad_)
        {
            this.texture = texture_;
            this.quad = quad_;
        }
    }

    List<List<DC>> buffer;
    public int MaxBufferSize;

    public DDRendererWithBuffer(UnityEngine.Shader shader, int maxBufferSize)
        : base(shader)
    {
        buffer = new List<List<DC>>();
        MaxBufferSize = maxBufferSize;
    }

    protected override void DrawQuad2(DDTexture texture, Quad quad)
    {
        var dc = new DC(texture, quad);
        for (int i = buffer.Count - 1; i >= 0; --i)
        {
            var buff_i = buffer[i];
            if (buff_i[0].texture == dc.texture)
            {
                buff_i.Add(dc);
                goto added;
            }

            for (int j = 0; j < buff_i.Count; ++j)
            {
                if (buff_i[j].quad._rect.HasIntersection(quad._rect))
					goto has_intersection_or_new;
            }
        }

    has_intersection_or_new:
        buffer.Add(new List<DC> { dc });

    added:
        int count = buffer.Aggregate(0, (x, it) => x + it.Count);
        if (count > MaxBufferSize)
        {
            var buff_0 = buffer[0];
            for (int j = 0; j < buff_0.Count; ++j)
            {
                var dc_j = buff_0[j];
                base.DrawQuad2(dc_j.texture, dc_j.quad);
            }
            buffer.RemoveAt(0);
        }
    }

    internal override void EndScene()
    {
//        using (var stop = DDDebug.UsingMeasure("DDRenderer"))
        {
            for (int i = 0; i < buffer.Count; ++i)
            {
                var buff_i = buffer[i];
                for (int j = 0; j < buff_i.Count; ++j)
                {
                    var dc_j = buff_i[j];
                    base.DrawQuad2(dc_j.texture, dc_j.quad);
                }
            }
            buffer.Clear();
            base.EndScene();
        }
    }
}


#endif