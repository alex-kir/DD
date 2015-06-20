//
//  DDNavigationView.cs
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

public class DDNavigationView : DDView
{
	public class Page : DDView
	{
		internal protected DDNavigationView NavigationView;
		internal protected DDButtonView backButton;
		DDSprite header;
		DDView content;

		public Page(float width, float height) : base(width, height)
		{
			backButton = new DDButtonView("", 32, 32);
			backButton.ZOrder = 1000;
			backButton.SetPosition(25, height - 25);
			backButton.SetBackgroundImage("DDNavigationView_Back", false);
			backButton.Action = () => NavigationView.PopView();
			backButton.AutoresizingMask = Autoresizing.Bottom | Autoresizing.Right;
			SubViews.Add(backButton);
			
            header = this.Children.Add(new DDSprite("DDNavigationViewHeader"), 999);
			header.SetPosition(Size.Width / 2, Size.Height - 25);
			header.ScaleXY = new DDVector(Size.Width, 50) / header.Size;
			header.Color = new DDColor(0.94f, 0.94f, 0.94f);
		}

		protected void SetContent(DDView view)
		{
			if (content != null)
				this.SubViews.Remove(content);
			content = view;

			view.AutoresizingMask = Autoresizing.Width | Autoresizing.Height;
			view.Position = (this.Size + new DDVector(0, -50)) * DDVector.CenterMiddle;
			view.ResizeView(Size.Width, Size.Height - 50);
			this.SubViews.Add(view);
		}

		public override void OnAfterResize()
		{
			base.OnAfterResize();
			header.SetPosition(Size.Width / 2, Size.Height - 25);
			header.ScaleXY = new DDVector(Size.Width, 50) / header.Size;
		}

		public virtual void OnPageAppear()
		{
		}
	}

	Stack<DDNavigationView.Page> _views = new Stack<DDNavigationView.Page>();
	public Stack<DDNavigationView.Page> StackedViews { get { return _views; } }

    public DDNavigationView(float width, float height)
        : base(width, height)
    {
    }

	public void PushView(DDNavigationView.Page view)
    {
		view.NavigationView = this;
        view.AutoresizingMask = Autoresizing.Width | Autoresizing.Height;
        view.ResizeView(Size.Width, Size.Height);
        view.AnchorPoint = DDVector.CenterMiddle;
        if (_views.Count == 0)
        {
            view.Position = this.Size * DDVector.CenterMiddle;
        }
        else
        {
            _views.First()
				.StartAction(aa => aa.MoveTo(0.2f, -Size.Width * 0.5f, Size.Height * 0.5f)
			                           + aa.Hide());
            // TODO: Animation with resizing
            view.SetPosition(Size.Width * 1.5f, Size.Height * 0.5f);
			view.StartAction(aa => aa.Delay(0.1f)
			                 + aa.MoveTo(0.6f, Size.Width * 0.5f, Size.Height * 0.5f).EaseBounceOut());
			view.StartAction(aa => aa.Delay(0.2f) + aa.Sound("DDNavigationView_Sound", false));
			UserInteractionEnabled = false;
			this.StartAction(aa=> aa.Delay(0.5f)+ aa.Exec(() => UserInteractionEnabled = true));
        }
        _views.Push(view);
        SubViews.Add(view);

        view.backButton.Visible = (_views.Count > 1);
    }

    public void PopView()
    {
        if (_views.Count > 1)
        {
            var view = _views.Pop();
            view.StartAction(aa =>
			    aa.Exec(() => UserInteractionEnabled = false)
             	+ aa.MoveTo(0.2f, Size.Width * 1.5f, Size.Height * 0.5f)
                + aa.Exec(() => { this.SubViews.Remove(view); view.Dispose(); DDTextureManager.Instance.PurgeUnusedTexture(); })
			    + aa.Exec(() => UserInteractionEnabled = true));
            
			var first = _views.First();
			first.StartAction(aa => aa.Show() + aa.Delay(0.1f)
			                  + aa.MoveTo(0.6f, Size.Width * 0.5f, Size.Height * 0.5f).EaseBounceOut());
			first.StartAction(aa => aa.Delay(0.2f) + aa.Sound("DDNavigationView_Sound", false));
			first.OnPageAppear();
        }
    }

    public void PopToRoot()
    {
    }

}
