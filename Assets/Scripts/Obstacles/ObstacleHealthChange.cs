using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHealthChange : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public float healthChange;
    // Only change the health value within this range
    public Vector2 healthMinMax;
    public bool instantChange;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (instantChange)
        {
            Collide(collision, healthChange);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!instantChange)
        {
            Collide(collision, healthChange * Time.fixedDeltaTime);
        }
    }

    private void Collide(Collision2D collision, float healthChangeForCurrentFrame)
    {
        var otherGameObject = collision.collider.gameObject;
        if (otherGameObject.CompareTag("Player"))
        {
            otherGameObject.GetComponent<PlayerController>().ChangeHealth(healthChangeForCurrentFrame, healthMinMax.x, healthMinMax.y);
        }
    }
}
