using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Realtime;
using Photon.Pun;
#pragma warning disable 649


public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{

    [Tooltip("The current Health of our player")]
    public float Health = 100f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public CharacterController controller;
    public float gravityScale;
    public float moveSpeed;
    private Vector3 moveDirection;

    public void Awake()
    {

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }


    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();

#if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
    }


    public void Update()
    {
        // we only process Inputs and check health if we are the local player
        if (photonView.IsMine)
        {
            this.ProcessInputs();

            if (this.Health <= 0f)
            {
                WorldController.Instance.LeaveRoom();
            }
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        //this.Health -= 0.1f;
    }

    public void OnTriggerStay(Collider other)
    {

        //this.Health -= 0.1f * Time.deltaTime;
    }


#if !UNITY_5_4_OR_NEWER
                /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
                void OnLevelWasLoaded(int level)
                {
                    this.CalledOnLevelWasLoaded(level);
                }
#endif



    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }




#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
#endif

    void ProcessInputs()
    {
        /*
        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;


        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);
        */
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.Health);
        }
        else
        {
            this.Health = (float)stream.ReceiveNext();
        }
    }

}
