using UnityEngine;

public class BarrierTracker : MonoBehaviour
{
    public int barriersInside = 0;


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            barriersInside--;
            Debug.Log("Barrier removed from ecosystem!");
        }
    }
}
