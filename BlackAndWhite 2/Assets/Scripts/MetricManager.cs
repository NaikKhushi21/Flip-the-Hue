using UnityEngine;
using System.Collections;
using System.IO;

// This class encapsulates all of the metrics that need to be tracked in your game. These may range
// from number of deaths, number of times the player uses a particular mechanic, or the total time
// spent in a level. These are unique to your game and need to be tailored specifically to the data
// you would like to collect. The examples below are just meant to illustrate one way to interact
// with this script and save data.
public class MetricManager : MonoBehaviour
{
    public static MetricManager instance;
    // You'll have more interesting metrics, and they will be better named.
    private int m_metric1;
    private int m_metric2;

    private int levelResets;
    private int trapResets;
    private string metrics;
    private float levelTimer;

    private void Awake()
    {
        // Singleton setup: If an instance exists, destroy the new one
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Prevent this object from being destroyed between scenes
            metrics = "Here are my metrics:\n" + "Level1:\n";
            levelTimer = 0.0f;
            levelResets = 0;
            trapResets = 0;
            m_metric1 = 0;
            m_metric2 = 0;
        }
        else
        {
            Destroy(gameObject);  // Ensure only one MetricManager exists
        }
    }

    void Update()
    {
        levelTimer += Time.deltaTime;
    }

    // Public method to add to Metric 1.
    public void AddToMetric1 (int valueToAdd)
    {
        m_metric1 += valueToAdd;
    }

    // Public method to add to Metric 2.
    public void AddToMetric2 (int valueToAdd)
    {
        m_metric2 += valueToAdd;
    }

    public void AddToResets (int valueToAdd)
    {
        levelResets += valueToAdd;
    }

    public void AddToTrapResets(int valueToAdd)
    {
        trapResets += valueToAdd;
    }

    public void NextLevel(int levelNum)
    {
        metrics += "Level Time: " + levelTimer.ToString() + "\n";
        if (levelResets != 0)
        {
            metrics += "Avg Flips: " + ((float)m_metric1 / (float)levelResets).ToString() + "\n";
            metrics += "Trap Resets: " + trapResets.ToString() + "\n";
            metrics += "Avg Jumps: " + ((float)m_metric2 / (float)levelResets).ToString() + "\n";
        }
        else
        {
            metrics += "Avg Flips: " + ((float)m_metric1).ToString() + "\n";
            metrics += "Trap Resets: " + trapResets.ToString() + "\n";
            metrics += "Avg Jumps: " + ((float)m_metric2).ToString() + "\n";
        }
        metrics += "Level" + levelNum.ToString() + ":\n";
        levelTimer = 0.0f;
        levelResets = 0;
        trapResets = 0;
        m_metric1 = 0;
        m_metric2 = 0;
    }

    // Converts all metrics tracked in this script to their string representation
    // so they look correct when printing to a file.
    private string ConvertMetricsToStringRepresentation ()
    {
        metrics += "Level Time: " + levelTimer.ToString() + "\n";
        if (levelResets != 0)
        {
            metrics += "Avg Flips: " + ((float)m_metric1 / (float)levelResets).ToString() + "\n";
            metrics += "Trap Resets: " + trapResets.ToString() + "\n";
            metrics += "Avg Jumps: " + ((float)m_metric2 / (float)levelResets).ToString() + "\n";
        }
        else
        {
            metrics += "Avg Flips: " + ((float)m_metric1).ToString() + "\n";
            metrics += "Trap Resets: " + trapResets.ToString() + "\n";
            metrics += "Avg Jumps: " + ((float)m_metric2).ToString() + "\n";
        }
        return metrics;
    }

    // Uses the current date/time on this computer to create a uniquely named file,
    // preventing files from colliding and overwriting data.
    private string CreateUniqueFileName ()
    {
        string dateTime = System.DateTime.Now.ToString ();
        dateTime = dateTime.Replace ("/", "_");
        dateTime = dateTime.Replace (":", "_");
        dateTime = dateTime.Replace (" ", "___");
        return "YourGameName_metrics_" + dateTime + ".txt"; 
    }

    // Generate the report that will be saved out to a file.
    private void WriteMetricsToFile ()
    {
        string totalReport = "Report generated on " + System.DateTime.Now + "\n\n";
        totalReport += "Total Report:\n";
        totalReport += ConvertMetricsToStringRepresentation ();
        totalReport = totalReport.Replace ("\n", System.Environment.NewLine);
        string reportFile = CreateUniqueFileName ();

        #if !UNITY_WEBPLAYER 
        File.WriteAllText (reportFile, totalReport);
        #endif
    }

    // The OnApplicationQuit function is a Unity-Specific function that gets
    // called right before your application actually exits. You can use this
    // to save information for the next time the game starts, or in our case
    // write the metrics out to a file.
    private void OnApplicationQuit ()
    {
        WriteMetricsToFile ();
    }
}
