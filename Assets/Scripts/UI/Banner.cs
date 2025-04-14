using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    public Image background;
    public Image highlight;
    public Image pfp;

    public void SetBanner(int bannerID)
    {
        (Color outline, Sprite background) = AssetLoader.LoadBanner(bannerID);
        this.background.sprite = background;
        highlight.color = outline;
    }

    public void SetProfilePicture(int pictureID)
    {
        Sprite pfp = AssetLoader.LoadPFP(pictureID);
        this.pfp.sprite = pfp;
    }
}
