using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    #region Variables

    /// <summary>
    /// Cached WaitForFixedUpdate object.
    /// Wait until next fixed frame rate update function.
    /// </summary>
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new();

    /// <summary>
    /// Dictionary for cached WaitForSeconds objects.
    /// <para> Key : Amount of seconds. </para>
    /// <para> Value : Suspend the coroutine execution for the amount of seconds, which is given as Key, using scaled time. </para>
    /// </summary>
    private static readonly Dictionary<float, WaitForSeconds> waitForSecondsDictionary = new();

    #endregion Variables

    #region Methods

    /// <summary>
    /// Return the WaitForSeconds object corresponding the time passed as a parameter.
    /// Return a cached object or creates new object if there is no cached object.
    /// </summary>
    /// <returns>The cached or new WaitForSeconds object corresponding the time</returns>
    public static WaitForSeconds WaitForSeconds(float second)
    {
        if (waitForSecondsDictionary.ContainsKey(second))
        {
            return waitForSecondsDictionary[second];
        }

        WaitForSeconds waitForSeconds = new(second);
        waitForSecondsDictionary.Add(second, waitForSeconds);
        return waitForSeconds;
    }

    /// <summary>
    /// Convert the position in the DNF coordinate into the position in the world coordinate.
    /// </summary>
    /// <param name="dnfPosition">The position value in the DNF coordinate to be converted into the world coordinate</param>
    /// <returns>The position value converted into world coordinate</returns>
    public static Vector3 ConvertDNFPosToWorldPos(Vector3 dnfPosition)
    {
        return new Vector3(dnfPosition.x, dnfPosition.y + dnfPosition.z * GlobalDefine.CONV_RATE, 0f);
    }

    #endregion Methods
}
