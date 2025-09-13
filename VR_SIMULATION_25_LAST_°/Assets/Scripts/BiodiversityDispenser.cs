using UnityEngine;

public class BiodiversityDispenser : MonoBehaviour
{
    public enum SpawnState { Low, Medium, High }
    private SpawnState currentState = SpawnState.Low;

    [Header("Biodiversity Settings")]
    public GameObject biodiversityUnitPrefab;
    public float launchForce = 1f;
    public int maxUnitsToSpawn = 60;

    private float timer = 0f;
    private int unitsSpawned = 0;
    private float downgradeTimer = 0f;

    private float GetSpawnInterval()
    {
        return currentState switch
        {
            SpawnState.High => 1f,
            SpawnState.Medium => 3f,
            _ => 5f
        };
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive()) return;

        if (unitsSpawned >= maxUnitsToSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= GetSpawnInterval())
        {
            SpawnUnit();
            timer = 0f;
        }

        downgradeTimer += Time.deltaTime;

        if (currentState == SpawnState.High && downgradeTimer >= 15f)
        {
            Debug.Log("[DISPENSER] Downgrading from HIGH to MEDIUM");
            SetDispenserState(SpawnState.Medium);
        }
        else if (currentState == SpawnState.Medium && downgradeTimer >= 30f)
        {
            Debug.Log("[DISPENSER] Downgrading from MEDIUM to LOW");
            SetDispenserState(SpawnState.Low);
        }
    }


    public void SetDispenserState(SpawnState newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
            Debug.Log($"[DISPENSER] State changed to: {currentState}");
        }
    }

    public void ResetDowngradeTimer()
    {
        downgradeTimer = 0f;
        Debug.Log("[DISPENSER] Downgrade timer reset due to accelerator rotation");
    }

    void SpawnUnit()
    {
        if (unitsSpawned >= maxUnitsToSpawn)
            return;

        GameObject unit = Instantiate(biodiversityUnitPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = unit.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        }

        unitsSpawned++;
        Debug.Log($"[DISPENSER] Spawned unit #{unitsSpawned} at state {currentState}");

        if (unitsSpawned == maxUnitsToSpawn)
        {
            Debug.Log("[DISPENSER] Reached max unit limit (60). Spawning complete.");
        }
    }
}
