using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DDAnimationBuilder
{
    public static DDAnimationBuilder _shared = new DDAnimationBuilder(null);
    public static DDAnimationBuilder Shared{ get { return _shared; } }

    public DDNode Node { get; private set; }

    public DDAnimationBuilder(DDNode node)
    {
        Node = node;
    }

    public DDIntervalAnimation MoveTo(float duration, float x, float y)
    {
        return MoveTo(duration, new DDVector(x, y));
    }

    public DDIntervalAnimation MoveTo(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.Position, (it, val) => it.Position = val, DDVector.Lerp);
    }

	public DDIntervalAnimation MoveTo(float duration, params DDVector [] points)
	{
		var ret = new DDIntervalAnimation.SequenceInterval();
		foreach (var p in points)
            ret.Add(MoveTo(duration / points.Length, p));
		return ret;
	}

    public DDIntervalAnimation WavedMoveTo(float duration, float count, float size, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.Position, (it, val) => { it.Position = val; },
            (v1, v2, t) =>
            {
                var ox = v2 - v1;
                var oy = new DDVector(-ox.Y, ox.X);
                oy = oy / oy.Length * size;
                return v1 + ox * t + oy * DDMath.Sin(DDMath.Lerp(0, 2 * DDMath.PI * count, t));
            });
    }

    public DDIntervalAnimation RotateTo(float duration, float to)
    {
        return new DDIntervalAnimation<float>(duration, to, it => it.Rotation, (it, val) => it.Rotation = val, DDMath.Lerp);
    }

    public DDIntervalAnimation ScaleTo(float duration, float x, float y)
    {
        return  ScaleTo(duration, new DDVector(x, y));
    }

    public DDIntervalAnimation ScaleTo(float duration, float xy)
    {
        return ScaleTo(duration, new DDVector(xy, xy));
    }

    public DDIntervalAnimation ScaleTo(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY, (it, val) => it.ScaleXY = val, DDVector.Lerp);
    }

    public DDIntervalAnimation ScaleToSizeInner(float duration, DDVector xy)
    {
        return ScaleTo(duration, DDMath.MinXOrY(xy / this.Node.Size));
    }

    public DDIntervalAnimation ScaleToSizeInner(float duration, float x, float y)
    {
        return ScaleTo(duration, DDMath.MinXOrY(new DDVector(x, y) / this.Node.Size));
    }

    public DDIntervalAnimation SizeTo(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.Size, (it, val) => it.Size = val, DDVector.Lerp);
    }

    public DDIntervalAnimation Delay(float duration)
    {
        return new DDDelayTime(duration);
    }

    public DDIntervalAnimation Exec(System.Action act)
    {
        Action safeAction = () =>
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                DDDebug.Log(ex);
            }
            ;
        };
        return new DDInstantAction(act == null ? null : safeAction);
    }

    public DDIntervalAnimation Sound(string name, bool wait)
    {
        return new DDPlayEffect(name, wait);
    }

    public DDIntervalAnimation Kill()
    {
        return Exec(() => { Node.RemoveFromParent(); });
    }

    public DDIntervalAnimation FlashClip()
    {
        return new DDPlayClip((DDFlashClip)Node);
    }

    public DDAnimation Repeat(DDAnimation action)
    {
        return new DDRepeatForever(action);
    }

    public DDIntervalAnimation Repeat(int count, DDIntervalAnimation action)
    {
        return new DDRepeat(action, count);
    }

    public DDIntervalAnimation Spawn(params DDIntervalAnimation [] actions)
	{
		var ret = new DDIntervalAnimation.SpawnInterval();
		actions.ToList ().ForEach (it => ret.Add(it));
		return ret;
	}

    public DDIntervalAnimation ColorTo(float duration, DDColor color, float alpha)
    {
        return ColorTo(duration, new DDColor(color, alpha));
    }
    public DDIntervalAnimation ColorTo(float duration, float r, float g, float b, float alpha)
    {
        return ColorTo(duration, new DDColor(r, g, b, alpha));
    }
    public DDIntervalAnimation ColorTo(float duration, DDColor color)
    {
        return new DDIntervalAnimation<DDColor>(duration, color, it => it.Color, (it, val) => it.Color = val, DDColor.Lerp);
    }

    public DDIntervalAnimation ColorBlackTo(float duration, DDColor color)
	{
		return new DDIntervalAnimation<DDColor>(duration, color, it => it.ColorBlack, (it, val) => it.ColorBlack = val, DDColor.Lerp);
	}
    // -----------------------------------

    public DDIntervalAnimation Ease(DDIntervalAnimation action, Func<float, float> func)
    {
        return new DDEase(action, func);
    }

    public DDIntervalAnimation EaseIn(DDIntervalAnimation action)
    {
        return new DDEaseIn(action, 2.0f);
    }

    public DDIntervalAnimation EaseOut(DDIntervalAnimation action)
    {
        return new DDEaseOut(action, 2.0f);
    }

    public DDIntervalAnimation EaseElasticIn(DDIntervalAnimation action)
    {
        return new DDEaseElasticIn(action, 0.4f);
    }
    public DDIntervalAnimation EaseElasticOut(DDIntervalAnimation action, float period = 0.4f)
    {
        return new DDEaseElasticOut(action, period);
    }

    public DDIntervalAnimation Show()
    {
        return new DDInstantAction(() => { Node.Visible = true; });
    }

    public DDIntervalAnimation Hide()
    {
        return new DDInstantAction(() => { Node.Visible = false; });
    }

    public DDAnimation Update(Action<float> onUpdate, float delay = 0)
    {
        return new DDUpdate(onUpdate, delay);
    }

    public DDAnimation Update(Action onUpdate, float delay = 0)
    {
        return new DDUpdate(dt => onUpdate(), delay);
    }
}

public static class DDActionBuilderExtentions
{
//    public static DDAnimation StartAction<T>(this T self, Func<DDActionBuilder, DDAnimation> ab) where T : DDNode
//    {
//        var action = ab(new DDActionBuilder(self));
//        DDActionManager.Instance.AddAction(action, self);
//        return action;
//    }

    public static DDIntervalAnimation Ease(this DDIntervalAnimation action, Func<float,float> func)
	{
        return new DDEase(action, func);
	}

    public static DDIntervalAnimation EaseElasticIn(this DDIntervalAnimation action, float period = 0.4f)
    {
        return new DDEaseElasticIn(action, period);
    }

    public static DDIntervalAnimation EaseElasticOut(this DDIntervalAnimation action, float period = 0.4f)
    {
        return new DDEaseElasticOut(action, period);
    }

    public static DDIntervalAnimation EaseNurbs(this DDIntervalAnimation action, params float [] args)
    {
        return new DDEase(action, it => DDMath.Nurbs(it, args));
    }

	public static DDIntervalAnimation EaseBounceOut(this DDIntervalAnimation action, float period = 0.4f)
	{
		return new DDEaseBounceOut(action);
	}

    public static DDIntervalAnimation EaseTimeScale(this DDIntervalAnimation action, float multiplier)
    {
        return new DDEaseTimeScale(action, multiplier);
    }


}
