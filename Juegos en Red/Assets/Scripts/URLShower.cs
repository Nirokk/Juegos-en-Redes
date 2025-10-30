using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class URLShower : MonoBehaviour
{

    public string url;
    public string url2;
    public Button showJoke;
    public TextMeshProUGUI displayText;
    public Image imageDisplay;
    public Joke jokeData;
    
    
    
    // Start is called before the first frame update
    void Start()
    {     
        showJoke.onClick.AddListener(CheckLink);
  
    }
    void CheckLink()
    {
        StartCoroutine(GetJsonData());
    }
    IEnumerator GetJsonData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching JSON: " + webRequest.error);
            }
            else
            {
                string jsonText = webRequest.downloadHandler.text;
                Debug.Log("Received JSON: " + jsonText);
                ProcessJsonData(jsonText);
                
            }
        }
        
    }
    IEnumerator GetIcon(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching Image: " + webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageDisplay.sprite = sprite;
            }
        }
    }
    void ProcessJsonData(string jsonText)
    {
        
        jokeData = JsonConvert.DeserializeObject<Joke>(jsonText);
        displayText.text = jokeData.value;
        StartCoroutine(GetIcon(jokeData.icon_url));



    }
    
    [System.Serializable]
    public class Joke
    {
        public string icon_url;
        public string id;
        public string url;
        public string value;
    }
    
    
}
