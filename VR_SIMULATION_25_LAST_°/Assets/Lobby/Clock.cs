using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    [Header("Start Time Settings")]
    [Range(0, 23)] public int startHour = 17;
    [Range(0, 59)] public int startMinute = 0;
    [Range(0, 59)] public int startSecond = 0;
    public bool useRealTime = false;

    [Header("Tilt Settings")]
    [Tooltip("Tilt angle of the clock face in degrees.")]
    public float tiltAngle = 45f;

    private float timeInSeconds;
    private Quaternion tiltRotation;

    void Start()
    {
        // Apply tilt rotation (you can adjust the axis here if needed)
        tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        if (useRealTime)
        {
            System.DateTime now = System.DateTime.UtcNow.AddHours(-4);
            timeInSeconds = now.Hour * 3600 + now.Minute * 60 + now.Second;
        }
        else
        {
            timeInSeconds = startHour * 3600 + startMinute * 60 + startSecond;
        }
    }

    void Update()
    {
        timeInSeconds += Time.deltaTime;
        UpdateClockDisplayFromSeconds(timeInSeconds);
    }

    private void UpdateClockDisplayFromSeconds(float totalSeconds)
    {
        float hours = (totalSeconds / 3600) % 24;
        float minutes = (totalSeconds / 60) % 60;
        float seconds = totalSeconds % 60;
        UpdateClockDisplay(hours, minutes, seconds);
    }

    private void UpdateClockDisplay(float hours, float minutes, float seconds)
    {
        float hourRotation = (hours % 12) * 30f + (minutes / 60f) * 30f;
        float minuteRotation = minutes * 6f;
        float secondRotation = seconds * 6f;

        // Apply the tilt to each hand rotation
        if (hourHand != null)
            hourHand.localRotation = tiltRotation * Quaternion.Euler(-hourRotation, 0, 0);
        if (minuteHand != null)
            minuteHand.localRotation = tiltRotation * Quaternion.Euler(-minuteRotation, 0, 0);
        if (secondHand != null)
            secondHand.localRotation = tiltRotation * Quaternion.Euler(-secondRotation, 0, 0);
    }
}
