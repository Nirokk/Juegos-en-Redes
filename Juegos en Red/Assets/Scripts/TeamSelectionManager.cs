using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionManager : MonoBehaviour
{

    [SerializeField] private GameObject NoTeamPanel;
    // Start is called before the first frame update
    void Start()
    {
        NoTeamPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
