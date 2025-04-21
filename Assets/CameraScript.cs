using UnityEngine;

public class CameraScript : MonoBehaviour
{
   // public float mouseSens = 2f;
   // float cameraVerticalRotation = 0f;
   // public Transform player;

    public GameObject topViewCamera;
    public GameObject sideViewCamera;

    void Start()
    {

    }


    void Update()
    {


        if (Input.GetKeyDown("1"))
        {
            swtichToTopView(true);
        }
        else if (Input.GetKeyDown("2")) 
        {
            swtichToTopView(false);
        }
        // Moving camera with mouse
        /*
        float inputX = Input.GetAxis("Mouse X") * mouseSens;
        float inputY = Input.GetAxis("Mouse Y") * mouseSens;

        // Camera Rotation
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        player.Rotate(Vector3.up * inputX);
        */
    }

    private void swtichToTopView(bool flag)
    {
        topViewCamera.SetActive(flag);
        sideViewCamera.SetActive(!flag);
    }
}
