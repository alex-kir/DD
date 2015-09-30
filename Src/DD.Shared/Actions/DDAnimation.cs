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
    public static void Start(DDAnimation action, DDNode target) { action.Start(target); }
    public static void Step(DDAnimation action, DDNode target, float f) { action.Step(target, f); }
#if false
    protected bool _isStarted = false;
    protected virtual void Start(DDNode target)
    {
        if (IsDone)
            _isStarted = false;

        if (_isStarted)
            throw new System.Exception("Action already started");
        _isStarted = true;
    }
    protected virtual void Step(DDNode target, float t)
    {
        if (!_isStarted)
            throw new System.Exception("Action is not started yet");
    }
#else
    protected virtual void Start(DDNode target) { }
    protected virtual void Step(DDNode target, float t) { }
#endif
    internal string Name;
//    internal bool IsStarted { get; private set; }
    public virtual bool IsDone { get { return true; } }

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
    { }

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
            for (; _index < _actions.Count; )
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
    Action _action;

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
        _action = action;
    }

    protected override void Start(DDNode target)
    {
        if (_action != null)
            _action();
    }
}

public class DDPlace : DDInstantAction
{
    private DDVector _position;

    public DDPlace(DDVector position)
    {
        _position = position;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        target.SetPosition(_position.X, _position.Y);
    }
}

public class DDToggleVisibility : DDInstantAction
{
    protected override void Start(DDNode target)
    {
        base.Start(target);
        target.Visible = !target.Visible;
    }
}

public class DDHide : DDInstantAction
{
    protected override void Start(DDNode target)
    {
        target.Visible = false;
    }
}

public class DDShow : DDInstantAction
{
    protected override void Start(DDNode target)
    {
        target.Visible = true;
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

public class DDReverseTime : DDIntervalAnimation
{
    private DDIntervalAnimation _action;

    public DDReverseTime(DDIntervalAnimation action)
        : base(action == null ? 0 : action.Duration)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }
        _action = action;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        DDAnimation.Start(_action, target);
    }

    protected override void Update(DDNode target, float t)
    {
        DDIntervalAnimation.Update((DDIntervalAnimation)_action, target, 1f - t);
    }
}

//public class DDMoveTo : DDIntervalAnimation
//{
//    protected DDVector _endPosition;
//    protected DDVector _startPosition;
//    protected DDVector _delta;
//
//    public DDMoveTo(float duration, DDVector position)
//        : base(duration)
//    {
//        _endPosition = position;
//    }
//
//    public DDMoveTo(float duration, float x, float y)
//        : this(duration, new DDVector(x, y))
//    { }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//        _startPosition = target.Position;
//        _delta = new DDVector(_endPosition.X - _startPosition.X, _endPosition.Y - _startPosition.Y);
//    }
//
//    protected override void Update(DDNode target, float t)
//    {
//        DDVector pos = target.Position;
//        pos.X = _startPosition.X + (_delta.X * t);
//        pos.Y = _startPosition.Y + (_delta.Y * t);
//        target.Position = pos;
//    }
//}

//public class DDMoveBy : DDMoveTo
//{
//    public DDMoveBy(float duration, DDVector moveByAmount)
//        : base(duration, DDVector.Empty)
//    {
//        _delta = moveByAmount;
//    }
//
//    public DDMoveBy(float duration, float x, float y)
//        : this(duration, new DDVector(x, y))
//    { }
//
//    protected override void Start(DDNode target)
//    {
//        DDVector tmp = _delta;
//        base.Start(target);
//        _delta = tmp;
//    }
//}

//public class DDRotateTo : DDIntervalAnimation
//{
//    private DDVector _startRotation;
//    private DDVector _rotation;
//
//    public DDRotateTo(float duration, float rotation)
//        : base(duration)
//    {
//        _rotation = new DDVector(rotation, rotation);
//    }
//
//    public DDRotateTo(float duration, float rotationX, float rotationY)
//        : base(duration)
//    {
//        _rotation = new DDVector(rotationX, rotationY);
//    }
//
//    public DDRotateTo(float duration, DDVector rotation)
//        : base(duration)
//    {
//        _rotation = rotation;
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//        _startRotation = target.RotationXY;
//    }
//
//    protected override void Update(DDNode target, float t)
//    {
//        target.RotationXY = _startRotation + ((_rotation - _startRotation) * t);
//    }
//}
//
//public class DDRotateBy : DDIntervalAnimation
//{
//    private float _angle;
//    private float _startAngle;
//
//    public DDRotateBy(float duration, float angle)
//        : base(duration)
//    {
//        _angle = angle;
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//        _startAngle = target.Rotation;
//    }
//
//    protected override void Update(DDNode target, float t)
//    {
//        target.Rotation = _startAngle + (_angle * t);
//    }
//}

//public class DDScaleTo : DDIntervalAnimation
//{
//    protected float _startScaleX;
//    protected float _startScaleY;
//    protected float _endScaleX;
//    protected float _endScaleY;
//    protected float _deltaX;
//    protected float _deltaY;
//
//    public DDScaleTo(float duration, float scale)
//        : base(duration)
//    {
//        _endScaleX = _endScaleY = scale;
//    }
//
//    public DDScaleTo(float duration, float scaleX, float scaleY)
//        : base(duration)
//    {
//        _endScaleX = scaleX;
//        _endScaleY = scaleY;
//    }
//
//    public DDScaleTo(float duration, DDVector scale)
//        : base(duration)
//    {
//        _endScaleX = scale.X;
//        _endScaleY = scale.Y;
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//
//        _startScaleX = target.ScaleX;
//        _startScaleY = target.ScaleY;
//
//        _deltaX = _endScaleX - _startScaleX;
//        _deltaY = _endScaleY - _startScaleY;
//    }
//
//    protected override void Update(DDNode target, float t)
//    {
//        target.ScaleX = _startScaleX + (_deltaX * t);
//        target.ScaleY = _startScaleY + (_deltaY * t);
//    }
//}
//
//public class DDScaleBy : DDScaleTo
//{
//    public DDScaleBy(float duration, float scale)
//        : this(duration, scale, scale)
//    {
//    }
//
//    public DDScaleBy(float duration, float scaleX, float scaleY)
//        : base(duration, scaleX, scaleY)
//    {
//    }
//
//    protected override void Start(DDNode target)
//    {
//        base.Start(target);
//
//        _deltaX = _startScaleX * _endScaleX - _startScaleX;
//        _deltaY = _startScaleY * _endScaleY - _startScaleY;
//    }
//}

public class DDJumpBy : DDIntervalAnimation
{
    protected DDVector _startPosition;
    protected DDVector _delta;
    private float _height;
    private int _jumps;

    public DDJumpBy(float duration, DDVector position, float height, int jumps)
        : base(duration)
    {
        _delta = position;
        _height = height;
        _jumps = jumps;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        _startPosition = target.Position;
    }

    protected override void Update(DDNode target, float t)
    {
        float y = _height * (float)Math.Abs(Math.Sin(t * (float)Math.PI * _jumps));
        y += _delta.Y * t;

        float x = _delta.X * t;
        target.SetPosition(_startPosition.X + x, _startPosition.Y + y);
    }
}

public class DDJumpTo : DDJumpBy
{
    public DDJumpTo(float duration, DDVector position, float height, int jumps)
        : base(duration, position, height, jumps)
    {
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        _delta = new DDVector(_delta.X - _startPosition.X, _delta.Y - _startPosition.Y);
    }

}

public struct DDBezierConfig
{
    public DDVector StartPosition { get; set; }
    public DDVector EndPosition { get; set; }
    public DDVector ControlPoint1 { get; set; }
    public DDVector ControlPoint2 { get; set; }

    public DDBezierConfig(DDVector start, DDVector end, DDVector cp1, DDVector cp2)
        : this()
    {
        StartPosition = start;
        EndPosition = end;
        ControlPoint1 = cp1;
        ControlPoint2 = cp2;
    }

    public DDBezierConfig Negate()
    {
        DDBezierConfig ret = new DDBezierConfig();

        ret.StartPosition = StartPosition.Negate();
        ret.EndPosition = EndPosition.Negate();
        ret.ControlPoint1 = ControlPoint1.Negate();
        ret.ControlPoint2 = ControlPoint2.Negate();

        return ret;
    }

}

public class DDBezierBy : DDIntervalAnimation
{
    private static float BezierAt(float a, float b, float c, float d, float t)
    {
        return (float)(Math.Pow(1 - t, 3) * a + 3 * t * (Math.Pow(1 - t, 2)) * b + 3 * Math.Pow(t, 2) * (1 - t) * c + Math.Pow(t, 3) * d);
    }

    private DDBezierConfig _config;
    private DDVector _startPosition;

    public DDBezierBy(float duration, DDBezierConfig config)
        : base(duration)
    {
        _config = config;
    }

    protected override void Start(DDNode target)
    {
        base.Start(target);
        _startPosition = target.Position;
    }

    protected override void Update(DDNode target, float t)
    {
        float xa = _config.StartPosition.X;
        float xb = _config.ControlPoint1.X;
        float xc = _config.ControlPoint2.X;
        float xd = _config.EndPosition.X;

        float ya = _config.StartPosition.Y;
        float yb = _config.ControlPoint1.Y;
        float yc = _config.ControlPoint2.Y;
        float yd = _config.EndPosition.Y;

        float x = BezierAt(xa, xb, xc, xd, t);
        float y = BezierAt(ya, yb, yc, yd, t);

        target.SetPosition(_startPosition.X + x, _startPosition.Y + y);
    }
}

public class DDBlink : DDIntervalAnimation
{
    private uint _times;

    public DDBlink(float duration, uint times)
        : base(duration)
    {
        _times = times;
    }

    protected override void Update(DDNode target, float t)
    {
        float slice = 1f / _times;
        float m = t % slice;
        target.Visible = m > (slice / 2f);
    }
}

public class DDFadeIn : DDIntervalAnimation
{
    public DDFadeIn(float duration)
        : base(duration)
    {
    }

    protected override void Update(DDNode target, float t)
    {
        target.Color = new DDColor(target.Color, t);
    }
}

public class DDFadeOut : DDIntervalAnimation
{
    public DDFadeOut(float duration)
        : base(duration)
    {
    }

    protected override void Update(DDNode target, float t)
    {
        target.Color = new DDColor(target.Color, 1f - t);
    }
}

public class DDDelayTime : DDIntervalAnimation
{
    public DDDelayTime(float duration)
        : base(duration)
    {
    }

    protected override void Update(DDNode target, float t)
    { }
}