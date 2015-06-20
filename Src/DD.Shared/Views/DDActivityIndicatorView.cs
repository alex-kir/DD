//
//  DDActivityIndicatorView.cs
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
using System.Collections.Generic;
using System;

public class DDActivityIndicatorView : DDView
{
    public DDActivityIndicatorView()
        : base(50, 50)
    {
        this.AutoresizingMask = Autoresizing.Left | Autoresizing.Right | Autoresizing.Top | Autoresizing.Bottom;
        var dots = new List<DDSprite>();
        var center = this.Size * DDVector.CenterMiddle;
        for (int i = 0; i < 360; i += 360 / 12)
        {
            var dot = this.Children.Add(new DDSprite("DDActivityIndicatorViewDot"));
            dot.Position = center + DDVector.FromAngle(i) * 20;
            dot.Scale = 0.5f;
            dots.Add(dot);

        }
        int index = 0;
		Action animation = () =>
		{
			for (int i = 0; i < dots.Count; i++)
			{
				int j = (index + i) % dots.Count;
				float k = DDMath.Nurbs(j / (float)dots.Count, 1, 0.05f, 0.05f, 0.05f);
				dots[i].Color = new DDColor(this.Color, k);
			}
			index++;
		};
		animation();
        this.StartAction(aa => aa.Repeat(aa.Exec(animation) + aa.Delay(0.1f)));
    }
}
