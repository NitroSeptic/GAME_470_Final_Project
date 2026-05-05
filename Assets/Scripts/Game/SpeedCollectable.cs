using UnityEngine;
using Mirror;

public class SpeedCollectable : NetworkBehaviour
{
    [ClientRpc]

    public void Rpc_SetParent()
    {
        var collectableParent = GameObject.Find("collectables_container")?.transform;
        if (collectableParent != null)
        {
            transform.SetParent(collectableParent);
        }
    }
}
