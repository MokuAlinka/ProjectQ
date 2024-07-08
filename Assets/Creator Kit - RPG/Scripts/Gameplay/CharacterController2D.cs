using System;
using System.Collections;
using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Tilemaps;

namespace RPGM.Gameplay
{
    /// <summary>
    /// A simple controller for animating an 8 directional sprite using Physics, now using grid-based movement.
    /// </summary>
    public class CharacterController2D : MonoBehaviour
    {
        public float speed = 1;
        public Vector3 nextMoveCommand;
        public Animator animator;
        public Tilemap wallTilemap; // 引用瓦片地图

        new Rigidbody2D rigidbody2D;
        SpriteRenderer spriteRenderer;
        PixelPerfectCamera pixelPerfectCamera;

        enum State
        {
            Idle, Moving
        }

        State state = State.Idle;
        Vector3 targetPosition;
        float gridSize = 1.0f; // 每个格子的大小

        void IdleState()
        {
            if (nextMoveCommand != Vector3.zero)
            {
                targetPosition = transform.position + nextMoveCommand * gridSize;

                // 检查目标位置是否有墙壁
                if (!IsWall(targetPosition))
                {
                    UpdateAnimator(nextMoveCommand);
                    state = State.Moving;
                }

                nextMoveCommand = Vector3.zero;
            }
        }

        void MoveState()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (transform.position == targetPosition)
            {
                state = State.Idle;
                UpdateAnimator(nextMoveCommand);
            }

            // Flip the sprite based on movement direction
            if (nextMoveCommand.x != 0)
            {
                spriteRenderer.flipX = nextMoveCommand.x > 0;
            }
        }

        void UpdateAnimator(Vector3 direction)
        {
            if (animator)
            {
                animator.SetInteger("WalkY", direction.y < 0 ? 1 : direction.y > 0 ? -1 : 0);
                animator.SetInteger("WalkX", direction.x < 0 ? -1 : direction.x > 0 ? 1 : 0);
            }
        }

        void Update()
        {
            switch (state)
            {
                case State.Idle:
                    IdleState();
                    break;
                case State.Moving:
                    MoveState();
                    break;
            }
        }

        void LateUpdate()
        {
            if (pixelPerfectCamera != null)
            {
                transform.position = pixelPerfectCamera.RoundToPixel(transform.position);
            }
        }

        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
            wallTilemap = GameObject.Find("WallTilemap").GetComponent<Tilemap>();
        }

        bool IsWall(Vector3 targetPosition)
        {
            // 将目标位置转换为瓦片地图的网格位置
            Vector3Int cellPosition = wallTilemap.WorldToCell(targetPosition);

            // 检查该位置是否有瓦片（即墙壁）
            TileBase tile = wallTilemap.GetTile(cellPosition);
            return tile != null;
        }
    }
}
