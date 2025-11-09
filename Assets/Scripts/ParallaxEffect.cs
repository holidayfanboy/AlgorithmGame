using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float scrollSpeed = 0.5f;
    private float offset;
    private Material mat;
    [SerializeField] private PlayerData playerData;
    void Start()
    {
        if (playerData == null)
        {
            Debug.LogWarning("ParallaxEffect: PlayerData reference not set. Attempting to find in scene.");
            playerData = FindObjectOfType<PlayerData>();
        }
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (playerData.isMove)
        {
            offset += scrollSpeed * Time.deltaTime;
            mat.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}

