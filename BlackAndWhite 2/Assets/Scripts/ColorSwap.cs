using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

public class BackgroundColorSwapper : MonoBehaviour
{

    public GameObject background;
    public GameObject[] blackObstacles;
    public GameObject[] whiteObstacles;

    public int maxSwaps = 3;
    private int swapCount = 0;

    public TextMeshProUGUI levelText;

    private SpriteRenderer spriteRenderer1;

    private SpriteRenderer spriteRenderer2;
    public TextMeshProUGUI flipsLeftText;


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

        UpdateFlipsLeftUI();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (swapCount < maxSwaps)
            {
                if (MetricManager.instance != null)
                {
                    MetricManager.instance.AddToMetric1(1);
                }
                SwapColors();
                swapCount++;
                UpdateFlipsLeftUI();
            }
            else
            {
                Debug.Log("Swap limit reached for this level!");
            }
        }
    }

    void SwapColors()
    {
        if (spriteRenderer1 != null)
        {
            if (spriteRenderer1.color == Color.white)
            {
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


    // void UpdateObstacleColliders()
    // {
    //     bool isBackgroundBlack = IsBackgroundBlack();

    //     foreach (GameObject obstacle in blackObstacles)
    //     {
    //         if (obstacle != null) // Check if the obstacle is not null
    //         {
    //             Collider2D collider = obstacle.GetComponent<Collider2D>();
    //             if (collider != null)
    //             {
    //                 collider.enabled = !isBackgroundBlack;
    //             }
    //         }
    //     }

    //     foreach (GameObject obstacle in whiteObstacles)
    //     {
    //         if (obstacle != null) // Check if the obstacle is not null
    //         {
    //             Collider2D collider = obstacle.GetComponent<Collider2D>();
    //             if (collider != null)
    //             {
    //                 collider.enabled = isBackgroundBlack;
    //             }
    //         }
    //     }
    // }

    void UpdateObstacleColliders()
    {
        bool isBackgroundBlack = IsBackgroundBlack();  // 检查当前背景颜色

        // 更新黑色障碍物的渲染器和碰撞体
        foreach (GameObject obstacle in blackObstacles)
        {
            if (obstacle != null) 
            {
                // 获取渲染器和碰撞体
                SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
                Collider2D collider = obstacle.GetComponent<Collider2D>();

                // 根据背景颜色启用/禁用渲染器和碰撞体
                if (sr != null)
                {
                    sr.enabled = !isBackgroundBlack;  // 仅当背景为黑色时显示
                }
                if (collider != null)
                {
                    collider.enabled = !isBackgroundBlack;  // 仅当背景为黑色时启用碰撞
                }
            }
        }

        // 更新白色障碍物的渲染器和碰撞体
        foreach (GameObject obstacle in whiteObstacles)
        {
            if (obstacle != null) 
            {
                SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
                Collider2D collider = obstacle.GetComponent<Collider2D>();

                if (sr != null)
                {
                    sr.enabled = isBackgroundBlack;  // 仅当背景为白色时显示
                }
                if (collider != null)
                {
                    collider.enabled = isBackgroundBlack;  // 仅当背景为白色时启用碰撞
                }
            }
        }
    }

    void UpdateFlipsLeftUI()
    {
        if (flipsLeftText != null)
        {
            int flipsLeft = maxSwaps - swapCount;
            flipsLeftText.text = $"Flips: {swapCount} / {maxSwaps}";
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