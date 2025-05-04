// ImageFlowerMovement.cs
using UnityEngine;

/// <summary>
/// �g�摜�ԁh �̈ړ��i���́{�Ђ�Ђ�c�h��j�A���ʉ�]�A�����j���A�����蔻����Ǘ�
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ImageFlowerMovement : MonoBehaviour
{
    [Header("���ړ�")]
    [Tooltip("��ƂȂ�ړ���")]
    public float baseMoveForce = 1.0f;
    [Tooltip("���ړ��͂̍ŏ��l")]
    public float minMoveForce = 0.5f;
    [Tooltip("���ړ��͂̍ő�l")]
    public float maxMoveForce = 1.5f;
    [Tooltip("�ړ��͍X�V�Ԋu�̍ŏ��b��")]
    public float minChangeInterval = 0.5f;
    [Tooltip("�ړ��͍X�V�Ԋu�̍ő�b��")]
    public float maxChangeInterval = 2f;

    [Header("�c�h�� (Flutter)")]
    [Tooltip("�㉺�h��̐U��")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("�h��̎��g�� (Hz)")]
    public float verticalFrequency = 1f;

    [Header("��]")]
    [Tooltip("��]���x�̍ŏ��l (�x/�b)")]
    public float rotationSpeedMin = 30f;
    [Tooltip("��]���x�̍ő�l (�x/�b)")]
    public float rotationSpeedMax = 90f;

    [Header("�j������ (����)")]
    [Tooltip("�����n�_����̍ő勗�� (��������j��)")]
    public float destroyDistance = 20f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // Flutter �p
    private float currentMoveForce;
    private float nextChangeTime;
    private float phaseOffset;
    // �����n�_
    private Vector3 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("ImageFlowerMovement: SpriteRenderer ��������܂���I", this);

        // �����蔻��p Collider2D �̃Z�b�g�A�b�v
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = false;
        col.radius = Mathf.Max(sr.sprite.bounds.extents.x, sr.sprite.bounds.extents.y);

        // Flutter ������
        spawnPos = transform.position;
        currentMoveForce = baseMoveForce;
        ScheduleNextForceChange();
        phaseOffset = Random.value * Mathf.PI * 2f;
    }

    void Start()
    {
        // ��]���x�������_���Ɍ���
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        // �������ւ̗�
        rb.AddForce(Vector2.left * currentMoveForce, ForceMode2D.Force);
    }

    void Update()
    {
        // ��]
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // �c flutter
        float yOffset = Mathf.Sin((Time.time + phaseOffset) * verticalFrequency * Mathf.PI * 2f)
                        * verticalAmplitude * Time.deltaTime;
        transform.Translate(Vector3.up * yOffset, Space.World);

        // ���ړ��͂������_���X�V
        if (Time.time >= nextChangeTime)
            ScheduleNextForceChange();

        // �����j��
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
        // �Փ˂Ŕj���������ꍇ�͗L����
        // SelfDestroy();
    }
}
