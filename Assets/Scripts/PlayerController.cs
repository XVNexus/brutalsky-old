using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    public GlobalData globalData;
    private new Rigidbody2D rigidbody;
    public Transform healthBar;
    public CameraController cameraController;

    [Header("Main Settings")]
    public int playerNum;

    [Header("Move Power")]
    public float movePower;

    [Header("Attack Settings")]
    public float attackPower;
    public Vector2 attackMinMaxSpeed;
    public float attackCooldown;
    private float _attackCooldown = 0f;

    private int _health = 100;
    private float _healthSmooth = 100f;

    private bool _abilityKeyInUse = false;
    private Vector2 velocityLastFrame = new Vector2();

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateCooldowns();
        UpdateUI();
        var speed = rigidbody.velocity.magnitude;
        if (Input.GetAxis($"P{playerNum} Ability") > 0f && !_abilityKeyInUse && speed > attackMinMaxSpeed.x && speed < attackMinMaxSpeed.y && _attackCooldown == 0f)
        {
            rigidbody.AddForce(rigidbody.velocity.normalized * attackPower, ForceMode2D.Impulse);
            _attackCooldown = attackCooldown;
            _abilityKeyInUse = true;
        }
        if (Input.GetAxis($"P{playerNum} Ability") == 0f)
        {
            _abilityKeyInUse = false;
        }
    }

    void FixedUpdate()
    {
        var moveVector = new Vector2(Input.GetAxis($"P{playerNum} Horizontal"), Input.GetAxis($"P{playerNum} Vertical")).normalized;
        rigidbody.AddForce(moveVector * movePower);
        globalData.playerVelocities[playerNum - 1] = velocityLastFrame;
        velocityLastFrame = rigidbody.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Calculate impact force for use in camera shake effects and damage calculation
        var impact = collision.relativeVelocity * rigidbody.mass;

        // Use the collision velocities to determine how much to shove the camera and how much to shake the camera
        // If a player collides with a wall, the camera will be shoved, but if two players collide into each other going the same speed, the camera will be shook
        var thisVelocity = globalData.playerVelocities[playerNum - 1];
        var otherVelocity = new Vector2();
        if (collision.collider.CompareTag("Player"))
        {
            otherVelocity = globalData.playerVelocities[2 - playerNum];
        }
        var combinedVelocity = thisVelocity + otherVelocity;
        var highestSpeed = Mathf.Max(thisVelocity.magnitude, otherVelocity.magnitude);
        var combinedSpeed = combinedVelocity.magnitude;
        var shovePercent = Mathf.Clamp01(combinedSpeed / highestSpeed); // 1.0 = all shove, 0.0 = all shake, 0.5 = half shove half shake, etc.
        var shakePercent = 1f - shovePercent;
        cameraController.Shove(collision.contacts[0].normal * Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f) * shovePercent);
        cameraController.Shake(Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f) * shakePercent);

        var damage = 0;
        _health -= damage;
    }

    void UpdateUI()
    {
        if (Mathf.Abs(_health - _healthSmooth) < .1f)
        {
            _healthSmooth = _health;
        }
        else
        {
            _healthSmooth += (_health - _healthSmooth) * 5f * Time.deltaTime;
        }
        var localScale = healthBar.localScale;
        var localPosition = healthBar.localPosition;
        localScale.x = _healthSmooth / 100f;
        localPosition.x = -.5f + localScale.x / 2f;
        healthBar.localScale = localScale;
        healthBar.localPosition = localPosition;
    }

    void UpdateCooldowns()
    {
        if (_attackCooldown > 0f)
        {
            _attackCooldown = Mathf.Max(_attackCooldown - Time.deltaTime, 0f);
        }
    }
}
