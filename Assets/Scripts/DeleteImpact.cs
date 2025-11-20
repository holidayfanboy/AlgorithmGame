using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteImpact : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}
