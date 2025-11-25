using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Player Equipment")]
public class PlayerEquipmentSO : ScriptableObject
{
    [Header("Armaduras equipadas")]
    public GameObject helmet;
    public GameObject chest;
    public GameObject boots;
    public GameObject weapon;

    [Header("Stats futuros (si querés ampliarlo)")]
    public int strength;
    public int defense;
    public int agility;
}
