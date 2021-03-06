﻿using System;


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
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY * it.Size, (it, val) => it.Scale = DDMath.MinXOrY(val / it.Size), DDVector.Lerp);
    }

    public static DDIntervalAnimation ScaleToSizeInner(float duration, float x, float y)
    {
        return ScaleToSizeInner(duration, new DDVector(x, y));
    }

    public static DDIntervalAnimation ScaleToFitIn(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY * it.Size, (it, val) => it.Scale = DDMath.MinXOrY(val / it.Size), DDVector.Lerp);
    }

    public static DDIntervalAnimation ScaleToFitIn(float duration, float x, float y)
    {
        return ScaleToFitIn(duration, new DDVector(x, y));
    }

    public static DDIntervalAnimation ScaleToFill(float duration, DDVector xy)
    {
        return new DDIntervalAnimation<DDVector>(duration, xy, it => it.ScaleXY * it.Size, (it, val) => it.ScaleXY = val / it.Size, DDVector.Lerp);
    }

    public static DDIntervalAnimation ScaleToFill(float duration, float x, float y)
    {
        return ScaleToFill(duration, new DDVector(x, y));
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
        DDDebug.Assert(onUpdate != null, "onUpdate must be not null");
        float tmp = 0;
        return new DDAnimation(null, (n, t) => {
            tmp += t;
            if (tmp >= delay) {
                try {
                    onUpdate(tmp);
                }
                catch (Exception ex) {
                    DDDebug.LogException(ex);
                }
                tmp = 0;
            }
        }, () => false);
    }

    public static DDAnimation Update(Action onUpdate, float delay = 0)
    {
        return Update(dt => onUpdate(), delay);
    }

    public static DDIntervalAnimation Delay(float duration)
    {
        return new DDIntervalAnimation<float>(duration, 0, n => 0, (n, v) => {}, DDMath.Lerp);
    }

    public static DDIntervalAnimation Exec(System.Action act)
    {
        Action safeAction = () =>
        {
            try
            {
                DDDirector.Instance.PostMessage(act);
            }
            catch (Exception ex)
            {
                DDDebug.Log(ex);
            }
            ;
        };
        return new DDInstantAction(act == null ? null : safeAction);
    }

    public static DDInstantAction Exec(System.Action<DDNode> act)
    {
        Action<DDNode> safeAction = it =>
        {
            try
            {
                DDDirector.Instance.PostMessage(() => act(it));
            }
            catch (Exception ex)
            {
                DDDebug.Log(ex);
            }
            ;
        };
        return new DDInstantAction(act == null ? null : safeAction);
    }

    public static DDInstantAction Kill()
    {
        return Exec(it => it.RemoveFromParent());
    }

    public static DDInstantAction Show()
    {
        return Exec(it => { it.Visible = true; });
    }

    public static DDInstantAction Hide()
    {
        return Exec(it => { it.Visible = false; });
    }

    public static DDAnimation Repeat(this DDAnimation self)
    {
        return new DDRepeatForever(self);
    }

    public static DDAnimation Repeat(this DDIntervalAnimation self, int times)
    {
        return new DDRepeat(self, times);
    }

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

    public static DDAnimation PlayEffect(string name, bool wait = false)
    {
        return new DDPlayEffect(name, wait);
    }
}


