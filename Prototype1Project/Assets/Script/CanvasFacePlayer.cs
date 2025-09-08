using UnityEngine;

public class CanvasFacePlayer : MonoBehaviour
{
    [SerializeField] Transform canvas;
    [SerializeField] Transform player;

    void Update()
    {
        if (player == null || canvas == null)
            return;

        Vector3 dir = transform.position - player.position;
        canvas.rotation = Quaternion.LookRotation(dir);
    }
}
