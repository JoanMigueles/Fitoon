using UnityEngine;
using Firebase.Database;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

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
    public void UpdatePlayerData()
    {
        UserData user = new UserData(SaveData.player.medals, SaveData.player.title, SaveData.player.bannerID, SaveData.player.pfp, SaveData.player.gymKey);
        string json = JsonUtility.ToJson(user);
        dbReference.Child("Users").Child(SaveData.player.username).SetRawJsonValueAsync(json);
    }


    /// <summary>
    /// Delete the player data in the database.
    /// </summary>
    public void DeletePlayerData()
    {
        try
        {
            dbReference.Child("Users").Child(SaveData.player.username).RemoveValueAsync();
        }
        catch(Exception e)
        {
            Debug.LogError("Error deleting player data: " + e.Message);
        }
    }

    /// <summary>
    /// Checks if the username already exists in the database.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="callback"></param>
    public void CheckUsername(string username, Action<bool> callback)
    {
        dbReference.Child("Users").Child(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking username: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                callback(snapshot.Exists && snapshot.ChildrenCount > 0);
            }
        });
    }

    /// <summary>
    /// Force update the player data in the database. ONLY FOR TESTING PURPOSES.
    /// </summary>
    public void UpdatePlayerData(string name, int medals, string title, int bannerID, int pfp, int gymKey)
    {
        UserData user = new UserData(medals, title, bannerID, pfp, gymKey);
        string json = JsonUtility.ToJson(user);
        dbReference.Child("Users").Child(name).SetRawJsonValueAsync(json);
    }

    /// <summary>
    /// Checks if a gym ID exists in the database.
    /// </summary>
    /// <param name="gymKey"></param> Gym ID to checked.
    /// <param name="callback"></param> Callback function to handle the result.
    public void CheckGymKey(int gymKey, Action<bool> callback)
    {
        Query query = dbReference.Child("Gyms").OrderByChild("gymKey").EqualTo(gymKey).LimitToFirst(1);
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

    public void CheckGymStatus(int gymKey, Action<bool> callback)
    {
        dbReference.Child("Gyms").OrderByChild("gymKey").EqualTo(gymKey).LimitToFirst(1).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                callback(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    CheckAuthKey(Convert.ToInt32(child.Child("authKey").Value), (authorised) =>
                    {
                        callback(authorised);
                    });
                }
            }
        });
    }

    /// <summary>
    /// Registers a new gym in the database.
    /// Checks if the gym name already exists and if the auth key is valid before registering.
    /// </summary>
    /// <param name="gymName"></param> Gym name to register.
    /// <param name="authKey"></param> Auth key to check.
    /// <param name="callback"></param> Callback function to handle the result.
    public void RegisterGym(string gymName, int authKey, Action<Tuple<int, int>> callback)
    {
        CheckGymName(gymName, (exists) =>
        {
            if (exists)
            {
                callback(new Tuple<int, int>(1, 0)); // Gym name already taken
            }
            else
            {
                CheckAuthKey(authKey, (authorised) =>
                {
                    if (!authorised) callback(new Tuple<int, int>(2, 0)); // Wrong auth key
                    else
                    {
                        int gymKey = math.abs(gymName.GetHashCode());
                        GymData gym = new GymData(gymKey, authKey);
                        string json = JsonUtility.ToJson(gym);
                        dbReference.Child("Gyms").Child(gymName).SetRawJsonValueAsync(json);
                        callback(new Tuple<int, int>(0, gymKey)); // Gym registered successfully
                    }
                });
            }
        });
    }

    /// <summary>
    /// Gets the global leaderboard
    /// </summary>
    /// <param name="gymKey"></param>
    /// <returns></returns>
    public void GetLeaderboard(Action<List<Tuple<string, UserData>>> callback, int? gymKey = null)
    {
        List<Tuple<string, UserData>> leaderboard = new List<Tuple<string, UserData>>();
        dbReference.Child("Users").OrderByChild("medals").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting leaderboard: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string username = child.Key;
                    UserData userData = new UserData(
                        Convert.ToInt32(child.Child("medals").Value),
                        child.Child("title").Value.ToString(),
                        Convert.ToInt32(child.Child("bannerID").Value),
                        Convert.ToInt32(child.Child("profileID").Value),
                        Convert.ToInt32(child.Child("gymKey").Value)
                    );
                    int gymKey2 = Convert.ToInt32(child.Child("gymKey").Value);
                    if (gymKey == null) leaderboard.Add(new Tuple<string, UserData>(username, userData));
                    else if (gymKey == gymKey2) leaderboard.Add(new Tuple<string, UserData>(username, userData));
                }
                leaderboard.Reverse();
                callback(leaderboard);
            }
        });
    }

    /// <summary>
    /// Gets the global position of the player in the leaderboard.
    /// </summary>
    /// <returns></returns>
    public void GetGlobalPosition(Action<int> callback)
    {
        int position = 0;
        dbReference.Child("Users").OrderByChild("medals").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting global position: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    position++;
                    if(child.Key == SaveData.player.username)
                    {
                        callback((int) snapshot.ChildrenCount - position);
                        break;
                    }
                }
            }
        });
    }

    /// <summary>
    /// Gets the gyms leaderboard with their names and scores.
    /// </summary>
    /// <returns></returns>
    public void GetGymsLeaderboardWithNames(Action<List<Tuple<string, int>>> callback)
    {
        List<Tuple<string, int>> leaderboard = new List<Tuple<string, int>>();
        dbReference.Child("Gyms").OrderByChild("gymKey").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting gyms leaderboard: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string gymName = child.Key;
                    int gymKey = Convert.ToInt32(child.Child("gymKey").Value);

                    var taskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();
                    GetLeaderboard((gymLeaderBoard) =>
                    {
                        int gymMedals = 0;
                        foreach (var user in gymLeaderBoard)
                        {
                            gymMedals += user.Item2.medals;
                        }
                        leaderboard.Add(new Tuple<string, int>(gymName, gymMedals));
                        taskCompletionSource.SetResult(true);
                    }, gymKey);
                    taskCompletionSource.Task.Wait();
                }
                leaderboard.Reverse();
                callback(leaderboard);
            }
        });
    }

    /// <summary>
    /// Gets the gyms leaderboard with their keys.
    /// </summary>
    /// <returns></returns>
    public void GetGymsLeaderboardWithKeys(Action<List<Tuple<int, int>>> callback)
    {
        List<Tuple<int, int>> leaderboard = new List<Tuple<int, int>>();
        dbReference.Child("Gyms").OrderByChild("gymKey").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting gyms leaderboard: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    int gymKey = Convert.ToInt32(child.Child("gymKey").Value);
                    var taskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();
                    GetLeaderboard((gymLeaderBoard) =>
                    {
                        int gymMedals = 0;
                        foreach (var user in gymLeaderBoard)
                        {
                            gymMedals += user.Item2.medals;
                        }
                        leaderboard.Add(new Tuple<int, int>(gymKey, gymMedals));
                        taskCompletionSource.SetResult(true);
                    }, gymKey);
                    taskCompletionSource.Task.Wait();
                }
                leaderboard.Reverse();
                callback(leaderboard);
            }
        });
    }

    /// <summary>
    /// Gets the position of a gym in the leaderboard.
    /// </summary>
    /// <param name="gymKey"></param> WON'T BE USED IN THE FINAL VERSION
    /// <returns></returns>
    public void GetGymPosition(int gymKey, Action<int> callback)
    {
        GetGymsLeaderboardWithKeys((gymsLeaderboard) =>
        {
            foreach (var gym in gymsLeaderboard)
            {
                if (gym.Item1 == gymKey)
                {
                    callback(gymsLeaderboard.IndexOf(gym) + 1);
                    return;
                }
            }
        });
    }

    public void GetGymMedals(int gymKey, Action<Tuple<String, int>> callback)
    {
        dbReference.Child("Gyms").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting gym medals: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string gymName = null;
                int gymMedals = 0;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    if (Convert.ToInt32(child.Child("gymKey").Value) == gymKey)
                    {
                        gymName = child.Key;
                        var taskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();
                        GetGymsLeaderboardWithKeys((gymLeaderBoard) =>
                        {
                            foreach (Tuple<int, int> gym in gymLeaderBoard)
                            {
                                if (gym.Item1 == gymKey)
                                {
                                    gymMedals = gym.Item2;
                                    break;
                                }
                            }
                            taskCompletionSource.SetResult(true);
                        });
                        taskCompletionSource.Task.Wait();
                        break;
                    }
                }
                callback(new Tuple<string, int>(gymName, gymMedals));
            }
        });
    }

    public void Test()
    {
        // PASSED
        // Test updating player data
        // for (int i = 0; i < 50; i++)
        // {
        //     UpdatePlayerData("TestUser " + i, i * 100, "TestTitle", 0, 0, i / 10);
        // }

        // PASSSED
        // Test pushing gyms
        // for(int i = 0; i < 5; i++)
        // {
        //     RegisterGym("TestGym " + i, i, 123456789);
        // }

        // PASSED
        // Test getting global leaderboard
        // GetLeaderboard((leaderboard) =>
        // {
        //     Debug.Log("Global Leaderboard:");
        //     foreach (var user in leaderboard)
        //     {
        //         Debug.Log("User: " + user.Item1 + ", Score: " + user.Item2);
        //     }
        // });

        // PASSED
        // Test getting a specific gym leaderboard
        // GetLeaderboard((leaderboard) =>
        // {
        //     Debug.Log("Gym 3 Leaderboard:");
        //     foreach (var user in leaderboard)
        //     {
        //         Debug.Log("User: " + user.Item1 + ", Score: " + user.Item2);
        //     }
        // }, 3);

        // PASSED
        // Test getting global position
        // GetGlobalPosition((position) =>
        // {
        //     Debug.Log("Global Position: " + position);
        // });

        // PASSED
        // Test getting gyms leaderboard
        // GetGymsLeaderboardWithNames((leaderboard) =>
        // {
        //     Debug.Log("Gyms Leaderboard:");
        //     foreach (var gym in leaderboard)
        //     {
        //         Debug.Log("Gym: " + gym.Item1 + ", Score: " + gym.Item2);
        //     }
        // });

        // PASSED
        // Test getting gyms leaderboard with keys
        // GetGymsLeaderboardWithKeys((leaderboard) =>
        // {
        //     Debug.Log("Gyms Leaderboard with keys:");
        //     foreach (var gym in leaderboard)
        //     {
        //         Debug.Log("GymKey: " + gym.Item1 + ", Score: " + gym.Item2);
        //     }
        // });

        // PASSED
        // Test getting gym position
        // GetGymPosition(1, (position) =>
        // {
        //     Debug.Log("Gym Position: " + position);
        // });
    }
}
