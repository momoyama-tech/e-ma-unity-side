// FlowerMovement.cs
using UnityEngine;

/// <summary>
/// �Ԃ̕����ړ��i�������ւ̗́j�A���ʉ�]�A�����_���}�e���A�����蓖�āA�j�����Ǘ�
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(MeshRenderer))]
public class FlowerMovement : MonoBehaviour
{
    [Tooltip("�������ւ̈ړ��͂̋���")]
    public float moveForce = 1.0f;

    [Tooltip("�j������ƂȂ�X���W�i��ʍ��[��肳��ɍ��j")]
    public float destroyPositionX = -100.0f;

    [Tooltip("��]���x�̍ŏ��l�i�x/�b�j")]
    public float rotationSpeedMin = 30f;

    [Tooltip("��]���x�̍ő�l�i�x/�b�j")]
    public float rotationSpeedMax = 90f;

    [Header("�����_���}�e���A�� (10��)")]
    [Tooltip("�\���Ɏg���}�e���A����ݒ� (10���)")]
    public Material[] materials;

    private Rigidbody2D rb;
    private MeshRenderer mr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        mr = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (mr != null && materials != null && materials.Length > 0)
        {
            mr.material = materials[Random.Range(0, materials.Length)];
        }
        else
        {
            Debug.LogError("MeshRenderer �܂��� materials ���ݒ肳��Ă��܂���BFlower prefab ���m�F���Ă��������B", this);
        }

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax) * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector2.left * moveForce, ForceMode2D.Force);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        if (transform.position.x < destroyPositionX)
            SelfDestroy();
    }

    public void SetSpawner(FlowerSpawner owner)
    {
        spawner = owner;
    }

    private void SelfDestroy()
    {
        if (spawner != null)
            spawner.ReleaseFlower(this.gameObject);
    }
}