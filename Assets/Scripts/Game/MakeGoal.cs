<<<<<<< HEAD
using Mirror.BouncyCastle.Ocsp;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    public Text redScoreText;
    public Text blueScoreText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int red, int blue)
    {
        redScoreText.text = "Red Team Score: " + red.ToString();
        blueScoreText.text = "Blue Team Score: " + blue.ToString();
    }
=======
using UnityEngine;
using Mirror;

public class MakeGoal : NetworkBehaviour
{
    public PlayerObjectController.Team goalForTeam;
    public Transform ballSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.CompareTag("Ball"))
        {
            GameMechanics.Instance.AddGoal(goalForTeam);

            NetworkServer.Destroy(other.gameObject);
            GameMechanics.Instance.RespawnBall();
        }
    }
>>>>>>> d1f72a7c141fdb9929ec8d7608e60295c74d92b5
}