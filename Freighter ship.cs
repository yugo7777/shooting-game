using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Freightership : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float HP = 1000.0f;
    public Slider HPslider;
    public TextMeshProUGUI hpText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HPslider.value = HP;
        updateHPtext();
    }
    public void TakeDamage(float damage)
    {
        HP -= damage;
        HPslider.value = HP;
        Debug.Log("Freighter ship health: " + HP);
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    void updateHPtext()
    {
        hpText.text = "HP:" + HP.ToString();
    }
}
