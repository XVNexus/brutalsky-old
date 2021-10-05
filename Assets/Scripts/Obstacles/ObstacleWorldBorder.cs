using UnityEngine;

public class ObstacleWorldBorder : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) collider.gameObject.GetComponent<PlayerController>().Kill();
    }
}
