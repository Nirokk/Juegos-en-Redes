using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StadiumLightsManager : MonoBehaviour
{
    public GameObject lightPrefab;
    public float spawnRadius = 5f;

    void Start()
    {
        SpawnLight();
    }

    void SpawnLight()
    {
        // Pick random start and end points inside circle
        Vector2 startPos = Random.insideUnitCircle * spawnRadius;
        Vector2 endPos = Random.insideUnitCircle * spawnRadius;

        // Avoid very short paths
        while (Vector2.Distance(startPos, endPos) < 2f)
        {
            endPos = Random.insideUnitCircle * spawnRadius;
        }

        // Create light
        GameObject newLight = Instantiate(lightPrefab, startPos, Quaternion.identity);

        // Give it a target + callback to spawn next when done
        newLight.GetComponent<MovingLight2D>().Init(endPos, SpawnLight);
    }
}
