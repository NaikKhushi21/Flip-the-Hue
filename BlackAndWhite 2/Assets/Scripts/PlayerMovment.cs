using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Import SceneManagement to load the next level
 
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f; // Speed of the player
    public float jumpForce = 2.0f; // Force of the jump
    public float dashDistance = 2.0f; // Distance the player dashes
    public float dashCooldown = 1.0f; // Cooldown before the player can dash again
    public float dashTime = 0.2f; // Time taken to complete the dash
 
    private bool isGrounded; // Is the player on the ground?
    private bool canJumpFromObstacle;
    private bool isDashing = false; // Is the player currently dashing?
    private float lastDashTime = -Mathf.Infinity; // Time when last dash occurred
    private Rigidbody2D rb;
    private Vector3 originalPosition;
    public BackgroundColorSwapper colorSwapScript; // Reference to the BackgroundColorSwapper script
 
    void Start()
    {
        // Get the Rigidbody2D component attached to this object
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
 
        // Find the BackgroundColorSwapper script if not assigned
        if (colorSwapScript == null)
        {
            colorSwapScript = FindObjectOfType<BackgroundColorSwapper>();
        }
    }
 
    void Update()
    {
        // If dashing, skip normal movement logic
        if (isDashing) return;
 
        // Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0);
        transform.Translate(movement * speed * Time.deltaTime);
 
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || canJumpFromObstacle))
        {
            if (MetricManager.instance != null)
            {
                MetricManager.instance.AddToMetric2(1);
            }
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            if (canJumpFromObstacle && !isGrounded)
            {
                canJumpFromObstacle = false;
            }
            isGrounded = false; // Player is now in the air
        }
 
        // Dashing
        if (Input.GetKeyDown(KeyCode.V) && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash(moveHorizontal));
        }
    }
 
    private IEnumerator PerformDash(float moveHorizontal)
    {
        isDashing = true;
        lastDashTime = Time.time;
 
        // Determine dash direction based on user input
        float dashDirection = moveHorizontal != 0 ? Mathf.Sign(moveHorizontal) : (transform.localScale.x > 0 ? 1 : -1);
 
        // Save the original position to calculate the dash distance
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(dashDirection * dashDistance, 0, 0);
 
        float elapsedTime = 0;
 
        // Perform the dash over a short period of time
        while (elapsedTime < dashTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
 
        // Ensure the player reaches the target position
        transform.position = targetPosition;
        isDashing = false;
    }
 
    // Check if the player collides with the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true; // Player is on the ground
        }
    }
 
    // Trigger event when the player first enters the trap or flag
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
        {
            if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player touched Whitetrap while the background is black. Resetting position.");
                ResetPosition(); // Reset player to the original position
            }
        }
 
        if (collision.gameObject.tag == "Blacktrap")
        {
            if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player touched Blacktrap while the background is white. Resetting position.");
                ResetPosition(); // Reset player to the original position
            }
        }
 
        // Check if the collided object is the red flag to stop movement and jump to the next level
        if (collision.gameObject.tag == "RedFlag")
        {
            speed = 0f;
            rb.velocity = Vector2.zero;  // Stop the player's movement
            LoadNextLevel(); // Call method to load the next level
        }
    }
 
    // Trigger stay event to reset the player if standing on the Whitetrap continuously
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
        {
            if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Whitetrap while the background is black. Resetting position.");
                ResetPosition(); // Reset player to the original position
            }
        }
 
        if (collision.gameObject.tag == "Blacktrap")
        {
            if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Blacktrap while the background is black. Resetting position.");
                ResetPosition(); // Reset player to the original position
            }
        }
    }
 
    private void ResetPosition()
    {
        rb.velocity = Vector2.zero;  // Stop the player's movement immediately
        transform.position = originalPosition;
        if (MetricManager.instance != null)
        {

            MetricManager.instance.AddToResets(1);
            MetricManager.instance.AddToTrapResets(1);
        }
    }
 
    public void SetJumpAllowed(bool allowed)
    {
        canJumpFromObstacle = allowed;
    }
 
    private void LoadNextLevel()
    {
        // Get the current active scene index and load the next scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
 
        // Check if the next scene index is within the bounds of your scenes
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (MetricManager.instance != null)
            {
                MetricManager.instance.NextLevel(nextSceneIndex + 1);
            }
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load. This is the last level.");
        }
    }
}