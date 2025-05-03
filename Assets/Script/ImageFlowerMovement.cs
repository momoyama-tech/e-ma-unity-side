using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ImageFlowerMovement : MonoBehaviour
{
    [Tooltip("左方向への移動力の強さ")]
    public float moveForce = 1.0f;

    [Tooltip("破棄判定距離（生成地点からの距離）")]
    public float destroyDistance = 20.0f;

    [Tooltip("回転速度の最小値（度/秒）")]
    public float rotationSpeedMin = 30f;

    [Tooltip("回転速度の最大値（度/秒）")]
    public float rotationSpeedMax = 90f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // 生成地点を記録
    private Vector3 _spawnPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("SpriteRenderer が見つかりません！", this);

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = false;
        col.radius = Mathf.Max(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y);

        // ここで生成地点を記録
        _spawnPosition = transform.position;
    }

    void Start()
    {
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector2.left * moveForce, ForceMode2D.Force);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // 生成地点からの距離で破棄判定
        float sqrDist = (transform.position - _spawnPosition).sqrMagnitude;
        if (sqrDist > destroyDistance * destroyDistance)
            SelfDestroy();
    }

    public void SetSpawner(FlowerSpawner owner)
    {
        spawner = owner;
    }

    private void SelfDestroy()
    {
        if (spawner != null)
            spawner.ReleaseFlower(gameObject);

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // 当たり判定時にも破棄したい場合はここを有効化
        // SelfDestroy();
    }
}
