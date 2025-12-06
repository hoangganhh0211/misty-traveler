using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    static readonly Dictionary<float, WaitForSeconds> _timeInterval =
                new Dictionary<float, WaitForSeconds>(new FloatComparer());

    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval =
                new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        if(!_timeInterval.TryGetValue(seconds, out waitForSeconds))
        {
            _timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }
        return waitForSeconds;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealtime;

        if (!_realTimeInterval.TryGetValue(seconds, out waitForSecondsRealtime))
        {
            _realTimeInterval.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }
        return waitForSecondsRealtime;
    }
}