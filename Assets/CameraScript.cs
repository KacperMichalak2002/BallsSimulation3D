using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float mouseSens = 2f;
    float cameraVerticalRotation = 0f;
    public Transform player;

    void Start()
    {

    }


    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSens;
        float inputY = Input.GetAxis("Mouse Y") * mouseSens;

        // Camera Rotation
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        player.Rotate(Vector3.up * inputX);


    }
}
