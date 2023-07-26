using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class SetLightAsSunDirectionShaderGlobal : MonoBehaviour
{
    private Light SunLight;

    // Start is called before the first frame update
    void Start()
    {
        SunLight = GetComponent<Light>();

        //TODO: Maybe make it also update this Shader Global if the direction changes during play
        Shader.SetGlobalVector("SunDirection", SunLight.gameObject.transform.eulerAngles);
        Shader.SetGlobalVector("SunColor", SunLight.color * SunLight.intensity);
    }

    #if UNITY_EDITOR
    private void Update()
    {
        Shader.SetGlobalVector("SunDirection", SunLight.gameObject.transform.eulerAngles);
        Shader.SetGlobalVector("SunColor", SunLight.color * SunLight.intensity);
    }
    #endif
}
