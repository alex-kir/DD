//
//  DDAction.cs
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

public class DDAnimation
{
    public static void Start(DDAnimation action, DDNode target)
    {
        action.Start(target);
    }

    public static void Step(DDAnimation action, DDNode target, float f)
    {
        action.Step(target, f);
    }

    private readonly Action<DDNode> startAction;
    private readonly Action<DDNode, float> stepAction;
    private readonly Func<bool> isDoneFunc;

    internal string Name;
    public virtual bool IsDone { get { return isDoneFunc == null || isDoneFunc(); } }


    public DDAnimation(Action<DDNode> startAction = null, Action<DDNode, float> stepAction = null, Func<bool> isDoneFunc = null)
    {
        this.startAction = startAction;
        this.stepAction = stepAction;
        this.isDoneFunc = isDoneFunc;
    }

    protected virtual void Start(DDNode target)
    {
        if (startAction != null)
            startAction(target);
    }

    protected virtual void Step(DDNode target, float t)
    {
        if (stepAction != null)
            stepAction(target, t);
    }

    public class Sequence : DDAnimation
    {
        List<DDAnimation> _actions = new List<DDAnimation>();
        int _index = 0;

        public Sequence Add(DDAnimation action)
        {
            if (action != null)
            {
                _actions.Add(action);
            }
            return this;
        }

        public override bool IsDone { get { return _index == _actions.Count; } }

        protected override void Start(DDNode target)
        {
            base.Start(target);
            _index = 0;
            DDAnimation.Start(_actions[_index], target);
        }

        protected override void Step(DDNode target, float t)
        {
            base.Step(target, t);
            DDAnimation.Step(_actions[_index], target, t);
            if (_actions[_index].IsDone)
            {
                _index++;
                if (!IsDone)
                {
                    DDAnimation.Start(_actions[_index], target);
                }
            }
        }
    }

    public class Spawn : DDAnimation
    {
        List<DDAnimation> _actions = new List<DDAnimation>();

        public Spawn Add(DDAnimation action)
        {
            if (action != null)
            {
                _actions.Add(action);
            }
            return this;
        }

        public override bool IsDone { get { return _actions.All(it => it.IsDone); } }

        protected override void Start(DDNode target)
        {
            base.Start(target);
            foreach (var action in _actions)
                DDAnimation.Start(action, target);
        }

        protected override void Step(DDNode target, float t)
        {
            base.Step(target, t);
            foreach (var action in _actions)
            {
                if (!action.IsDone)
                    DDAnimation.Step(action, target, t);
            }
        }
    }

    public static DDAnimation.Sequence operator +(DDAnimation a1, DDAnimation a2)
    {
        return new DDAnimation.Sequence().Add(a1).Add(a2);
    }

    public static DDAnimation.Spawn operator *(DDAnimation a1, DDAnimation a2)
    {
        return new Spawn().Add(a1).Add(a2);
    }

    public static DDAnimation.Spawn operator &(DDAnimation a1, DDAnimation a2)
    {
        return new Spawn().Add(a1).Add(a2);
    }
}

public abstract class DDIntervalAnimation : DDAnimation
{
    public float Duration { get; protected set; }

    private float _elapsed;
    private bool _firstTick;

    public DDIntervalAnimation(float duration)
    {
        if (duration == 0)
        {
            duration = 1E-08f;
        }

        Duration = duration;
    }

    public sealed override bool IsDone
    {
        get { return _elapsed >= Duration; }
    }

    public float Elapsed
    {
        get { return _elapsed; }
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        _elapsed = 0;
        _firstTick = true;
    }

    protected sealed override void Step(DDNode target, float dt)
    {
        if (_firstTick)
        {
            _firstTick = false;
            _elapsed = 0;
        }
        else
        {
            _elapsed += dt;
        }

        Update(target, Math.Min(1, Elapsed / Duration));
    }

    public static void Update(DDIntervalAnimation action, DDNode target, float t01)
    {
        action.Update(target, DDMath.Min(1, t01));
        action._elapsed = action.Duration * t01;
    }

    protected virtual void Update(DDNode target, float t01)
    {
    }

    public class SequenceInterval : DDIntervalAnimation
    {
        List<DDIntervalAnimation> _actions = new List<DDIntervalAnimation>();
        List<float> _ends = new List<float>();
        int _index = 0;

        public SequenceInterval()
            : base(0)
        {
            _ends.Add(0);
        }

        public SequenceInterval Add(DDIntervalAnimation action)
        {
            if (action != null)
            {
                Duration = Duration + action.Duration;
                _actions.Add(action);
                _ends.Add(Duration);
            }
            return this;
        }

        protected override void Start(DDNode target)
        {
            base.Start(target);
            _index = 0;
            DDAnimation.Start(_actions[_index], target);
        }

        protected override void Update(DDNode target, float t01)
        {
            base.Update(target, t01);
            for (; _index < _actions.Count;)
            {
                float tt01 = DDMath.Lerp(0, 1, _ends[_index] / Duration, _ends[_index + 1] / Duration, t01);
                DDIntervalAnimation.Update(_actions[_index], target, DDMath.Min(1, tt01));
                if (t01 < _ends[_index + 1] / Duration)
                    break;
                _index++;
                if (_index < _actions.Count)
                    DDAnimation.Start(_actions[_index], target);
            }
        }
    }

    public class SpawnInterval : DDIntervalAnimation
    {
        List<DDIntervalAnimation> _actions = new List<DDIntervalAnimation>();

        public SpawnInterval()
            : base(0)
        {
        }

        public SpawnInterval Add(DDIntervalAnimation action)
        {
            if (action != null)
            {
                Duration = DDMath.Max(Duration, action.Duration);
                _actions.Add(action);
            }
            return this;
        }

        protected override void Start(DDNode target)
        {
            base.Start(target);
            foreach (var action in _actions)
                DDAnimation.Start(action, target);
        }

        protected override void Update(DDNode target, float t)
        {
            base.Update(target, t);
            foreach (var action in _actions)
            {
                if (!action.IsDone)
                    DDIntervalAnimation.Update(action, target, t / (action.Duration / Duration));
            }
        }
    }

    public static DDIntervalAnimation.SequenceInterval operator +(DDIntervalAnimation a1, DDIntervalAnimation a2)
    {
        return new DDIntervalAnimation.SequenceInterval().Add(a1).Add(a2);
    }

    public static DDIntervalAnimation.SpawnInterval operator *(DDIntervalAnimation a1, DDIntervalAnimation a2)
    {
        return new DDIntervalAnimation.SpawnInterval().Add(a1).Add(a2);
    }

    public static DDIntervalAnimation.SpawnInterval operator &(DDIntervalAnimation a1, DDIntervalAnimation a2)
    {
        return new DDIntervalAnimation.SpawnInterval().Add(a1).Add(a2);
    }
}

public class DDInstantAction : DDIntervalAnimation
{
    Action<DDNode> _action;

    public DDInstantAction()
        : base(0)
    {
        Duration = 0;
        _action = null;
    }

    public DDInstantAction(Action action, Action reverseAction = null)
        : base(0)
    {
        Duration = 0;
        _action = it => {
            if (action != null)
                action();
        };
    }

    public DDInstantAction(Action<DDNode> action, Action reverseAction = null)
        : base(0)
    {
        Duration = 0;
        _action = action;
    }

    protected override void Start(DDNode target)
    {
        if (_action != null)
            _action(target);
    }
}

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

public class DDRepeat : DDIntervalAnimation
{
    private DDIntervalAnimation _other;
    private int _times;
    private int _total;

    public DDRepeat(DDIntervalAnimation other, int times)
        : base(other == null ? 0 : other.Duration * times)
    {
        if (other == null)
        {
            throw new ArgumentNullException("other");
        }

        _times = times;
        _other = other;

        _total = 0;
    }

    protected override void Start(DDNode target)
    {
        _total = 0;
        base.Start(target);
        DDAnimation.Start(_other, target);
    }

    protected override void Update(DDNode target, float dt)
    {
        float t = dt * _times;
        float r = t % 1f;
        if (t > _total + 1)
        {
            DDIntervalAnimation.Update(_other, target, 1f);
            ++_total;
            DDAnimation.Start(_other, target);
            DDIntervalAnimation.Update(_other, target, Math.Min(r, 1f));
        }
        else
        {
            if (dt == 1f)
            {
                r = 1f;
                ++_total;
            }
            DDIntervalAnimation.Update(_other, target, Math.Min(r, 1f));
        }
    }
}

public class DDRepeatForever : DDAnimation
{
    private DDAnimation _repeatedAction;

    public DDRepeatForever(DDAnimation repeatedAction)
    {
        _repeatedAction = repeatedAction;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        DDAnimation.Start(_repeatedAction, target);
    }

    protected override void Step(DDNode target, float t)
    {
        DDAnimation.Step(_repeatedAction, target, t);
        if (_repeatedAction.IsDone)
        {
            DDAnimation.Start(_repeatedAction, target);
        }
    }

    public override bool IsDone
    {
        get { return false; }
    }
}

//public class DDReverseTime : DDIntervalAnimation
//{
//    private DDIntervalAnimation _action;
//
//    public DDReverseTime(DDIntervalAnimation action)
//        : base(action == null ? 0 : action.Duration)
//    {
//        if (action == null)
//        {
//            throw new ArgumentNullException("action");
//        }
//        _action = action;
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//        DDAnimation.Start(_action, target);
//    }
//
//    protected override void Update(DDNode target, float t)
//    {
//        DDIntervalAnimation.Update((DDIntervalAnimation)_action, target, 1f - t);
//    }
//}
