using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class player : MonoBehaviour
{
    // ????
    public float movespeed = 50.0f;
    private Vector3 position;

    // ???
    public GameObject projectile;
    public float fireRate = 0.5f;
    private float fireTime;

    // ??????
    public GameObject missileProjectile;
    public float missileFireRate = 1.5f;
    private float missileFireTime;

    // HP??
    public float health = 100.0f;  // 1.0f????
    public Slider healthslider;
    public GameObject deathEffect;

    // ???????????
    public AudioClip bulletSound;
    [Range(0f, 1f)]  // ???0?1??????
    public float soundVolume = 0.7f;  // 100.0f????????
    private AudioSource playerAudioSource;  // ???AudioSource??

    public TextMeshProUGUI hpText;
    public float spawnOffsetY = 5.0f;

    public Image[] missileIcons;

    // ????????
    private float missileLife = 3.0f;

    void Start()
    {
        // ????????????????
        SetupAudio();

        // AudioListener??????????
        CheckAudioListeners();

        if (healthslider == null)
        {
            GameObject sliderObj = GameObject.Find("Slider");
            if (sliderObj != null)
            {
                healthslider = sliderObj.GetComponent<Slider>();
                Debug.Log("[player] Slider found and assigned.");
            }
            else
            {
                Debug.LogWarning("[player] Slider not found!");
            }
        }

        if (hpText == null)
        {
            GameObject textObj = GameObject.Find("hpText");
            if (textObj != null)
            {
                hpText = textObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("[player] hpText found and assigned.");
            }
            else
            {
                Debug.LogWarning("[player] hpText not found!");
            }
        }

        // HP???
        if (healthslider != null) healthslider.maxValue = 100;
        if (healthslider != null) healthslider.value = health;
        if (hpText != null) hpText.text = "HP: " + health.ToString();

        // ???????????
        if (missileProjectile == null)
        {
            Debug.LogWarning("[player] Missile prefab not assigned!");
        }

        // ???????????
        UpdateMissileIcons();
    }

    // ?????????????????
    private void SetupAudio()
    {
        // ???AudioSource???
        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
            playerAudioSource.playOnAwake = false;
            playerAudioSource.spatialBlend = 0f; // 2D??????
            playerAudioSource.priority = 128; // ?????
        }

        // ?????????
        if (bulletSound == null)
        {
            Debug.LogError("[player] bulletSound is not assigned! Please set it in the Inspector.");
        }
        else
        {
            Debug.Log("[player] bulletSound is properly assigned: " + bulletSound.name);
        }
    }

    // AudioListener??????????
    private void CheckAudioListeners()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        Debug.Log("[player] Number of AudioListeners in scene: " + listeners.Length);
        foreach (AudioListener listener in listeners)
        {
            Debug.Log("[player] AudioListener found on: " + listener.gameObject.name);
        }
    }

    void Update()
    {
        position = transform.position;

        Movement();
        Boundary();
        transform.position = position;

        shoot();
        fireMissile();

        // HP??
        healthslider.value = health;
        updateHPtext();
    }

    // ?????
    private void shoot()
    {
        if (Input.GetKey("space") && Time.time > fireTime)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, spawnOffsetY, 0);
            Instantiate(projectile, spawnPosition, transform.rotation);

            // ????????????
            PlayBulletSound();

            fireTime = Time.time + fireRate;
        }
    }

    // ??????????
    private void PlayBulletSound()
    {
        if (bulletSound != null)
        {
            // ??1: ??AudioSource???????
            if (playerAudioSource != null)
            {
                playerAudioSource.PlayOneShot(bulletSound, soundVolume);
                Debug.Log("[player] Playing bullet sound through AudioSource. Volume: " + soundVolume);
            }
            // ??2: ?????????PlayClipAtPoint???
            else
            {
                // ??????????????????????????
                AudioSource.PlayClipAtPoint(bulletSound, Camera.main.transform.position, soundVolume);
                Debug.Log("[player] Playing bullet sound at point (camera). Volume: " + soundVolume);
            }
        }
        else
        {
            Debug.LogError("[player] Cannot play bullet sound: bulletSound is NULL!");
        }
    }

    private void fireMissile()
    {
        if (Input.GetMouseButton(1) && Time.time > missileFireTime && missileProjectile != null && missileLife > 0)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, spawnOffsetY, 0);
            GameObject missile = Instantiate(missileProjectile, spawnPosition, transform.rotation);

            // ????????GuidedMissile??????????
            GuidedMissile guidedMissile = missile.GetComponent<GuidedMissile>();
            if (guidedMissile != null)
            {
                guidedMissile.Missilelife = missileLife;
            }

            // ???????
            missileLife -= 1.0f;
            Debug.Log("[player] Missile fired! Remaining: " + missileLife);

            missileFireTime = Time.time + missileFireRate;

            // ???????????
            UpdateMissileIcons();
        }
    }

    private void Movement()
    {
        if (Input.GetKey("w"))
        {
            position.z += movespeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            position.z -= movespeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            position.x -= movespeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            position.x += movespeed * Time.deltaTime;
        }
    }

    private void Boundary()
    {
        // obtain the instance of the gamemanager
        if (gamemanager.instance != null)
        {
            // xboundary check
            if (position.x > gamemanager.instance.xMaxBoundary)
                position.x = gamemanager.instance.xMaxBoundary;
            else if (position.x < gamemanager.instance.xMinBoundary)
                position.x = gamemanager.instance.xMinBoundary;

            // zboundary check
            if (position.z > gamemanager.instance.zMaxBoundary)
                position.z = gamemanager.instance.zMaxBoundary;
            else if (position.z < gamemanager.instance.zMinBoundary)
                position.z = gamemanager.instance.zMinBoundary;
        }
    }

    void updateHPtext()
    {
        hpText.text = "HP:" + health.ToString();
    }

    public void takendamage(float damage)
    {
        health -= damage;
        healthslider.value = health;

        if (health <= 0)
        {
            Destroy(this.gameObject);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            SceneManager.LoadScene("Game over scene");
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > 100f) health = 100f;
        if (healthslider != null) healthslider.value = health;
        updateHPtext();
    }

    // ??????????????
    public void RestoreMissiles(float amount)
    {
        missileLife += amount;
        Debug.Log("Missiles restored! Current: " + missileLife);
        UpdateMissileIcons();
    }

    // ???????????
    private void UpdateMissileIcons()
    {
        if (missileIcons != null && missileIcons.Length > 0)
        {
            int availableMissiles = Mathf.Min(Mathf.FloorToInt(missileLife), missileIcons.Length);

            for (int i = 0; i < missileIcons.Length; i++)
            {
                if (missileIcons[i] != null)
                {
                    missileIcons[i].enabled = (i < availableMissiles);
                }
            }
        }
    }
}