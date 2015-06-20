//
//  DDButtonView.cs
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
using System.Linq;

public class DDButtonView : DDTextView
{
    public Action Action;

    DDNode background = null;
    string backgroundImageName = null;
	bool isBackgroundImageSliced = true;

    public DDColor SelectedColor = new DDColor(0.85f, 0.85f, 0.85f);

    public DDButtonView(string text, float w, float h)
        : base(text, w, h)
    {
        BackgroundColor = DDColor.White;
    }

    internal override void OnTouches(params DDTouch[] touches)
    {
		if (touches.FirstOrDefault(it => it.Phase == DDTouchPhase.Began) != null)
		{
	        if (Action != null)
	            DDDirector.Instance.PostMessage(Action);
			DDAudio.Instance.PlayEffect("DDButton_Sound");
			if (BackgroundSprite.Visible)
			{
		        var color = BackgroundColor;
		        BackgroundColor = SelectedColor;
		        BackgroundSprite.StartAction(aa => aa.ColorTo(0.1f, color));
			}
			else if (background != null)
			{
				background.StartAction(aa => aa.ColorTo(0.1f, DDColor.Gray) + aa.ColorTo(0.1f, DDColor.White));
			}
		}
    }

    public override void OnAfterResize()
    {
        base.OnAfterResize();

        if (backgroundImageName != null)
			SetBackgroundImage(backgroundImageName, isBackgroundImageSliced);
    }

    public void SetBackgroundImage(string name, bool isSliced)
    {
        // TODO improve performance
        if (background != null)
            background.RemoveFromParent();
        backgroundImageName = name;
		isBackgroundImageSliced = isSliced;

		if (isBackgroundImageSliced)
            background = this.Children.Add(new DDSlicedSprite(name, Size), -2);
		else
            background = this.Children.Add(new DDSprite(name), -2);

        background.ScaleXY = this.Size / background.Size;
        background.AnchorPoint = DDVector.LeftBottom;
		BackgroundSprite.Visible = false;
    }
}
