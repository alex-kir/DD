//
//  DDRectangle.cs
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

public struct DDRectangle
{
    public static readonly DDRectangle Empty = new DDRectangle();
	
	float minX, minY, maxX, maxY;

    public float Left { get { return minX; } }
    public float Right { get { return maxX; } }
    public float Top { get { return maxY; } }
    public float Bottom { get { return minY; } }

    public DDVector LeftBottom { get { return new DDVector(Left, Bottom); } }
    public DDVector RightBottom { get { return new DDVector(Right, Bottom); } }
    public DDVector LeftTop { get { return new DDVector(Left, Top); } }
    public DDVector RightTop { get { return new DDVector(Right, Top); } }

    public DDVector CenterTop { get { return new DDVector((Left + Right) / 2, Top); } }
    public DDVector CenterBottom { get { return new DDVector((Left + Right) / 2, Bottom); } }
    public DDVector LeftMiddle { get { return new DDVector(Left, (Top + Bottom) / 2); } }
    public DDVector RightMiddle { get { return new DDVector(Right, (Top + Bottom) / 2); } }

	
    public DDVector Center { get { return new DDVector((minX + maxX) / 2, (minY + maxY) / 2); } }

    public bool IsEmpty { get { return minX == maxX && minY == maxY; } }

    public DDVector Origin
    {
        get
        {
            return new DDVector(Left, Bottom);
        }
    }

    public DDVector Size
    {
        get
        {
            return new DDVector(maxX - minX, maxY - minY);
        }
    }

    public DDRectangle(DDVector p1, DDVector p2)
    {
		minX = DDMath.Min(p1.X, p2.X);
		maxX = DDMath.Max(p1.X, p2.X);
		minY = DDMath.Min(p1.Y, p2.Y);
		maxY = DDMath.Max(p1.Y, p2.Y);
    }

    public DDRectangle(float x1, float y1, float x2, float y2)
        : this(new DDVector(x1, y1), new DDVector(x2, y2))
    { }

	public void SetRectangle(DDVector xy1, DDVector xy2, DDVector xy3, DDVector xy4)
	{
		float minT;
		float maxT;

		// -- X -- //

		if (xy1.X < xy2.X) {
			minX = xy1.X;
			maxX = xy2.X;
		} else {
			minX = xy2.X;
			maxX = xy1.X;
		}

		if (xy3.X < xy4.X) {
			minT = xy3.X;
			maxT = xy4.X;
		} else {
			minT = xy4.X;
			maxT = xy3.X;
		}

		if (minX > minT)
			minX = minT;

		if (maxX < maxT)
			maxX = maxT;

		// -- Y -- //

		if (xy1.Y < xy2.Y) {
			minY = xy1.Y;
			maxY = xy2.Y;
		} else {
			minY = xy2.Y;
			maxY = xy1.Y;
		}

		if (xy3.Y < xy4.Y) {
			minT = xy3.Y;
			maxT = xy4.Y;
		} else {
			minT = xy4.Y;
			maxT = xy3.Y;
		}

		if (minY > minT)
			minY = minT;

		if (maxY < maxT)
			maxY = maxT;
	}

    internal bool Contains(DDVector pos)
    {
        if (pos.X < Left)
            return false;
        if (pos.Y < Bottom)
            return false;
        if (pos.X > Right)
            return false;
        if (pos.Y > Top)
            return false;
        return true;
    }

    public void Shift(DDVector v)
    {
        minX = minX + v.X;
		maxX = maxX + v.X;
		minY = minY + v.Y;
		maxY = maxY + v.Y;
    }

    public void Crop(float left, float bottom, float right, float top)
    {
		minX += left;
		minY += bottom;
		maxX -= right;
		maxY -= top;
    }

    public DDRectangle GetCropped(float left, float bottom, float right, float top)
    {
        return new DDRectangle(minX + left, minY + bottom, maxX - right, maxY - top);
    }

	public DDRectangle Grid(int x, int y, int xx, int yy)
	{
		var sz = this.Size / new DDVector(xx, yy);
		return new DDRectangle((sz * new DDVector(x, y)) + LeftBottom, (sz * new DDVector(x + 1, y + 1)) + LeftBottom);
	}
	
    public void Extends(DDVector point)
    {
        minX = DDMath.Min(minX, point.X);
        maxX = DDMath.Max(maxX, point.X);
        minY = DDMath.Min(minY, point.Y);
        maxY = DDMath.Max(maxY, point.Y);
    }

    public void Extends(DDRectangle rect)
    {
        if (rect.IsEmpty)
            return;
        minX = DDMath.Min(minX, rect.Left);
        maxX = DDMath.Max(maxX, rect.Right);
        minY = DDMath.Min(minY, rect.Bottom);
        maxY = DDMath.Max(maxY, rect.Top);
    }

	public bool HasIntersection(DDRectangle other)
	{
		if (this.minX > other.maxX)
			return false;
		if (this.minY > other.maxY)
			return false;
		if (this.maxX < other.minX)
			return false;
		if (this.maxY < other.minY)
			return false;
		return true;
    }

	public DDRectangle Intersection(DDRectangle other)
	{
		if (this.minX > other.maxX)
			return DDRectangle.Empty;
		if (this.minY > other.maxY)
			return DDRectangle.Empty;
		if (this.maxX < other.minX)
			return DDRectangle.Empty;
		if (this.maxY < other.minY)
			return DDRectangle.Empty;

		var xx = new float[] { this.minX, this.maxX, other.minX, other.maxX }.DDOrderBy(it => it);
		var yy = new float[] { this.minY, this.maxY, other.minY, other.maxY }.DDOrderBy(it => it);
		return new DDRectangle(xx[1], yy[1], xx[2], yy[2]);
	}
	
    public override string ToString()
    {
        return string.Format("[DDRectangle: L={0}, B={1}, R={2}, T={3}]", Left, Bottom, Right, Top);
    }
}