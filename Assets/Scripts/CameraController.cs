using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Shove settings")]
    public float shoveDamping;
    public float shoveSpeed;
    public float shoveForce;

    [Header("Rock settings")]
    public float rockFade;

    [Header("Shake settings")]
    public float shakeFade;

    private Vector2 position; // Center position of camera
    private Vector2 offset; // Offset from center position

    private Vector2 shoveOffset = new Vector2();
    private Vector2 shoveVelocity = new Vector2();

    private float rockPower = 0f;

    private Vector2 shakeOffset = new Vector2();
    private float shakePower = 0f;

    public void Shove(Vector2 force) // Push the camera in a certain direction to create a "shove" effect
    {
        shoveVelocity += force;
    }

    public void Rock(float force) // Rock the camera by randomly pushing it around
    {
        rockPower += force;
    }

    public void Shake(float force) // Shake the camera by randomly offsetting the position each frame
    {
        shakePower += force;
    }

    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
    {
        UpdateShove();
        UpdateRock();
        UpdateShake();
        UpdateOffset();
    }

    void UpdateShove()
    {
        var targetVelocity = -shoveOffset;
        var targetAcceleration = targetVelocity - shoveVelocity;
        var acceleration = new Vector2(
                Mathf.Lerp(targetVelocity.x, targetAcceleration.x, shoveDamping) * shoveForce,
                Mathf.Lerp(targetVelocity.y, targetAcceleration.y, shoveDamping) * shoveForce
            );
        shoveVelocity += acceleration * Time.fixedDeltaTime;
        shoveOffset += shoveVelocity * shoveSpeed * Time.fixedDeltaTime;
    }

    void UpdateRock()
    {
        rockPower -= rockPower * rockFade * Time.fixedDeltaTime;
    }

    void UpdateShake()
    {
        shakeOffset = Random.insideUnitCircle * shakePower;
        shakePower -= shakePower * shakeFade * Time.fixedDeltaTime;
    }

    void UpdateOffset()
    {
        offset = shoveOffset + shakeOffset;
        // Add offset to center position
        var newPosition = transform.position;
        newPosition.x = position.x + offset.x;
        newPosition.y = position.y + offset.y;
        // Apply position + offset to camera transform
        transform.position = newPosition;
    }
}
