//
//  DDVerticalFlowView.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DDVerticalFlowView : DDView
{
    List<DDNode> _saved;
    List<DDView> _subviews;
    float[] _subviewLowerBounds;

    public DDVerticalFlowView(float w, float h)
        : base(w, h)
    {
        BackgroundColor = new DDColor(0.94f, 0.94f, 0.94f);
        _saved = _children.ToList();
    }

    internal override void OnAddSubview(DDView view)
    {
        base.OnAddSubview(view);
        AutoArrageSubviews();
    }

    internal override void OnRemoveSubview(DDView item)
    {
        base.OnRemoveSubview(item);
        AutoArrageSubviews();
    }

    internal override void OnClearSubviews()
    {
        base.OnClearSubviews();
        AutoArrageSubviews();
    }

	public override void OnAfterResize ()
	{
		base.OnAfterResize ();
		AutoArrageSubviews();
	}

    private void AutoArrageSubviews()
    {
        float y = 0;
        foreach (var view in SubViews.Reverse())
        {
            view.SetPosition(this.Size.Width / 2, y + view.Size.Height / 2);
            view.AutoresizingMask = Autoresizing.Width | Autoresizing.Bottom;
            view.ResizeView(this.Size.Width, view.Size.Height);
            y += view.Size.Height + 1;
        }
        this.SetSize(this.Size.Width, y);
        _subviews = SubViews.ToList();
        _subviewLowerBounds = _subviews.DDSelect(it => it.Position.Y - it.Size.Y).ToArray();
    }

    public class ReverseComparer<T>: IComparer<T> where T : IComparable
    {
        public static ReverseComparer<T> Default = new ReverseComparer<T>();
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }
        
    public override void Visit(DDRenderer renderer)
    {
        if (!Visible)
        {
            return;
        }

        foreach (var child in _saved)
            child.Visit(renderer);


        float top = WorldToNodeTransform().TransformPoint(DDDirector.Instance.WinSize).Y;
        float bottom = WorldToNodeTransform().TransformPoint(DDVector.Zero).Y;

        int index = Array.BinarySearch<float>(_subviewLowerBounds, top, ReverseComparer<float>.Default);
        if (index < 0)
            index = (-index) - 1;
        for (int i = index, n = _subviews.Count; i < n; i++)
        {
            var subview = _subviews[i];
            subview.Visit(renderer);
            if (subview.Position.Y + subview.Size.Y < bottom)
                break;
        }
    }
}
