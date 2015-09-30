//
//  DDActions.cs
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

public class DDIntervalAnimation<T> : DDIntervalAnimation
{
    private T _from;
    private readonly T _to;
    private readonly Func<DDNode,T> _getter;
    private readonly Action<DDNode, T> _setter;
    private readonly Func<T, T, float, T> _lerp;

    public DDIntervalAnimation(float duration, T to, Func<DDNode, T> getter, Action<DDNode, T> setter, Func<T, T, float, T> lerp)
        : base(duration)
    {
        _to = to;
        _getter = getter;
        _setter = setter;
        _lerp = lerp;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        _from = _getter(target);
    }

    protected override void Update(DDNode target, float t)
    {
        _setter(target, _lerp(_from, _to, t));
    }
}

public class DDColorTo : DDIntervalAnimation<DDColor>
{
    public DDColorTo(float duration, DDColor color)
        : base(duration, color, n => n.Color, (n, v) => n.Color = v, DDColor.Lerp)
    { }

    public DDColorTo(float duration, float r, float g, float b, float a = 1)
        : base(duration, new DDColor(r, g, b, a), n => n.Color, (n, v) => n.Color = v, DDColor.Lerp)
    { }

    public DDColorTo(float duration, DDColor rgb, float a)
        : base(duration, new DDColor(rgb.R, rgb.G, rgb.B, a), n => n.Color, (n, v) => n.Color = v, DDColor.Lerp)
    { }

}

public class DDUpdate : DDAnimation
{
    Action<float> action = null;
    float delay = 0;
    float elapsed = 0;

    public DDUpdate(Action action) : this(dt => action(), 0) { }

    public DDUpdate(Action action, float delay) : this(dt => action(), delay) { }
    public DDUpdate(Action<float> action) : this(action, 0) { }

    public DDUpdate(Action<float> action, float delay)
    {
        this.action = action;
        this.delay = delay;
    }

    protected override void Step(DDNode target, float t)
    {
        this.elapsed += t;
        if (this.elapsed > this.delay)
        {
            this.action(this.elapsed);
            this.elapsed = 0;
        }
    }

    public override bool IsDone
    {
        get { return false; }
    }
}

public class DDAnimate : DDIntervalAnimation
{
    string _name;
    public DDAnimate(string name, float duration = 1f / 24f)
        : base(duration)
    {
        _name = name;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        DDSprite spr = target as DDSprite;
        if (spr != null)
            spr.SetTexture(_name);
    }
}

//public class DDKill : DDInstantAction
//{
//    public DDKill()
//        : base(null)
//    {
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//        target.RemoveFromParent();
//    }
//}