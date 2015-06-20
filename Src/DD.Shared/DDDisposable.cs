//
//  DDDisposable.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013-2014 - Alexander Kirienko
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

#if NETFX_CORE
using WindowsStore;
#endif

public class DDDisposable : IDisposable
{
	public class Collection : ICollection<IDisposable>, IDisposable
	{
		List<IDisposable> _collection = new List<IDisposable>();
		bool _disposed = false;

		public void Dispose ()
		{
			_disposed = true;
			_collection.ForEach(it => it.Dispose());
			_collection.Clear();
		}

		public void Add (IDisposable item)
		{
			if (_disposed)
				item.Dispose();
			else
				_collection.Add(item);
		}

		public void Clear ()
		{
			_collection.Clear();
		}

		public bool Contains (IDisposable item)
		{
			return _collection.Contains(item);
		}

		public void CopyTo (IDisposable[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public bool Remove (IDisposable item)
		{
			return _collection.Remove(item);
		}

		public int Count {
			get {
				return _collection.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public IEnumerator<IDisposable> GetEnumerator ()
		{
			return _collection.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _collection.GetEnumerator();
		}
	}

	Action _action;
	public DDDisposable(Action action)
	{
		if (action == null)
			throw new ArgumentNullException("action");
		_action = action;
	}

	public void Dispose()
	{
		_action();
	}
}

