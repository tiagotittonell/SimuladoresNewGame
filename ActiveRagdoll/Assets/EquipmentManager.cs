using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Referencia al ScriptableObject")]
    public PlayerEquipmentSO equipmentData;

    [Header("Referencias al modelo (huesos donde van las piezas)")]
    public Transform helmetSlot;
    public Transform chestSlot;
    public Transform bootsSlot;
    public Transform weaponSlot;

    private GameObject currentHelmet;
    private GameObject currentChest;
    private GameObject currentBoots;
    private GameObject currentWeapon;

    void Start()
    {
        ApplyAllEquipment();
    }

    public void ApplyAllEquipment()
    {
        EquipHelmet(equipmentData.helmet);
        EquipChest(equipmentData.chest);
        EquipBoots(equipmentData.boots);
        EquipWeapon(equipmentData.weapon);
    }

    public void EquipHelmet(GameObject prefab)
    {
        if (currentHelmet != null) Destroy(currentHelmet);
        if (prefab == null) return;

        currentHelmet = Instantiate(prefab, helmetSlot);
    }

    public void EquipChest(GameObject prefab)
    {
        if (currentChest != null) Destroy(currentChest);
        if (prefab == null) return;

        currentChest = Instantiate(prefab, chestSlot);
    }

    public void EquipBoots(GameObject prefab)
    {
        if (currentBoots != null) Destroy(currentBoots);
        if (prefab == null) return;

        currentBoots = Instantiate(prefab, bootsSlot);
    }

    public void EquipWeapon(GameObject prefab)
    {
        if (currentWeapon != null) Destroy(currentWeapon);
        if (prefab == null) return;

        currentWeapon = Instantiate(prefab, weaponSlot);
    }
}
