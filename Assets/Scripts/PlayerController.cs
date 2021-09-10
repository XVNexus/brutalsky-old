using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    private new Rigidbody2D rigidbody;
    public Transform healthBar;
    public CameraController cameraController;

    [Header("Keybinds")]
    public KeyCode buttonAttack;

    [Header("Move Power")]
    public float movePower;

    [Header("Attack Settings")]
    public float attackPower;
    public float attackMinSpeed;
    public float attackMaxSpeed;
    public float attackCooldown;
    private float _attackCooldown = 0f;

    private int _health = 100;
    private float _healthSmooth = 100f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateCooldowns();
        UpdateUI();
        var speed = rigidbody.velocity.magnitude;
        if (Input.GetKeyDown(buttonAttack) && speed > attackMinSpeed && speed < attackMaxSpeed && _attackCooldown == 0f)
        {
            rigidbody.AddForce(rigidbody.velocity.normalized * attackPower, ForceMode2D.Impulse);
            _attackCooldown = attackCooldown;
        }
    }


    void FixedUpdate()
    {
        var moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rigidbody.AddForce(moveVector * movePower);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var impact = collision.relativeVelocity * rigidbody.mass;
        cameraController.Shove(impact.normalized * Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f));
        var damage = Mathf.RoundToInt(impact.magnitude);
        _health -= damage;
        Debug.Log(_health);
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
