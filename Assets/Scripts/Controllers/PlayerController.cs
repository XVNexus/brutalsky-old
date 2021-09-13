using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    public GameManager gameManager;
    private new Rigidbody2D rigidbody;
    public Transform healthBar;
    public SpriteRenderer healthRing;
    public SpriteRenderer powerRing;
    public CameraController cameraController;
    public ParticleSystem[] deathEffects;

    [Header("Main Settings")]
    public int playerNum;
    public int health = 100;

    [Header("Move Power")]
    public float movePower;

    [Header("Attack Settings")]
    public float attackPower;
    public Vector2 attackMinMaxSpeed;
    public float attackCooldown;
    private float attackCooldownTimer = 0f;

    private float healthSmoothed = 100f;
    // Used when adding/subtracting fractional values from health to store the non-integer part of the delta for later
    private float healthFractionalBuffer = 0f;

    private bool abilityKeyInUse = false;
    public Vector2 velocityThisFrame = new Vector2();
    public Vector2 velocityLastFrame = new Vector2();

    public void Heal(float amount)
    {
        ChangeHealth(amount);
    }

    public void Damage(float amount)
    {
        ChangeHealth(-amount);
    }

    public void ChangeHealth(float delta, float healthMin = 0f, float healthMax = 100f)
    {
        if (health >= healthMin && health <= healthMax)
        {
            // Grab any value stored in the fractional health delta buffer and make sure that health doesn't get outside of the range 0 - 100
            var deltaClamped = Mathf.Clamp(delta + healthFractionalBuffer, healthMin - health, healthMax - health);
            healthFractionalBuffer = 0f;
            // Get the integer component of the health delta that can be immediately applied to health value
            var deltaInt = (int)deltaClamped;
            health += deltaInt;
            // If there is a fractional part of the health value that can't be applied to the integer health scale immediately,
            // save it to a buffer and apply it later when enough accumulates
            healthFractionalBuffer += deltaClamped - deltaInt;
            if (deltaInt > 0)
            {
                healthRing.color = new Color(.2f, 1f, .2f);
            }
            else if (deltaInt < 0)
            {
                healthRing.color = new Color(1f, .2f, .2f);
            }
        }
        // If health runs out, trigger player death
        if (health == 0f)
        {
            OnDie();
        }
    }

    private void OnDie()
    {
        foreach (var effect in deathEffects)
        {
            effect.transform.position = gameObject.transform.position;
            effect.Play();
        }
        Destroy(gameObject);
        cameraController.Shake(5f);
        gameManager.ReloadGame(3f);
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateCooldowns();
        UpdateUI();
        var speed = rigidbody.velocity.magnitude;
        if (Input.GetAxis($"P{playerNum} Ability") > 0f && !abilityKeyInUse && speed > attackMinMaxSpeed.x && speed < attackMinMaxSpeed.y && attackCooldownTimer == 0f)
        {
            rigidbody.AddForce(rigidbody.velocity.normalized * attackPower, ForceMode2D.Impulse);
            attackCooldownTimer = attackCooldown;
            abilityKeyInUse = true;
        }
        if (Input.GetAxis($"P{playerNum} Ability") == 0f)
        {
            abilityKeyInUse = false;
        }
    }

    void FixedUpdate()
    {
        // Update movement
        var moveVector = new Vector2(Input.GetAxis($"P{playerNum} Horizontal"), Input.GetAxis($"P{playerNum} Vertical")).normalized;
        rigidbody.AddForce(moveVector * movePower);

        // Update stored velocity
        velocityLastFrame = velocityThisFrame;
        velocityThisFrame = rigidbody.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Calculate impact force for use in camera shake effects and damage calculation
        var impact = collision.relativeVelocity * rigidbody.mass;

        // Use the collision velocities to determine how much to shove the camera and how much to shake the camera
        // If a player collides with a wall, the camera will be shoved, but if two players collide into each other going the same speed, the camera will be shook
        var thisVelocity = velocityLastFrame;
        var otherVelocity = new Vector2();
        var otherGameObject = collision.collider.gameObject;
        if (otherGameObject.CompareTag("Player"))
        {
            otherVelocity = otherGameObject.GetComponent<PlayerController>().velocityLastFrame;
        }
        var combinedVelocity = thisVelocity + otherVelocity;
        var highestSpeed = Mathf.Max(thisVelocity.magnitude, otherVelocity.magnitude);
        var combinedSpeed = combinedVelocity.magnitude;
        var shovePercent = Mathf.Clamp01(combinedSpeed / highestSpeed); // 1.0 = all shove, 0.0 = all shake, 0.5 = half shove half shake, etc.
        var shakePercent = 1f - shovePercent;
        cameraController.Shove(collision.contacts[0].normal * Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f) * shovePercent);
        cameraController.Shake(Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f) * .5f * shakePercent);

        // Calculate damage based on impact and speed
        Damage(Mathf.Pow(Mathf.Max(impact.magnitude - 25f, 0f), 1.5f) / Mathf.Max(velocityLastFrame.magnitude, 2f));
    }

    private void UpdateUI()
    {
        // Update health bar
        if (Mathf.Abs(health - healthSmoothed) < .1f)
        {
            healthSmoothed = health;
        }
        else
        {
            healthSmoothed += (health - healthSmoothed) * 5f * Time.deltaTime;
        }
        var localScale = healthBar.localScale;
        var localPosition = healthBar.localPosition;
        localScale.x = healthSmoothed / 100f;
        localPosition.x = -.5f + localScale.x / 2f;

        // Update health ring
        healthBar.localScale = localScale;
        healthBar.localPosition = localPosition;
        var ringColor = healthRing.color;
        ringColor.a -= Time.deltaTime;
        healthRing.color = ringColor;

        // Update power ring
        var speed = rigidbody.velocity.magnitude;
        var attackPossible = speed > attackMinMaxSpeed.x && speed < attackMinMaxSpeed.y && attackCooldownTimer == 0f;
        // This will be used soon but right now it's only a placeholder
        var defendPossible = false;
        var targetColor = new Color();
        if (attackPossible)
        {
            targetColor = new Color(.2f, 1f, 1f, .25f);
        }
        else if (defendPossible)
        {
            // Add color 
            targetColor = new Color(1f, 1f, .2f, .25f);
        }
        else
        {
            targetColor = new Color(0f, 0f, 0f, 0f);
        }
        var currentColor = powerRing.color;
        currentColor.r += (targetColor.r - currentColor.r) * 10f * Time.deltaTime;
        currentColor.g += (targetColor.g - currentColor.g) * 10f * Time.deltaTime;
        currentColor.b += (targetColor.b - currentColor.b) * 10f * Time.deltaTime;
        currentColor.a += (targetColor.a - currentColor.a) * 10f * Time.deltaTime;
        powerRing.color = currentColor;
    }

    private void UpdateCooldowns()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer = Mathf.Max(attackCooldownTimer - Time.deltaTime, 0f);
        }
    }
}
