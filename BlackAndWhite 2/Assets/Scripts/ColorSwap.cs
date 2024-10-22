using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BackgroundColorSwapper : MonoBehaviour
{
    public GameObject background;
    public GameObject[] blackObstacles; 
    public GameObject[] whiteObstacles; 

    public TextMeshProUGUI levelText;

    private SpriteRenderer spriteRenderer1;

    void Start()
    {
        if (background != null)
        {
            spriteRenderer1 = background.GetComponent<SpriteRenderer>();
            spriteRenderer1.color = Color.white;
        }
        else
        {
            Debug.LogError("There is error here");
        }

        UpdateObstacleColliders();
        UpdateTextColor();
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
        if (spriteRenderer1 != null)
        {
            if(spriteRenderer1.color == Color.white) {
                spriteRenderer1.color = Color.black;
            }
            else
            {
                spriteRenderer1.color = Color.white;
            }

            UpdateObstacleColliders();
            UpdateTextColor();
        }
    }

    void UpdateObstacleColliders()
    {
        bool isBackgroundBlack = IsBackgroundBlack();

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

    void UpdateTextColor()
    {
        Color textColor = IsBackgroundBlack() ? Color.white : Color.black;


        if (levelText != null)
        {
            levelText.color = textColor;
        }

    }
}
