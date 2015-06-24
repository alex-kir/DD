//
//  DDScrollView.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DDScrollView : DDView
{
    const float bounceDistance = 100;
    const float maxSpeed = 1500;
    const float friction = maxSpeed / 2.5f; // maxSpeed / seconds
    const float squareStopVelocity = 2 * 2;

    bool isScrollStarted = false;
    DDVector lastTouchPosition = DDVector.Zero;

    bool cancelThrough = false;
    float startTouchTime = 0;
    DDVector startViewPosition = DDVector.Zero;
    DDVector startTouchPosition = DDVector.Zero;
    DDVector prevViewPosition;

    DDVector velocity = DDVector.Zero;
    List<DDVector> velocityBuffer = new List<DDVector> { DDVector.Zero };

    private DDView _contentView;
    public DDView ContentView
    {
        get { return _contentView; }
        set
        {
            if (_contentView != null)
                SubViews.Remove(_contentView);
            _contentView = value;
            if (_contentView != null)
            {
                SubViews.Add(_contentView);
            }
        }
    }

    public DDScrollView(float w, float h)
        : base(w, h)
    {
        this.StartAction(aa => aa.Update(OnUpdate));
    }

	internal override void OnTouches (params DDTouch[] touches)
	{
		foreach (var touch in touches.Where(it => it.Phase == DDTouchPhase.Began).Take(1))
		{
			cancelThrough = false;
			isScrollStarted = true;
            lastTouchPosition = WorldToNodeTransform() * touch.Position;
            startTouchPosition = WorldToNodeTransform() * touch.Position;
			startViewPosition = _contentView.Position;
			startTouchTime = DDScheduler.Instance.TimeSinceStart;
		}

		foreach (var touch in touches.Where(it => it.Phase == DDTouchPhase.Moved).Take(1))
		{
			if (isScrollStarted)
			{
                var currentTouchPosition = (WorldToNodeTransform() * touch.Position);
                var newPosition = startViewPosition + (currentTouchPosition - startTouchPosition);
                var trimmedPosition = TrimPosition(newPosition);

                if (ContentView.Size.X - this.Size.X < 0.01f)
                    newPosition.X = trimmedPosition.X;
                if (ContentView.Size.Y - this.Size.Y < 0.01f)
                    newPosition.Y = trimmedPosition.Y;

                _contentView.Position = (newPosition + trimmedPosition) / 2;

                if (startViewPosition.DistanceSquare(_contentView.Position) > 13 ||
                    startTouchPosition.DistanceSquare(WorldToNodeTransform() * touch.Position) > 13)
					cancelThrough = true;
                lastTouchPosition = WorldToNodeTransform() * touch.Position;
			}
		}

		foreach (var touch in touches.Where(it => it.Phase == DDTouchPhase.Ended).Take(1))
		{
			isScrollStarted = false;

			float timeDiff = DDScheduler.Instance.TimeSinceStart - startTouchTime;
			if (!cancelThrough && timeDiff < 0.5f)
			{
                base.OnTouches(new DDTouch { Finger=touch.Finger, Phase=DDTouchPhase.Began, Position = touch.Position });
                base.OnTouches(touch);
			}

            prevViewPosition = _contentView.Position;
            velocityBuffer.Clear();
		}
	}

    DDRectangle GetTrimingRect()
    {
        return new DDRectangle(TrimPosition(new DDVector(-1000000, -1000000)), TrimPosition(new DDVector(1000000, 1000000)));
    }

    private DDVector TrimPosition(DDVector newPosition)
    {
        float minX = this.Size.Width - _contentView.Size.Width / 2;
        float maxX = _contentView.Size.Width / 2;
        if (minX > maxX)
            minX = maxX;

        float minY = this.Size.Height - _contentView.Size.Height / 2;
        float maxY = _contentView.Size.Height / 2;

        return new DDVector(
            DDMath.Max(minX, DDMath.Min(maxX, newPosition.X)),
            DDMath.Max(minY, DDMath.Min(maxY, newPosition.Y)));
    }

    void OnUpdate(float dt)
    {
        if (isScrollStarted)
        {
            velocity = ((_contentView.Position - prevViewPosition) / dt);
            if (velocity.Length > maxSpeed)
                velocity = velocity * maxSpeed / velocity.Length;

            if (velocityBuffer.Count > 2)
                velocityBuffer.RemoveAt(0);
            velocityBuffer.Add(velocity);

            velocity = new DDVector(velocityBuffer.DDOrderBy(x => DDMath.Abs(x.X)).DDLast().X,
                velocityBuffer.DDOrderBy(x => DDMath.Abs(x.Y)).DDLast().Y);
        }
        else
        {
            if (velocity != DDVector.Zero)
            {
                var frictionVector = velocity.GetNormalized().Negate();
                var velocityDiff = (frictionVector * friction) * dt;
                if (velocityDiff.LengthSquare > velocity.LengthSquare)
                {
                    velocity = DDVector.Zero;
                }
                else
                {
                    velocity = velocity + velocityDiff;

                    var newPosition = _contentView.Position + velocity * dt;
                    var trimmedPosition = TrimPosition(newPosition);

                    if (ContentView.Size.X - this.Size.X < 0.01f)
                        newPosition.X = trimmedPosition.X;
                    if (ContentView.Size.Y - this.Size.Y < 0.01f)
                        newPosition.Y = trimmedPosition.Y;

                    if (newPosition.Distance(trimmedPosition) > 1)
                    {
                        var tmp1 = (trimmedPosition - newPosition).Length;
                        velocity *= DDMath.LerpClamp(1, 0, 0, bounceDistance, tmp1);
                    }

                    _contentView.Position = newPosition;
                }
            }
            else
            {
                var newPosition = _contentView.Position;
                var trimmedPosition = TrimPosition(newPosition);

                if (ContentView.Size.X - this.Size.X < 0.01f)
                    newPosition.X = trimmedPosition.X;
                if (ContentView.Size.Y - this.Size.Y < 0.01f)
                    newPosition.Y = trimmedPosition.Y;

                _contentView.Position = (newPosition + trimmedPosition) / 2;
            }
        }

        prevViewPosition = _contentView.Position;

    }

    public override void OnAfterResize()
    {
        base.OnAfterResize();
        _contentView.Position = TrimPosition(_contentView.Position);
    }

    private void ScrollTo(float x, float y)
    {
        var rt = GetTrimingRect();
        _contentView.SetPosition(DDMath.Lerp(rt.Left, rt.Right, x), DDMath.Lerp(rt.Top, rt.Bottom, y));
    }

    public void ScrollToStart()
    {
        var rt = GetTrimingRect();
        _contentView.SetPosition(DDMath.Lerp(rt.Left, rt.Right, 0.5f), DDMath.Lerp(rt.Top, rt.Bottom, 1));
    }
}
