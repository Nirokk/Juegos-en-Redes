using UnityEngine;
using Photon.Pun;   
using UnityEngine.Rendering.Universal;

public class StadiumLightsManager : MonoBehaviourPun 
{
    [Header("Spotlights")]
    public GameObject lightPrefab;
    public float spawnRadius = 5f;

    [Header("Global Light")]
    public Light2D globalLight;
    public float fadeDuration = 5f;

    private float fadeTimer;
    private bool fading = true;

    private void Start()
    {
        fadeTimer = fadeDuration;

        if (PhotonNetwork.IsMasterClient)      
            SpawnLight();                      
    }

    private void Update()
    {
        //bool
        if (MatchTimer.damagePhase)
        {
            fading = false;
            globalLight.intensity = 0.75f;
            return;
        }
        //low taper fade
        if (fading && globalLight != null)
        {
            fadeTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            globalLight.intensity = t;

            if (fadeTimer <= 0)
            {
                fading = false;
                globalLight.intensity = 0f;
            }
        }

        
    }

    //sincronizando
    public void SpawnLight()                  
    {
        if (!PhotonNetwork.IsMasterClient)    
            return;

        Vector2 startPos = Random.insideUnitCircle * spawnRadius;
        Vector2 endPos = Random.insideUnitCircle * spawnRadius;

        while (Vector2.Distance(startPos, endPos) < 2f)
            endPos = Random.insideUnitCircle * spawnRadius;

        //photon inst
        GameObject obj = PhotonNetwork.Instantiate(
            lightPrefab.name,
            startPos,
            Quaternion.identity
        );

        obj.GetComponent<MovingLight2D>().SetTarget(endPos);  
    }
}
