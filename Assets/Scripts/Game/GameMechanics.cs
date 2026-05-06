using Mirror;
using Mirror.BouncyCastle.Crypto.Engines;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameMechanics : NetworkBehaviour
{
    public static GameMechanics Instance;

    [SyncVar] public int redScore = 0;
    [SyncVar] public int blueScore = 0;

    private void Awake()
    {
        Instance = this;
    }
    public Camera scene_camera;
    public Text numberOfHealthCollectedText;
    public Transform collectables;
    public HealthCollectable healthCollectable_prefab;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public DamageCollectable damageCollectable_prefab;
    public SpeedCollectable speedCollectable_prefab;

    public int numberOfHealthCollected = 0;

    private int powerup_spawned = 0;

    void Start()
    {
        LocalInit();

        //Server only procedures
        if (isServer)
        {
            StartCoroutine(Srv_SpawnHealthCoroutine());
            StartCoroutine(Srv_spawnDamageCoroutine());
            StartCoroutine(Srv_spawnSpeedCoroutine());
        }
        
    }

    public void RespawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        NetworkServer.Spawn(newBall);
    }
    [Server]
    public void AddGoal(PlayerObjectController.Team scoringTeam)
    {
        if (scoringTeam == PlayerObjectController.Team.Red)
        {
            redScore++;
        }
        else if (scoringTeam == PlayerObjectController.Team.Blue)
        {
            blueScore++;
        }

        RpcUpdateScoreUI(redScore, blueScore);
    }

    [ClientRpc]
    void RpcUpdateScoreUI(int red, int blue)
    {
        ScoreUI.Instance.UpdateScore(red, blue);
    }

    [Server]
    private IEnumerator Srv_SpawnHealthCoroutine()
    {
        yield return new WaitForSeconds(0.1f); //wait for network readiness

        while (true)
        {
            yield return new WaitForSeconds(1.5f); //Check once per second

            //Check how many health prefabs there are currently  //Nate here, this now checks for all collectables, just letting you know that : )
            powerup_spawned = collectables.childCount;

            while(powerup_spawned < 4) // nate here again, changed this number to make sure the other powerups always leave room for a health powerup, so they cant be removed from the spawn pool
            {
                Vector3 ranLocation = new Vector3(Random.Range(-20, 20), 0.6f, Random.Range(-20, 20));

                //Only works locally (on the server)
                GameObject healthToAdd = Instantiate(healthCollectable_prefab, ranLocation, Quaternion.identity, collectables).gameObject;
                NetworkServer.Spawn(healthToAdd); //Spawn the object on the network (all clients)
                StartCoroutine(DelayedParentSet(healthToAdd));
                powerup_spawned++;
            }
        }
    }

    [Server]
    private IEnumerator Srv_spawnDamageCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            yield return new WaitForSeconds(2.51f); // making the timers different to adjust rarity of spawns for different powerups, allowing for a variety to spawn at different times -Nate

            powerup_spawned = collectables.childCount;
            while(powerup_spawned < 3)
            {
                Vector3 ranLocation = new Vector3(Random.Range(-20, 20), 0.6f, Random.Range(-20, 20));

                GameObject damageToAdd = Instantiate(damageCollectable_prefab, ranLocation, Quaternion.identity, collectables).gameObject;
                NetworkServer.Spawn(damageToAdd);
                StartCoroutine(DelayedParentSet(damageToAdd));
                powerup_spawned++;
            }           
        }
    }

    [Server]
    private IEnumerator Srv_spawnSpeedCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            yield return new WaitForSeconds(2.2f);

            powerup_spawned = collectables.childCount;
            while (powerup_spawned < 3)
            {
                Vector3 ranLocation = new Vector3(Random.Range(-20, 20), 0.6f, Random.Range(-20, 20));

                GameObject speedToAdd = Instantiate(speedCollectable_prefab, ranLocation, Quaternion.identity, collectables).gameObject;
                NetworkServer.Spawn(speedToAdd);
                StartCoroutine(DelayedParentSet(speedToAdd));
                powerup_spawned++;
            }
        }
    }

    private IEnumerator DelayedParentSet(GameObject spawned)
    {
        yield return new WaitForSeconds(0.3f); //make sure clients spawned the object already

        var hc = spawned.GetComponent<HealthCollectable>();
        if(hc != null)
        {
            hc.Rpc_SetParent();
        }

        var dc = spawned.GetComponent<DamageCollectable>();
        if(dc != null)
        {
            dc.Rpc_SetParent();
        }
    }

    public void LocalInit()
    {
        scene_camera.gameObject.SetActive(false);
        Debug.Log("LocalInit");
        numberOfHealthCollectedText.text = "HEALTH COLLECTED: 0";
    }

    [ClientRpc]
    public void Rpc_OnHealthCollected()
    {
        numberOfHealthCollected++;
        
        numberOfHealthCollectedText.text = "HEALTH COLLECTED: " + numberOfHealthCollected;
    }
}
