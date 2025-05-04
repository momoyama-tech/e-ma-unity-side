// ImageFlowerMovement.cs
using UnityEngine;

/// <summary>
/// g‰æ‘œ‰Ôh ‚ÌˆÚ“®i‰¡—Í{‚Ğ‚ç‚Ğ‚çc—h‚êjA•½–Ê‰ñ“]A‹——£”jŠüA“–‚½‚è”»’è‚ğŠÇ—
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ImageFlowerMovement : MonoBehaviour
{
    [Header("‰¡ˆÚ“®")]
    [Tooltip("Šî€‚Æ‚È‚éˆÚ“®—Í")]
    public float baseMoveForce = 1.0f;
    [Tooltip("‰¡ˆÚ“®—Í‚ÌÅ¬’l")]
    public float minMoveForce = 0.5f;
    [Tooltip("‰¡ˆÚ“®—Í‚ÌÅ‘å’l")]
    public float maxMoveForce = 1.5f;
    [Tooltip("ˆÚ“®—ÍXVŠÔŠu‚ÌÅ¬•b”")]
    public float minChangeInterval = 0.5f;
    [Tooltip("ˆÚ“®—ÍXVŠÔŠu‚ÌÅ‘å•b”")]
    public float maxChangeInterval = 2f;

    [Header("c—h‚ê (Flutter)")]
    [Tooltip("ã‰º—h‚ê‚ÌU•")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("—h‚ê‚Ìü”g” (Hz)")]
    public float verticalFrequency = 1f;

    [Header("‰ñ“]")]
    [Tooltip("‰ñ“]‘¬“x‚ÌÅ¬’l (“x/•b)")]
    public float rotationSpeedMin = 30f;
    [Tooltip("‰ñ“]‘¬“x‚ÌÅ‘å’l (“x/•b)")]
    public float rotationSpeedMax = 90f;

    [Header("”jŠü”»’è (‹——£)")]
    [Tooltip("¶¬’n“_‚©‚ç‚ÌÅ‘å‹——£ (’´‚¦‚½‚ç”jŠü)")]
    public float destroyDistance = 20f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // Flutter —p
    private float currentMoveForce;
    private float nextChangeTime;
    private float phaseOffset;
    // ¶¬’n“_
    private Vector3 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("ImageFlowerMovement: SpriteRenderer ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñI", this);

        // “–‚½‚è”»’è—p Collider2D ‚ÌƒZƒbƒgƒAƒbƒv
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = false;
        col.radius = Mathf.Max(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y);

        // Flutter ‰Šú‰»
        spawnPos = transform.position;
        currentMoveForce = baseMoveForce;
        ScheduleNextForceChange();
        phaseOffset = Random.value * Mathf.PI * 2f;
    }

    void Start()
    {
        // ‰ñ“]‘¬“x‚ğƒ‰ƒ“ƒ_ƒ€‚ÉŒˆ’è
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        // ‰¡•ûŒü‚Ö‚Ì—Í
        rb.AddForce(Vector2.left * currentMoveForce, ForceMode2D.Force);
    }

    void Update()
    {
        // ‰ñ“]
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // c flutter
        float yOffset = Mathf.Sin((Time.time + phaseOffset) * verticalFrequency * Mathf.PI * 2f)
                        * verticalAmplitude * Time.deltaTime;
        transform.Translate(Vector3.up * yOffset, Space.World);

        // ‰¡ˆÚ“®—Í‚ğƒ‰ƒ“ƒ_ƒ€XV
        if (Time.time >= nextChangeTime)
            ScheduleNextForceChange();

        // ‹——£”jŠü
        if ((transform.position - spawnPos).sqrMagnitude
            > destroyDistance * destroyDistance)
        {
            SelfDestroy();
        }
    }

    private void ScheduleNextForceChange()
    {
        currentMoveForce = Random.Range(minMoveForce, maxMoveForce);
        nextChangeTime = Time.time + Random.Range(minChangeInterval, maxChangeInterval);
    }

    public void SetSpawner(FlowerSpawner owner)
        => spawner = owner;

    private void SelfDestroy()
    {
        if (spawner != null)
            spawner.ReleaseFlower(gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Õ“Ë‚Å”jŠü‚µ‚½‚¢ê‡‚Í—LŒø‰»
        // SelfDestroy();
    }
}
