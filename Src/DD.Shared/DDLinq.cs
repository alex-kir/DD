//
//  DDLinq.cs
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
using System.Linq;
using System.Collections.Generic;

public static class DDLinq
{
    public static int DDSum<T>(this IEnumerable<T> self, Func<T, int> getter)
    {
		var sum = 0;
		foreach (var item in self)
			sum += getter(item);
        return sum;
    }

	public static float DDSum<T>(this IEnumerable<T> self, Func<T, float> getter)
	{
		var sum = 0.0f;
		foreach (var item in self)
			sum += getter(item);
        return sum;
	}

	public static double DDSum<T>(this IEnumerable<T> self, Func<T, double> getter)
	{
		var sum = 0.0;
		foreach (var item in self)
			sum += getter(item);
        return sum;
	}
	
//	public static float DDMax<T>(this IEnumerable<T> self, Func<T, float> getter)
//	{
//		bool first = true;
//		float max = 0;
//		foreach(var item in self)
//		{
//			float current = getter(item);
//			if (first)
//			{
//				max = current;
//				first = false;
//			}
//			else
//			{
//				if (max < current)
//					max = current;
//			}
//		}
//		return max;
//	}

	public static TResult DDMax<T, TResult>(this IEnumerable<T> self, Func<T, TResult> getter) where TResult : System.IComparable
	{
		var elements = self.GetEnumerator();
		
		if (!elements.MoveNext())
			throw new ArgumentException("self");
		
		var max = getter(elements.Current);
		
		while (elements.MoveNext())
		{
			var next = getter(elements.Current);
			if (max.CompareTo(next) < 0)
				max = next;
		}
		
		return max;
	}

    public static TResult DDMin<T, TResult>(this IEnumerable<T> self, Func<T, TResult> getter) where TResult : System.IComparable
    {
        var elements = self.GetEnumerator();

        if (!elements.MoveNext())
            throw new ArgumentException("self");

        var min = getter(elements.Current);

        while (elements.MoveNext())
        {
            var next = getter(elements.Current);
            if (min.CompareTo(next) > 0)
                min = next;
        }

        return min;
    }

	public static List<T> DDOrderBy<T>(this IEnumerable<T> self, Func<T, float> getter)
	{
		List<T> ret = new List<T>();
		List<float> tmp = new List<float>();
		foreach (var item in self)
		{
			float key = getter(item);
            bool inserted = false;
            for (int i = 0; !inserted && i < tmp.Count; i++)
            {
                if (tmp[i] > key)
                {
                    tmp.Insert(i, key);
                    ret.Insert(i, item);
                    inserted = true;
                }
            }

            if (!inserted)
			{
				tmp.Add(key);
				ret.Add(item);
			}
		}

		return ret;
	}

    public static IEnumerable<TResult> DDZip<T1, T2, TResult>(this IEnumerable<T1> self, IEnumerable<T2> other, Func<T1, T2, TResult> getter)
    {
        var c1 = self.GetEnumerator();
        var c2 = other.GetEnumerator();
        while (c1.MoveNext() && c2.MoveNext())
        {
            yield return getter(c1.Current, c2.Current);
        }
    }
    
	public static IEnumerable<TResult> DDSelect<T, TResult>(this IEnumerable<T> self, Func<T, TResult> selector)
	{
		foreach (var item in self)
		{
			yield return selector(item);
		}
	}
	
	public static T DDLast<T>(this IEnumerable<T> self)
	{
		T ret = default(T);
		bool has = false;
		foreach (var item in self)
		{
			has = true;
			ret = item;
		}
		if (!has)
			throw new InvalidOperationException();
		return ret;
	}
	
	public static T DDFirst<T>(this IEnumerable<T> self)
	{
		foreach (var item in self)
		{
			return item;
		}
		throw new InvalidOperationException();
	}

	public static string DDJoinToString<T>(this IEnumerable<T> self, string separator)
	{
		return string.Join (separator, self.Select (it => it + "").ToArray ());
	}

}