using UnityEngine;
using Mirror;

public class DamageCollectable : NetworkBehaviour
{
    public int DamageUpAmount = 1;

    [ClientRpc]
    public void Rpc_SetParent()
    {
        var collectableParent = GameObject.Find("collectables_container")?.transform;
        if(collectableParent != null )
        {
            transform.SetParent(collectableParent);
        }
    }
}


