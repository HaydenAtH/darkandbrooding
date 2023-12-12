using System.Collections.Generic;
using UnityEngine;


// ! Delete?
public class PointLightManager : MonoBehaviour
{
    // Reference to the material that uses the point light positions
    public Material material;

    // Array to hold the positions of the point lights
    Vector4[] pointLightPositions;

    void Start()
    {
        // Find all PointLight components in the scene
        Light[] pointLights = Object.FindObjectsOfType<Light>();

        // Initialize the pointLightPositions array with the correct size
        pointLightPositions = new Vector4[pointLights.Length];

        // Fill the pointLightPositions array with the positions of the point lights
        for (int i = 0; i < pointLights.Length; i++)
        {
            pointLightPositions[i] = new Vector4(pointLights[i].transform.position.x, pointLights[i].transform.position.y, pointLights[i].transform.position.z, 0f);
        }

        // Pass the pointLightPositions array to the material
        material.SetVectorArray("pointLightPositions", pointLightPositions);
    }

    void Update() {
        // Find all PointLight components in the scene
        Light[] allLights = Object.FindObjectsOfType<Light>();
        List<Light> pointLights = new List<Light>();

        foreach (Light l in allLights){
            if (l.enabled && l.type == LightType.Point){
                pointLights.Add(l);
            }
        }

        // Initialize the pointLightPositions array with the correct size
        pointLightPositions = new Vector4[pointLights.Count];

        // Fill the pointLightPositions array with the positions of the point lights
        for (int i = 0; i < pointLights.Count; i++)
        {
            pointLightPositions[i] = new Vector4(pointLights[i].transform.position.x, pointLights[i].transform.position.y, pointLights[i].transform.position.z, 0f);
        }

        // Pass the pointLightPositions array to the material
        material.SetVectorArray("pointLightPositions", pointLightPositions);
    }
}
