//------------------------------------
//             OmniShade
//     Copyright© 2022 OmniShade     
//------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/**
 * This class creates a menu in Edit->OmniShade to switch between normal and fallback versions.
 */
public static class OmniShadeMenu {
    const string MENU_NORMAL = "Tools/" + OmniShade.NAME + "/Switch To Normal";
    const string MENU_FALLBACK = "Tools/" + OmniShade.NAME + "/Switch To Fallback";
    const string CONVERT = "Convert Selected Materials To OmniShade";
    const string MENU_CONVERT = "Tools/" + OmniShade.NAME + "/" + CONVERT;
    
    [MenuItem(MENU_NORMAL, true)]
    static bool SwitchNormalValidate() {
        OmniShadeMenu.CheckSelected();
        return true;
    }

    [MenuItem(MENU_FALLBACK, true)]
    static bool SwitchFallbackValidate() {
        OmniShadeMenu.CheckSelected();
        return true;
    }

    static void CheckSelected() {
        bool isFallback = OmniShade.IsFallbackShader();
        Menu.SetChecked(MENU_NORMAL, !isFallback);
        Menu.SetChecked(MENU_FALLBACK, isFallback);
    }

    [MenuItem(MENU_NORMAL)]
    static void SwitchNormal() {
        OmniShade.SetNormalShader();
    }

    [MenuItem(MENU_FALLBACK)]
    static void SwitchFallback() {
        OmniShade.SetFallbackShader();
    }

    [MenuItem(MENU_CONVERT, false, 1)]
    static void ConvertMaterial() {
        var textureMapping = new Dictionary<string, string>() {
            { "_BaseMap", "_MainTex" },
            { "_MetallicGlossMap", "_SpecularTex" },
            { "_BumpMap", "_NormalTex" },
            { "_OcclusionMap", "_LightmapTex" },
            { "_DetailAlbedoMap", "_DetailTex" },
            { "_EmissionMap", "_EmissiveTex" },
        };

        // Loop selected materials
		foreach (var selected in Selection.objects) {
            // Skip if not a material
			if (selected.GetType() != typeof(Material))
                continue;

            var mat = selected as Material;
            Undo.RecordObject(mat, CONVERT);

            // Fetch textures from mapping
            var texToReplace = new Dictionary<string, Texture>();
            foreach (var texMap in textureMapping) {
                if (mat.HasProperty(texMap.Key) && mat.GetTexture(texMap.Key) != null) {
                    texToReplace.Add(texMap.Value, mat.GetTexture(texMap.Key));
                    mat.SetTexture(texMap.Key, null);
                }
            }
            // Get emission color
            Vector4 emissive = Vector4.zero;
            if (mat.HasProperty("_EmissionColor"))
                emissive = mat.GetVector("_EmissionColor");

            // Replace shader
            string shaderName = mat.shader.name;
            bool isURP = shaderName.Contains("Universal Render Pipeline") || shaderName.Contains("URP");
            string newShaderName = isURP ? OmniShade.STANDARD_URP_SHADER : OmniShade.STANDARD_SHADER;
            mat.shader = Shader.Find(newShaderName);

            // Replace textures
            foreach (var texToRep in texToReplace) {
                if (mat.HasProperty(texToRep.Key))
                    mat.SetTexture(texToRep.Key, texToRep.Value);
            }
            // Replace emission color
            if (mat.HasProperty("_Emissive"))
                mat.SetColor("_Emissive", emissive);
            // Enable specular
            if (mat.HasProperty("_Specular")) {
                mat.SetFloat("_Specular", 1);
                mat.EnableKeyword("SPECULAR");
            }
		}
    }
}
