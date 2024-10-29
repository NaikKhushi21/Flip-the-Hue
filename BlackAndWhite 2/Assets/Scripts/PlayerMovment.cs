using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 2.0f;
    public float dashDistance = 2.0f;
    public float dashCooldown = 1.0f;
    public float dashTime = 0.2f;

    private bool isGrounded;
    private bool canJumpFromObstacle;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private Rigidbody2D rb;
    private Vector3 originalPosition;
    public BackgroundColorSwapper colorSwapScript;

    public GameObject levelPassedText;
    public Image fadeImage;
    private float fadeDuration = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;

        if (colorSwapScript == null)
        {
            colorSwapScript = FindObjectOfType<BackgroundColorSwapper>();
        }

        if (levelPassedText != null) levelPassedText.SetActive(false);
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        if (isDashing) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        /*if (colorSwapScript.background)
        {
            if (this.gameObject.transform.position.x < colorSwapScript.background.transform.position.x - colorSwapScript.background.transform.localScale.x / 2)
            {
                this.gameObject.transform.position = new Vector3(colorSwapScript.background.transform.position.x - colorSwapScript.background.transform.localScale.x / 2, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            }
            if (this.gameObject.transform.position.x > colorSwapScript.background.transform.position.x + colorSwapScript.background.transform.localScale.x / 2)
            {
                this.gameObject.transform.position = new Vector3(colorSwapScript.background.transform.position.x + colorSwapScript.background.transform.localScale.x / 2, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            }
        }*/
        Vector2 movement = new Vector2(moveHorizontal, 0);
        transform.Translate(movement * speed * Time.deltaTime);

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
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.V) && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash(moveHorizontal));
        }
    }

    private IEnumerator PerformDash(float moveHorizontal)
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDirection = moveHorizontal != 0 ? Mathf.Sign(moveHorizontal) : (transform.localScale.x > 0 ? 1 : -1);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(dashDirection * dashDistance, 0, 0);

        float elapsedTime = 0;

        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;
            float step = (dashDistance / dashTime) * Time.deltaTime;
            Vector3 nextPosition = transform.position + new Vector3(dashDirection * step, 0, 0);

            // Perform a BoxCast to check for obstacles
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, new Vector2(dashDirection, 0), step);
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                // Stop at the obstacle
                transform.position = new Vector3(hit.point.x - dashDirection * 0.05f, transform.position.y, transform.position.z);
                isDashing = false;
                yield break;
            }

            transform.position = nextPosition;
            yield return null;
        }

        isDashing = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
        {
            if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player touched Whitetrap while the background is black. Resetting position.");
                ResetPosition();
            }
        }

        if (collision.gameObject.tag == "Blacktrap")
        {
            if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player touched Blacktrap while the background is white. Resetting position.");
                ResetPosition();
            }
        }

        if (collision.gameObject.tag == "RedFlag")
        {
            speed = 0f;
            rb.velocity = Vector2.zero;
            LoadNextLevel();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
        {
            if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Whitetrap while the background is black. Resetting position.");
                ResetPosition();
            }
        }

        if (collision.gameObject.tag == "Blacktrap")
        {
            if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Blacktrap while the background is black. Resetting position.");
                ResetPosition();
            }
        }
    }

    private void ResetPosition()
    {
        rb.velocity = Vector2.zero;
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        MetricManager.instance.NextLevel(currentSceneIndex);
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LevelTransition(nextSceneIndex));
        }
        else
        {
            Debug.Log("No more levels to load. This is the last level.");
        }
    }

    private IEnumerator LevelTransition(int nextSceneIndex)
    {
        if (levelPassedText != null) levelPassedText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}
