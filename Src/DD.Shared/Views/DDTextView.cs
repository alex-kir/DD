//
//  DDTextView.cs
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

public class DDTextView : DDView
{
    DDLabel label;
    string _text = "";
    public string Text
    {
        get { return _text; }
        set {
			if (_text != value)
			{
				_text = value;
				label.Text = Linewrap(_text);
			}
		}
    }

    public DDVector TextAlign
    {
        get { return label.AnchorPoint; }
        set { label.AnchorPoint = value; label.Position = this.Size * value; }
    }

    public float TextSize {
        get { return label.Scale * label.Font.LineHeight; }
        set { label.Scale = value / label.Font.LineHeight; }
    }

    public DDColor TextColor
    {
        get { return label.Color; }
        set { label.Color = value; }
    }

    public bool LinewrapEnabled = false;
	private bool _elasticHeight = false;

    public DDTextView(string text, float width, float height, bool elasticHeight = false)
        : base(width, height)
    {
		_elasticHeight = elasticHeight;
        label = this.Children.Add(new DDLabel(""), -1);
        Text = text;
        TextAlign = DDVector.CenterMiddle;
        TextSize = 30;
        TextColor = DDColor.Black;
		if (_elasticHeight)
		{
			this.SetSize(width, label.Size.Height * label.ScaleY);
			//BackgroundSprite.ScaleXY = this.Size / BackgroundSprite.Size;
		}
    }

    public override void OnAfterResize()
    {
        label.Text = Linewrap(_text);
		if (_elasticHeight)
		{
			this.SetSize(this.Size.Width, label.Size.Height * label.ScaleY);
		}
		label.Position = this.Size * label.AnchorPoint;
		base.OnAfterResize();
	}

    private string Linewrap(string text)
    {
        if (!LinewrapEnabled)
            return text;

        var words = new Queue<string>(text.Split(new[] { ' ' }, StringSplitOptions.None));
        if (words.Count == 0)
            return text;

        List<string> lines = new List<string>();

        string line = words.Dequeue();
        while (words.Count > 0)
        {
            if (label.Font.MeasureString(line + " " + words.Peek()).Width * label.Scale < this.Size.Width)
            {
                line = line + " " + words.Dequeue();
            }
            else
            {
                lines.Add(line);
                line = (words.Count == 0) ? null : words.Dequeue();
            }
        }

        if (line != null)
            lines.Add(line);

        return string.Join("\n", lines.ToArray());
    }
}
