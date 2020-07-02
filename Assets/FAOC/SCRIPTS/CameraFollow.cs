using UnityEngine;

using Photon.Pun;


public class CameraFollow : MonoBehaviourPun
{
    public Transform cameraTarget;
    float cameraPitch = 40.0f;
    float cameraYaw = 0;
    float cameraDistance = 10.0f;
    bool lerpYaw = false;
    bool lerpDistance = false;

    public GameObject cameraLockPosition;
    public float cameraPitchSpeed = 2.0f;
    public float cameraPitchMin = -10.0f;
    public float cameraPitchMax = 80.0f;
    public float cameraYawSpeed = 5.0f;
    public float cameraDistanceSpeed = 5.0f;
    public float cameraDistanceMin = 2.0f;
    public float cameraDistanceMax = 20.0f;
    public float moveDirectionSpeed = 6.0f;
    public float turnSpeed = 3.0f;
    public float jumpSpeed = 8.0f;
    public float gravitySpeed = 20.0f;

    void Start()
    {
        cameraTarget = cameraLockPosition.transform;
    }

    void Update()
    {

    }

    public void LateUpdate()
    {

        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        // If mouse button down then allow user to look around
        if (Input.GetMouseButton(0))
        {
            cameraPitch -= Input.GetAxis("Mouse Y") * cameraPitchSpeed;
            cameraPitch = Mathf.Clamp(cameraPitch, cameraPitchMin, cameraPitchMax);
            cameraYaw += Input.GetAxis("Mouse X") * cameraYawSpeed;
            cameraYaw = cameraYaw % 360.0f;
            lerpYaw = false;
        }
        else
        {
            // If moving then make camera follow
            if (lerpYaw)
            {
                cameraYaw = Mathf.LerpAngle(cameraYaw, cameraTarget.eulerAngles.y, 5.0f * Time.deltaTime);
            }
        }

        if (Input.GetMouseButton(1))
        {
            cameraPitch -= Input.GetAxis("Mouse Y") * cameraPitchSpeed;
            cameraPitch = Mathf.Clamp(cameraPitch, cameraPitchMin, cameraPitchMax);
            cameraYaw += Input.GetAxis("Mouse X") * cameraYawSpeed;
            cameraYaw = cameraYaw % 360.0f;
            lerpYaw = false;

            // Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, cameraYaw, 0) * Time.deltaTime);
            // rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        }
        else
        {
            // If moving then make camera follow
            if (lerpYaw)
            {
                cameraYaw = Mathf.LerpAngle(cameraYaw, cameraTarget.eulerAngles.y, 5.0f * Time.deltaTime);
            }
        }

        // Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * cameraDistanceSpeed;
            cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
            lerpDistance = false;
        }

        // Calculate camera position
        Vector3 newCameraPosition = cameraTarget.position + (Quaternion.Euler(cameraPitch, cameraYaw, 0) * Vector3.back * cameraDistance);

        // make sure camera does not go into obejcts aka Does new position put us inside anything? 
        /*
        RaycastHit hitInfo;
        if (Physics.Linecast(cameraTarget.position, newCameraPosition, out hitInfo))
        {
            Debug.Log("WE ARE INSIDE");
            newCameraPosition = hitInfo.point;
            lerpDistance = true;
        }
        else
        {
            if (lerpDistance)
            {
                float newCameraDistance = Mathf.Lerp(Vector3.Distance(cameraTarget.position, Camera.main.transform.position), cameraDistance, 5.0f * Time.deltaTime);
                newCameraPosition = cameraTarget.position + (Quaternion.Euler(cameraPitch, cameraYaw, 0) * Vector3.back * newCameraDistance);
            }
        }
        */

        Camera.main.transform.position = newCameraPosition;
        Camera.main.transform.LookAt(cameraTarget.position);
    }
}
