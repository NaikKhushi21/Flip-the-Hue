using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorSwapper : MonoBehaviour
{
    public GameObject sprite1; 
    public GameObject sprite2; 
    public GameObject[] blackObstacles; 
    public GameObject[] whiteObstacles; 

    private SpriteRenderer spriteRenderer1;
    private SpriteRenderer spriteRenderer2;

    void Start()
    {
        if (sprite1 != null && sprite2 != null)
        {
            spriteRenderer1 = sprite1.GetComponent<SpriteRenderer>();
            spriteRenderer2 = sprite2.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("There is error here");
        }

        UpdateObstacleColliders();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwapColors();
        }
    }

    void SwapColors()
    {
        if (spriteRenderer1 != null && spriteRenderer2 != null)
        {
            Color tempColor = spriteRenderer1.color;
            spriteRenderer1.color = spriteRenderer2.color;
            spriteRenderer2.color = tempColor;

            UpdateObstacleColliders();
        }
    }

    void UpdateObstacleColliders()
    {
        bool isBackgroundBlack = (spriteRenderer1.color == Color.black);

        foreach (GameObject obstacle in blackObstacles)
        {
            Collider2D collider = obstacle.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = !isBackgroundBlack; 
            }
        }

        foreach (GameObject obstacle in whiteObstacles)
        {
            Collider2D collider = obstacle.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = isBackgroundBlack; 
            }
        }
    }

    public bool IsBackgroundBlack()
    {
        if (spriteRenderer1 != null)
        {
            return spriteRenderer1.color == Color.black;
        }
        return false;
    }
}
