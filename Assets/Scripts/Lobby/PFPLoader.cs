using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFPLoader : MonoBehaviour
{
    static PFPList pfpList;
    public static Sprite LoadPFP(int i)
    {
        if (pfpList == null)
            pfpList = Resources.Load<PFPList>("PFPList");
        return pfpList.sprites[i];
    }
}
