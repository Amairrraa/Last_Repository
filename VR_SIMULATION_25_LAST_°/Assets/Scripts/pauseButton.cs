using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Drag your pause menu panel here
    private InputDevice leftController;
    private bool wasThumbstickPressed = false;

    void Start()
    {
        // Try to find the left controller at start
        GetLeftController();
    }

    void Update()
    {
        // Refresh controller reference if needed
        if (!leftController.isValid)
        {
            GetLeftController();
            return;
        }

        // Check for left thumbstick click
        if (GetLeftThumbstickClick())
        {
            if (!wasThumbstickPressed)
            {
                wasThumbstickPressed = true;
                TogglePauseMenu();
            }
        }
        else
        {
            wasThumbstickPressed = false;
        }
    }

    private void GetLeftController()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            leftController = leftHandDevices[0];
            Debug.Log("Found left controller: " + leftController.name);
        }
    }

    private bool GetLeftThumbstickClick()
    {
        if (leftController.isValid)
        {
            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isClicked))
            {
                return isClicked;
            }
        }
        return false;
    }

    private void TogglePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            // Toggle the active state of the pause menu panel
            bool newState = !pauseMenuPanel.activeSelf;
            pauseMenuPanel.SetActive(newState);

            // Also pause/unpause game time
            Time.timeScale = newState ? 0f : 1f;

            Debug.Log("Pause menu " + (newState ? "opened" : "closed") + " via left thumbstick");
        }
    }
}