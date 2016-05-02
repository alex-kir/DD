//
//  DDVector.cs
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

public struct DDVector
{
	public static readonly DDVector Empty = new DDVector(0, 0);
	public static readonly DDVector Zero = new DDVector(0, 0);
	public static readonly DDVector One = new DDVector(1, 1);
    public static readonly DDVector Vertical = new DDVector(0, 1);
    public static readonly DDVector Horizontal = new DDVector(1, 0);


	public static readonly DDVector LeftTop = new DDVector(0, 1);
	public static readonly DDVector LeftMiddle = new DDVector(0, 0.5f);
	public static readonly DDVector LeftBottom = new DDVector(0, 0);
	public static readonly DDVector CenterTop = new DDVector(0.5f, 1);
	public static readonly DDVector CenterMiddle = new DDVector(0.5f, 0.5f);
	public static readonly DDVector CenterBottom = new DDVector(0.5f, 0);
	public static readonly DDVector RightTop = new DDVector(1, 1);
	public static readonly DDVector RightMiddle = new DDVector(1, 0.5f);
	public static readonly DDVector RightBottom = new DDVector(1, 0);
	
    public float X;
    public float Y;
    public float Width { get { return X; } set { X = value; } }
    public float Height { get { return Y; } set { Y = value; } }
	
	public DDVector (float x_, float y_)
	{
		X = x_;
		Y = y_;
	}

    public DDVector (DDVector xy)
    {
        this.X = xy.X;
        this.Y = xy.Y;
    }

#if DD_PLATFORM_UNITY3D

	public DDVector(UnityEngine.Vector2 v)
	{
		X = v.x;
		Y = v.y;
	}
	
	public static implicit operator UnityEngine.Vector3(DDVector v)
	{
		return new UnityEngine.Vector3(v.X, v.Y, 0);
	}
#endif

#if DD_PLATFORM_ANDROID

    public DDVector(System.Drawing.Size v)
	{
		X = v.Width;
		Y = v.Height;
	}
	
	public static implicit operator System.Drawing.Size(DDVector v)
	{
        return new System.Drawing.Size((int)v.X, (int)v.Y);
	}

    public static implicit operator DDVector(System.Drawing.Size v)
    {
        return new DDVector(v.Width, v.Height);
    }

#endif 

#if DD_PLATFORM_IOS

    public DDVector(nfloat x, nfloat y)
    {
        X = (float)x;
        Y = (float)y;
    }

    public DDVector(System.Drawing.SizeF v)
    {
    	X = v.Width;
    	Y = v.Height;
    }

    public static implicit operator System.Drawing.SizeF(DDVector v)
    {
    	return new System.Drawing.SizeF(v.X, v.Y);
    }

    public static implicit operator DDVector(System.Drawing.SizeF v)
    {
    	return new DDVector(v.Width, v.Height);
    }

    public static implicit operator DDVector(CoreGraphics.CGSize v)
    {
        return new DDVector(v.Width, v.Height);
    }

    public static DDVector operator *(DDVector v1, nfloat n)
    {
        return new DDVector(v1.X * n, v1.Y * n);
    }


#endif 

#if DD_LIBRARY_BOX2D_XNA

    public DDVector(Microsoft.Xna.Framework.Vector2 v)
    {
        x = v.X;
        y = v.Y;
    }

    public static implicit operator Microsoft.Xna.Framework.Vector2(DDVector v)
    {
        return new Microsoft.Xna.Framework.Vector2((int)v.X, (int)v.Y);
    }

    public static implicit operator DDVector(Microsoft.Xna.Framework.Vector2 v)
    {
        return new DDVector(v.X, v.Y);
    }

#endif 

    public DDVector Negate()
	{
		return new DDVector(-X, -Y);
	}

	public DDVector Abs()
	{
		return new DDVector(Math.Abs(X), Math.Abs(Y));
	}
	
	public static DDVector operator + (DDVector v1, DDVector v2)
	{
		return new DDVector(v1.X + v2.X, v1.Y + v2.Y);
	}
	public static DDVector operator + (DDVector v1, float [] v2)
	{
        return new DDVector(v1.X + v2[0], v1.Y + v2[1]);
	}

	public static DDVector operator - (DDVector v1, DDVector v2)
	{
		return new DDVector(v1.X - v2.X, v1.Y - v2.Y);
	}

	public static DDVector operator - (DDVector v1)
	{
		return new DDVector(-v1.X, -v1.Y);
	}

	public static DDVector operator * (DDVector v1, DDVector v2)
	{
		return new DDVector(v1.X * v2.X, v1.Y * v2.Y);
	}

    public static DDVector operator *(DDVector v1, float n)
    {
        return new DDVector(v1.X * n, v1.Y * n);
    }

    public static DDVector operator *(float n, DDVector v1)
    {
        return new DDVector(v1.X * n, v1.Y * n);
    }

	public static DDVector operator / (DDVector v1, DDVector v2)
	{
		return new DDVector(v1.X / v2.X, v1.Y / v2.Y);
	}

	public static DDVector operator / (DDVector v1, float n)
	{
		return new DDVector(v1.X / n, v1.Y / n);
	}
	
	public static bool operator == (DDVector v1, DDVector v2)
	{
		return v1.X == v2.X && v1.Y == v2.Y;
	}
	
	public static bool operator != (DDVector v1, DDVector v2)
	{
		return v1.X != v2.X || v1.Y != v2.Y;
	}
	
	public override bool Equals(object obj)
	{
		DDVector ?other = obj as DDVector?;
		if (Object.ReferenceEquals(this, other))
			return true;
		return this == other;
	}
	
	public override int GetHashCode()
	{
		return X.GetHashCode() + Y.GetHashCode();
	}
	
	public override string ToString()
	{
		return string.Format("[DDVector: X={0}, Y={1}]", X, Y);
	}

    internal float Distance(DDVector other)
    {
        return (float)Math.Sqrt(DistanceSquare(other));
    }

    internal float DistanceSquare(DDVector other)
    {
        return (this.X - other.X) * (this.X - other.X) + (this.Y - other.Y) * (this.Y - other.Y);
    }

    public float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }

    public float LengthSquare { get { return X * X + Y * Y; } }

    public DDVector GetNormalized()
    {
        float len2 = LengthSquare;
        return new DDVector(Math.Abs(X) * X / len2, Math.Abs(Y) * Y / len2);
    }

    internal static DDVector FromAngle(float angleInDegrees)
    {
        float angle = angleInDegrees / 180 * DDMath.PI;
        return new DDVector(DDMath.Cos(angle), DDMath.Sin(angle));
    }

    internal static DDVector Lerp(DDVector v1, DDVector v2, float t)
    {
        return v1 + (v2 - v1) * t;
    }


}