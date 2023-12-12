float _PointLightCount = 2;
// Declare a uniform array to hold the positions of the point lights
uniform float3 pointLightPositions[2];

// Calculates attenuation based on distance and attenuation parameters
float CalculateAttenuation(float distanceToLight, float3 attenuation)
{
    float distanceSquared = distanceToLight * distanceToLight;
    float attenuationFactor = 1 / (attenuation.x + attenuation.y * distanceToLight + attenuation.z * distanceSquared);
    return attenuationFactor;
}

// Calculates lighting from point lights
float3 CalculatePointLight_float(float3 pointLightColor, float3 pointLightAttenuation, float3 worldPos, float3 normal, out float3 diffuseLighting)
{
    /*
    float3 toLight = worldPos - pointLightPositions[0];
    float distanceToLight = length(toLight);
    float3 normalizedLightDir = toLight / distanceToLight;

    // Calculate the attenuation factor using inverse square law
    float attenuation = 1.0 / (pointLightAttenuation.x + pointLightAttenuation.y * distanceToLight + pointLightAttenuation.z * distanceToLight * distanceToLight);

    // Calculate the diffuse lighting contribution
    float diffuse = max(0, dot(normal, normalizedLightDir));
    diffuseLighting = pointLightColor * diffuse * attenuation;
    diffuseLighting = length(diffuseLighting);

    // Return the point light intensity
    return diffuseLighting;
    */
    float3 toLight = worldPos - pointLightPositions[0];
    float distanceToLight = length(toLight);
    float3 normalizedLightDir = toLight / distanceToLight;

    // Calculate the attenuation factor using inverse square law
    float attenuation = 1.0 / (pointLightAttenuation.x + pointLightAttenuation.y * distanceToLight + pointLightAttenuation.z * distanceToLight * distanceToLight);

    // Scale the attenuation by the distance from the light source
    attenuation *= 1.0 / distanceToLight;

    // Calculate the diffuse lighting contribution
    float diffuse = max(0, dot(normal, normalizedLightDir));
    diffuseLighting = pointLightColor * diffuse * attenuation;
    diffuseLighting = length(diffuseLighting);

    // Return the point light intensity
    return diffuseLighting;
}

