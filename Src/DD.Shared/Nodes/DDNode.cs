//
//  DDNode.cs
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


public partial class DDNode : IDisposable
{
	DDDisposable.Collection _disposeables = new DDDisposable.Collection();
	public ICollection<IDisposable> Disposables { get { return _disposeables; } }

    #region Transforms

    private DDMatrix _nodeToParentMatrix = new DDMatrix();
    private bool _isNodeToParentMatrixDirty;

    private DDVector _position;
    public DDVector Position
    {
        get { return _position; }
        set
        {
            if (_position != value)
            {
                SetPosition(value);
            }
        }
    }

    public void SetPosition(float x, float y)
    {
        _position.X = x;
        _position.Y = y;
        _isNodeToParentMatrixDirty = true;
    }

    public void SetPosition(DDVector point)
    {
        _position = point;
        _isNodeToParentMatrixDirty = true;
    }

    private DDVector _rotation;
    public DDVector RotationXY
    {
        get { return _rotation; }
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                _isNodeToParentMatrixDirty = true;
            }
        }
    }

    public virtual float Rotation
    {
        get { return (_rotation.X + _rotation.Y) / 2; }
        set { RotationXY = new DDVector(value, value); }
    }

    public void SetRotation(float rotationX, float rotationY)
    {
        RotationXY = new DDVector(rotationX, rotationY);
    }

    public void SetRotation(float rotation)
    {
        RotationXY = new DDVector(rotation, rotation);
    }

    public void SetRotation(DDVector rotation)
    {
        RotationXY = rotation;
    }


    private DDVector _scale;
    public DDVector ScaleXY
    {
        get { return _scale; }
        set
        {
            if (_scale != value)
            {
                _scale = value;
                _isNodeToParentMatrixDirty = true;
            }
        }
    }

    public float ScaleX
    {
        get { return _scale.X; }
        set { ScaleXY = new DDVector(value, _scale.Y); }
    }

    public float ScaleY
    {
        get { return _scale.Y; }
        set { ScaleXY = new DDVector(_scale.X, value); }
    }

    public float Scale
    {
        get { return (_scale.X + _scale.Y) / 2; }
        set { ScaleXY = new DDVector(value, value); }
    }
	
	public void SetScale(float scaleX, float scaleY)
	{
		ScaleXY = new DDVector(scaleX, scaleY);
	}

    public void SetScale(DDVector scale)
	{
		ScaleXY = scale;
	}

	public void SetScale(float scale)
	{
		ScaleXY = new DDVector(scale, scale);
	}

    public void SetScaleIn(float width, float height)
    {
        this.SetScaleIn(new DDVector(width, height));
    }

    public void SetScaleIn(DDVector size)
    {
        this.Scale = DDMath.MinXOrY(size / this.Size);
    }

	public void SetScaleFill(float width, float height)
	{
		this.SetScaleFill(new DDVector(width, height));
	}

	public void SetScaleFill(DDVector size)
	{
		this.ScaleXY = size / this.Size;
	}


    private DDVector _anchorPoint;
    public DDVector AnchorPoint
    {
        get { return _anchorPoint; }
        set
        {
            if (_anchorPoint != value)
            {
                _anchorPoint = value;
                _isNodeToParentMatrixDirty = true;
            }
        }
    }

    public void SetAnchorPoint(float x, float y)
    {
        AnchorPoint = new DDVector(x, y);
    }

	public void SetAnchorPoint(DDVector anchorPoint)
	{
		AnchorPoint = anchorPoint;
	}

    private DDVector _size;

    public DDVector Size
    {
        get { return _size; }
        set
        {
            if (_size != value)
            {
                _size = value;
                _isNodeToParentMatrixDirty = true;
            }
        }
    }

	public void SetSize(DDVector xy)
	{
		Size = xy;
	}
	    
	public void SetSize(float x, float y)
	{
		Size = new DDVector(x, y);
	}

    private DDVector _origin;
    public DDVector Origin
    {
        get { return _origin; }
        set
        {
            if (_origin != value)
            {
                _origin = value;
                _isNodeToParentMatrixDirty = true;
            }
        }
    }

    public void SetOrigin(float x, float y)
    {
        Origin = new DDVector(x, y);
    }

	public DDRectangle Bounds { get { return new DDRectangle(Origin, Origin + Size); } set { SetBounds (value); } }

	public void SetBounds(DDRectangle rect)
	{ 
		Origin = rect.LeftBottom;
		Size = rect.Size;
	}

    public void PlaceStretched(float x, float y, float w, float h)
    {
        SetPosition(x, y);
        ScaleXY = new DDVector(w, h) / Size;
    }

    public void PlaceIn(float x, float y, float w, float h)
    {
        SetPosition(x, y);
        SetScaleIn(w, h);
    }

	public void PlaceIn(float x, float y, float w, float h, DDVector anchor)
	{
		SetPosition(x, y);
		SetScaleIn(w, h);
		SetAnchorPoint(anchor);
	}

    public DDMatrix NodeToParentTransform()
    {
        if (_isNodeToParentMatrixDirty)
        {
            //using (var sw = DDDirector.Instance.StartMeasureForUsing("DDNode.NodeToParentTransform"))
            {
                _nodeToParentMatrix.MakeTransform(Position, RotationXY, ScaleXY);
				var shift = Size * AnchorPoint + Origin;
                if (shift.X != 0 || shift.Y != 0)
                    _nodeToParentMatrix.Translate(-shift);
                _isNodeToParentMatrixDirty = false;
            }
        }
        return _nodeToParentMatrix;
        
    }

    DDMatrix _nodeToWorldMatrix;
    bool _nodeToWorldMatrixDirty = true;

    public DDMatrix NodeToWorldTransform()
    {
        if (_nodeToWorldMatrixDirty || _isNodeToParentMatrixDirty)
        {
            _nodeToWorldMatrix = NodeToParentTransform();
            if (Parent != null)
                _nodeToWorldMatrix = Parent.NodeToWorldTransform() * _nodeToWorldMatrix;
            _nodeToWorldMatrixDirty = false;
        }
        return _nodeToWorldMatrix;
    }

    public DDMatrix WorldToNodeTransform()
    {
        return NodeToWorldTransform().Invert();
    }

	public bool Contains(DDVector point)
	{
		return new DDRectangle(Origin, Origin + Size).Contains(point);
	}

    #endregion

    #region Colors

    private DDColor _color;
    public DDColor Color
    {
        get { return _color; }
        set { _color = value; }
    }
	
	public void SetColor(DDColor color, float alpha)
	{
		Color = new DDColor(color, alpha);
	}

	public void SetColor(float red, float green, float blue, float alpha)
	{
		Color = new DDColor(red, green, blue, alpha);
	}

    private bool _combinedColorDirty = true;
    private DDColor _combinedColor;
    public DDColor CombinedColor
    {
        get
        {
            if (_combinedColorDirty)
            {
                _combinedColor = Parent != null ? _color * Parent.CombinedColor : _color;
                _combinedColorDirty = false;
            }
            return _combinedColor;
        }
    }

	private DDColor _colorBlack;
	public DDColor ColorBlack
	{
		get { return _colorBlack; }
		set { _colorBlack = value; }
	}

	public void SetColorBlack(DDColor color)
	{
		_colorBlack = color;
	}

	public void SetColorBlack(float red, float green, float blue)
	{
		_colorBlack = new DDColor(red, green, blue);
	}

	[Obsolete("Use ColorBlack instead")]
	public DDColor Color2
    {
		get { return _colorBlack; }
		set { _colorBlack = value; }
    }

	[Obsolete("Use SetColorBlack instead")]
    public void SetColor2(DDColor color2)
    {
        ColorBlack = new DDColor(color2);
    }
	[Obsolete("Use SetColorBlack instead")]
    public void SetColor2(float red, float green, float blue)
    {
        ColorBlack = new DDColor(red, green, blue);
    }

	private bool _combinedColorBlackDirty = true;
	private DDColor _combinedBlackColor;
	public DDColor CombinedColorBlack
    {
        get
        {
			if (_combinedColorBlackDirty)
            {
				_combinedBlackColor = Parent != null ? (_colorBlack.Negative() * Parent.CombinedColorBlack.Negative()).Negative() : _colorBlack;
				_combinedColorBlackDirty = false;
            }
			return _combinedBlackColor;
        }
    }

    #endregion

    #region Hierarchy
	
	private int _zOrder = 0;
    public int ZOrder
	{
		get { return _zOrder; }
		set
		{
			if (_zOrder != value)
			{
				_zOrder = value;
				if (Parent != null)
				{
					Parent.Children.Reorder(this);
				}
			}	
		}
	}

	protected DDNodeCollection _children;
	public DDNodeCollection Children { get { return _children; } }

//    public IEnumerable<DDNode> AllChildren
//    {
//        get
//        {
//            foreach (var ch in Children)
//            {
//                yield return ch;
//                foreach (var ch2 in ch.AllChildren)
//                {
//                    yield return ch2;
//                }
//            }
//        }
//    }

    public DDNode Parent { get; set; }

//    public T AddChild<T>(T child) where T : DDNode
//    {
//        return AddChild(child, child._zOrder);
//    }
//
//    public T AddChild<T>(T child, int zOrder) where T : DDNode
//    {
//        if (child == null)
//        {
//            throw new ArgumentNullException("child");
//        }
//        if (child.Parent != null)
//        {
//            throw new ArgumentException("Child already has a parent", "child");
//        }
//
//        InsertChild(child, zOrder);
//        child.Parent = this;
//
//        if (IsRunning)
//        {
//            child.OnEnter();
//        }
//
//        return child;
//    }
//
//    private void InsertChild(DDNode child, int z)
//    {
//        int i = 0;
//        bool added = false;
//
//        foreach (DDNode node in _children)
//        {
//            if (node._zOrder > z)
//            {
//                added = true;
//                _children.Insert(i, child);
//                break;
//            }
//            ++i;
//        }
//
//        if (!added)
//        {
//            _children.Add(child);
//        }
//
//        child._zOrder = z;
//    }
//
//
//    public void ReorderChild(DDNode child, int z)
//    {
//        if (child == null)
//        {
//            throw new ArgumentNullException("child");
//        }
//
//        _children.Remove(child);
//        InsertChild(child, z);
//    }
//
//    public void RemoveChild(DDNode child)
//    {
//        RemoveChild(child, true);
//    }
//
//    public virtual void RemoveChild(DDNode child, bool cleanup)
//    {
//        if (child != null)
//        {
//            if (Children.Contains(child))
//            {
//                DetachChild(child, cleanup);
//            }
//        }
//    }

//	public void ChangeParent(DDNode newParent)
//    {
//		if (newParent == null)
//		{
//			throw new ArgumentNullException("newParent");
//		}
//		if (Parent == null)
//		{
//			throw new NullReferenceException("Parent is null");
//		}
//
//		var oldParent = Parent;
//		oldParent._children.Remove(this);
//		Parent = null;
//		newParent.InsertChild(this, this.ZOrder);
//		Parent = newParent;
//
//		var pos = oldParent.NodeToWorldTransform() * Position;
//		Position = newParent.WorldToNodeTransform() * pos;
//    }

//    public void RemoveAllChildren()
//    {
//        RemoveAllChildren(true);
//    }
//
//    public void RemoveAllChildren(bool cleanup)
//    {
//        foreach (DDNode child in _children)
//        {
//            if (IsRunning)
//            {
//                child.OnExit();
//            }
//
//            if (cleanup)
//            {
//                child.CleanUp();
//            }
//
//            child.Parent = null;
//        }
//
//        _children.Clear();
//    }

//    private void DetachChild(DDNode child, bool cleanup)
//    {
//        if (IsRunning)
//        {
//            child.OnExit();
//        }
//
//        if (cleanup)
//        {
//            child.CleanUp();
//        }
//
//        child.Parent = null;
//
//        _children.Remove(child);
//    }
//
    public void RemoveFromParent()
    {
        if (Parent != null)
        {
			Parent.Children.Remove(this);
        }
    }

    #endregion Hierarchy

    public bool IsRunning { get; protected set; }
    public bool Visible { get; set; }
    public bool VisibleCombined { get { return Visible && (Parent == null ? true : Parent.VisibleCombined); } }

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
	
	public DDNode(float width, float height) : this()
    {
		SetSize(width, height);
	}

	public DDNode(DDVector size) : this()
	{
		SetSize(size);
	}

	public DDNode()
	{
		IsRunning = false;
        Position = DDVector.Zero;
        RotationXY = DDVector.Zero;
        ScaleXY = DDVector.One;

        AnchorPoint = DDVector.CenterMiddle;
        Size = DDVector.Empty;
        Origin = DDVector.Zero;

        Color = DDColor.White;
		ColorBlack = DDColor.Black;
        _isNodeToParentMatrixDirty = true;
        Visible = true;

		_children = new DDNodeCollection(this);
    }

	public void Dispose()
	{
		_disposeables.Dispose();
		var cc = _children;
		_children = null;
		if (cc != null)
			foreach (var child in cc)
				child.Dispose();
	}

//    public void CleanUp()
//    {
//        StopAllActions();
//
//		DDTouchDispatcher.Instance.RemoveNodeHandlers(this);
//
//        foreach (DDNode child in _children)
//        {
//            child.CleanUp();
//        }
//		Dispose();
//    }

    public virtual void OnEnter()
    {
        foreach (DDNode child in _children)
        {
            child.OnEnter();
        }
        IsRunning = true;
    }

    public virtual void OnExit()
    {
        IsRunning = false;
        foreach (DDNode child in _children)
        {
            child.OnExit();
        }
    }
	
	public static DDTexture _GetUsedTexture(DDNode node)
	{
		return node.GetUsedTexture();
	}
	
	protected virtual DDTexture GetUsedTexture()
	{
		return null;
	}
	
    public virtual void Draw(DDRenderer renderer)
    {
    }

    public virtual void Visit(DDRenderer renderer)
    {
        if (!Visible)
        {
            return;
        }

        if (_children.Count == 0)
        {
            Draw(renderer);
            return;
        }

        foreach (DDNode child in _children)
        {
            if (child._zOrder < 0)
            {
                child.Visit(renderer);
            }
            else
            {
                break;
            }
        }

        Draw(renderer);

        foreach (DDNode child in _children)
        {
            if (child._zOrder >= 0)
            {
                child.Visit(renderer);
            }
        }
    }

    public int GlobalNodeIndex { get; private set; }

    internal void ResetCache(int globalNodeIndex)
    {
        GlobalNodeIndex = globalNodeIndex;
        _combinedColorDirty = true;
		_combinedColorBlackDirty = true;
        _nodeToWorldMatrixDirty = true;
    }

    public override string ToString()
    {
        return string.Format("[DDNode: Position={0}, AnchorPoint={1}, Size={2}, Origin={3}", Position, AnchorPoint, Size, Origin);
    }

    #region Actions

    public virtual DDAnimation RunAction(DDAnimation action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        DDActionManager.Instance.AddAction(action, this);

        return action;
    }

    public void StopAllActions()
    {
        DDActionManager.Instance.RemoveAllActionsForTarget(this);
    }

    internal void StopAllActions(bool recoursive)
    {
        StopAllActions();
        if (recoursive)
            foreach (var child in Children)
                child.StopAllActions(recoursive);
    }

    public void StopAction(DDAnimation action)
    {
        DDActionManager.Instance.RemoveAction(action, this);
    }

    #endregion

    #region Timers




    #endregion


    #region Touches

    //HashSet<DDTouchDispatcher.DDTouchHandler> _handlers = new HashSet<DDTouchDispatcher.DDTouchHandler>();

    public DDTouchHandler AddTouchHandler(DDTouchPhase phase, Action<DDTouchEventArgs> action)
    {
        return DDTouchDispatcher.Instance.AddHandler(this, 0, phase, action);
    }

    public void AddTouchHandler(int priority, DDTouchPhase phase, Action<DDTouchEventArgs> action)
    {
        DDTouchDispatcher.Instance.AddHandler(this, priority, phase, action);
    }

    //public void RemoveAllTouchHandlers()
    //{
    //    foreach (var h in _handlers)
    //        DDTouchDispatcher.Instance.RemoveHandler(h);
    //}

    #endregion


}