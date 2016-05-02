using System;
using System.Collections.Generic;
using System.Linq;


public class DDNodeAnimations : ICollection<DDAnimation>
{
    readonly List<DDAnimation> animations = new List<DDAnimation>();
    readonly DDNode owner;
    bool registered = false;

    //public DDAnimationBuilder Builder { get { return new DDAnimationBuilder(owner); } }

    public DDNodeAnimations (DDNode owner)
	{
        this.owner = owner;
	}

    internal void Register()
    {
        if (!registered && owner.IsRunning && animations.Count > 0)
        {
            DDScheduler.Instance.RegisterForAnimations(owner);
            registered = true;
        }
    }

    internal void Unregister()
    {
        if (registered)
        {
            DDScheduler.Instance.UnregisterForAnimations(owner);
            registered = false;
        }
    }

    internal void OnTick(float dt)
    {
        foreach (var animation in animations) {
            try {
                DDAnimation.Step(animation, owner, dt);
            }
            catch (Exception ex) {
                DDDebug.LogException(ex);
            }
        }
        animations.RemoveAll(a => a.IsDone);
        if (animations.Count == 0)
            Unregister();
    }

    public void Queue(string name, DDAnimation item)
    {
        float delay = animations.Where(it => it.Name == name).OfType<DDIntervalAnimation>()
            .Aggregate(0f, (a, b) => Math.Max(a, b.Duration - b.Elapsed));
        var anim = delay > 0 ? DDAnimations.Delay(delay) + item : item;
        anim.Name = name;
        Add(anim);
    }

    #region ICollection implementation
    public void Add(string name, DDAnimation item)
    {
        item.Name = name;
        animations.RemoveAll(it => it.Name == name);
        Add(item);
    }

    public void Add(DDAnimation item)
    {
        animations.Add(item);
        DDAnimation.Start(item, owner);
        Register();
    }

    public void Clear()
    {
        animations.Clear();
        Unregister();
    }
    public bool Contains(DDAnimation item)
    {
        throw new NotImplementedException();
    }
    public void CopyTo(DDAnimation[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public void Remove(string name)
    {
        animations.RemoveAll(it => it.Name == name);
        if (animations.Count == 0)
            Unregister();
    }

    public bool Remove(DDAnimation item)
    {
        var ret = animations.Remove(item);
        if (animations.Count == 0)
            Unregister();
        return ret;
    }

    public int Count
    {
        get
        {
            return animations.Count;
        }
    }
    public bool IsReadOnly
    {
        get
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region IEnumerable implementation
    public IEnumerator<DDAnimation> GetEnumerator()
    {
        return animations.GetEnumerator();
    }
    #endregion
    #region IEnumerable implementation
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return animations.GetEnumerator();
    }
    #endregion
}

