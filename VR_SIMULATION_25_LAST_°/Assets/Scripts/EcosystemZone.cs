using UnityEngine;

public class EcosystemZone : MonoBehaviour
{
    public enum EcosystemType { Freshwater, Marine, Terrestrial }
    public EcosystemType zoneType;

    private BarrierTracker barrierTracker;

    private void Start()
    {
        barrierTracker = GetComponent<BarrierTracker>();
        if (barrierTracker == null)
        {
            Debug.LogWarning("BarrierTracker not found on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BiodiversityUnit"))
        {
            if (barrierTracker != null && barrierTracker.barriersInside > 0)
            {
                Debug.Log($"[BIODIVERSITY] Cannot score in {zoneType}: Barriers still inside ({barrierTracker.barriersInside})");
                return;
            }

            GameManager.Instance.AddBiodiversity(zoneType);
            Destroy(other.gameObject); // Optional: remove after scoring
        }
    }
}
