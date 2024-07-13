using System.Collections;
using UnityEngine;

public class HumanController : MonoBehaviour, IPlayer
{
    private GameObject weaponText;
    private CarController carController;
    public Weapon currentWeapon { get; set; }
    
    [field: SerializeField]
    public Transform weaponSpawn { get; private set; }

    void Start()
    {
        carController = GetComponent<CarController>();
        weaponText = GameObject.FindGameObjectWithTag("WeaponText");
        weaponText.SetActive(false);
    }

    void Update()
    {
        HandleWeaponsInput();
    }
    
    private void HandleWeaponsInput()
    {
        if (Input.GetButtonDown("Fire"))
        {
            if (currentWeapon == Weapon.Dart)
            {
                EventManager.TriggerEvent<FireDartEvent, IPlayer>(this);
            }
        }
    }
    
    public void SetDart()
    {
        currentWeapon = Weapon.Dart;
        weaponText.SetActive(true);
    }

    public void RemoveWeapon()
    {
        currentWeapon = Weapon.None;
        weaponText.SetActive(false);
    }

    public void DecreaseSpeedToZero(float duration)
    {
        carController.isGameOver = true;
        RemoveWeapon();
        StartCoroutine(DecreaseSpeedToZeroCoroutine(duration));
    }

    private IEnumerator DecreaseSpeedToZeroCoroutine(float duration)
    {
        var elapsed = 0f;
        var initialMaxSpeed = carController.maxSpeed;
        while (elapsed < duration)
        {
            carController.maxSpeed = Mathf.Lerp(initialMaxSpeed, 0, elapsed/duration);
            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
