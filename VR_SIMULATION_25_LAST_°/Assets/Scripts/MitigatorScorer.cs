using UnityEngine;

public class MitigatorScorer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            GameManager.Instance.AddBarrierPoint();
        }
    }
}
