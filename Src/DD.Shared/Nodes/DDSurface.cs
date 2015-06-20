//
//  DDSurface.cs
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


public class DDSurface : DDNode
{
	public class Vertex
	{
        public int indexX;
        public int indexY;
		public DDVector XY;
		public DDVector UV;
		public DDColor Color;
		public DDColor ColorBlack;
	}
	
	Action<Vertex> action;
	DDTextureFrame frame;
	Vertex [,] vertices;
	DDRenderer.Quad [,] quads;
	
	public DDSurface(string texName, int countX, int countY, Action<Vertex> action)
	{
		this.frame = DDTextureManager.Instance.GetNamedTextureFrame(texName);
		this.Size = this.frame.Size;
		this.action = action;
		
		this.vertices = new DDSurface.Vertex[countX + 1, countY + 1];
		for (int x = 0; x < vertices.GetLength(0); x++)
		{
			for (int y = 0; y < vertices.GetLength(1); y++)
			{
                this.vertices[x, y] = new Vertex() { indexX = x, indexY = y };
			}
		}
		
		this.quads = new DDRenderer.Quad[countX, countY];
		for (int x = 0; x < countX; x++)
		{
			for (int y = 0; y < countY; y++)
			{
                this.quads[x, y] = new DDRenderer.Quad();
			}
		}
	}
	
	public override void Draw(DDRenderer renderer)
    {
        int xx = vertices.GetLength(0);
        int yy = vertices.GetLength(1);
        DDVector onQuad = new DDVector();
        for (int x = 0; x < xx; x++)
        {
            onQuad.X = x / (xx - 1.0f);
            for (int y = 0; y < yy; y++)
            {
                var vertex = this.vertices[x, y];
                onQuad.Y = y / (yy - 1.0f);

                vertex.XY = frame.GetVertexXY(onQuad);
                vertex.UV = frame.GetVertexUV(onQuad);
                vertex.Color = this.Color;
                vertex.ColorBlack = this.ColorBlack;

                this.action(vertex);
            }
        }

        var matrix = NodeToWorldTransform();
        var combinedColor = (this.Parent == null) ? DDColor.White : this.Parent.CombinedColor;
		var combinedColorBlackNegative = this.CombinedColorBlack.Negative();

        for (int x = 1; x < xx; x++)
        {
            for (int y = 1; y < yy; y++)
            {
                var v1 = vertices[x - 1, y - 1];
                var v2 = vertices[x - 1, y - 0];
                var v3 = vertices[x - 0, y - 0];
                var v4 = vertices[x - 0, y - 1];
			
				var quad = quads[x - 1, y - 1];
			
				quad.Matrix = matrix;
			
                quad.xy1 = v1.XY;
                quad.xy2 = v2.XY;
                quad.xy3 = v3.XY;
                quad.xy4 = v4.XY;

                quad.uv1 = v1.UV;
                quad.uv2 = v2.UV;
                quad.uv3 = v3.UV;
                quad.uv4 = v4.UV;

                quad.white_color1 = v1.Color * combinedColor;
                quad.white_color2 = v2.Color * combinedColor;
                quad.white_color3 = v3.Color * combinedColor;
                quad.white_color4 = v4.Color * combinedColor;

                quad.black_color1 = (v1.ColorBlack.Negative() * combinedColorBlackNegative).Negative();
                quad.black_color2 = (v2.ColorBlack.Negative() * combinedColorBlackNegative).Negative();
                quad.black_color3 = (v3.ColorBlack.Negative() * combinedColorBlackNegative).Negative();
                quad.black_color4 = (v4.ColorBlack.Negative() * combinedColorBlackNegative).Negative();

				renderer.DrawQuad(frame.Texture, quad);
            }
        }
    }
	
	protected override DDTexture GetUsedTexture()
	{
		return frame.Texture;
	}
}