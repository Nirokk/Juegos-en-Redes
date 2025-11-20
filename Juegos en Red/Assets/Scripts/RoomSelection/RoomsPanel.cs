using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomsPanel : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private RoomItemUI roomUIPrefab;

    private List<RoomItemUI> roomsUI = new List<RoomItemUI>();


    void Start()
    {
        InvokeRepeating(nameof(PopulateRoomsList), 0f, 5f);
        NetworkManager.Instance.OnJoinedRoom += GoToTeamSelection;
    }

    //Note: Consider doing this only after the player ask for it
    public void PopulateRoomsList()
    {
        
        ClearRoomsList();

        List<RoomInfo> allRooms = NetworkManager.Instance.GetAllRooms();
       
        Debug.Log("Number of rooms available: " + allRooms.Count);
        foreach (RoomInfo room in allRooms)
        {
            RoomItemUI roomUI = Instantiate(roomUIPrefab, contentTransform);
            roomUI.SetUp(room.Name, HandleJoinRoomRequest);
            roomsUI.Add(roomUI);
            roomUI.gameObject.SetActive(true);
           
                
        }

    }

    private void ClearRoomsList()
    {
        foreach (RoomItemUI room in roomsUI)
        {
            Destroy(room.gameObject);
        }

        roomsUI.Clear();
    }

    private void HandleJoinRoomRequest(string roomName)
    {
        NetworkManager.Instance.JoinSelectedRoom(roomName);

    }


    private void GoToTeamSelection()
    {
        SceneManager.LoadScene("TeamSelection");
        //mandar ambos players al team selection pero para la segunda entrega duh
    }
}
