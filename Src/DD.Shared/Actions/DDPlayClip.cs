//
//  DDPlayClip.cs
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

public class DDPlayClip : DDIntervalAnimation
{
    DDIntervalAnimation _action;
	
	public DDPlayClip(DDFlashClip clip) : base(clip.GetClipDuration())
	{
        _action = new DDFlashClip.FlashClipAction(clip.GetClipDuration());
	}

	public DDPlayClip(DDFlashClip clip, string name) : base(clip.GetClipDuration(name))
	{
        _action = new DDFlashClip.FlashClipNamedAction(clip.GetClipDuration(), name);
	}

    protected override void Start(DDNode target)
	{
		base.Start(target);
        DDAnimation.Start(_action, target);
	}

    protected override void Update(DDNode target, float t)
    {
        base.Update(target, t);
        DDIntervalAnimation.Update(_action, target, t);
    }

    //public override DDAction Clone()
    //{
    //    return new DDPlayClip(Clip, Name);
    //}

    //void StartClip()
    //{
    //foreach(var layer in layers.Values)
    //    {
    //        layer.RootNode.Visible = true;
    //        foreach (var nodeAction in layer.AllNodesActions)
    //        {
    //            DDActionManager.Instance.RemoveAction(nodeAction.Value, nodeAction.Key);
    //            nodeAction.Value.Start(nodeAction.Key);
    //        }
    //    }
    //    }
}