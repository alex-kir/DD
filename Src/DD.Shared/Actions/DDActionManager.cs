//
//  DDActionManager.cs
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

[Obsolete]
public class DDActionManager
{
    private static DDActionManager _instance = new DDActionManager();
    public static DDActionManager Instance { get { return _instance; } }

    public int DebugActionCount { get; private set; }

    private Dictionary<DDNode, List<DDAnimation>> _actionsByTarget = new Dictionary<DDNode, List<DDAnimation>>();
    private DDTimer _timer;

    private DDActionManager()
    {
        _timer = new DDTimer(-1, 0, (e) => { Tick(e); });
        DDScheduler.Instance.Schedule(_timer);
    }

    [Obsolete]
    private void Tick(DDTimerEventArgs e)
    {
        DebugActionCount = 0;
        foreach (var kv in _actionsByTarget)
        {
            var target = kv.Key;
            if (!target.IsRunning)
                continue;

            var actions = kv.Value;
            foreach (var action in actions)
            {
                DDAnimation.Step(action, target, e.DeltaTime);
            }
            DebugActionCount += actions.Count;
            actions.RemoveAll(a => a.IsDone);
        }
    }

    [Obsolete]
    public void AddAction(DDAnimation action, DDNode target)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }
        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        DDDirector.Instance.PostMessage(() =>
        {
            if (!_actionsByTarget.ContainsKey(target))
                _actionsByTarget[target] = new List<DDAnimation>();
            _actionsByTarget[target].Add(action);
            DDAnimation.Start(action, target);
        });
    }

    [Obsolete]
    public void RemoveAction(DDAnimation action, DDNode target)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        DDDirector.Instance.PostMessage(() =>
        {
            if (_actionsByTarget.ContainsKey(target))
            {
                _actionsByTarget[target].Remove(action);
                if (_actionsByTarget[target].Count == 0)
                {
                    _actionsByTarget.Remove(target);
                }
            }
        });
    }

    [Obsolete]
    public void RemoveAllActionsForTarget(DDNode target)
    {
        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        DDDirector.Instance.PostMessage(() =>
        {
            if (_actionsByTarget.ContainsKey(target))
            {
                _actionsByTarget.Remove(target);
            }
        });
    }

    [Obsolete]
    public int CountActionsForTarget(DDNode target)
    {
        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        if (_actionsByTarget.ContainsKey(target))
        {
            return _actionsByTarget[target].Count;
        }
        return 0;
    }
}