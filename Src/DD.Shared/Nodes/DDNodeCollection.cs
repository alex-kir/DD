using System;
using System.Collections.Generic;
using System.Linq;


public class DDNodeCollection : ICollection<DDNode>
{
	DDNode _owner;
	private List<DDNode> _collection = null;
	private List<DDNode> Collection { get { return _collection ?? (_collection = new List<DDNode>()); } }

	public int Count { get { return _collection == null ? 0 : _collection.Count; } }

	public IEnumerable<DDNode> AllRecursive {
		get {
			return this.Concat (this.SelectMany (it => it.Children.AllRecursive));
		}
	}

	internal DDNodeCollection (DDNode owner)
	{
		_owner = owner;
	}

    public T Add<T>(T item, int zOrder) where T : DDNode
	{
        item.ZOrder = zOrder;
        return Add(item);
    }

    public T Add<T>(T item) where T : DDNode
    {
        Add((DDNode)item);
		return item;
	}

    public void Add(DDNode item)
    {
        if (item.Parent != null)
        {
            throw new ArgumentException("Child already has a parent", "child");
        }


        Collection.Add(item);
        item.Parent = _owner;
        Reorder (item);

        if (_owner.IsRunning)
            item.OnEnter();
    }

	public void ChangeParent(DDNode item)
	{
		if (item.Parent == null)
		{
			throw new NullReferenceException("item.Parent is null");
		}

		var oldParent = item.Parent;
		oldParent.Children._collection.Remove(item);
		item.Parent = null;

		Collection.Add(item);
		item.Parent = _owner;
		Reorder (item);

		var pos = oldParent.NodeToWorldTransform() * item.Position;
		item.Position = _owner.WorldToNodeTransform() * pos;
	}


    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(DDNode item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(DDNode[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool IsReadOnly
    {
        get
        {
            throw new NotImplementedException();
        }
    }

//	public void Clear ()
//	{
//		throw new NotImplementedException ();
//	}
//
//	public bool Contains (DDNode item)
//	{
//		throw new NotImplementedException ();
//	}
//
//	public void CopyTo (DDNode[] array, int arrayIndex)
//	{
//		throw new NotImplementedException ();
//	}
//
	public bool Remove (DDNode item)
	{
		if (_owner.IsRunning)
			item.OnExit();
		return Collection.Remove(item);
	}
//
//	public int Count {
//		get {
//			throw new NotImplementedException ();
//		}
//	}
//
//	public bool IsReadOnly {
//		get {
//			throw new NotImplementedException ();
//		}
//	}

	public IEnumerator<DDNode> GetEnumerator ()
	{
		if (_collection == null)
			return Enumerable.Empty<DDNode>().GetEnumerator();
		else
			return _collection.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	{
		if (_collection == null)
			return Enumerable.Empty<DDNode>().GetEnumerator();
		else
			return _collection.GetEnumerator();
	}

	public void Reorder (DDNode item)
	{
		var c = Collection;
		var i = c.IndexOf (item);
		if (i == -1)
			DDDebug.Error("DDNodeCollection.Reorder(), item is not in collection");
		for (int j = i + 1; j < c.Count; i++,j++)
		{
			if (c [i].ZOrder < c [j].ZOrder)
				break;
			var tmp = c [i];
			c [i] = c [j];
			c [j] = tmp;
		}

		for (int j = i - 1; j >= 0; i--,j--)
		{
			if (c [i].ZOrder >= c [j].ZOrder)
				break;
			var tmp = c [i];
			c [i] = c [j];
			c [j] = tmp;
		}

	}
}

