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

    public static DDAnimation Repeat(this DDAnimation self)
    {
        return new DDRepeatForever(self);
    }
}


