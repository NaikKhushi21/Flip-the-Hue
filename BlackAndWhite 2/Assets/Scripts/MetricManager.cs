using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class MetricManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public FirebaseUser user;
    public FirebaseDatabase database;
    public DatabaseReference databaseReference;

    public static MetricManager instance;
    private int m_metric1;
    private int m_metric2;
    private int levelResets;
    private int trapResets;
    private float levelTimer;

    private List<LevelMetrics> allLevelMetrics;

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
        }
        else
        {
            Destroy(gameObject);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
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
        });
    }

    private void InitializeFirebase()
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
    }

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
        if (databaseReference == null)
        {
            Debug.LogError("DatabaseReference is null. Ensure Firebase is initialized before uploading metrics.");
            return;
        }

        if (allLevelMetrics.Count == 0)
        {
            Debug.LogWarning("No metrics to upload.");
            return;
        }

        string userId = user != null ? user.UserId : "Guest_" + Guid.NewGuid().ToString();
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

        databaseReference.Child("PlayerMetrics").Child(userId).Child(sessionKey)
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
    }

    private void OnApplicationQuit()
    {
        UploadMetricsToFirebase();
    }

    public void TestUpload()
    {
        var testMetrics = new { levelTime = 120.0, avgFlips = 5, trapResets = 2, avgJumps = 10 };
        string json = JsonUtility.ToJson(testMetrics);
        databaseReference.Child("Metrics").Child("testUser").Child("Level1").SetRawJsonValueAsync(json);
    }
}

[Serializable]
public class LevelMetrics
{
    public float levelTime;
    public float avgFlips;
    public int trapResets;
    public float avgJumps;
}