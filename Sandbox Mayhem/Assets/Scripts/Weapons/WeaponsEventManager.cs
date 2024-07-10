using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class WeaponsEventManager : MonoBehaviour
    {
        public GameObject dartPrefab;
        private UnityAction<IPlayer> dartEventListener;
        
        private void Awake()
        {
            dartEventListener = fireDartEventHandler;
        }

        private void OnEnable()
        {
            EventManager.StartListening<FireDartEvent, IPlayer>(dartEventListener);
        }
        
        private void OnDisable()
        {
            EventManager.StopListening<FireDartEvent, IPlayer>(dartEventListener);
        }

        void fireDartEventHandler(IPlayer trigger)
        {
            if (dartPrefab)
            {
                Transform spawn = trigger.weaponSpawn;
                GameObject projectile = Instantiate(dartPrefab, spawn.position, spawn.rotation);
                
                // Ignore collisions with player
                var projectileCollider = projectile.GetComponentInChildren<Collider>();
                foreach (var triggerCollider in trigger.transform.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(projectileCollider, triggerCollider);
                }
                
                DartController dartController = projectile.GetComponent<DartController>();
                dartController.SetTarget(trigger.transform.forward);
                dartController.SetSpeed(trigger.transform.GetComponent<Rigidbody>().velocity.magnitude);
            }
            trigger.RemoveWeapon();
        }
    }
}