//
//  DDColor.cs
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


public struct DDColor
{
    public static readonly DDColor White = new DDColor(1, 1, 1, 1);
    public static readonly DDColor WhiteTransparent = new DDColor(1, 1, 1, 0);
    public static readonly DDColor LigthGray = new DDColor(0.75f, 0.75f, 0.75f, 1f);
    public static readonly DDColor Gray = new DDColor(0.5f, 0.5f, 0.5f, 1f);
    public static readonly DDColor DarkGray = new DDColor(0.25f, 0.25f, 0.25f, 1f);
    public static readonly DDColor Black = new DDColor(0f, 0f, 0f, 1f);

    public static readonly DDColor DarkGreen = new DDColor(0, 0.5f, 0, 1f);
    public static readonly DDColor LigthGreen = new DDColor(0.5f, 1, 0.5f, 1f);

    public static readonly DDColor Red = new DDColor(1f, 0f, 0f, 1f);
    public static readonly DDColor Orange = new DDColor(1f, 0.5f, 0f, 1f);
    public static readonly DDColor Yellow = new DDColor(1f, 1f, 0f, 1f);
    public static readonly DDColor Brown = new DDColor(0.5f, 0.375f, 0f, 1f);
    public static readonly DDColor Green = new DDColor(0f, 1f, 0f, 1f);
    public static readonly DDColor Cyan = new DDColor(0f, 1f, 1f, 1f);
    public static readonly DDColor Blue = new DDColor(0f, 0f, 1f, 1f);
    public static readonly DDColor Violet = new DDColor(0.5f, 0f, 1f, 1f);
    public static readonly DDColor Magenta = new DDColor(1f, 0f, 1f, 1f);

    public float R, G, B, A;

    public DDColor(float r_, float g_, float b_, float a_ = 1)
    {
        R = r_;
        G = g_;
        B = b_;
        A = a_;
    }

    public DDColor(DDColor color, float a_)
    {
		R = color.R;
		G = color.G;
		B = color.B;
		A = a_;
	}

	public DDColor(DDColor color)
	{
		R = color.R;
		G = color.G;
		B = color.B;
		A = color.A;
	 }

    public static DDColor FromArgbInt(int argb)
    {
        return new DDColor(
            ((argb >> 16) & 0xff) / 255.0f,
            ((argb >> 8) & 0xff) / 255.0f,
            (argb & 0xff) / 255.0f,
            (argb >> 24) / 255.0f
            );
    }

    public static DDColor FromRgbInt(int rgb)
    {
        return new DDColor(
            ((rgb >> 16) & 0xff) / 255.0f,
            ((rgb >> 8) & 0xff) / 255.0f,
            (rgb & 0xff) / 255.0f
        );
    }

	public override string ToString()
	{
		return string.Format("[DDColor R={0}, G={1}, B={2}, A={3}]",
			R.ToString("0.00"), G.ToString("0.00"), B.ToString("0.00"), A.ToString("0.00"));
	}

#if DD_PLATFORM_UNITY3D
    public DDColor(UnityEngine.Color v)
	{
        R = v.r;
        G = v.g;
        B = v.b;
        A = v.a;
    }

    public static implicit operator UnityEngine.Color(DDColor v)
	{
        return new UnityEngine.Color(v.R, v.G, v.B, v.A);
	}
#endif

    public static bool operator ==(DDColor c1, DDColor c2)
    {
        return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
    }

    public static bool operator !=(DDColor c1, DDColor c2)
    {
        return c1.R != c2.R || c1.G != c2.G || c1.B != c2.B || c1.A != c2.A;
    }

    public override bool Equals(object obj)
    {
        DDColor? other = obj as DDColor?;
        if (!other.HasValue)
            return false;
        return this == other.Value;
    }

    public override int GetHashCode()
    {
        return (R + G + B + A).GetHashCode();
    }

    public static DDColor operator * (DDColor c1, DDColor c2)
    {
        return new DDColor(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B, c1.A * c2.A);
    }

    public static DDColor Lerp(DDColor c1, DDColor c2, float t)
    {
        return new DDColor(
            DDMath.Lerp(c1.R, c2.R, t),
            DDMath.Lerp(c1.G, c2.G, t),
            DDMath.Lerp(c1.B, c2.B, t),
            DDMath.Lerp(c1.A, c2.A, t));
    }

    internal DDColor Negative()
    {
        return new DDColor(1 - R, 1 - G, 1 - B);
    }
}