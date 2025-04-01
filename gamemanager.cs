using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance = null;

    [Header("area of activity")]
    public float xMinBoundary = 0f;
    public float xMaxBoundary = 200.0f;
    public float zMinBoundary = 7.0f;
    public float zMaxBoundary = 375.0f;

    [Header("ship selection")]
    public int selectedPlayer = -1;
    public GameObject[] playerprefabs;
    public GameObject player;

    [Header("start button")]
    public GameObject startbutton;
    [Header("restart button")]
    public GameObject restartbutton;
    public GameObject exitbutton;

    [Header("Scrolling Background Settings")]
    public GameObject backgroundPrefab;
    public float scrollSpeed = 5f;
    public float spawnZ = 565f;
    public float addNewBackgroundAtZ = -100f;
    public float deleteBackgroundAtZ = -725f;

    private List<GameObject> activeBackgrounds = new List<GameObject>();
    private bool hasSpawnedNext = false;

    [Header("Level Up UI")]
    public Image levelUpRadial;

    [Header("Level Display")]
    public TextMeshProUGUI levelText;

    [Header("score system")]
    public int currentscore = 0;
    public TextMeshProUGUI scoretext;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateLevelSlider(int killCount)
    {
        if (levelUpRadial != null)
        {
            levelUpRadial.fillAmount = (killCount % 5) / 5f;
        }
    }

    public void UpdateLevelText(int currentKillCount)
    {
        if (levelText != null)
        {
            int level = (currentKillCount / 5) + 1;
            levelText.text = "Lv." + level;
        }
    }

    void Start()
    {
        if (backgroundPrefab != null)
        {
            GameObject firstBG = Instantiate(backgroundPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            activeBackgrounds.Add(firstBG);
        }

        if (levelUpRadial != null)
        {
            levelUpRadial.fillAmount = (projectile.enemyKillCount % 5) / 5f;
        }

        if (levelText != null)
        {
            int level = (projectile.enemyKillCount / 5) + 1;
            levelText.text = "Lv." + level;
        }
        updatescoreUI();
    }

    void Update()
    {
        ScrollBackgrounds();
    }

    private void ScrollBackgrounds()
    {
        for (int i = 0; i < activeBackgrounds.Count; i++)
        {
            GameObject bg = activeBackgrounds[i];
            if (bg == null) continue;

            bg.transform.Translate(Vector3.back * scrollSpeed * Time.deltaTime);
            float bgZ = bg.transform.position.z;

            if (!hasSpawnedNext && bgZ < addNewBackgroundAtZ)
            {
                SpawnBackground(spawnZ);
                hasSpawnedNext = true;
            }

            if (bgZ < deleteBackgroundAtZ)
            {
                Destroy(bg);
                activeBackgrounds.RemoveAt(i);
                i--;
            }
        }

        if (activeBackgrounds.Count > 0)
        {
            GameObject lastBG = activeBackgrounds[activeBackgrounds.Count - 1];
            if (lastBG.transform.position.z < (spawnZ - (spawnZ - addNewBackgroundAtZ)))
            {
                hasSpawnedNext = false;
            }
        }
    }

    private void SpawnBackground(float zPos)
    {
        Vector3 spawnPos = new Vector3(0f, 0f, zPos);
        GameObject bg = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity);
        activeBackgrounds.Add(bg);
    }

    public void selectplayer(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerprefabs.Length)
        {
            Debug.LogWarning("please select player correctly?");
            return;
        }
        selectedPlayer = playerIndex;
        Debug.Log(playerIndex + " is picked");
    }

    public void StartGame()
    {
        if (selectedPlayer == -1)
        {
            Debug.Log("player didn't select");
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Shmup");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Shmup")
        {
            Vector3 spawnPosition = new Vector3(25, 0, 70);
            Instantiate(playerprefabs[selectedPlayer], spawnPosition, Quaternion.identity);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Start scene");
    }
    public void addscore(int amount)
    {
        currentscore += amount;
        updatescoreUI();
    }
    private void updatescoreUI()
    {
        if(scoretext != null)
        {
            scoretext.text = "score" + currentscore;
        }
    }
}
