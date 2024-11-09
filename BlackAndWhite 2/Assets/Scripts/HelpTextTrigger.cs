using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  

public class HelpTextTrigger : MonoBehaviour
{
    public GameObject helpText; 
    public float displayDuration = 5f;  
    private static GameObject currentHelpText = null;  

    private void Start()
    {
        helpText.SetActive(false);  
    }

    private void OnEnable()
    {
        ResetCurrentHelpText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowHelpText();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideHelpText();
        }
    }
    //show help text
    private void ShowHelpText()
    {
        if (currentHelpText != null)
        {
            currentHelpText.SetActive(false);
            StopAllCoroutines();  
        }

        helpText.SetActive(true);
        currentHelpText = helpText;

        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideHelpText();
    }

    private void HideHelpText()
    {
        if (helpText != null)
        {
            helpText.SetActive(false);
        }

         if (currentHelpText == helpText)
        {
            currentHelpText = null;
        }
    }

    public static void ResetCurrentHelpText()
    {
        if (currentHelpText != null)
        {
            currentHelpText.SetActive(false);
            currentHelpText = null;
        }
    }
}