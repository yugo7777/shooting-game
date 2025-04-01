using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    // ミサイルの基本設定
    public float lifetime = 5.0f;
    public float moveSpeed = 30.0f;
    public float damage = 75.0f;
    public float rotationSpeed = 3.0f;
    public float Missilelife = 3.0f;

    // 近接爆発の設定
    public bool useProximityDetonation = true;
    public float proximityDistance = 1.5f;

    // エフェクト
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    // 発射時のサウンド
    public AudioClip launchSound;
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    // 内部変数
    private Transform target;
    private bool hasCollided = false;
    private SphereCollider missileCollider;

    void Start()
    {
        // ミサイルの寿命を設定
        Destroy(gameObject, lifetime);

        // コライダーの設定確認
        EnsureCollider();

        // 最も近い敵を見つける
        FindClosestEnemy();

        // 発射時に音を再生
        PlayLaunchSound();
    }

    // コライダーの確認と設定
    private void EnsureCollider()
    {
        missileCollider = GetComponent<SphereCollider>();
        if (missileCollider == null)
        {
            missileCollider = gameObject.AddComponent<SphereCollider>();
            missileCollider.isTrigger = true;
            missileCollider.radius = 1.0f;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 目標との距離
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 近接爆発機能を使用している場合、一定距離内で爆発
            if (useProximityDetonation && distanceToTarget <= proximityDistance)
            {
                ExplodeNearTarget();
            }
            else
            {
                // 目標への方向
                Vector3 direction = target.position - transform.position;
                direction.Normalize();

                // 目標方向への回転
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 前進
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            // 目標がない場合は直進
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // 新しい目標を探す
            FindClosestEnemy();
        }
    }

    // 近接爆発処理
    private void ExplodeNearTarget()
    {
        if (target != null && !hasCollided)
        {
            DamageTarget(target.gameObject);
        }
    }

    // 最も近い敵を見つける
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy.transform;
        }
    }

    // 爆発エフェクトとサウンドを再生
    private void PlayExplosionEffects()
    {
        // 爆発エフェクト（視覚）
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 爆発サウンド
        if (explosionSound != null && soundVolume > 0f)
        {
            GameObject audioObj = new GameObject("ExplosionAudio");
            audioObj.transform.position = transform.position;

            AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
            tempAudio.clip = explosionSound;
            tempAudio.volume = soundVolume;
            tempAudio.spatialBlend = 0.5f;
            tempAudio.priority = 0;
            tempAudio.Play();

            Destroy(audioObj, explosionSound.length + 0.5f);
        }
    }

    // 発射時のサウンドを再生するメソッド
    private void PlayLaunchSound()
    {
        if (launchSound != null && soundVolume > 0f)
        {
            GameObject audioObj = new GameObject("LaunchAudio");
            audioObj.transform.position = transform.position;
            AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
            tempAudio.clip = launchSound;
            tempAudio.volume = soundVolume;
            tempAudio.spatialBlend = 0.5f;
            tempAudio.priority = 0;
            tempAudio.Play();
            Destroy(audioObj, launchSound.length + 0.5f);
        }
    }

    // 敵にダメージを与える共通処理
    private void DamageTarget(GameObject enemyObject)
    {
        if (hasCollided) return;
        hasCollided = true;

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // 爆発エフェクトとサウンドを再生
        PlayExplosionEffects();

        // ミサイルを破壊
        Destroy(gameObject);
    }

    // トリガー衝突検出
    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;

        if (other.CompareTag("Enemy"))
        {
            DamageTarget(other.gameObject);
        }
        else if (other.CompareTag("rock"))
        {
            hasCollided = true;

            // 爆発エフェクトとサウンドを再生
            PlayExplosionEffects();

            // 岩を破壊
            Destroy(other.gameObject);

            // ミサイルを破壊
            Destroy(gameObject);
        }
        else if (other.CompareTag("freighter"))
        {
            hasCollided = true;

            Freightership freighter = other.GetComponent<Freightership>();
            if (freighter != null)
            {
                freighter.TakeDamage(damage);
            }

            // 爆発エフェクトとサウンドを再生
            PlayExplosionEffects();

            // ミサイルを破壊
            Destroy(gameObject);
        }
    }

    // 物理衝突検出
    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamageTarget(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("rock"))
        {
            hasCollided = true;

            // 爆発エフェクトとサウンドを再生
            PlayExplosionEffects();

            // 岩を破壊
            Destroy(collision.gameObject);

            // ミサイルを破壊
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("freighter"))
        {
            hasCollided = true;

            Freightership freighter = collision.gameObject.GetComponent<Freightership>();
            if (freighter != null)
            {
                freighter.TakeDamage(damage);
            }

            // 爆発エフェクトとサウンドを再生
            PlayExplosionEffects();

            // ミサイルを破壊
            Destroy(gameObject);
        }
    }
}
