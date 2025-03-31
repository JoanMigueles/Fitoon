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
            Debug.Log("Firebase Database initialized.");
        }
        else Destroy(gameObject);
    }

    public void UpdateData(int score, int gymID)
    {
        Debug.Log("Updating data for user: " + SaveData.player.username);
        Debug.Log("Score: " + score + ", Gym ID: " + gymID);
        UserData user = new UserData(score, gymID);
        string json = JsonUtility.ToJson(user);
        reference.Child("Users").Child(SaveData.player.username).SetRawJsonValueAsync(json);
    }

    public void A()
    {
        UpdateData(500, 1);
        // UpdateData("Player2", 200, 2);
        // UpdateData("Player3", 300, 3);
    }
}
