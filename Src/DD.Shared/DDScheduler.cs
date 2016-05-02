//
//  DDScheduler.cs
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

//public class DDTimer
//{
//    private float _elapsed = -1;
//    public int Priority { get; set; }
//    public float Interval { get; set; }
//    public Action<DDTimerEventArgs> Action { get; set; }
//
//    public DDTimer(int priority, float interval, Action<DDTimerEventArgs> action)
//    {
//        Priority = priority;
//        Interval = interval;
//        Action = action;
//    }
//
//    internal void OnTick(float delta)
//    {
//        if (_elapsed == -1)
//        {
//            _elapsed = 0;
//        }
//        _elapsed += delta;
//
//        if (_elapsed >= Interval)
//        {
//            Action(new DDTimerEventArgs(this, delta, _elapsed));
//            _elapsed = 0;
//        }
//    }
//}
//
//public class DDTimerEventArgs
//{
//    public float DeltaTime { get; private set; }
//    public float ElapsedTime { get; private set; }
//    public DDTimer Timer { get; private set; }
//
//    public DDTimerEventArgs(DDTimer timer, float deltaTime, float elapsedTime)
//    {
//        this.Timer = timer;
//        this.DeltaTime = deltaTime;
//        this.ElapsedTime = elapsedTime;
//    }
//}

public class DDScheduler
{
    private static DDScheduler _instance = new DDScheduler();
    public static DDScheduler Instance { get { return _instance; } }

    private readonly HashSet<DDNode> _animatedNodes = new HashSet<DDNode>();

    public float TimeScale { get; set; }
	public float TimeSinceStart { get; private set; }
    public float TimeDelta { get; private set; }

    private DDScheduler()
    {
        TimeScale = 1f;
		TimeSinceStart = 0;
    }

    public void OnTick(float dt)
    {
        dt *= TimeScale;
        TimeDelta = dt;
		TimeSinceStart += dt;

        foreach (var node in _animatedNodes)
        {
            node.Animations.OnTick(dt);
        }
    }

    public void RegisterForAnimations(DDNode node)
    {
        DDDirector.Instance.PostMessage(() => {
            _animatedNodes.Add(node);
        });
    }

    public void UnregisterForAnimations(DDNode node)
    {
        DDDirector.Instance.PostMessage(() => {
            _animatedNodes.Remove(node);
        });
    }
}