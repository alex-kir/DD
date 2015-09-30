using System;


public static class DDAnimations
{
    public static DDIntervalAnimation MoveTo(float duration, float x, float y)
    {
        return MoveTo(duration, new DDVector(x, y));
    }

    public static DDIntervalAnimation MoveTo(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.Position, (it, val) => it.Position = val, DDVector.Lerp);
    }

    public static DDIntervalAnimation MoveTo(float duration, params DDVector [] points)
    {
        var ret = new DDIntervalAnimation.SequenceInterval();
        foreach (var p in points)
            ret.Add(MoveTo(duration / points.Length, p));
        return ret;
    }

    public static DDIntervalAnimation ScaleTo(float duration, float x, float y)
    {
        return  ScaleTo(duration, new DDVector(x, y));
    }

    public static DDIntervalAnimation ScaleTo(float duration, float xy)
    {
        return ScaleTo(duration, new DDVector(xy, xy));
    }

    public static DDIntervalAnimation ScaleTo(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY, (it, val) => it.ScaleXY = val, DDVector.Lerp);
    }

    public static DDIntervalAnimation ScaleToSizeInner(float duration, DDVector xy)
    {
//        return ScaleTo(duration, DDMath.MinXOrY(xy / this.Node.Size));
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY * it.Size, (it, val) => it.ScaleXY = it.Size / val, DDVector.Lerp);
    }

    public static DDIntervalAnimation ScaleToSizeInner(float duration, float x, float y)
    {
        return ScaleToSizeInner(duration, new DDVector(x, y));
    }

    public static DDIntervalAnimation ColorTo(float duration, DDColor color, float alpha)
    {
        return ColorTo(duration, new DDColor(color, alpha));
    }
    public static DDIntervalAnimation ColorTo(float duration, float r, float g, float b, float alpha)
    {
        return ColorTo(duration, new DDColor(r, g, b, alpha));
    }
    public static DDIntervalAnimation ColorTo(float duration, DDColor color)
    {
        return new DDIntervalAnimation<DDColor>(duration, color, it => it.Color, (it, val) => it.Color = val, DDColor.Lerp);
    }

    public static DDIntervalAnimation ColorBlackTo(float duration, DDColor color)
    {
        return new DDIntervalAnimation<DDColor>(duration, color, it => it.ColorBlack, (it, val) => it.ColorBlack = val, DDColor.Lerp);
    }

    public static DDAnimation Update(Action<float> onUpdate, float delay = 0)
    {
        return new DDUpdate(onUpdate, delay);
    }

    public static DDAnimation Update(Action onUpdate, float delay = 0)
    {
        return new DDUpdate(dt => onUpdate(), delay);
    }

    public static DDIntervalAnimation Delay(float duration)
    {
        return new DDDelayTime(duration);
    }

    public static DDIntervalAnimation Exec(System.Action act)
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

    public static DDIntervalAnimation Exec(System.Action<DDNode> act)
    {
        Action<DDNode> safeAction = it =>
        {
            try
            {
                act(it);
            }
            catch (Exception ex)
            {
                DDDebug.Log(ex);
            }
            ;
        };
        return new DDInstantAction(act == null ? null : safeAction);
    }

    public static DDIntervalAnimation Kill()
    {
        return Exec(it => { it.RemoveFromParent(); });
    }

    public static DDAnimation Repeat(this DDAnimation self)
    {
        return new DDRepeatForever(self);
    }
}


