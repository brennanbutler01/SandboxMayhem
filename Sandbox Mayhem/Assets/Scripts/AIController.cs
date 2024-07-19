using System;
using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour, IPlayer
{
    [field: SerializeField]
    public Transform weaponSpawn { get; private set; }
    public Weapon currentWeapon { get; set; }

    private CarController carController;
    private Rigidbody rigidBody;
    
    public void Start()
    {
        carController = GetComponent<CarController>();
        rigidBody = GetComponent<Rigidbody>();
    }

    public void RemoveWeapon()
    {
        currentWeapon = Weapon.None;
    }
    
    public void BoxCollision(GameObject box) { }

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
    
    public void DecreaseSpeedToZero(float duration)
    {
        carController.isGameOver = true;
        carController.isMovementEnabled = false;
        StartCoroutine(DecreaseSpeedToZeroCoroutine(duration));
    }

    private IEnumerator DecreaseSpeedToZeroCoroutine(float duration)
    {
        var elapsed = 0f;
        var initialVelocity = rigidBody.velocity;
        while (elapsed < duration)
        {
            rigidBody.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsed/duration);
            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}