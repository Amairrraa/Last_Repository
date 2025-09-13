using UnityEngine;
using UnityEngine.XR;

public class VRCameraCycle : MonoBehaviour
{
    [Header("Assign Cameras")]
    public GameObject xrCamera;      // XR Main Camera (inside XR Origin)
    public GameObject robotCamera;   // Robot Camera
    public GameObject topCamera;     // Top-down Camera
    public GameObject sideCamera;    // Side Camera

    private GameObject[] cameras;
    private int currentIndex = 0;

    private bool buttonPressedLastFrame = false;

    void Start()
    {
        // Include the new side camera
        cameras = new GameObject[] { xrCamera, robotCamera, topCamera, sideCamera };
        SetActiveCamera(0); // Start with XR
    }

    void Update()
    {
        // Right-hand controller
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Joystick click (press stick)
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool pressed))
        {
            if (pressed && !buttonPressedLastFrame)
            {
                CycleCamera();
            }
            buttonPressedLastFrame = pressed;
        }
    }

    void CycleCamera()
    {
        currentIndex = (currentIndex + 1) % cameras.Length;
        print("Switched to: " + cameras[currentIndex].name);
        SetActiveCamera(currentIndex);
    }

    void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == index);
        }
    }
}
