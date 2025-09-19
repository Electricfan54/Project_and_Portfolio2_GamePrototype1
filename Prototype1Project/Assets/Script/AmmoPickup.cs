using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public ammoType type;
    public int amount;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup != null)
        {
            pickup.PickupAmmo(type, amount);
            Destroy(gameObject);
        }
    }
}
