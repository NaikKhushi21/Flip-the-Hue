using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  

public class HelpTextTrigger : MonoBehaviour
{
    public TextMeshProUGUI helpText;  
    private bool hasTriggered = false;  

    private void Start()
    {
        helpText.gameObject.SetActive(false);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            helpText.gameObject.SetActive(true);  
            hasTriggered = true;  
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            helpText.gameObject.SetActive(false);
        }
    }
}