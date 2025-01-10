using RPGM.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTigger : MonoBehaviour
{
    public Transform player; // ��Ҷ���
    public CameraController cameraController; // �����������
    private Collider2D selfCollider; // ������ײ��

    void Awake()
    {
        selfCollider = GetComponent<Collider2D>(); // ��ȡ������ײ��
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
            // �������Ƿ���������ײ����ײ
            if (selfCollider != null && player != null)
            {
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null && selfCollider.IsTouching(playerCollider))
                {
                    Debug.Log($"{gameObject.name} is colliding with the player!");
                    // �ڴ˴�������ײ�߼���������½���
                    cameraController.SetFocus(transform);
                }
            }
        }
    }
}
