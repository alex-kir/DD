//
//  DDTextInputView.cs
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

public class DDTextInputView : DDTextView
{
    string _text = "";
    public string InputText 
	{ 
		get { return _text; } 
		set 
		{
			_text = value;
			Text = " " + _text + "|";
		}
	}
	public DDKeyboardView.Layout KeyboardLayout;

    public DDTextInputView(string text, float width, float height)
        : base(text, width, height)
    {
        TextAlign = DDVector.LeftMiddle;
        TextColor = DDColor.Black;
        BackgroundColor = new DDColor(0.74f, 0.74f, 0.74f);
        InputText = text;
    }

    public override void OnAfterResize()
    {
        base.OnAfterResize();
    }

	internal override void OnTouches (params DDTouch[] touches)
	{
		base.OnTouches(touches);
		if (touches.FirstOrDefault(it => it.Phase == DDTouchPhase.Began) != null)
		{
			var keyboard = new DDKeyboardView(KeyboardLayout ?? DDKeyboardView.DefaultLayout);
	        keyboard.TextInputView = this;
	        DDWindow.FindWindow(this).ShowKeyboard(keyboard);
		}
	}

    internal void OnKeyboardInsertChar(string ch)
    {
        InputText = InputText + ch;
    }

    internal void OnKeyboardDeleteChar()
    {
		if (InputText.Length > 0)
			InputText = InputText.Substring(0, InputText.Length - 1);
    }


}
