//
//  DDTouchDispatcher.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
using WindowsStore;
#endif

[Flags]
public enum DDTouchPhase
{
    Began = 1,
    Moved = 2,
    Ended = 4,
    Canceled = 8, // TODO for future
}

public class DDTouchHandler
{
    public int Priority { get; set; }
    public DDTouchPhase Phase { get; set; }
    public DDNode Node { get; set; }
    public Action<DDTouchEventArgs> Action { get; set; }
}

public class DDTouchEventArgs
{
    public DDVector Position { get; set; }
    public DDTouchPhase Phase { get; set; }
    public DDNode Node { get; set; }
    public bool Processed { get; set; }
}

public class DDTouch
{
	public int Finger;
	public DDVector Position;
	public DDTouchPhase Phase;
	private DDUserData _userData = null;
	public DDUserData UserData
	{
		get
		{
			if (_userData == null)
				_userData = new DDUserData();
			return _userData;
		}
	}

	public DDVector GetPosition(DDNode node)
	{
		return node.WorldToNodeTransform().TransformPoint(Position);
	}
}

public class DDTouchHandler2
{
	public int Priority = 0;
	public Action<DDTouch[], DDNode> Action = null;
	public DDNode Owner;
}

public class DDTouchDispatcher
{
    private static DDTouchDispatcher _instance = new DDTouchDispatcher();
    public static DDTouchDispatcher Instance { get { return _instance; } }
	
	private DDTouchDispatcher()
	{ }

    List<DDTouchHandler> _handlers = new List<DDTouchHandler>();

	public DDTouchHandler AddHandler(DDTouchHandler handler)
    {
        DDDirector.Instance.PostMessage(() =>
        {
            _handlers.Add(handler);
        });
        return handler;
    }

    private void ReorderHandlers()
    {
        //if (isNodeOrderChanged)
        {
            _handlers.Sort((it1, it2) =>
            {
                int ret = it2.Node.GlobalNodeIndex.CompareTo(it1.Node.GlobalNodeIndex);
                if (ret == 0)
                    return it2.Priority.CompareTo(it1.Priority);
                return ret;
            });
        }
    }

    public DDTouchHandler AddHandler(DDNode node, int priority, DDTouchPhase phase, Action<DDTouchEventArgs> action)
    {
        var handler = new DDTouchHandler()
        {
            Priority = priority,
            Phase = phase,
            Node = node,
            Action = action,
        };
        return AddHandler(handler);
    }

    public void RemoveHandler(DDTouchHandler handler)
    {
        DDDirector.Instance.PostMessage(() =>
        {
            _handlers.Remove(handler);
        });
    }

    internal void RemoveNodeHandlers(DDNode node)
    {
        DDDirector.Instance.PostMessage(() =>
        {
            var toRemove = from h in _handlers
                           where h.Node == node
                           select h;
            foreach (var h in toRemove.ToList())
            {
                _handlers.Remove(h);
            }
        });
    }

    public void OnTouch(int id, float screenx, float screeny, DDTouchPhase phase)
    {
        var worldPos = new DDVector(screenx, screeny);
        ReorderHandlers();
        foreach (var handler in _handlers)
        {
            if (!handler.Node.IsRunning)
                continue;
            if ((handler.Phase & phase) == 0)
                continue;
            if (!handler.Node.VisibleCombined)
                continue;


            var nodePos = handler.Node.WorldToNodeTransform().TransformPoint(worldPos);

            if (new DDRectangle(handler.Node.Origin, handler.Node.Origin + handler.Node.Size).Contains(nodePos))
            {
                var args = new DDTouchEventArgs {
                    Phase = phase,
                    Position = nodePos,
                    Node = handler.Node,
                    Processed = false,
                };
                handler.Action(args);
                if (args.Processed)
                    break;
            }
        }
    }

	List<DDTouchHandler2> _handlers2 = new List<DDTouchHandler2>();

	public void OnTouches(params DDTouch[] touches)
	{
		_handlers2 = _handlers2.DDOrderBy(it => it.Priority).ToList();
		_handlers2.ForEach(it => it.Action(touches, it.Owner));
	}

	public void AddHandler(DDNode owner, int priority, Action<DDTouch[]> action)
	{
		var handler2 = new DDTouchHandler2
		{
			Priority = priority,
			Owner = owner,
			Action = (tt, o) => { action(tt); },
		};
		
		DDDirector.Instance.PostMessage(() =>
		                                {
			_handlers2.Add(handler2);
		});
		
		owner.Disposables.Add(new DDDisposable(() => {
			DDDirector.Instance.PostMessage(() => {
				_handlers2.Remove(handler2);
			});
		}));
	}
}