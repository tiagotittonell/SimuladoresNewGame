using UnityEngine;

[CreateAssetMenu(fileName = "PlayerEquipmentData", menuName = "Game/Player Equipment Data")]
public class PlayerEquipmentData : ScriptableObject
{
    public GameObject equippedHelmet;
    public GameObject equippedChest;
    public GameObject equippedLegs;
    public GameObject equippedWeapon;

    // Puedes agregar stats
    public int damageBonus;
    public int defenseBonus;
}
