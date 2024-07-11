using UnityEngine;

public class AIController : MonoBehaviour, IPlayer
{
    [field: SerializeField]
    public Transform weaponSpawn { get; private set; }
    public Weapon currentWeapon { get; set; }

    public void RemoveWeapon()
    {
        currentWeapon = Weapon.None;
    }

    public void SetDart()
    {
        currentWeapon = Weapon.Dart;
    }
    
    void Update()
    {
        HandleWeapons();
    }
    
    private void HandleWeapons()
    {
        if (currentWeapon == Weapon.Dart)
        {
            RaycastHit hit;
            if (Physics.Raycast(weaponSpawn.position, weaponSpawn.forward, out hit, 60f))
            {
                if (hit.collider.GetComponent<HumanController>())
                {
                    EventManager.TriggerEvent<FireDartEvent, IPlayer>(this);
                }
            }    
        }
    }
}