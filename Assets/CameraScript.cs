using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // public float mouseSens = 2f;
    // float cameraVerticalRotation = 0f;
    // public Transform player;

    public GameObject topViewCamera;
    public GameObject sideViewCamera;
    public BallScript[] balls;

    private bool showGUI = true;

    private Vector3[] velocities = new Vector3[3];
    private float[] masses = new float[3];
    private float[] radii = new float[3];

    private string[] massInputs = new string[3];
    private string[] radiusInputs = new string[3];
    private string[] velocityXInputs = new string[3];
    private string[] velocityYInputs = new string[3];
    private string[] velocityZInputs = new string[3];

    private string previousFocusedControl = "";

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            velocities[i] = new Vector3(1, 0, 0);
            masses[i] = 1f;
            radii[i] = 0.5f;

            massInputs[i] = masses[i].ToString();
            radiusInputs[i] = radii[i].ToString();
            velocityXInputs[i] = velocities[i].x.ToString();
            velocityYInputs[i] = velocities[i].y.ToString();
            velocityZInputs[i] = velocities[i].z.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            SwitchToTopView(true);
        }
        else if (Input.GetKeyDown("2"))
        {
            SwitchToTopView(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSimulation();
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

    private void OnGUI()
    {
        if (!showGUI) return;

        GUI.Box(new Rect(10, 10, 300, 470), "Ball Properties");

        string currentFocusedControl = GUI.GetNameOfFocusedControl();

        for (int i = 0; i < 3; i++)
        {
            float yPos = 25 + i * 150;

            GUI.Label(new Rect(20, yPos, 280, 20), $"Ball {i + 1} Properties");

            // Mass
            GUI.Label(new Rect(20, yPos + 25, 100, 20), "Mass (1 - 10):");
            GUI.SetNextControlName($"mass_{i}");
            massInputs[i] = GUI.TextField(new Rect(120, yPos + 25, 100, 20), massInputs[i]);

            // Radius
            GUI.Label(new Rect(20, yPos + 50, 100, 20), "Radius (0.5 - 5):");
            GUI.SetNextControlName($"radius_{i}");
            radiusInputs[i] = GUI.TextField(new Rect(120, yPos + 50, 100, 20), radiusInputs[i]);

            // Velocity X
            GUI.Label(new Rect(20, yPos + 75, 100, 20), "Velocity X (-10 - 10):");
            GUI.SetNextControlName($"velX_{i}");
            velocityXInputs[i] = GUI.TextField(new Rect(120, yPos + 75, 100, 20), velocityXInputs[i]);

            // Velocity Y
            GUI.Label(new Rect(20, yPos + 100, 100, 20), "Velocity Y (-10 - 10):");
            GUI.SetNextControlName($"velY_{i}");
            velocityYInputs[i] = GUI.TextField(new Rect(120, yPos + 100, 100, 20), velocityYInputs[i]);

            // Velocity Z
            GUI.Label(new Rect(20, yPos + 125, 100, 20), "Velocity Z (-10 - 10):");
            GUI.SetNextControlName($"velZ_{i}");
            velocityZInputs[i] = GUI.TextField(new Rect(120, yPos + 125, 100, 20), velocityZInputs[i]);
        }

        if (currentFocusedControl != previousFocusedControl && !string.IsNullOrEmpty(previousFocusedControl))
        {
            ApplyField(previousFocusedControl);
        }

        previousFocusedControl = currentFocusedControl;
    }

    private void ApplyField(string controlName)
    {
        string[] parts = controlName.Split('_');
        if (parts.Length != 2) return;

        string type = parts[0];
        int index;
        if (!int.TryParse(parts[1], out index)) return;

        float parsed;
        switch (type)
        {
            case "mass":
                if (float.TryParse(massInputs[index], out parsed) && parsed >= 1f && parsed <= 10f)
                {
                    masses[index] = parsed;
                }
                else
                {
                    masses[index] = 1f;
                }
                massInputs[index] = masses[index].ToString();
                break;

            case "radius":
                if (float.TryParse(radiusInputs[index], out parsed) && parsed >= 0.5f && parsed <= 5.0f)
                {
                    radii[index] = parsed;
                }
                else
                {
                    radii[index] = 0.5f;
                }
                radiusInputs[index] = radii[index].ToString();
                break;

            case "velX":
                if (float.TryParse(velocityXInputs[index], out parsed) && parsed >= -10.0f && parsed <= 10.0f)
                {
                    velocities[index].x = parsed;
                }
                else
                {
                    velocities[index].x = 0f;
                }
                velocityXInputs[index] = velocities[index].x.ToString();
                break;

            case "velY":
                if (float.TryParse(velocityYInputs[index], out parsed) && parsed >= -10.0f && parsed <= 10.0f)
                {
                    velocities[index].y = parsed;
                }
                else
                {
                    velocities[index].y = 0f;
                }
                velocityYInputs[index] = velocities[index].y.ToString();
                break;

            case "velZ":
                if (float.TryParse(velocityZInputs[index], out parsed) && parsed >= -10.0f && parsed <= 10.0f)
                {
                    velocities[index].z = parsed;
                }
                else
                {
                    velocities[index].z = 0f;
                }
                velocityZInputs[index] = velocities[index].z.ToString();
                break;
        }

        if (balls[index] != null)
        {
            balls[index].SetProperties(velocities[index], masses[index], radii[index]);
        }
    }



    private void StartSimulation()
    {
        showGUI = false;

        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i] != null)
            {
                balls[i].SetProperties(velocities[i], masses[i], radii[i]);
                balls[i].StartMovement();
            }
        }
    }

    private void SwitchToTopView(bool flag)
    {
        topViewCamera.SetActive(flag);
        sideViewCamera.SetActive(!flag);
    }
}
