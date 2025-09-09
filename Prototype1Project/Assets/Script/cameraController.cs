using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int Sens;
    [SerializeField] int lockVertMin, lockVertMax;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float MouseX = Input.GetAxisRaw("Mouse X") * Sens * Time.deltaTime;
        float MouseY = Input.GetAxisRaw("Mouse Y") * Sens * Time.deltaTime;

        rotX -= MouseY;
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * MouseX);
    }
}
