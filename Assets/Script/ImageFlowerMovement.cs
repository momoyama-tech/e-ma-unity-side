using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ImageFlowerMovement : MonoBehaviour
{
    [Tooltip("�������ւ̈ړ��͂̋���")]
    public float moveForce = 1.0f;

    [Tooltip("�j�����苗���i�����n�_����̋����j")]
    public float destroyDistance = 20.0f;

    [Tooltip("��]���x�̍ŏ��l�i�x/�b�j")]
    public float rotationSpeedMin = 30f;

    [Tooltip("��]���x�̍ő�l�i�x/�b�j")]
    public float rotationSpeedMax = 90f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // �����n�_���L�^
    private Vector3 _spawnPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("SpriteRenderer ��������܂���I", this);

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = false;
        col.radius = Mathf.Max(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y);

        // �����Ő����n�_���L�^
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

        // �����n�_����̋����Ŕj������
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
        // �����蔻�莞�ɂ��j���������ꍇ�͂�����L����
        // SelfDestroy();
    }
}
