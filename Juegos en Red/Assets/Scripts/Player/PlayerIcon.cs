using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;
using Photon.Realtime;

public class PlayerIcon : MonoBehaviour
{

    [SerializeField] private TMP_Text _playerNameText;
    public Image playerColor;
    private Player _playerName;

    public void SetUp(Player playerName)
    {
        this._playerName = playerName;
        _playerNameText.text = playerName.NickName;
    }


}
