using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{

    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Button joinButton;

    private Action<string> onClickedJoin;
    private string roomName;
    public void SetUp(string roomName, Action<string> onJoinCallback)
    {
        this.roomName = roomName;
        roomNameText.text = this.roomName;
        joinButton.onClick.AddListener(HandleJoinRoomClicked);
        onClickedJoin += onJoinCallback;
    }

    private void HandleJoinRoomClicked()
    {
        onClickedJoin?.Invoke(this.roomName);
    }
}
