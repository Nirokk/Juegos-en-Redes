using Photon.Pun;
using UnityEngine;
using LootLocker.Requests;

public class PlayerStats : MonoBehaviourPun
{
    public int personalKills = 0;

    [PunRPC]
    public void AddPersonalKill(int killerActorNumber)
    {
        if (photonView.Owner.ActorNumber == killerActorNumber)
        {
            personalKills++;
            Debug.Log($"🔥 Kill sumada a {photonView.Owner.NickName}. Total: {personalKills}");
            SaveKillsToLootLocker();
        }
    }

    private void SaveKillsToLootLocker()
    {
        LootLockerBootStrap.SubmitScore(personalKills, "mostkills", (success) =>
        {
            if (success)
                Debug.Log($"📌 Kills guardadas = {personalKills}");
            else
                Debug.LogError("❌ Error enviando kills.");
        });
    }
}
