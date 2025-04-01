using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemManager : MonoBehaviour
{
    public ItemType itemType = ItemType.Heal;
    public float healAmount = 20.0f;
    public float SpeedUp = 50.0f;
    public float moveSpeed = 10.0f;
    public float rotationSpeed = 20.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    public enum ItemType
    {
        Heal,
        Speed
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player p = other.GetComponent<player>();
            if(p != null && itemType == ItemType.Heal)
            {
                p.Heal(healAmount);
            }else if(p != null && itemType == ItemType.Speed)
            {
                p.movespeed += SpeedUp;
            }
                Destroy(this.gameObject);
        }
    }
    private void MoveForward()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }
}
