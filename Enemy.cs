using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 100.0f;
    public float damage = 25.0f;
    private float damageRate = 0.2f;
    private float damageTime;

    public bool chasePlayer = false;
    private Transform player;

    public float enemyHealth = 100.0f;
    private float currentHealth;
    public float damagecounter = 0f;

    public GameObject deathEffect;

    void Start()
    {
        if (chasePlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        currentHealth = enemyHealth;
    }

    void Update()
    {
        if (chasePlayer)
        {
            FollowPlayer();
        }
        else
        {
            MoveForward();
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            transform.LookAt(player);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void MoveForward()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Enemy health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ???????
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ?????????????
        projectile.enemyKillCount++;

        // ????????????
        if (gamemanager.instance != null)
        {
            // ??????????
            gamemanager.instance.UpdateLevelText(projectile.enemyKillCount);

            // 5???????????????????
            if (projectile.enemyKillCount % 5 == 0)
            {
                projectile.LevelUpProjectileDamage(50.0f);

                if (gamemanager.instance != null)
                {
                    gamemanager.instance.UpdateLevelSlider(0);
                }
            }
            else
            {
                if (gamemanager.instance != null)
                {
                    gamemanager.instance.UpdateLevelSlider(projectile.enemyKillCount % 5);
                }
            }

            // ??????
            gamemanager.instance.addscore(100);
        }

        // ??????????
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<player>().takendamage(damage);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.CompareTag("rock"))
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Debug.Log("rock hit");
        }
        else if(other.gameObject.CompareTag("freighter"))
        {
            other.gameObject.GetComponent<Freightership>().TakeDamage(damage);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}