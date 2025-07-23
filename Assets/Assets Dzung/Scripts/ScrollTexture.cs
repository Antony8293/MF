using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    private Renderer rend;
    public  Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.material.mainTextureOffset += offset*Time.deltaTime;
    }
}

