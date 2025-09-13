using UnityEngine;

public class HangingLevelDetector : MonoBehaviour
{
    private int highestLevel = 0;

    private void OnTriggerEnter(Collider other)
    {
        int level = other.tag switch
        {
            "Level1" => 1,
            "Level2" => 2,
            "Level3" => 3,
            "Level4" => 4,
            _ => -1
        };

        if (level > highestLevel)
        {
            highestLevel = level;
            GameManager.Instance?.SetProtectionLevel((GameManager.ProtectionLevel)level);
        }
    }
}