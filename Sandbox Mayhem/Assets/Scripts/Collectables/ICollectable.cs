using UnityEngine;

namespace Collectables
{
    public interface ICollectable
    {
        void Collect(GameObject collector);
        void OnTriggerEnter(Collider other);
    }
}
