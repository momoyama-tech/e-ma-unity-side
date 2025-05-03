using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class FallingFlower : MonoBehaviour
{
    public float moveForce = 1f;
    public float destroyX = -12f;
    public float rotMin = 30f, rotMax = 90f;
    Rigidbody2D _rb; float _rot;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
    }
    void Start()
    {
        _rot = Random.Range(rotMin, rotMax) * (Random.value < .5f ? -1 : 1);
    }
    void FixedUpdate()
    {
        _rb.AddForce(Vector2.left * moveForce, ForceMode2D.Force);
    }
    void Update()
    {
        transform.Rotate(0, 0, _rot * Time.deltaTime);
        if (transform.position.x < destroyX) Destroy(gameObject);
    }
}
