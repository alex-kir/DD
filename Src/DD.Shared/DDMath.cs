//
//  DDMath.cs
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
using System.Collections.Generic;


public class DDMath
{
    public const float PI = (float)Math.PI;

	private static System.Random _rnd = new System.Random();

    public static T RandomFrom<T>(params T [] values)
	{
		int n = values.Length;
		if (n == 0)
			return default(T);
		if (n == 1)
			return values[0];
		int m = _rnd.Next(0, n);
		return values[m];
	}

	public static T RandomFrom<T>(List<T> values)
	{
		return RandomFrom(values.ToArray());
	}

    public static float RandomFloat()
    {
        return (float)_rnd.NextDouble();
    }

    public static float RandomFloat(float start, float end)
    {
        return start + RandomFloat() * (end - start);
    }

    public static int RandomInt(int a, int b)
    {
        return _rnd.Next(Math.Min(a, b), Math.Max(a, b) + 1);
    }

    public static float Cos(float angle)
    {
        #if DD_PLATFORM_UNITY3D
        return UnityEngine.Mathf.Cos(angle);
        #else
        return (float)Math.Cos(angle);
        #endif
    }

    public static float Sin(float angle)
    {
        #if DD_PLATFORM_UNITY3D
		return UnityEngine.Mathf.Sin(angle);
        #else
        return (float)Math.Sin(angle);
        #endif
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float Lerp(float y1, float y2, float x1, float x2, float x)
    {
        float t01 = (x - x1) / (x2 - x1);
        return y1 + (y2 - y1) * t01;
    }

    public static float Clamp(float min, float max, float value)
    {
        return Max(min, Min(max, value));
    }

	public static int Clamp(int min, int max, int value)
	{
		return Max(min, Min(max, value));
	}

    public static float LerpClamp(float y1, float y2, float x1, float x2, float x)
    {
        return Clamp(Min(y1, y2), Max(y1, y2), Lerp(y1, y2, x1, x2, x));
    }

    public static float Nurbs(float t, params float [] args)
    {
        if (args.Length == 1)
            return args[0];

        var res = new float[args.Length - 1];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = Lerp(args[i], args[i + 1], t);
        }

        return Nurbs(t, res);
    }

    internal static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    internal static int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    internal static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    internal static int Min(int a, int b)
    {
        return a < b ? a : b;
    }

    internal static float MaxXOrY(DDVector v)
    {
        return v.X > v.Y ? v.X : v.Y;
    }

    internal static float MinXOrY(DDVector v)
    {
        return v.X < v.Y ? v.X : v.Y;
    }
	
    internal static int RoundToInt(float f)
    {
        return (int)Math.Round(f, 0, MidpointRounding.AwayFromZero);
    }

    internal static float RadianToDegrees(float radians)
    {
        return radians / PI * 180;
    }

    internal static float Sqrt(float p)
    {
        return (float)Math.Sqrt(p);
    }

    internal static float Atan(float p)
    {
        return (float)Math.Atan(p);
    }
    public static float Angle(DDVector v1, DDVector v2)
    {
        var vv1 = v1 / v1.Length;
        var vv2 = v2 / v2.Length;
        return RadianToDegrees((float)(Math.Atan2(vv2.Y, vv2.X) - Math.Atan2(vv1.Y, vv1.X)));
    }

    internal static float Abs(float p)
    {
        return p < 0 ? -p : p;
    }
}