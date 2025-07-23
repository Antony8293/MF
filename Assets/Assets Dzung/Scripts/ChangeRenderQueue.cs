using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRenderQueue : MonoBehaviour
{
    public void ChangePropertyShaderRope()
    {
        GetComponent<Renderer>().material.renderQueue = 3998;

        GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
        GetComponent<Renderer>().material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }
    public void ChangePropertyShaderRopeHead()
    {
        GetComponent<Renderer>().material.renderQueue = 3999;

        GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
        // GetComponent<Renderer>().material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }
    public void ChangePropertyShaderWool()
    {
        GetComponent<Renderer>().material.renderQueue = 4000;

        GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
        // GetComponent<Renderer>().material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }
    public void ResetPropertyShader()
    {
        GetComponent<Renderer>().material.renderQueue = 2000;

        GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
        // GetComponent<Renderer>().material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
    }
}
