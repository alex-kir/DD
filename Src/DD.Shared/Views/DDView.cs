//
//  DDView.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DDView : DDNode
{
    [Flags]
    public enum Autoresizing /// That is flexible
    {
        None = 0,
        Left = 1 << 0,
        Width = 1 << 2,
        Right = 1 << 1,
        Bottom = 1 << 3,
        Height = 1 << 4,
        Top = 1 << 5,
    };

    public Autoresizing AutoresizingMask;

    public DDViewCollection SubViews { get; protected set; }


    public DDSprite BackgroundSprite { get; private set; }
    public DDColor BackgroundColor
    {
        get { return BackgroundSprite.Color; }
        set { BackgroundSprite.Color = value; BackgroundSprite.Visible = value.A > 0; }
    }

	public bool UserInteractionEnabled { get; set; }

    public DDView(float width, float height)
    {
        this.SetSize(width, height);
		this.UserInteractionEnabled = true;

        SubViews = new DDViewCollection(this);

        BackgroundSprite = Children.Add(new DDSprite(), -1000);
        BackgroundSprite.ScaleXY = this.Size / BackgroundSprite.Size;
        BackgroundSprite.AnchorPoint = DDVector.LeftBottom;
        BackgroundColor = DDColor.WhiteTransparent;
    }

    public void ResizeView(float width, float height)
    {
        ResizeView(new DDVector(width, height));
    }

    public virtual void ResizeView(DDVector size)
    {
        if (Size == size)
            return;

        foreach (var subview in SubViews)
        {
            var mask = subview.AutoresizingMask;
            bool x1 = (mask & Autoresizing.Left) != 0;
            bool x2 = (mask & Autoresizing.Width) != 0;
            bool x3 = (mask & Autoresizing.Right) != 0;

            bool y1 = (mask & Autoresizing.Bottom) != 0;
            bool y2 = (mask & Autoresizing.Height) != 0;
            bool y3 = (mask & Autoresizing.Top) != 0;

            float x = subview.Position.X;
            float y = subview.Position.Y;
            float width = subview.Size.Width;
            float height = subview.Size.Height;
            _resize(Size.Width, size.Width, x, width, x1, x2, x3, out x, out width);
            _resize(Size.Height, size.Height, y, height, y1, y2, y3, out y, out height);
            subview.SetPosition(x, y);
            subview.ResizeView(width, height);
        }

        Size = size;

        OnAfterResize();
    }

    private static void _resize(float psz, float npsz, float pos, float sz, bool f1, bool f2, bool f3, out float rpos, out float rsz)
    {
        if (f1 && f2 && !f3) // Left + Width
        {
            float d1 = psz - (pos + sz / 2);
            rpos = (pos / (psz - d1)) * (npsz - d1);
            rsz = (sz / (psz - d1)) * (npsz - d1);
        }
        else if (f1 && !f2 && f3) // Left + Right
        {
            rpos = (pos / psz) * npsz;
            rsz = sz;
        }
        else if (!f1 && f2 && f3) // Width + Right
        {
            float d1 = pos - sz / 2;
            rpos = ((pos - d1) / (psz - d1)) * (npsz - d1) + d1;
            rsz = (sz / (psz - d1)) * (npsz - d1);
        }
        else if (f1 && !f2 && !f3) // Left
        {
            rpos = npsz - (psz - pos);
            rsz = sz;
        }
        else if (!f1 && f2 && !f3) // Width
        {
            float d1 = psz - sz;
            rsz = (sz / (psz - d1)) * (npsz - d1);
            //rpos = ((pos - sz / 2) / psz) * npsz + (rsz / 2);
            rpos = (pos - sz / 2) + (rsz / 2);
        }
        else if (!f1 && !f2 && f3) // Right
        {
            rpos = pos;
            rsz = sz;
        }
        else // (!x1 && !x2 && !x3) || (x1 && x2 && x3)
        {
            rpos = (pos / psz) * npsz;
            rsz = (sz / psz) * npsz;
        }
    }

    public virtual void OnAfterResize()
    {
        BackgroundSprite.ScaleXY = this.Size / BackgroundSprite.Size;
		BackgroundSprite.SetPosition(0, 0);
    }

    internal virtual void OnAddSubview(DDView view)
    {
    }

    internal virtual void OnClearSubviews()
    {
    }

    internal virtual void OnRemoveSubview(DDView item)
    {
    }

	internal virtual void OnTouches(params DDTouch [] touches)
	{
		if (UserInteractionEnabled)
		{
			foreach (var view in SubViews)
	        {
				var tt = touches.Where(it => view.Contains(it.GetPosition(view))).ToArray();
				if (tt.Length > 0)
					view.OnTouches(tt);
	        }
		}
	}
}
