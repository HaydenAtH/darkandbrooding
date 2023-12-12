

float3 AdditionalLights(float3 color){
    // Get the additional lights and return the direction
    uint numAdditionalLights = GetAdditionalLightsCount();
        for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
            Light light = GetAdditionalLight(lightI);
            color += light.color * saturate(dot(light.direction, normal));
        }

    OUTPUT_COLOR(color);

}

#endif