// FlowerMovement.cs
using UnityEngine;

/// <summary>
/// �Ԃ̕����ړ��i�������ւ̗́j�A���ʉ�]�A�����_���}�e���A�����蓖�āA�j�����Ǘ�
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(MeshRenderer))]
public class FlowerMovement : MonoBehaviour
{
    [Tooltip("�������ւ̈ړ��͂̋����i���ϒl�j")]
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

    [Header("�c�h�� (Flutter)")]
    [Tooltip("�㉺�ɗh���U��")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("�h����g�� (Hz)")]
    public float verticalFrequency = 1f;

    [Header("���x�u��")]
    [Tooltip("���ړ��͂̍ŏ��l")]
    public float minMoveForce = 0.5f;
    [Tooltip("���ړ��͂̍ő�l")]
    public float maxMoveForce = 1.5f;
    [Tooltip("�ړ��͍X�V�Ԋu�̍ŏ��b��")]
    public float minChangeInterval = 0.5f;
    [Tooltip("�ړ��͍X�V�Ԋu�̍ő�b��")]
    public float maxChangeInterval = 2f;

    private Rigidbody2D rb;
    private MeshRenderer mr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // Flutter �p�v���C�x�[�g�ϐ�
    private float currentMoveForce;
    private float nextChangeTime;
    private float phaseOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        mr = GetComponent<MeshRenderer>();

        // flutter ������
        currentMoveForce = moveForce;
        ScheduleNextForceChange();
        phaseOffset = Random.value * Mathf.PI * 2f;
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

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        // ���݂̃����_���ړ��͂��������ɓK�p
        rb.AddForce(Vector2.left * currentMoveForce, ForceMode2D.Force);
    }

    void Update()
    {
        // �c�h�� (sin �g�łЂ�Ђ�)
        float yOffset = Mathf.Sin((Time.time + phaseOffset) * verticalFrequency * Mathf.PI * 2f)
                        * verticalAmplitude * Time.deltaTime;
        transform.Translate(Vector3.up * yOffset, Space.World);

        // ��]
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // ���̈ړ��͍X�V�^�C�~���O�ɂȂ����烉���_���X�V
        if (Time.time >= nextChangeTime)
            ScheduleNextForceChange();

        // �j������
        if (transform.position.x < destroyPositionX)
            SelfDestroy();
    }

    // ���ړ��͂������_���ɍX�V���A���̃^�C�~���O������
    private void ScheduleNextForceChange()
    {
        currentMoveForce = Random.Range(minMoveForce, maxMoveForce);
        nextChangeTime = Time.time + Random.Range(minChangeInterval, maxChangeInterval);
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
