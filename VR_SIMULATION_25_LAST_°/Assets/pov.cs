using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SwitchPOVs2 : MonoBehaviour
{
    /* Selecting controller, choosing POV switching button 
     * and setting the selected controller's initialization time */
    [Header("POV Switch Input Devices")]
    [SerializeField] XRNode SwitchDevice;
    [SerializeField] InputHelpers.Button SwitchButton;
    [SerializeField] float PressingDelay = 0.5f;
    [SerializeField] float InitTime;

    // Getting the driver's eyes and the robot's camera.
    [Header("POVs")]
    [SerializeField] Camera[] Views;

    // Variables to be used in void Update().
    InputDevice SwitchController;
    bool isInit = false;
    bool buttonPressed = false;
    float nextButtPressingTime = 0;
    int i = 0;

    void Update()
    {
        /* Code will not run until initialization 
         * time elapses after the start of the program. */
        if (Time.time > InitTime)
        {
            /* Initilization must only be done once,
             * thus the need for a condition. */
            if (!isInit)
            {
                // Getting the controller using XR Nodes.
                SwitchController = InputDevices.GetDeviceAtXRNode(SwitchDevice);
                isInit = true;
            }

            // Reading digital state of the chosen button.
            SwitchController.IsPressed(SwitchButton, out buttonPressed);

            /* If the button is pressed and the delay passed:
             * 1. Increase the index i by one.
             * 2. Enable the i-th camera and disable
             * the others using a for loop. 
             * 3. Modify the next time where we can change the POV.*/
            if (buttonPressed && Time.time > nextButtPressingTime)
            {
                if (i < Views.Length - 1) i++;
                else i = 0;

                for (int j = 0; j < Views.Length; j++)
                {
                    if (j == i) Views[j].enabled = true;
                    else Views[j].enabled = false;
                }

                nextButtPressingTime = Time.time + PressingDelay;
            }

        }
    }
}
