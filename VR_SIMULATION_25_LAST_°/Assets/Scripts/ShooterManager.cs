using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProjectionManager Projection;
    [SerializeField] private Transform BallSpawn;
    [SerializeField] private BiodiversityUnitManager BallPrefab;

    [Header("Settings")]
    [SerializeField] private float Force = 20f;
    [SerializeField] private float ArcAmount = 0.5f;
    [SerializeField] private float AimStep = 0.1f;
    [SerializeField] private float SlideSpeed = 5f;
    [SerializeField] private float SlideStopDistance = 0.1f;

    private BiodiversityUnitManager currentBall;
    private BiodiversityUnitManager slidingBall;
    private bool isSliding = false;

    private void Update()
    {
        HandleControls();
        HandleSliding();

        if (currentBall != null)
        {
            Projection.SimulateTrajectory(BallPrefab, BallSpawn, Force, ArcAmount);
        }
        else
        {
            Projection.ClearLine();
        }
    }

    private void LateUpdate()
    {
        if (currentBall != null)
        {
            currentBall.transform.position = BallSpawn.position;
            currentBall.transform.rotation = BallSpawn.rotation;
        }
    }

    private void HandleControls()
    {
        if (Input.GetKeyDown(KeyCode.S) && currentBall != null)
        {
            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            Vector3 launchDirection = (BallSpawn.forward + BallSpawn.up * ArcAmount).normalized;
            rb.AddForce(launchDirection * Force, ForceMode.Impulse);
            currentBall = null;
        }

        if (Input.GetKey(KeyCode.U))
        {
            ArcAmount = Mathf.Clamp(ArcAmount + AimStep * Time.deltaTime, 0f, 2f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ArcAmount = Mathf.Clamp(ArcAmount - AimStep * Time.deltaTime, 0f, 2f);
        }
    }

    private void HandleSliding()
    {
        if (!isSliding || slidingBall == null) return;

        Vector3 direction = (BallSpawn.position - slidingBall.transform.position).normalized;
        slidingBall.transform.position += direction * SlideSpeed * Time.deltaTime;

        if (Vector3.Distance(slidingBall.transform.position, BallSpawn.position) <= SlideStopDistance)
        {
            slidingBall.transform.position = BallSpawn.position;
            currentBall = slidingBall;
            slidingBall = null;
            isSliding = false;

            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentBall == null && slidingBall == null && other.CompareTag("BiodiversityUnit"))
        {
            slidingBall = other.GetComponent<BiodiversityUnitManager>();
            if (slidingBall != null && slidingBall.gameObject.scene.IsValid())
            {
                Rigidbody rb = slidingBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                isSliding = true;
            }
        }
    }
}