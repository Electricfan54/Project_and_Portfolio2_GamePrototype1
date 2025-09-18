using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] ammoType type;
    [SerializeField] int amount;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup != null)
        {
            pickup.PickupAmmo(type, amount);
        }
    }
}
