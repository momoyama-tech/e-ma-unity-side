// ImageFlowerMovement.cs
using UnityEngine;

/// <summary>
/// “画像花” の移動（横力＋ひらひら縦揺れ）、平面回転、距離破棄、当たり判定を管理
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ImageFlowerMovement : MonoBehaviour
{
    [Header("横移動")]
    [Tooltip("基準となる移動力")]
    public float baseMoveForce = 1.0f;
    [Tooltip("横移動力の最小値")]
    public float minMoveForce = 0.5f;
    [Tooltip("横移動力の最大値")]
    public float maxMoveForce = 1.5f;
    [Tooltip("移動力更新間隔の最小秒数")]
    public float minChangeInterval = 0.5f;
    [Tooltip("移動力更新間隔の最大秒数")]
    public float maxChangeInterval = 2f;

    [Header("縦揺れ (Flutter)")]
    [Tooltip("上下揺れの振幅")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("揺れの周波数 (Hz)")]
    public float verticalFrequency = 1f;

    [Header("回転")]
    [Tooltip("回転速度の最小値 (度/秒)")]
    public float rotationSpeedMin = 30f;
    [Tooltip("回転速度の最大値 (度/秒)")]
    public float rotationSpeedMax = 90f;

    [Header("破棄判定 (距離)")]
    [Tooltip("生成地点からの最大距離 (超えたら破棄)")]
    public float destroyDistance = 20f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // Flutter 用
    private float currentMoveForce;
    private float nextChangeTime;
    private float phaseOffset;
    // 生成地点
    private Vector3 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("ImageFlowerMovement: SpriteRenderer が見つかりません！", this);

        // 当たり判定用 Collider2D のセットアップ
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = false;
        col.radius = Mathf.Max(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y);

        // Flutter 初期化
        spawnPos = transform.position;
        currentMoveForce = baseMoveForce;
        ScheduleNextForceChange();
        phaseOffset = Random.value * Mathf.PI * 2f;
    }

    void Start()
    {
        // 回転速度をランダムに決定
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        // 横方向への力
        rb.AddForce(Vector2.left * currentMoveForce, ForceMode2D.Force);
    }

    void Update()
    {
        // 回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // 縦 flutter
        float yOffset = Mathf.Sin((Time.time + phaseOffset) * verticalFrequency * Mathf.PI * 2f)
                        * verticalAmplitude * Time.deltaTime;
        transform.Translate(Vector3.up * yOffset, Space.World);

        // 横移動力をランダム更新
        if (Time.time >= nextChangeTime)
            ScheduleNextForceChange();

        // 距離破棄
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
        // 衝突で破棄したい場合は有効化
        // SelfDestroy();
    }
}
