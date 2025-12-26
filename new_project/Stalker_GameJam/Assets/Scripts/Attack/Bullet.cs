using UnityEngine;

public class Bullet : Attack
{
    [Header("Bullet")]
    public float moveSpeed = 10f;
    public float lifeTime = 3f;
    public float range = 50f;

    private Vector3 spawnPos;
    private float dieAtTime;
    private void Awake()
    {
        spawnPos = transform.position;
    }

    private void OnEnable()
    {
        spawnPos = transform.position;
        dieAtTime = Time.time + lifeTime;
    }

    private void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if (Time.time >= dieAtTime)
        {
            Destroy(gameObject);
            return;
        }

        if (range > 0f && Vector3.SqrMagnitude(transform.position - spawnPos) >= range * range)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Creature target = other.GetComponentInParent<Creature>();
        if (target == null) return;

        ApplyHit(target);
    }

    protected override void OnHit(Creature target)
    {
        Destroy(gameObject);
    }

    // Creature가 무기 데이터로 세팅하게끔
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
