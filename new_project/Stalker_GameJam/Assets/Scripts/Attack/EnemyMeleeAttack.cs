using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : Attack
{
    [Header("Melee Hitbox")]
    [SerializeField] private float moveSpeed = 0f;     // 0이면 제자리 판정(휘두름)
    [SerializeField] private float lifeTime = 0.15f;   // 근접은 짧게
    [SerializeField] private float range = 1.5f;       // 이동형일 때만 의미 있음
    [SerializeField] private bool destroyOnHit = true;

    private Vector3 spawnPos;
    private float dieAtTime;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        if (Time.time >= dieAtTime)
        {
            Destroy(gameObject);
            return;
        }

        // 이동형 근접(짧게 전진)일 때만 거리 제한 체크
        if (moveSpeed > 0f && range > 0f)
        {
            if ((transform.position - spawnPos).sqrMagnitude >= range * range)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (moveSpeed <= 0f) return;

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

    protected override void OnHit(Creature target)
    {
        if (destroyOnHit)
            Destroy(gameObject);
    }

    // Bullet.Configure처럼 “무기 데이터 기반 세팅” + 팀킬 방지까지 포함
    public void ConfigureMelee(int dmg, float speed, float rng, float kb, float stun, Transform origin, CreatureType immune)
    {
        damage = dmg;
        moveSpeed = speed;
        range = rng;

        knockbackForce = kb;
        stunDuration = stun;
        knockbackOrigin = origin;

        immuneTargetType = immune;
    }
}
