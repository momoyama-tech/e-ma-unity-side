// FlowerMovement.cs
using UnityEngine;

/// <summary>
/// 花の物理移動（左方向への力）、平面回転、ランダムマテリアル割り当て、破棄を管理
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(MeshRenderer))]
public class FlowerMovement : MonoBehaviour
{
    [Tooltip("左方向への移動力の強さ")]
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
            Debug.LogError("MeshRenderer または materials が設定されていません。Flower prefab を確認してください。", this);
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