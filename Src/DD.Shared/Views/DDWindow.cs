//
//  DDWindow.cs
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

using System.Collections;

public class DDWindow : DDScene
{
	public float DPI = 132; // iPad 1 ppi

    DDView _windowView;

    DDKeyboardView _keyboardView = null;

    DDView _rootView = null;
    public DDView RootView
    {
        get { return _rootView; }
        set
        {
            _windowView.SubViews.Remove(_rootView);
            _rootView = value;
            _windowView.SubViews.Add(_rootView);
            UpdateWindowViewsPotions();
        }
    }

	public static DDWindow FindWindow(DDNode view)
	{
		while (view != null)
		{
			DDWindow ret = view as DDWindow;
			if (ret != null)
				return ret;
			view = view.Parent;
		}
		return null;
	}

    public static T FindWindow<T>(DDNode view) where T : DDWindow
    {
        while (view != null)
        {
            T ret = view as T;
            if (ret != null)
                return ret;
            view = view.Parent;
        }
        return null;
    }

    public DDWindow()
    {
        this.Size = DDDirector.Instance.WinSize;
        _windowView = this.Children.Add(new DDView(this.Size.Width, this.Size.Height));
        _windowView.Position = this.Size * DDVector.CenterMiddle;

        this.StartAction(aa => aa.Update(OnUpdate));
		DDTouchDispatcher.Instance.AddHandler(this, 0, OnTouches);
    }

    public void ShowKeyboard(DDKeyboardView keyboard)
    {
        DDDirector.Instance.PostMessage(delegate
        {
            if (_keyboardView != null && _keyboardView != keyboard)
            {
                _windowView.SubViews.Remove(_keyboardView);
                _keyboardView = null;
            }

            _windowView.SubViews.Add(keyboard);
            _keyboardView = keyboard;

            UpdateWindowViewsPotions();
        });
    }

    public void HideKeyboard()
    {
        DDDirector.Instance.PostMessage(delegate
        {
            if (_keyboardView != null)
            {
                _windowView.SubViews.Remove(_keyboardView);
                _keyboardView = null;
                UpdateWindowViewsPotions();
            }
        });
    }

    private void UpdateWindowViewsPotions()
    {
        if (_rootView != null && _keyboardView != null)
        {
            var wsz = _windowView.Size;
            _rootView.ResizeView(wsz.Width, wsz.Height - 300);
            _rootView.SetPosition(wsz.Width / 2, wsz.Height / 2 + 150);
            _rootView.AutoresizingMask = DDView.Autoresizing.Width | DDView.Autoresizing.Height;
            _rootView.ZOrder = 0;

            _keyboardView.ResizeView(wsz.Width, 300);
            _keyboardView.SetPosition(wsz.Width / 2, 150);
            _keyboardView.AutoresizingMask = DDView.Autoresizing.Width | DDView.Autoresizing.Top;
            _keyboardView.ZOrder = 1;
        }
        else if (_rootView != null)
        {
            var wsz = _windowView.Size;
            _rootView.ResizeView(wsz);
            _rootView.SetPosition(wsz * DDVector.CenterMiddle);
            _rootView.AutoresizingMask = DDView.Autoresizing.Width | DDView.Autoresizing.Height;
            _rootView.ZOrder = 0;
        }
        else if (_keyboardView != null)
        {
            var wsz = _windowView.Size;
            _keyboardView.ResizeView(wsz.Width, 300);
            _keyboardView.SetPosition(wsz.Width / 2, 150);
            _keyboardView.AutoresizingMask = DDView.Autoresizing.Width | DDView.Autoresizing.Top;
            _keyboardView.ZOrder = 1;
        }
        else
        {
        }
    }


    void OnUpdate()
    {
        float s = DDDirector.Instance.DPI / this.DPI;
        if (s < 0.75f)
            s = 0.5f;
        else if (s < 1.5f)
            s = 1;
        else if (s < 3)
            s = 2;
        else
            s = 4;

		this.Scale = s;
        this.Size = DDDirector.Instance.WinSize / this.Scale;
        _windowView.ResizeView(this.Size);
        _windowView.Position = this.Size * DDVector.CenterMiddle;
    }

	void OnTouches (DDTouch[] touches)
	{
		_windowView.OnTouches(touches);
	}
}
