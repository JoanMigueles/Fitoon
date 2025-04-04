public class UserData
{
    public int medals;
    public int gymKey;
    public string title;
    public int bannerID;
    public int profileID;

    public UserData(int medals, string title, int bannerID, int profileID, int gymKey)
    {
        this.medals = medals;
        this.title = title;
        this.bannerID = bannerID;
        this.profileID = profileID;
        this.gymKey = gymKey;
    }
}
