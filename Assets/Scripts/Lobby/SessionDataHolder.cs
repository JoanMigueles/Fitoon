using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Class to hold session data.
/// </summary>
public static class SessionDataHolder
{
    public static bool lookForLobby;
    public static void Reset()
	{
		lookForLobby = true;
	}
}
