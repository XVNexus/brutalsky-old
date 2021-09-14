using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Shove settings")]
    public float shoveDamping;
    public float shoveSpeed;
    public float shoveForce;

    [Header("Shake settings")]
    public float shakeFade;
    public float shakeInterval;

    [Header("Jagged shake settings")]
    public float jaggedShakeFade;

    private Vector2 position;

    private Vector2 shoveOffset = new Vector2();
    private Vector2 shoveVelocity = new Vector2();

    private float shakePower = 0f;

    private Vector2 jaggedShakeOffset = new Vector2();
    private float jaggedShakePower = 0f;

    // Push the camera in a certain direction to create a "shove" effect
    public void Shove(Vector2 force)
    {
        shoveVelocity += force;
    }

    // Shake the camera by randomly shoving it
    public void Shake(float force)
    {
        shakePower += force;
        // This is used to dynamically cause shake effects
        // If shakePower is currently 0, CancelInvoke is called to stop all attempts at updating camera shake when there is no need
        if (!IsInvoking("ApplyShake"))
        {
            InvokeRepeating("ApplyShake", 0f, shakeInterval);
        }
    }

    // Shake the camera by randomly offsetting the position each frame
    public void JaggedShake(float force)
    {
        jaggedShakePower += force;
    }

    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
    {
        UpdateShove();
        UpdateShake();
        UpdateJaggedShake();
        UpdateOffset();
    }

    private void UpdateShove()
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

    private void UpdateShake()
    {
        // If shakePower is above a certain threshold, keep reducing it
        if (shakePower >= .01f)
        {
            shakePower -= shakePower * shakeFade * Time.fixedDeltaTime;
        }
        // Once shakepower falls below a certain threshold, snap it to 0 as it would be too small to notice at that scale anyway
        else if (shakePower > 0f && shakePower < .01f)
        {
            shakePower = 0f;
            CancelInvoke("ApplyShake");
        }
    }

    // This method is used to randomly shove the camera to simulate shaking and is called every 0.1 seconds while shakePower > 0
    private void ApplyShake()
    {
        if (shakePower > 0f)
        {
            Shove(Random.insideUnitCircle.normalized * shakePower);
        }
    }

    private void UpdateJaggedShake()
    {
        jaggedShakeOffset = Random.insideUnitCircle * jaggedShakePower;
        if (jaggedShakePower >= .01f)
        {
            jaggedShakePower -= jaggedShakePower * jaggedShakeFade * Time.fixedDeltaTime;
        }
        else if (jaggedShakePower > 0f && shakePower < .01f)
        {
            jaggedShakePower = 0f;
        }
    }

    private void UpdateOffset()
    {
        var offset = shoveOffset + jaggedShakeOffset;
        Debug.Log($"{shoveOffset}");
        // Add offset to center position
        var newPosition = transform.position;
        newPosition.x = position.x + offset.x;
        newPosition.y = position.y + offset.y;
        // Apply position + offset to camera transform
        transform.position = newPosition;
    }
}
