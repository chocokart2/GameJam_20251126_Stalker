using UnityEngine;

public class Bullet : Attack
{
    [Header("Bullet")]
    public float moveSpeed = 10f;
    public float lifeTime = 3f;
    public float range = 50f;

    private Vector3 spawnPos;
    private float dieAtTime;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // ·çÆ®¿¡ Rigidbody
    }

    private void OnEnable()
    {
        spawnPos = transform.position;
        dieAtTime = Time.time + lifeTime;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = transform.position;
            rb.rotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (Time.time >= dieAtTime) { Destroy(gameObject); return; }

        if (range > 0f && (transform.position - spawnPos).sqrMagnitude >= range * range)
        {
            Destroy(gameObject);
            return;
        }

        if (rb != null)
        {
            Vector3 nextPos = rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
        }
        else
        {
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"HIT RAW: {other.name}");

        Creature target = other.GetComponentInParent<Creature>();
        if (target == null) return;

        ApplyHit(target);
        Debug.Log($"HIT: {other.name}");
    }

    protected override void OnHit(Creature target)
    {
        Destroy(gameObject);
    }

    public void Configure(int dmg, float speed, float rng, float stun, float kb, Transform origin)
    {
        damage = dmg;
        moveSpeed = speed;
        range = rng;
        stunDuration = stun;
        knockbackForce = kb;
        knockbackOrigin = origin;
    }
}
