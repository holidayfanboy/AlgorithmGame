using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Range(0f, 2f)]
    public float scrollSpeed = 0.5f;
    private float offset;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask playerLayer; // set to Player layer for raycast match (unused after left-edge logic)
    [SerializeField] private float leftCornerThreshold = 0.05f; // 5% from left edge
    [SerializeField] private float rightSpeedMultiplier = 100f; // Boost factor for moving right
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (playerData == null || mainCamera == null || player == null) return;

        if (playerData.isMove)
        {
            // Compute player's X position in viewport space (0 = left, 1 = right)
            float playerViewportX = mainCamera.WorldToViewportPoint(player.position).x;

            // Move camera until the player reaches the left corner threshold
            if (playerViewportX > leftCornerThreshold)
            {
                // Move camera RIGHT so the player appears further left in the viewport
                float delta = scrollSpeed * rightSpeedMultiplier * Time.deltaTime;
                offset += delta;
                transform.position += new Vector3(delta, 0f, 0f);
            }
            else
            {
                // Player reached left corner threshold; stop moving
                playerData.isMove = false;
            }
        }
    }
}
