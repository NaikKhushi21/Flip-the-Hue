using UnityEngine;
using System.Collections;

public class FlipCounterAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector3 originalScale;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;  // Save the original scale
        StartCoroutine(PulseForDuration(5f));  // Start pulsing for 5 seconds
    }

    private IEnumerator PulseForDuration(float duration)
    {
        animator.Play("PulseAnimation");  // Start the pulse animation
        yield return new WaitForSeconds(duration);  // Wait for the specified duration
        animator.enabled = false;  // Disable the animator to stop the animation
        transform.localScale = originalScale;  // Reset to the original scale
    }
}
