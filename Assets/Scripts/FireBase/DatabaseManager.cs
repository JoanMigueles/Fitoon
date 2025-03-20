using UnityEngine;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    private DatabaseReference reference;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void UpdateData(string userName, int score, int gymID)
    {
        User user = new User(userName, score, gymID);
        string json = JsonUtility.ToJson(user);
        reference.Child("Users").Child(SaveData.player.username).SetRawJsonValueAsync(json);
    }

    public void A()
    {
        UpdateData("Player1", 100, 1);
        UpdateData("Player2", 200, 2);
        UpdateData("Player3", 300, 3);
    }
}
