using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class SessionDataHolder
{
    public static int score;
    public static bool lookForLobby;
    public static void Reset()
	{
		score = 0;
		lookForLobby = true;
	}
}
