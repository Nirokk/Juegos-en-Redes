using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StadiumLightsManager : MonoBehaviour
{
    [Header("Spotlights")]
    public GameObject lightPrefab;
    public float spawnRadius = 5f;

    [Header("Global Light")]
    public Light2D globalLight;
    public float fadeDuration = 5f; // seconds until light is fully off

    private float fadeTimer;
    private bool fading = true;

    void Start()
    {
        // Start fading global light
        fadeTimer = fadeDuration;

        // Start the first spotlight
        SpawnLight();
    }

    void Update()
    {
        if (fading && globalLight != null)
        {
            fadeTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration); // goes from 1 → 0
            globalLight.intensity = t;

            if (fadeTimer <= 0)
            {
                fading = false; // stop once fully dark
                globalLight.intensity = 0f;
            }
        }
    }

    void SpawnLight()
    {
        // Pick random start and end points inside circle
        Vector2 startPos = Random.insideUnitCircle * spawnRadius;
        Vector2 endPos = Random.insideUnitCircle * spawnRadius;

        // Avoid too short paths
        while (Vector2.Distance(startPos, endPos) < 2f)
        {
            endPos = Random.insideUnitCircle * spawnRadius;
        }

        // Create spotlight
        GameObject newLight = Instantiate(lightPrefab, startPos, Quaternion.identity);

        // Give it a target + callback to spawn next when done
        newLight.GetComponent<MovingLight2D>().Init(endPos, SpawnLight);
    }
}
