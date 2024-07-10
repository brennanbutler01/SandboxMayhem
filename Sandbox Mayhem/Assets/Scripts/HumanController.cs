using UnityEngine;

public class HumanController : MonoBehaviour, IPlayer
{
    private GameObject weaponText;
    public Weapon currentWeapon { get; set; }
    
    [field: SerializeField]
    public Transform weaponSpawn { get; private set; }

    void Start()
    {
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


    public Transform WeaponSpawn()
    {
        throw new System.NotImplementedException();
    }
}
