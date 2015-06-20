//
//  DDDirector.cs
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
using System.Diagnostics;

public partial class DDDirector
{
    private static DDDirector _instance = new DDDirector();
    public static DDDirector Instance { get { return _instance; } }

    public DDScene Scene { get; private set; }
	public DDVector WinSize { get; protected set; }
	public float DPI { get; protected set; }
	public float FrameRate { get; protected set; }

    private List<Action> _messageLoop = new List<Action>();

    static int _nodeCount = 0;

	public DDDirector()
	{
		EscapePressed = Quit;
	}

    internal void OnTick(float t)
    {
		DDDebug.OnTick(t);

        using (var stop = DDDebug.Measure("OnTick"))
        {
            DDScheduler.Instance.OnTick(t);
            ProcessMessages();
            _nodeCount = 0;
            if (Scene != null)
                CallResetCache(Scene);
        }
    }

    public void OnDraw(DDRenderer renderer)
    {
        using (var stop = DDDebug.Measure("OnDraw"))
        {
	        renderer.BeginScene();
	        if (Scene != null)
	            Scene.Visit(renderer);
	        renderer.EndScene();
        }
		var debugInfoLabel = DDDebug.GetInfoLabel();
        if (debugInfoLabel != null)
        {
			CallResetCache(debugInfoLabel);
            renderer.BeginScene();
            debugInfoLabel.Visit(renderer);
	        renderer.EndScene();
    	}
    }

    private static void CallResetCache(DDNode node)
    {
		if (node.Visible)
		{
	        _nodeCount++;
            node.ResetCache(_nodeCount);
	        foreach (var child in node.Children)
	            CallResetCache(child);
		}
    }

	internal void OnTouch(int id, float x, float y, DDTouchPhase phase)
    {
        DDTouchDispatcher.Instance.OnTouch(id, x, y, phase);
    }

	internal void OnTouches(params DDTouch [] touches)
	{
		DDTouchDispatcher.Instance.OnTouches(touches);
	}

    public void PostMessage(Action action)
    {
        lock (_messageLoop)
        {
            _messageLoop.Add(action);
        }
    }

    public void ProcessMessages()
    {
        List<Action> messageLoopCopy = null;
        lock (_messageLoop)
        {
            if (_messageLoop.Count > 0)
            {
                messageLoopCopy = _messageLoop.ToList();
                _messageLoop.Clear();
            }
        }
        if (messageLoopCopy != null)
        {
            foreach (var action in messageLoopCopy)
            {
                action();
            }
        }
    }

    private void SetScene(DDScene scene)
    {
        if (Scene != null)
        {
            Scene.OnExit();
			Scene.Dispose();
        }
        StopAllAudioEffects();
        Scene = scene;
        Scene.OnEnter();
    }

    internal void ReplaceScene(DDScene scene, bool purgeUnusedTextures = true)
    {
        PostMessage(delegate
        {
            SetScene(scene);
			if (purgeUnusedTextures)
				DDTextureManager.Instance.PurgeUnusedTexture();
        });
    }

    public Action EscapePressed { get; set; }

    private void OnEscapePressed()
    {
        if (EscapePressed != null)
            EscapePressed();
    }

}