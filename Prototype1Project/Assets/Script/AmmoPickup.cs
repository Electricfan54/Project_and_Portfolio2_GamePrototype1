using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public ammoType type;
    public int amount;
    [SerializeField] GameObject model;
    [SerializeField] float modelRotateSpeed;

    private void Update()
    {
        model.transform.Rotate(0, modelRotateSpeed * Time.deltaTime, 0);
    }

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
