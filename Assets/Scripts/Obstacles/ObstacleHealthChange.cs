using UnityEngine;

public class ObstacleHealthChange : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public float healthChange;
    // Only change the health value within this range
    public Vector2 healthMinMax;
    public bool instantChange;
    public float triggerBoyancyDirection;
    public float triggerBoyancyForce;

    void OnCollisionEnter2D(Collision2D collision)
    {
        CollideEnter(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        CollideEnter(collider);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CollideStay(collision.collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        CollideStay(collider);
        var forceAngle = triggerBoyancyDirection * Mathf.Deg2Rad;
        collider.attachedRigidbody.AddForce(new Vector2(Mathf.Cos(forceAngle), Mathf.Sin(forceAngle)) * triggerBoyancyForce);
    }

    private void CollideEnter(Collider2D collider)
    {
        if (instantChange)
        {
            Collide(collider, healthChange);
        }
    }

    private void CollideStay(Collider2D collider)
    {
        if (!instantChange)
        {
            Collide(collider, healthChange * Time.fixedDeltaTime);
        }
    }

    private void Collide(Collider2D collider, float healthChangeForCurrentFrame)
    {
        var otherGameObject = collider.gameObject;
        if (otherGameObject.CompareTag("Player"))
        {
            otherGameObject.GetComponent<PlayerController>().ChangeHealth(healthChangeForCurrentFrame, healthMinMax.x, healthMinMax.y);
        }
    }
}
