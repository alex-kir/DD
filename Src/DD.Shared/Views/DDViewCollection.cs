//
//  DDViewCollection.cs
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
using System.Collections.Generic;
using System.Collections;

public class DDViewCollection : ICollection<DDView>
{
    List<DDView> _views = new List<DDView>();
    DDView _owner;

    public DDViewCollection(DDView owner)
    {
        _owner = owner;
    }

    public void Add(DDView view)
    {
        _views.Add(view);
        _owner.Children.Add(view);
        _owner.OnAddSubview(view);
    }

    public void Clear()
    {
        foreach (var view in _views)
        {
            _owner.Children.Remove(view);
        }
        _views.Clear();
        _owner.OnClearSubviews();
    }

    public bool Contains(DDView item)
    {
        return _views.Contains(item);
    }

    public void CopyTo(DDView[] array, int arrayIndex)
    {
        _views.CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return _views.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(DDView item)
    {
        var ret = _views.Remove(item);
        _owner.Children.Remove(item);
        _owner.OnRemoveSubview(item);
        return ret;
    }

    public IEnumerator<DDView> GetEnumerator()
    {
        return _views.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<DDView>)this).GetEnumerator();
    }
}
