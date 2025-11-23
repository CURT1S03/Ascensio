using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Directional Light used as the Sun")]
    public Light sun;

    [Tooltip("Directional Light used as the Moon")]
    public Light moon;

    [Tooltip("Procedural skybox material (Skybox/Procedural)")]
    public Material skyboxMaterial;

    [Tooltip("Minutes for a full 24h cycle in game time")]
    public float dayLengthInMinutes = 2f;

    [Range(0f, 1f)]
    public float timeOfDay = 0.25f;


    [Header("Sun Colors")]
    public Color nightLightColor   = new Color(0.52f, 0.62f, 1.00f);
    public Color sunriseLightColor = new Color(1.00f, 0.63f, 0.40f);
    public Color dayLightColor     = new Color(1.00f, 0.96f, 0.84f);
    public Color sunsetLightColor  = new Color(1.00f, 0.55f, 0.35f);


    [Header("Ambient Colors (cartoony)")]
    public Color nightAmbientColor = new Color(0.15f, 0.22f, 0.30f);
    public Color dayAmbientColor   = new Color(0.60f, 0.85f, 0.65f);


    [Header("Sky Colors (Procedural Skybox)")]
    public Color daySkyColor   = new Color(0.30f, 0.70f, 1.00f);
    public Color nightSkyColor = new Color(0.05f, 0.10f, 0.25f);


    [Header("Intensity / Exposure")]
    [Tooltip("Minimum sun intensity at night")]
    public float minSunIntensity = 0.6f;

    [Tooltip("Maximum sun intensity at noon")]
    public float maxSunIntensity = 2.0f;

    [Tooltip("Minimum skybox exposure at night")]
    public float minExposure = 1.2f;

    [Tooltip("Maximum skybox exposure at noon")]
    public float maxExposure = 1.6f;

    [Header("Moon Settings")]
    [Tooltip("Maximum moon intensity at midnight")]
    public float maxMoonIntensity = 0.7f;


    void Start()
    {
        if (sun == null)
            sun = RenderSettings.sun;

        RenderSettings.ambientMode = AmbientMode.Flat;

        if (skyboxMaterial != null)
            RenderSettings.skybox = skyboxMaterial;
    }


    void Update()
    {
        if (sun == null || dayLengthInMinutes <= 0f)
            return;

        timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);
        if (timeOfDay > 1f)
            timeOfDay -= 1f;

        UpdateLighting(timeOfDay);
    }


    void UpdateLighting(float t)
    {
        float sunAngle = t * 360f - 90f; // -90 = sunrise
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        float intensityFactor = Mathf.Sin(t * Mathf.PI);
        intensityFactor = Mathf.Clamp01(intensityFactor);

        sun.color = EvaluateSunColor(t);

        Color ambient = Color.Lerp(nightAmbientColor, dayAmbientColor, intensityFactor);
        RenderSettings.ambientLight = ambient;

        sun.intensity = Mathf.Lerp(minSunIntensity, maxSunIntensity, intensityFactor);

        if (moon != null)
        {
            float moonAngle = sunAngle + 180f;
            moon.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0f);

            // Moon brightest at night, off at noon
            float moonFactor = 1f - intensityFactor;
            moon.intensity = Mathf.Lerp(0f, maxMoonIntensity, moonFactor);

            moon.color = new Color(0.55f, 0.65f, 1.0f);
        }

        Material mat = skyboxMaterial != null ? skyboxMaterial : RenderSettings.skybox;
        if (mat != null)
        {
            float exposure = Mathf.Lerp(minExposure, maxExposure, intensityFactor);
            mat.SetFloat("_Exposure", exposure);

            Color skyColor = Color.Lerp(nightSkyColor, daySkyColor, intensityFactor);
            mat.SetColor("_SkyTint", skyColor);
        }

        DynamicGI.UpdateEnvironment();
    }


    Color EvaluateSunColor(float t)
    {
        if (t < 0.25f)
        {
            // Night -> Sunrise
            float k = t / 0.25f;
            return Color.Lerp(nightLightColor, sunriseLightColor, k);
        }
        else if (t < 0.5f)
        {
            // Sunrise -> Day
            float k = (t - 0.25f) / 0.25f;
            return Color.Lerp(sunriseLightColor, dayLightColor, k);
        }
        else if (t < 0.75f)
        {
            // Day -> Sunset
            float k = (t - 0.5f) / 0.25f;
            return Color.Lerp(dayLightColor, sunsetLightColor, k);
        }
        else
        {
            // Sunset -> Night
            float k = (t - 0.75f) / 0.25f;
            return Color.Lerp(sunsetLightColor, nightLightColor, k);
        }
    }
}
