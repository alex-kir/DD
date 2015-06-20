//
//  DDMatrix.cs
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
#define R1
#define TransformPoint_O1

using System;

public class DDMatrix
{
#if R1
    float[] _m = new float[16];

	public float A { get { return _m[00]; } set { _m[00] = value; } }
	public float B { get { return _m[01]; } set { _m[01] = value; } }
	public float C { get { return _m[04]; } set { _m[04] = value; } }
	public float D { get { return _m[05]; } set { _m[05] = value; } }
	public float X { get { return _m[12]; } set { _m[12] = value; } }
	public float Y { get { return _m[13]; } set { _m[13] = value; } }
#else
	float _m00, _m01, _m04, _m05, _m12, _m13;

	public float A { get { return _m00; } set { _m00 = value; } }
	public float B { get { return _m01; } set { _m01 = value; } }
	public float C { get { return _m04; } set { _m04 = value; } }
	public float D { get { return _m05; } set { _m05 = value; } }
	public float X { get { return _m12; } set { _m12 = value; } }
	public float Y { get { return _m13; } set { _m13 = value; } }
#endif
	
	
	
    public DDMatrix()
    {
        MakeIdentity();
    }
	
    public DDMatrix(DDVector translation, DDVector rotationIdDegrees, DDVector scale)
    {
        MakeTransform(translation, rotationIdDegrees, scale);
    }

	public void MakeIdentity()
    {
#if R1
        _m[00] = 1; _m[01] = 0; _m[02] = 0; _m[03] = 0;
        _m[04] = 0; _m[05] = 1; _m[06] = 0; _m[07] = 0;
        _m[08] = 0; _m[09] = 0; _m[10] = 1; _m[11] = 0;
        _m[12] = 0; _m[13] = 0; _m[14] = 0; _m[15] = 1;
#else
        _m00 = 1; _m01 = 0;
        _m04 = 0; _m05 = 1;
        _m12 = 0; _m13 = 0;
#endif
    }
	
	public void MakeTransform(DDVector translation, DDVector rotationIdDegrees, DDVector scale)
	{
		const float radian = 1f / 180f * DDMath.PI;
        float rx = rotationIdDegrees.X * radian;
        float ry = rotationIdDegrees.Y * radian;

        A = DDMath.Cos(ry) * scale.X;
        B = DDMath.Sin(ry) * scale.X;
        C = -DDMath.Sin(rx) * scale.Y;
        D = DDMath.Cos(rx) * scale.Y;

		X = translation.X;
		Y = translation.Y;
	}
	
	private static DDMatrix Multiply(DDMatrix m1, DDMatrix m2)
	{
		DDMatrix mr = new DDMatrix();
		
		mr.A = m1.A * m2.A + m1.B * m2.C;
		mr.B = m1.A * m2.B + m1.B * m2.D;
		//0
		//0
		mr.C = m1.C * m2.A + m1.D * m2.C;
		mr.D = m1.C * m2.B + m1.D * m2.D;
		//0
		//0
		//0
		//0
		//1
		//0
		mr.X = m1.X * m2.A + m1.Y * m2.C + m2.X;
		mr.Y = m1.X * m2.B + m1.Y * m2.D + m2.Y;
		//0
		//1
		return mr;
	}
	
    private DDMatrix Multiply(DDMatrix m)
    {
		return Multiply(this, m);
    }

#if R1

	public DDMatrix Invert()
    {
        DDMatrix ret = new DDMatrix();

        float[] inv = new float[16];
        float[] m = _m;

        inv[0] = m[5] * m[10] * m[15] -
                 m[5] * m[11] * m[14] -
                 m[9] * m[6] * m[15] +
                 m[9] * m[7] * m[14] +
                 m[13] * m[6] * m[11] -
                 m[13] * m[7] * m[10];

        inv[4] = -m[4] * m[10] * m[15] +
                  m[4] * m[11] * m[14] +
                  m[8] * m[6] * m[15] -
                  m[8] * m[7] * m[14] -
                  m[12] * m[6] * m[11] +
                  m[12] * m[7] * m[10];

        inv[8] = m[4] * m[9] * m[15] -
                 m[4] * m[11] * m[13] -
                 m[8] * m[5] * m[15] +
                 m[8] * m[7] * m[13] +
                 m[12] * m[5] * m[11] -
                 m[12] * m[7] * m[9];

        inv[12] = -m[4] * m[9] * m[14] +
                   m[4] * m[10] * m[13] +
                   m[8] * m[5] * m[14] -
                   m[8] * m[6] * m[13] -
                   m[12] * m[5] * m[10] +
                   m[12] * m[6] * m[9];

        float det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

        if (det != 0)
        {

            inv[1] = -m[1] * m[10] * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * m[10];

            inv[5] = m[0] * m[10] * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * m[10];

            inv[9] = -m[0] * m[9] * m[15] +
                      m[0] * m[11] * m[13] +
                      m[8] * m[1] * m[15] -
                      m[8] * m[3] * m[13] -
                      m[12] * m[1] * m[11] +
                      m[12] * m[3] * m[9];

            inv[13] = m[0] * m[9] * m[14] -
                      m[0] * m[10] * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * m[10] -
                      m[12] * m[2] * m[9];

            inv[2] = m[1] * m[6] * m[15] -
                     m[1] * m[7] * m[14] -
                     m[5] * m[2] * m[15] +
                     m[5] * m[3] * m[14] +
                     m[13] * m[2] * m[7] -
                     m[13] * m[3] * m[6];

            inv[6] = -m[0] * m[6] * m[15] +
                      m[0] * m[7] * m[14] +
                      m[4] * m[2] * m[15] -
                      m[4] * m[3] * m[14] -
                      m[12] * m[2] * m[7] +
                      m[12] * m[3] * m[6];

            inv[10] = m[0] * m[5] * m[15] -
                      m[0] * m[7] * m[13] -
                      m[4] * m[1] * m[15] +
                      m[4] * m[3] * m[13] +
                      m[12] * m[1] * m[7] -
                      m[12] * m[3] * m[5];

            inv[14] = -m[0] * m[5] * m[14] +
                       m[0] * m[6] * m[13] +
                       m[4] * m[1] * m[14] -
                       m[4] * m[2] * m[13] -
                       m[12] * m[1] * m[6] +
                       m[12] * m[2] * m[5];

            inv[3] = -m[1] * m[6] * m[11] +
                      m[1] * m[7] * m[10] +
                      m[5] * m[2] * m[11] -
                      m[5] * m[3] * m[10] -
                      m[9] * m[2] * m[7] +
                      m[9] * m[3] * m[6];

            inv[7] = m[0] * m[6] * m[11] -
                     m[0] * m[7] * m[10] -
                     m[4] * m[2] * m[11] +
                     m[4] * m[3] * m[10] +
                     m[8] * m[2] * m[7] -
                     m[8] * m[3] * m[6];

            inv[11] = -m[0] * m[5] * m[11] +
                       m[0] * m[7] * m[9] +
                       m[4] * m[1] * m[11] -
                       m[4] * m[3] * m[9] -
                       m[8] * m[1] * m[7] +
                       m[8] * m[3] * m[5];

            inv[15] = m[0] * m[5] * m[10] -
                      m[0] * m[6] * m[9] -
                      m[4] * m[1] * m[10] +
                      m[4] * m[2] * m[9] +
                      m[8] * m[1] * m[6] -
                      m[8] * m[2] * m[5];


            det = 1.0f / det;

            for (int i = 0; i < 16; i++)
                ret._m[i] = inv[i] * det;
        }

        return ret;
    }
	
#else
	
	public DDMatrix Invert()
    {
        DDMatrix ret = new DDMatrix();

        float[] inv = new float[16];
        float[] m = _m;

        inv[0] = m[5] * 1 * m[15] -
                 m[5] * m[11] * m[14] -
                 m[9] * m[6] * m[15] +
                 m[9] * m[7] * m[14] +
                 m[13] * m[6] * m[11] -
                 m[13] * m[7] * 1;

        inv[4] = -m[4] * 1 * m[15] +
                  m[4] * m[11] * m[14] +
                  m[8] * m[6] * m[15] -
                  m[8] * m[7] * m[14] -
                  m[12] * m[6] * m[11] +
                  m[12] * m[7] * 1;

        inv[8] = m[4] * m[9] * m[15] -
                 m[4] * m[11] * m[13] -
                 m[8] * m[5] * m[15] +
                 m[8] * m[7] * m[13] +
                 m[12] * m[5] * m[11] -
                 m[12] * m[7] * m[9];

        inv[12] = -m[4] * m[9] * m[14] +
                   m[4] * 1 * m[13] +
                   m[8] * m[5] * m[14] -
                   m[8] * m[6] * m[13] -
                   m[12] * m[5] * 1 +
                   m[12] * m[6] * m[9];

        float det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

        if (det != 0)
        {

            inv[1] = -m[1] * 1 * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * 1;

            inv[5] = m[0] * 1 * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * 1;

//            inv[9] = -m[0] * m[9] * m[15] +
//                      m[0] * m[11] * m[13] +
//                      m[8] * m[1] * m[15] -
//                      m[8] * m[3] * m[13] -
//                      m[12] * m[1] * m[11] +
//                      m[12] * m[3] * m[9];

            inv[13] = m[0] * m[9] * m[14] -
                      m[0] * 1 * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * 1 -
                      m[12] * m[2] * m[9];

//            inv[2] = m[1] * m[6] * m[15] -
//                     m[1] * m[7] * m[14] -
//                     m[5] * m[2] * m[15] +
//                     m[5] * m[3] * m[14] +
//                     m[13] * m[2] * m[7] -
//                     m[13] * m[3] * m[6];

//            inv[6] = -m[0] * m[6] * m[15] +
//                      m[0] * m[7] * m[14] +
//                      m[4] * m[2] * m[15] -
//                      m[4] * m[3] * m[14] -
//                      m[12] * m[2] * m[7] +
//                      m[12] * m[3] * m[6];

//            inv[10] = m[0] * m[5] * m[15] -
//                      m[0] * m[7] * m[13] -
//                      m[4] * m[1] * m[15] +
//                      m[4] * m[3] * m[13] +
//                      m[12] * m[1] * m[7] -
//                      m[12] * m[3] * m[5];

//            inv[14] = -m[0] * m[5] * m[14] +
//                       m[0] * m[6] * m[13] +
//                       m[4] * m[1] * m[14] -
//                       m[4] * m[2] * m[13] -
//                       m[12] * m[1] * m[6] +
//                       m[12] * m[2] * m[5];

//            inv[3] = -m[1] * m[6] * m[11] +
//                      m[1] * m[7] * 1 +
//                      m[5] * m[2] * m[11] -
//                      m[5] * m[3] * 1 -
//                      m[9] * m[2] * m[7] +
//                      m[9] * m[3] * m[6];
//
//            inv[7] = m[0] * m[6] * m[11] -
//                     m[0] * m[7] * 1 -
//                     m[4] * m[2] * m[11] +
//                     m[4] * m[3] * 1 +
//                     m[8] * m[2] * m[7] -
//                     m[8] * m[3] * m[6];
//
//            inv[11] = -m[0] * m[5] * m[11] +
//                       m[0] * m[7] * m[9] +
//                       m[4] * m[1] * m[11] -
//                       m[4] * m[3] * m[9] -
//                       m[8] * m[1] * m[7] +
//                       m[8] * m[3] * m[5];
//
//            inv[15] = m[0] * m[5] * 1 -
//                      m[0] * m[6] * m[9] -
//                      m[4] * m[1] * 1 +
//                      m[4] * m[2] * m[9] +
//                      m[8] * m[1] * m[6] -
//                      m[8] * m[2] * m[5];


            det = 1.0f / det;

            for (int i = 0; i < 16; i++)
                ret._m[i] = inv[i] * det;
        }

        return ret;
    }
	
#endif

    public DDVector TransformPoint(DDVector point)
    {
#if TransformPoint_O1
			return new DDVector(point.X * A + point.Y * C + X, point.X * B + point.Y * D + Y);
#else
	        float[] v = new float[4] { point.x, point.y, 0, 1 };
	        float[] vr = new float[4];
	
	        int xx1 = 4;
	        int yy1 = 1;
	        int xx2 = 4;
	        int yy2 = 4;
	        int xxr = xx2;
	        int yyr = yy1;
	        int kk = yy2; // MUST: kk == yy2 == xx1
	
	        for (int x = 0; x < xxr; x++)
	        {
	            for (int y = 0; y < yyr; y++)
	            {
	                float s = 0;
	                for (int k = 0; k < kk; k++)
	                {
	                    int i1 = k + y * xx1;
	                    int i2 = x + k * xx2;
	
	                    s += v[i1] * _m[i2];
	                }
	                vr[x + y * xxr] = s;
	            }
	        }
	
	        return new DDVector(vr[0], vr[1]);
#endif
			
//		string[] v = new string[4] { "point.X", "point.Y", "0", "1" };
//      string[] vr = new string[4];
//
//		string[] _m = new string []{
//			"A", "B", "0", "0",
//			"C", "D", "0", "0",
//			"0", "0", "1", "0",
//			"X", "Y", "0", "1",
//		};
//		
//        int xx1 = 4;
//        int yy1 = 1;
//        int xx2 = 4;
//        int yy2 = 4;
//        int xxr = xx2;
//        int yyr = yy1;
//        int kk = yy2; // MUST: kk == yy2 == xx1
//
//        for (int x = 0; x < xxr; x++)
//        {
//            for (int y = 0; y < yyr; y++)
//            {
//                List<string> sum = new List<string>();
//                for (int k = 0; k < kk; k++)
//                {
//                    int i1 = k + y * xx1;
//                    int i2 = x + k * xx2;
//
//                    sum.Add(v[i1] + " * " + _m[i2]);
//                }
//                vr[x + y * xxr] = string.Join(" + ", sum.ToArray());
//            }
//        }
//
//		Debug.Log(vr[0] + ", " + vr[1]);
    }
	
	public void Transform(DDVector translation, DDVector rotationIdDegrees, DDVector scale)
	{
		var m = new DDMatrix(translation, rotationIdDegrees, scale);
        _m = m.Multiply(this)._m;
	}
	
    public void Translate(float x, float y)
    {
        var m = new DDMatrix(new DDVector(x, y), DDVector.Zero, DDVector.One);
        _m = m.Multiply(this)._m;
    }

    public void Translate(DDVector v)
    {
        Translate(v.X, v.Y);
    }

    public void Rotate(float rotationIdDegress)
    {
        var m = new DDMatrix(DDVector.Zero, new DDVector(rotationIdDegress, rotationIdDegress), DDVector.One);
        _m = m.Multiply(this)._m;
    }

    public void Scale(float sx, float sy)
    {
        var m = new DDMatrix(DDVector.Zero, DDVector.Zero, new DDVector(sx, sy));
        _m = m.Multiply(this)._m;
    }

    public static DDMatrix operator *(DDMatrix m1, DDMatrix m2)
    {
        return m2.Multiply(m1);
    }

    public static DDVector operator *(DDMatrix m, DDVector v)
    {
        return m.TransformPoint(v);
    }

	public override string ToString()
    {
        return string.Format("[DDMatrix: A={0}, B={1}, C={2}, D={3}, X={4}, Y={5}]", A, B, C, D, X, Y);
    }
	
#if DD_PLATFORM_UNITY3D

	public static implicit operator UnityEngine.Matrix4x4(DDMatrix m)
    {
        UnityEngine.Matrix4x4 r = new UnityEngine.Matrix4x4();

        r.m00 = m._m[00]; r.m01 = m._m[01]; r.m02 = m._m[02]; r.m03 = m._m[03];
        r.m10 = m._m[04]; r.m11 = m._m[05]; r.m12 = m._m[06]; r.m13 = m._m[07];
        r.m20 = m._m[08]; r.m21 = m._m[09]; r.m22 = m._m[10]; r.m23 = m._m[11];
        r.m30 = m._m[12]; r.m31 = m._m[13]; r.m32 = m._m[14]; r.m33 = m._m[15];

        return r;
    }
	
#elif DD_PLATFORM_ANDROID
	
	public static implicit operator float[](DDMatrix m)
    {
        return m._m;
    }
	
#endif

}