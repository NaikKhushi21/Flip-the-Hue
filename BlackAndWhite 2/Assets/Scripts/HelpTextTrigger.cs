using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  

public class HelpTextTrigger : MonoBehaviour
{
    public GameObject helpText;
    private TextMeshProUGUI helpTextMesh;
    private bool hasTriggered = false;  

    private void Start()
    {
        helpText.SetActive(false);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            helpText.SetActive(true);  
            hasTriggered = true;  
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            helpText.SetActive(false);
        }
    }
}