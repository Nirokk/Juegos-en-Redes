using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GunFlash : MonoBehaviour
{
    public Light2D flashLight;
    public float flashIntensity = 1f;       // Intensidad cuando dispara
    public float flashDuration = 1f;        // Tiempo "encendida"
    public float fadeSpeed = 2f;            // Velocidad de apagado

    private float targetIntensity = 0f;

    private void Start()
    {
        if (flashLight == null)
            flashLight = GetComponent<Light2D>();

        flashLight.intensity = 0f;
    }

    private void Update()
    {
        // Transición cool
        flashLight.intensity = Mathf.Lerp(flashLight.intensity, targetIntensity, Time.deltaTime * fadeSpeed);
    }

    public void TriggerFlash()
    {
        //iniciado
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        //Encendido
        targetIntensity = flashIntensity;

        //Esperar duración del destello
        yield return new WaitForSeconds(flashDuration);

        //Apagado x.x
        targetIntensity = 0f;
    }
}