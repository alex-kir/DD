//
//  DDEaseBounce.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DDEaseBounce : DDActionEase
{
    public DDEaseBounce(DDIntervalAnimation action)
        : base(action)
    { }

    public float BounceTime(float time)
    {
        if (time < 1 / 2.75)
        {
            return 7.5625f * time * time;
        }
        else if (time < 2 / 2.75)
        {
            time -= 1.5f / 2.75f;
            return 7.5625f * time * time + 0.75f;
        }
        else if (time < 2.5 / 2.75)
        {
            time -= 2.25f / 2.75f;
            return 7.5625f * time * time + 0.9375f;
        }

        time -= 2.625f / 2.75f;
        return 7.5625f * time * time + 0.984375f;
    }
}