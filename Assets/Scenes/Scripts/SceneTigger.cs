using RPGM.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTigger : MonoBehaviour
{
    public Transform player; // 玩家对象
    public CameraController cameraController; // 摄像机控制器
    private Collider2D selfCollider; // 自身碰撞箱

    void Awake()
    {
        selfCollider = GetComponent<Collider2D>(); // 获取自身碰撞器
        if (selfCollider == null)
        {
            Debug.LogError($"Collider not found on {gameObject.name}. Please attach a Collider.");
        }

        if (cameraController == null)
        {
            Debug.LogError("CameraController is not assigned. Please set it in the inspector.");
        }
    }

    void Update()
    {
        if (selfCollider != null && player != null)
        {
            // 检测玩家是否与自身碰撞箱碰撞
            if (selfCollider != null && player != null)
            {
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null && selfCollider.IsTouching(playerCollider))
                {
                    Debug.Log($"{gameObject.name} is colliding with the player!");
                    // 在此处处理碰撞逻辑，例如更新焦点
                    cameraController.SetFocus(transform);
                }
            }
        }
    }
}
