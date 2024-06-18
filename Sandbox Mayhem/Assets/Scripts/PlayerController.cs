using System;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public bool isHuman;
    public int Coins { get; private set; }
    public string Id { private set; get; }
    private CarController Car { get; set; }
    public UnityEvent onPointsChanged = new (); // Event to notify points change
    private void Start()
    {
        Coins = 0;
        Id = Guid.NewGuid().ToString();
        Car = GetComponent<CarController>();
        if (Car == null)
        {
            Debug.LogError("PlayerController requires a car controller");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            
            Coins++;
            Debug.Log("Coins count is: " + Coins);
            
            if (isHuman)
            {
                onPointsChanged.Invoke();
            }
        }
    }
}