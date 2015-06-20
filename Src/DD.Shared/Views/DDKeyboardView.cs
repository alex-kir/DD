//
//  DDKeyboardView.cs
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

public class DDKeyboardView : DDView
{
    public static string ButtonSlicedSpriteName = null;
    public static string ButtonSound = null;
  
    public class Layout
    {
        public List<Line> Lines = new List<Line>();
		public void AddKey(int lineIndex, Key key)
		{
			while (lineIndex >= Lines.Count)
			{
				Lines.Add(new Line());
			}
			Lines[lineIndex].Keys.Add(key);
		}
    }

    public class Line
    {
        public List<Key> Keys = new List<Key>();
        public int Width { get { return Keys.DDSum(key => key.Width); } }
    }

    public class Key
    {
        public string Text;
		public string Command; // char, delete, hide
        public int Width;
    }

    static Layout _defaultLayout = null;
    public static Layout DefaultLayout
    {
        get
        {
            if (_defaultLayout == null)
            {
                _defaultLayout = new Layout();

                _defaultLayout.AddKey(0, new Key { Text = "1", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "2", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "3", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "4", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "5", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "6", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "7", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "8", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "9", Command = "char", Width = 1 });
                _defaultLayout.AddKey(0, new Key { Text = "0", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "Q", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "W", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "E", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "R", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "T", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "Y", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "U", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "I", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "O", Command = "char", Width = 1 });
                _defaultLayout.AddKey(1, new Key { Text = "P", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "A", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "S", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "D", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "F", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "G", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "H", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "J", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "K", Command = "char", Width = 1 });
                _defaultLayout.AddKey(2, new Key { Text = "L", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "Z", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "X", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "C", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "V", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "B", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "N", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "M", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "'", Command = "char", Width = 1 });
                _defaultLayout.AddKey(3, new Key { Text = "<-DEL", Command = "delete", Width = 2 });
                _defaultLayout.AddKey(4, new Key { Text = "HIDE", Command = "hide", Width = 2 });
                _defaultLayout.AddKey(4, new Key { Text = " ", Command = "char", Width = 5 });
                _defaultLayout.AddKey(4, new Key { Text = ",", Command = "char", Width = 1 });
                _defaultLayout.AddKey(4, new Key { Text = ".", Command = "char", Width = 1 });
                _defaultLayout.AddKey(4, new Key { Text = ";", Command = "char", Width = 1 });
              //_defaultLayout.AddKey(4, new Key { Text = "OK", Command = "ok", Width = 1 });
            }
            return _defaultLayout;
        }
    }

	static Layout _russianLayout = null;
	public static Layout RussianLayout
	{
		get
		{
			if (_russianLayout == null)
			{
				_russianLayout = new Layout();
				
				_russianLayout.AddKey(0, new Key { Text = "1", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "2", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "3", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "4", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "5", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "6", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "7", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "8", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "9", Command = "char", Width = 1 });
				_russianLayout.AddKey(0, new Key { Text = "0", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Й", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Ц", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "У", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "К", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Е", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Н", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Г", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Ш", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Щ", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "З", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Х", Command = "char", Width = 1 });
				_russianLayout.AddKey(1, new Key { Text = "Ъ", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Ф", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Ы", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "В", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "А", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "П", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Р", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "О", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Л", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Д", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Ж", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Э", Command = "char", Width = 1 });
				_russianLayout.AddKey(2, new Key { Text = "Ё", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Я", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Ч", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "С", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "М", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "И", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Т", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Ь", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Б", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "Ю", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "'", Command = "char", Width = 1 });
				_russianLayout.AddKey(3, new Key { Text = "<-DEL", Command = "delete", Width = 2 });
				_russianLayout.AddKey(4, new Key { Text = "HIDE", Command = "hide", Width = 2 });
				_russianLayout.AddKey(4, new Key { Text = " ", Command = "char", Width = 7 });
				_russianLayout.AddKey(4, new Key { Text = ",", Command = "char", Width = 1 });
				_russianLayout.AddKey(4, new Key { Text = ".", Command = "char", Width = 1 });
				_russianLayout.AddKey(4, new Key { Text = ";", Command = "char", Width = 1 });
			}
			return _russianLayout;
		}
	}

    public DDTextInputView TextInputView = null;

    public DDKeyboardView(Layout layout)
        : base(DDDirector.Instance.WinSize.Width, 300)
    {
        BackgroundColor = new DDColor(0.14f, 0.14f, 0.14f);

        var keyboardRect = new DDRectangle(DDVector.Zero, this.Size);
        var maxLineWidth = layout.Lines.DDMax(line => line.Width);
        for (int i = 0, ii = layout.Lines.Count; i < ii; i++)
        {
            var line = layout.Lines[i];
            var lineRect = keyboardRect.Grid(0, ii - i - 1, 1, ii);

            var width0 = lineRect.Size.Width / maxLineWidth;
            int prevKeyWidth = 0;
            for (int j = 0, jj = line.Keys.Count; j < jj; j++)
            {
                var key = line.Keys[j];

                var width = width0 * key.Width;
                var x = width0 * prevKeyWidth + width * 0.5f + width0 * (maxLineWidth - line.Width) * 0.5f;
                prevKeyWidth += key.Width;
				var button = new DDButtonView(key.Text, width - 2, lineRect.Size.Height - 2);
				if (key.Command == "hide")
				{
					button.Text = "";
					var sz = DDMath.Min(button.Size.Width, button.Size.Height);
					var img = new DDImageView("DDKeyboardView_Hide", sz, sz);
					img.AutoresizingMask = Autoresizing.Left | Autoresizing.Right | Autoresizing.Top | Autoresizing.Bottom;
					img.Position = button.Size * DDVector.CenterMiddle;
					button.SubViews.Add(img);
				}
				else if (key.Command == "delete")
				{
					button.Text = "";
					var sz = DDMath.Min(button.Size.Width, button.Size.Height);
					var img = new DDImageView("DDKeyboardView_Delete", sz, sz);
					img.AutoresizingMask = Autoresizing.Left | Autoresizing.Right | Autoresizing.Top | Autoresizing.Bottom;
					img.Position = button.Size * DDVector.CenterMiddle;
					button.SubViews.Add(img);
				}

                button.SetPosition(x, lineRect.Center.Y);
                button.AutoresizingMask = Autoresizing.None;
                button.Action = delegate { OnKey(key); };
                this.SubViews.Add(button);				
            }
        }
    }

    void OnKey(Key key)
    {
        //Action action = null;

        if (key.Command == "ok")
        {
            DDWindow.FindWindow(this).HideKeyboard();
            //action = () => { this.RemoveFromParent(); if (onOK != null) onOK(currentValue); };
        }
        else if (key.Command == "hide")
        {
            DDWindow.FindWindow(this).HideKeyboard();
            //action = () => { this.RemoveFromParent(); if (onCancel != null) onCancel(); };
        }
        else if (key.Command == "char")
        {
            TextInputView.OnKeyboardInsertChar(key.Text);
        }
        else if (key.Command == "delete")
        {
            TextInputView.OnKeyboardDeleteChar();
            //action = () => { if (currentValue.Length > 0) currentValue = currentValue.Substring(0, currentValue.Length - 1); };
        }
        //else
        //{
        //    //action = () => { currentValue= currentValue + text; if (onChange != null) onChange(currentValue); };
        //}

    }
}
