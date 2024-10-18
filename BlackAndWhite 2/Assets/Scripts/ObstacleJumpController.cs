using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleJumpController : MonoBehaviour
{
    private bool playerCanJump = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerCanJump = true;
                playerController.SetJumpAllowed(true); 
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetJumpAllowed(false); 
            }
            playerCanJump = false;
        }
    }
}
