using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader
{
    static PFPList pfpList;
	static BannerList bannerList;
	public static Sprite LoadPFP(int i)
    {
        if (pfpList == null)
            pfpList = Resources.Load<PFPList>("PFPList");
        return pfpList.sprites[i];
    }
    public static (Color, Sprite) LoadBanner(int i)
	{
		if (bannerList == null)
			bannerList = Resources.Load<BannerList>("BannerList");
		return (bannerList.Colors[i], bannerList.Banners[i]);
	}
}