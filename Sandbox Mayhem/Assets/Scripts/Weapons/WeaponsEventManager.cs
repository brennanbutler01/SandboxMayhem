using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class WeaponsEventManager : MonoBehaviour
    {
        public GameObject dartPrefab;
        private UnityAction<PlayerController> dartEventListener;

        private void Awake()
        {
            dartEventListener = fireDartEventHandler;
        }

        private void OnEnable()
        {
            EventManager.StartListening<FireDartEvent, PlayerController>(dartEventListener);
        }
        
        private void OnDisable()
        {
            EventManager.StopListening<FireDartEvent, PlayerController>(dartEventListener);
        }

        void fireDartEventHandler(PlayerController trigger)
        {
            if (dartPrefab)
            {
                Transform spawn = trigger.weaponSpawn;
                GameObject projectile = Instantiate(dartPrefab, spawn.position, spawn.rotation);
                
                // Ignore collisions with player
                var projectileCollider = projectile.GetComponentInChildren<Collider>();
                foreach (var triggerCollider in trigger.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(projectileCollider, triggerCollider);
                }
                
                DartController dartController = projectile.GetComponent<DartController>();
                dartController.SetTarget(trigger.transform.forward);
                dartController.SetSpeed(trigger.GetComponent<Rigidbody>().velocity.magnitude);
                dartController.player = trigger;
            }
            trigger.RemoveWeapon();
        }
    }
}