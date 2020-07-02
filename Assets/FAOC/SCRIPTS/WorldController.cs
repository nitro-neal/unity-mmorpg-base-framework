using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;

public class WorldController : MonoBehaviourPunCallbacks
{

    static public WorldController Instance;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    public GameObject mob;

    private List<GameObject> mobs = new List<GameObject>();
    private List<GameObject> players = new List<GameObject>();
    private GameObject rab;

    void Start()
    {
        Instance = this;

        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(-235.8997f, 3.234387f, -229.9808f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            players.Add(playerPrefab);
            loadMobs();
        }
    }

    void Update()
    {
        // "back" button of phone equals "Escape". quit app if that's pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }



        if (PhotonNetwork.IsMasterClient)
        {
            updateMobs();
        }

    }

    void loadMobs()
    {
        GameObject rabMob = PhotonNetwork.Instantiate(mob.name, new Vector3(-197.71f, 0.12f, -201.13f), Quaternion.identity, 0);
        mobs.Add(rabMob);
    }

    void updateMobs()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float mobAggroRange = 500f;
        float mobStoppingRange = 5;
        float rotateSpeed = 10;
        float walkspeed = 5;

        foreach (GameObject mob in mobs)
        {
            foreach (GameObject player in players)
            {
                float distanceSqr = (player.transform.position - mob.transform.position).sqrMagnitude;
                if (distanceSqr < mobAggroRange)
                {
                    if (distanceSqr > mobStoppingRange)
                    {
                        // Determine which direction to rotate towards
                        Vector3 targetDirection = player.transform.position - mob.transform.position;

                        // The step size is equal to speed times frame time.
                        float singleStep = rotateSpeed * Time.deltaTime;

                        // Rotate the forward vector towards the target direction by one step
                        Vector3 newDirection = Vector3.RotateTowards(mob.transform.forward, targetDirection, singleStep, 0.0f);

                        // Draw a ray pointing at our target in
                        Debug.DrawRay(mob.transform.position, newDirection, Color.red);

                        // Calculate a rotation a step closer to the target and applies rotation to this object
                        mob.transform.rotation = Quaternion.LookRotation(newDirection);
                        mob.transform.position = Vector3.MoveTowards(mob.transform.position, player.transform.position, walkspeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

