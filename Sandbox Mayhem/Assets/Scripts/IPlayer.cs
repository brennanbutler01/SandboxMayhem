using UnityEngine;

public enum Weapon
{
    None, 
    Dart
}

public interface IPlayer
{
    public Transform transform { get; }
    public Transform weaponSpawn { get; }
    public Weapon currentWeapon { get; set; }

    public void RemoveWeapon();
    public void SetDart();
    public void DecreaseSpeedToZero(float duration);
    public void BoxCollision(GameObject box);
}
