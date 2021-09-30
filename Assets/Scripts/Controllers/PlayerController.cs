using UnityEngine;

public enum AbilityState
{
    IDLE,       // Stage 1: ability is not charging or active and is ready to be used
    CHARGING,   // Stage 2: ability is currently being charged by holding the ability key
    ACTIVE,     // Stage 3: ability has been charged and has been activated
    COOLDOWN    // Stage 4: ability has been spent and is inusable until it has finished the cooldown time
}

[System.Serializable]
public struct AbilitySettings
{
    public float chargeTime;
    public float activeTime;
    public float cooldownTime;
    public Vector2 minMaxSpeed;
    public Color color;

    public AbilitySettings(float chargeTime, float activeTime, float cooldownTime, Vector2 minMaxSpeed, Color color)
    {
        this.chargeTime = chargeTime;
        this.activeTime = activeTime;
        this.cooldownTime = cooldownTime;
        this.minMaxSpeed = minMaxSpeed;
        this.color = color;
    }
}

public abstract class Ability
{
    public AbilitySettings settings;
    public AbilityState state { get; private set; } = AbilityState.IDLE;
    public float chargeTimer { get; private set; } = 0f;
    public float activeTimer { get; private set; } = 0f;
    public float cooldownTimer { get; private set; } = 0f;
    public float ChargeCompletePercent { get => chargeTimer / settings.chargeTime; }
    public float ActiveCompletePercent { get => (settings.activeTime - activeTimer) / settings.activeTime; }
    public float CooldownCompletePercent { get => (settings.cooldownTime - cooldownTimer) / settings.cooldownTime; }
    private bool activationQueued = false;

    public Ability(AbilitySettings settings)
    {
        this.settings = settings;
    }

    /// <summary>
    /// Start charging ability.
    /// </summary>
    /// <param name="player"></param>
    public void Initiate(PlayerController player)
    {
        if (state == AbilityState.IDLE)
        {
            state = AbilityState.CHARGING;
            OnInitiate(player);
        }
    }

    /// <summary>
    /// Stop charging and activate abililty.
    /// </summary>
    /// <param name="player"></param>
    public void Activate(PlayerController player)
    {
        var isPlayerSpeedInRange = IsSpeedInRange(player.rigidbody.velocity.magnitude);
        if (state == AbilityState.CHARGING && isPlayerSpeedInRange)
        {
            state = AbilityState.ACTIVE;
            activeTimer = settings.activeTime;
            OnActivate(player, ChargeCompletePercent);
            chargeTimer = 0f;
        }
        else if (!isPlayerSpeedInRange)
        {
            activationQueued = true;
        }
    }

    /// <summary>
    /// Deactivate ability manually even if the active timer hasn't finished yet
    /// </summary>
    /// <param name="player"></param>
    public void Deactivate(PlayerController player)
    {
        if (state == AbilityState.ACTIVE)
        {
            state = AbilityState.COOLDOWN;
            cooldownTimer = settings.cooldownTime;
            OnDeactivate(player, ActiveCompletePercent);
            activeTimer = 0f;
        }
    }

    public bool IsActivatable(float speed)
    {
        return state == AbilityState.IDLE && IsSpeedInRange(speed);
    }

    public bool IsSpeedInRange(float speed)
    {
        return speed >= settings.minMaxSpeed.x && speed <= settings.minMaxSpeed.y;
    }

    /// <summary>
    /// Called once when ability starts charging.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnInitiate(PlayerController player) { }

    /// <summary>
    /// Called every frame while ability is charging.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnCharging(PlayerController player, float chargeCompletePercent) { }

    /// <summary>
    /// Called once when ability activates.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnActivate(PlayerController player, float chargePercent) { }

    /// <summary>
    /// Called every frame while ability is active.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnActive(PlayerController player, float activeCompletePercent) { }

    /// <summary>
    /// Called once when ability deactivates.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnDeactivate(PlayerController player, float activePercent) { }

    /// <summary>
    /// Called every frame while ability cools down.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnCooldown(PlayerController player, float cooldownCompletePercent) { }

    /// <summary>
    /// Called once when ability finishes cooldown and becomes idle.
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnReady(PlayerController player) { }

    /// <summary>
    /// Call this function from Update() or FixedUpdate() to keep timers updated
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="player"></param>
    public virtual void UpdateTimers(float deltaTime, PlayerController player)
    {
        switch (state)
        {
            case AbilityState.IDLE:
                break;
            case AbilityState.CHARGING:
                if (activationQueued && IsSpeedInRange(player.rigidbody.velocity.magnitude))
                {
                    Activate(player);
                    activationQueued = false;
                }
                if (chargeTimer < settings.chargeTime)
                {
                    chargeTimer = Mathf.Min(chargeTimer + deltaTime, settings.chargeTime);
                    OnCharging(player, ChargeCompletePercent);
                }
                else
                {
                    OnCharging(player, 1f);
                }
                break;
            case AbilityState.ACTIVE:
                if (activeTimer > 0f)
                {
                    activeTimer = Mathf.Max(activeTimer - deltaTime, 0f);
                    OnActive(player, ActiveCompletePercent);
                }
                else
                {
                    Deactivate(player);
                }
                break;
            case AbilityState.COOLDOWN:
                if (cooldownTimer > 0f)
                {
                    cooldownTimer = Mathf.Max(chargeTimer - deltaTime, 0f);
                    OnCooldown(player, CooldownCompletePercent);
                }
                else
                {
                    state = AbilityState.IDLE;
                    OnReady(player);
                }
                break;
        }
    }
}

public class AbilityAttack : Ability
{
    public Vector2 powerMinMax;

    public AbilityAttack(AbilitySettings settings) : base(settings) { }

    public override void OnInitiate(PlayerController player)
    {
        player.chargeUpEffect.Play();
    }

    public override void OnCharging(PlayerController player, float chargeCompletePercent)
    {
        var emissionModule = player.chargeUpEffect.emission;
        emissionModule.rateOverTime = 50f * ChargeCompletePercent;
    }

    public override void OnActivate(PlayerController player, float chargeCompletePercent)
    {
        var power = Mathf.Lerp(powerMinMax.x, powerMinMax.y, chargeCompletePercent);
        player.rigidbody.AddForce(player.rigidbody.velocity.normalized * power, ForceMode2D.Impulse);
        player.chargeUpEffect.Stop();
        player.trailEffect.Play();
        player.ghostEffect.Play();
    }

    public override void OnDeactivate(PlayerController player, float activeCompletePercent)
    {
        player.trailEffect.Stop();
    }
}

public class AbilityDefend : Ability
{
    public AbilityDefend(AbilitySettings settings) : base(settings) { }

    public override void OnActivate(PlayerController player, float chargeCompletePercent)
    {
        //TODO: ADD FUNCTIONALITY FOR DEFEND ABILITY
    }
}

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    public GameManager gameManager;
    public new Rigidbody2D rigidbody;
    public Transform healthBar;
    public SpriteRenderer healthRing;
    public SpriteRenderer powerRing;
    public CameraController cameraController;
    public ParticleSystem chargeUpEffect;
    public ParticleSystem trailEffect;
    public ParticleSystem ghostEffect;
    public ParticleSystem[] deathEffects;

    [Header("Main Settings")]
    public int playerNum;
    public int health = 100;
    public float movePower;

    [Header("Ability Settings")]
    public AbilitySettings abilityAttackSettings;
    public Vector2 abilityAttackPowerMinMax;
    public AbilitySettings abilityDefendSettings;

    private AbilityAttack abilityAttack;
    private AbilityDefend abilityDefend;
    private Ability[] abilities;
    private Ability activeAbility;

    private float healthSmoothed = 100f;
    // Used when adding/subtracting fractional values from health to store the non-integer part of the delta for later
    private float healthFractionalBuffer = 0f;
    private bool abilityKeyDown = false;

    [HideInInspector]
    public Vector2 velocityThisFrame = new Vector2();
    [HideInInspector]
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
            if (deltaInt < 0)
            {
                cameraController.Shake(5f * -deltaInt / 100f);
            }
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

    public void OnDie()
    {
        foreach (var effect in deathEffects)
        {
            effect.transform.position = gameObject.transform.position;
            effect.Play();
        }
        cameraController.Shake(5f);
        gameManager.EndGame();
        Destroy(gameObject);
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        abilityAttack = new AbilityAttack(abilityAttackSettings);
        abilityAttack.powerMinMax = abilityAttackPowerMinMax;
        abilityDefend = new AbilityDefend(abilityDefendSettings);
        abilities = new Ability[] { abilityAttack };
    }

    void Update()
    {
        UpdateUI();
        if (Input.GetAxis($"P{playerNum} Ability") > 0f && !abilityKeyDown)
        {
            for (var i = 0; i < abilities.Length && activeAbility == null; i++)
            {
                var ability = abilities[i];
                if (ability.IsActivatable(rigidbody.velocity.magnitude))
                {
                    ability.Initiate(this);
                    activeAbility = ability;
                }
            }
            abilityKeyDown = true;
        }
        else if (Input.GetAxis($"P{playerNum} Ability") == 0f && abilityKeyDown)
        {
            if (activeAbility != null)
            {
                activeAbility.Activate(this);
            }
            abilityKeyDown = false;
        }
        /*if (Input.GetAxis($"P{playerNum} Ability") > 0f && speed > attackMinMaxSpeed.x && speed < attackMinMaxSpeed.y && attackCooldownTimer == 0f)
        {
            rigidbody.AddForce(rigidbody.velocity.normalized * attackPower, ForceMode2D.Impulse);
            attackCooldownTimer = attackCooldown;
            // Play charge trail effect if it's not already playing
            if (trailEffect.isStopped)
            {
                trailEffect.Play();
            }
        }*/
    }

    void FixedUpdate()
    {
        UpdateCooldowns();

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
        cameraController.Shake(Mathf.Pow(Mathf.Max(impact.magnitude - 20f, 0f) * .05f, 2f) * .2f * shakePercent);

        // Calculate damage based on impact and speed
        Damage(Mathf.Pow(Mathf.Max(impact.magnitude - 25f, 0f), 1.5f) / Mathf.Max(velocityLastFrame.magnitude, 2f));

        // Stop charge ability if it's active
        if (activeAbility is AbilityAttack)
        {
            if (activeAbility.state == AbilityState.ACTIVE)
            {
                activeAbility.Deactivate(this);
            }
        }
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
        var targetColor = new Color(0f, 0f, 0f, 0f);
        foreach (var ability in abilities)
        {
            if (ability.IsActivatable(rigidbody.velocity.magnitude) && activeAbility == null)
            {
                targetColor = ability.settings.color;
            }
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
        foreach (var ability in abilities)
        {
            ability.UpdateTimers(Time.fixedDeltaTime, this);
        }
        if (activeAbility != null)
        {
            if (activeAbility.state == AbilityState.COOLDOWN)
            {
                activeAbility = null;
            }
        }
    }
}
