using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;

public class GunFlash : MonoBehaviourPun
{
    public Light2D flashLight;
    public float flashIntensity = 1f;
    public float flashDuration = 1f;
    public float fadeSpeed = 2f;

    private float targetIntensity = 0f;

    private void Start()
    {
        if (flashLight == null)
            flashLight = GetComponent<Light2D>();

        flashLight.intensity = 0f;
    }

    private void Update()
    {
        flashLight.intensity = Mathf.Lerp(flashLight.intensity, targetIntensity, Time.deltaTime * fadeSpeed);
    }

    //se llama cuando dispara el que tiene el caño
    public void TriggerFlash()
    {
        //Sincronizacion con todos los jugadores
        photonView.RPC("RPC_Flash", RpcTarget.All);   
    }

    [PunRPC]
    private void RPC_Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        targetIntensity = flashIntensity;
        yield return new WaitForSeconds(flashDuration);
        targetIntensity = 0f;
    }
}