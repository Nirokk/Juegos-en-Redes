using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public bool hasGrenade;

    public void GiveGrenade()
    {
        hasGrenade = true;
        Debug.Log("Granada equipada!");
    }

    public bool TryUseGrenade()
    {
        Debug.Log("TRY USE GRENADE — hasGrenade = " + hasGrenade);
        if (!hasGrenade) return false;

        hasGrenade = false;
        return true;
    }
}
