// FlowerMovement.cs
using UnityEngine;

/// <summary>
/// 花の物理移動（左方向への力）、平面回転、ランダムマテリアル割り当て、破棄を管理
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(MeshRenderer))]
public class FlowerMovement : MonoBehaviour
{
    [Tooltip("左方向への移動力の強さ（平均値）")]
    public float moveForce = 1.0f;

    [Tooltip("破棄判定となるX座標（画面左端よりさらに左）")]
    public float destroyPositionX = -100.0f;

    [Tooltip("回転速度の最小値（度/秒）")]
    public float rotationSpeedMin = 30f;

    [Tooltip("回転速度の最大値（度/秒）")]
    public float rotationSpeedMax = 90f;

    [Header("ランダムマテリアル (10個)")]
    [Tooltip("表示に使うマテリアルを設定 (10種類)")]
    public Material[] materials;

    [Header("縦揺れ (Flutter)")]
    [Tooltip("上下に揺れる振幅")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("揺れ周波数 (Hz)")]
    public float verticalFrequency = 1f;

    [Header("速度ブレ")]
    [Tooltip("横移動力の最小値")]
    public float minMoveForce = 0.5f;
    [Tooltip("横移動力の最大値")]
    public float maxMoveForce = 1.5f;
    [Tooltip("移動力更新間隔の最小秒数")]
    public float minChangeInterval = 0.5f;
    [Tooltip("移動力更新間隔の最大秒数")]
    public float maxChangeInterval = 2f;

    private Rigidbody2D rb;
    private MeshRenderer mr;
    private float rotationSpeed;
    private FlowerSpawner spawner;

    // Flutter 用プライベート変数
    private float currentMoveForce;
    private float nextChangeTime;
    private float phaseOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        mr = GetComponent<MeshRenderer>();

        // flutter 初期化
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
            Debug.LogError("MeshRenderer または materials が設定されていません。Flower prefab を確認してください。", this);
        }

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax)
                      * (Random.value < 0.5f ? 1f : -1f);
    }

    void FixedUpdate()
    {
        // 現在のランダム移動力を横方向に適用
        rb.AddForce(Vector2.left * currentMoveForce, ForceMode2D.Force);
    }

    void Update()
    {
        // 縦揺れ (sin 波でひらひら)
        float yOffset = Mathf.Sin((Time.time + phaseOffset) * verticalFrequency * Mathf.PI * 2f)
                        * verticalAmplitude * Time.deltaTime;
        transform.Translate(Vector3.up * yOffset, Space.World);

        // 回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // 次の移動力更新タイミングになったらランダム更新
        if (Time.time >= nextChangeTime)
            ScheduleNextForceChange();

        // 破棄判定
        if (transform.position.x < destroyPositionX)
            SelfDestroy();
    }

    // 横移動力をランダムに更新し、次のタイミングを決定
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
