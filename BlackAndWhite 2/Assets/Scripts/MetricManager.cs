using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;
/*using Firebase;
using Firebase.Database;
using Firebase.Auth;*/

public class MetricManager : MonoBehaviour
{
    public static MetricManager instance;

     /*public DependencyStatus dependencyStatus;
     public FirebaseUser user;
     public FirebaseDatabase database;
     public DatabaseReference databaseReference;*/

     private int m_metric1;
     private int m_metric2;
     private int levelResets;
     private int trapResets;
     private float levelTimer;

    private bool hasPushedUpload;

     private List<LevelMetrics> allLevelMetrics;

    private string firebaseURL = "https://flipthehue-default-rtdb.firebaseio.com/";

    private void Awake()
     {
        
         if (instance == null)
         {
             instance = this;
             DontDestroyOnLoad(gameObject);

             levelTimer = 0.0f;
             levelResets = 0;
             trapResets = 0;
             m_metric1 = 0;
             m_metric2 = 0;
             allLevelMetrics = new List<LevelMetrics>();
            hasPushedUpload = false;
         }
         else
         {
             Destroy(gameObject);
         }

         /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
         {
             dependencyStatus = task.Result;
             if (dependencyStatus == DependencyStatus.Available)
             {
                 InitializeFirebase();
             }
             else
             {
                 Debug.LogError("Could not resolve firebase dependencies: " + dependencyStatus);
             }
         });*/
     }

     /*private void InitializeFirebase()
     {
        
         FirebaseApp app = FirebaseApp.DefaultInstance;
         database = FirebaseDatabase.DefaultInstance;
         databaseReference = database.RootReference;

         if (databaseReference != null)
         {
             Debug.Log("Firebase initialized and DatabaseReference set.");
         }
         else
         {
             Debug.LogError("DatabaseReference is null after initialization.");
         }
        
     }*/

     void Update()
     {
        
         levelTimer += Time.deltaTime;
        
     }

     public void AddToMetric1(int valueToAdd)
     {
         m_metric1 += valueToAdd;
     }

     public void AddToMetric2(int valueToAdd)
     {
         m_metric2 += valueToAdd;
     }

     public void AddToResets(int valueToAdd)
     {
         levelResets += valueToAdd;
     }

     public void AddToTrapResets(int valueToAdd)
     {
         trapResets += valueToAdd;
     }

     public void NextLevel(int levelNum)
     {
        
         var levelMetrics = new LevelMetrics
         {
             levelTime = levelTimer,
             avgFlips = levelResets > 0 ? (float)m_metric1 / levelResets : m_metric1,
             trapResets = trapResets,
             avgJumps = levelResets > 0 ? (float)m_metric2 / levelResets : m_metric2
         };
         allLevelMetrics.Add(levelMetrics);

         // Upload metrics for the completed level
         //UploadMetricsToFirebase();

         // Reset metrics for the next level
         levelTimer = 0.0f;
         levelResets = 0;
         trapResets = 0;
         m_metric1 = 0;
         m_metric2 = 0;
        
     }

     private void UploadMetricsToFirebase()
     {
        
         /*if (databaseReference == null)
         {
             Debug.LogError("DatabaseReference is null. Ensure Firebase is initialized before uploading metrics.");
             return;
         }*/

         if (allLevelMetrics.Count == 0)
         {
             Debug.LogWarning("No metrics to upload.");
             return;
         }

         string userId = "NewGuest_" + Guid.NewGuid().ToString();
         string sessionKey = "session_" + DateTime.Now.ToString("yyyyMMddHHmmss");

         var dataToUpload = new Dictionary<string, object>();

         for (int i = 0; i < allLevelMetrics.Count; i++)
         {
             var levelData = new Dictionary<string, object>
             {
                 { "levelTime", allLevelMetrics[i].levelTime },
                 { "avgFlips", allLevelMetrics[i].avgFlips },
                 { "trapResets", allLevelMetrics[i].trapResets },
                 { "avgJumps", allLevelMetrics[i].avgJumps }
             };
             dataToUpload[$"Level_{i + 1}"] = levelData;
         }

        string jsonUpload = JsonConvert.SerializeObject(dataToUpload);
        string url = firebaseURL + "PlayerMetrics/" + userId + "/" + sessionKey + ".json";

        StartCoroutine(PostData(url, jsonUpload));

         /*databaseReference.Child("PlayerMetrics").Child(userId).Child(sessionKey)
             .SetValueAsync(dataToUpload).ContinueWith(task =>
             {
                 if (task.IsCompleted)
                 {
                     Debug.Log("Metrics uploaded successfully.");
                 }
                 else
                 {
                     Debug.LogError("Failed to upload metrics: " + task.Exception);
                 }
             });
        */
     }

    private IEnumerator<UnityWebRequestAsyncOperation> PostData(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Metrics uploaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to upload metrics: " + request.error);
            }
        }
    }

    private void OnApplicationQuit()
     {
        if (!hasPushedUpload)
        {
            hasPushedUpload = true;
            UploadMetricsToFirebase();
        }
    }

    public void PushUpload()
    {
        if (!hasPushedUpload)
        {
            hasPushedUpload = true;
            UploadMetricsToFirebase();
        }
    }

     /*public void TestUpload()
     {
        
         var testMetrics = new { levelTime = 120.0, avgFlips = 5, trapResets = 2, avgJumps = 10 };
         string json = JsonUtility.ToJson(testMetrics);
         databaseReference.Child("Metrics").Child("testUser").Child("Level1").SetRawJsonValueAsync(json);
        
     }*/
 }

 [Serializable]
 public class LevelMetrics
 {
     public float levelTime;
     public float avgFlips;
     public int trapResets;
     public float avgJumps;
    
}