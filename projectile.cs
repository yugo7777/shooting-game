using UnityEngine;

public class projectile : MonoBehaviour
{
    public float lifetime = 2.0f;
    public float moveSpeed = 50.0f;

    public static float baseDamage = 50.0f;
    public float additionalDamage = 0f;
    public static int enemyKillCount = 0;

    public float damage;
    bool hasCollided = false;

    void Start()
    {
        // ?????baseDamage???
        damage = baseDamage + additionalDamage;

        // ?????
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ?????
        Move();
    }

    private void Move()
    {
        transform.position += Time.deltaTime * moveSpeed * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;
        hasCollided = true;

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Projectile hit enemy with damage: " + damage);
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("rock"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            Debug.Log("Rock hit");
        }
    }

    public static void LevelUpProjectileDamage(float additionalDamage)
    {
        baseDamage += additionalDamage;
        Debug.Log("Projectile damage leveled up: " + baseDamage);
    }
}