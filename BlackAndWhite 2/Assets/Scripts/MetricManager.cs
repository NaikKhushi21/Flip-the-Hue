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
     private List<SerializableVector3> m_metric2;
    private List<SerializableVector3> swapPos;
    private List<SerializableVector3> dashPos;
    private int levelResets;
     private int trapResets;
     private float levelTimer;
    private int dashMetric;
    private int totalLevelsPlayed;

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
             m_metric2 = new List<SerializableVector3>();
            swapPos = new List<SerializableVector3>();
            dashPos = new List<SerializableVector3>();
            dashMetric = 0;
             allLevelMetrics = new List<LevelMetrics>();
            hasPushedUpload = false;
            totalLevelsPlayed = 0;
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

     public void AddToMetric2(Vector3 valueToAdd)
     {
         m_metric2.Add(new SerializableVector3(valueToAdd));
     }

     public void AddToResets(int valueToAdd)
     {
         levelResets += valueToAdd;
     }

     public void AddToTrapResets(int valueToAdd)
     {
         trapResets += valueToAdd;
     }

    public void AddToNumDashes(int valueToAdd)
    {
        dashMetric += valueToAdd;
    }

    public void AddToDashPos(Vector3 valueToAdd)
    {
        dashPos.Add(new SerializableVector3(valueToAdd));
    }

    public void AddToSwapPos(Vector3 valueToAdd)
    {
        swapPos.Add(new SerializableVector3(valueToAdd));
    }

    public void ClearLevel()
    {
        levelTimer = 0.0f;
        levelResets = 0;
        trapResets = 0;
        m_metric1 = 0;
        dashMetric = 0;
        m_metric2.Clear();
        dashPos.Clear();
        swapPos.Clear();
    }

    public void NextLevel(int levelNum)
     {
        totalLevelsPlayed++;
        List<SerializableVector3> deathPosCopy = new List<SerializableVector3>();
        foreach (SerializableVector3 pos in m_metric2)
        {
            deathPosCopy.Add(pos);
        }

        List<SerializableVector3> dashPosCopy = new List<SerializableVector3>();
        foreach (SerializableVector3 pos in dashPos)
        {
            dashPosCopy.Add(pos);
        }

        List<SerializableVector3> swapPosCopy = new List<SerializableVector3>();
        foreach (SerializableVector3 pos in swapPos)
        {
            swapPosCopy.Add(pos);
        }

        var levelMetrics = new LevelMetrics
         {
             levelTime = levelTimer,
             avgFlips = levelResets > 0 ? (float)m_metric1 / levelResets : m_metric1,
             trapResets = trapResets,
             numDashes = levelResets > 0 ? (float)dashMetric / levelResets : dashMetric,
             deathPositions = deathPosCopy,
             dashPositions = dashPosCopy,
             swapPositions = swapPosCopy,
             level = levelNum
         };
         allLevelMetrics.Add(levelMetrics);

         // Upload metrics for the completed level
         //UploadMetricsToFirebase();

         // Reset metrics for the next level
         levelTimer = 0.0f;
         levelResets = 0;
         trapResets = 0;
         m_metric1 = 0;
        dashMetric = 0;
         m_metric2.Clear();
        dashPos.Clear();
        swapPos.Clear();
        
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

         string userId = "NewGuestTest_" + Guid.NewGuid().ToString();
         string sessionKey = "session_" + DateTime.Now.ToString("yyyyMMddHHmmss");

         var dataToUpload = new List<Dictionary<string, object>>();

         for (int i = 0; i < allLevelMetrics.Count; i++)
         {

            var deathPositionsList = new List<Dictionary<string, float>>();
            foreach (var position in allLevelMetrics[i].deathPositions)
            {
                deathPositionsList.Add(new Dictionary<string, float>
            {
                { "x", position.x },
                { "y", position.y },
                { "z", position.z }
            });
            }

            var dashPositionsList = new List<Dictionary<string, float>>();
            foreach (var position in allLevelMetrics[i].dashPositions)
            {
                dashPositionsList.Add(new Dictionary<string, float>
            {
                { "x", position.x },
                { "y", position.y },
                { "z", position.z }
            });
            }

            var swapPositionsList = new List<Dictionary<string, float>>();
            foreach (var position in allLevelMetrics[i].swapPositions)
            {
                swapPositionsList.Add(new Dictionary<string, float>
            {
                { "x", position.x },
                { "y", position.y },
                { "z", position.z }
            });
            }

            var levelData = new Dictionary<string, object>
             {
                 { "levelTime", allLevelMetrics[i].levelTime },
                 { "avgFlips", allLevelMetrics[i].avgFlips },
                 { "trapResets", allLevelMetrics[i].trapResets },
                { "avgDashes", allLevelMetrics[i].numDashes },
                 { "deathPositions", deathPositionsList },
                { "dashPositions", dashPositionsList },
                { "swapPositions", swapPositionsList }
             };
            var levelVerData = new Dictionary<string, object>();
            levelVerData[$"Level_{allLevelMetrics[i].level}"] = levelData;
            dataToUpload.Add(levelVerData);
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
    public float numDashes;
    public List<SerializableVector3> deathPositions;
    public List<SerializableVector3> dashPositions;
    public List<SerializableVector3> swapPositions;
    public int level;
}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}