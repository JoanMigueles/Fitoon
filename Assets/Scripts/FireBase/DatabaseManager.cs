using UnityEngine;
using Firebase.Database;
using System;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    private DatabaseReference dbReference;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Update the player data in the database.
    /// </summary>
    /// <param name="score"></param> WONT APPEAR IN THE FINAL VERSION
    /// <param name="gymID"></param> WONT APPEAR IN THE FINAL VERSION
    public void UpdatePlayerData(int score, int gymID)
    {
        UserData user = new UserData(score, gymID);
        string json = JsonUtility.ToJson(user);
        dbReference.Child("Users").Child(SaveData.player.username).SetRawJsonValueAsync(json);
    }

    /// <summary>
    /// Checks if a gym ID exists in the database.
    /// </summary>
    /// <param name="gymID"></param> Gym ID to checked.
    /// <param name="callback"></param> Callback function to handle the result.
    public void CheckGymKey(int gymKey, Action<bool> callback)
    {
        Query query = dbReference.Child("Gyms").OrderByChild("gymID").EqualTo(gymKey).LimitToFirst(1);
        query.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking gym ID: " + task.Exception);
                callback(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                callback(snapshot.Exists && snapshot.ChildrenCount > 0);
            }
        });
    }

    /// <summary>
    /// Checks if a gym name exists in the database.
    /// </summary>
    /// <param name="gymName"></param> Gym name to checked.
    /// <param name="callback"></param> Callback function to handle the result.
    public void CheckGymName(string gymName, Action<bool> callback)
    {
        Query query = dbReference.Child("Gyms").OrderByKey().EqualTo(gymName).LimitToFirst(1);
        query.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking gym name: " + task.Exception);
                callback(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                callback(snapshot.Exists && snapshot.ChildrenCount > 0);
            }
        });
    }

    /// <summary>
    /// Checks if the authKey introduced is the same as the one in the database.
    /// </summary>
    /// <param name="authKey"></param> Auth key to checked.
    /// <param name="callback"></param> Callback function to handle the result.
    public void CheckAuthKey(int authKey, Action<bool> callback)
    {
        dbReference.Child("AuthKey").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking auth key: " + task.Exception);
                callback(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                callback(snapshot.Value != null && snapshot.Value.ToString() == authKey.ToString());
            }
        });
    }

    /// <summary>
    /// Registers a new gym in the database.
    /// Checks if the gym name already exists and if the auth key is valid before registering.
    /// </summary>
    /// <param name="gymName"></param> Gym name to register.
    /// <param name="gymKey"></param> Gym ID to register.
    /// <param name="authKey"></param> Auth key to check.
    public void RegisterGym(string gymName, int gymKey, int authKey)
    {
        CheckGymName(gymName, (exists) =>
        {
            if (exists)
            {
                Debug.Log("Gym name already exists. Please choose a different name.");
                return;
            }
            else
            {
                CheckAuthKey(authKey, (authorised) =>
                {
                    if (!authorised) Debug.Log("Invalid auth key. Please check the key and try again.");
                    else
                    {
                        GymData gym = new GymData(gymKey, authKey);
                        string json = JsonUtility.ToJson(gym);
                        dbReference.Child("Gyms").Child(gymName).SetRawJsonValueAsync(json);
                    }
                });
            }
        });
    }

    public void Test()
    {
        string gymName = "TestGym";
        int authKey = 123456789;
        RegisterGym(gymName, (int) MathF.Abs(gymName.GetHashCode()), authKey);
    }
}
