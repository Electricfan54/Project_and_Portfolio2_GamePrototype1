using UnityEngine;

public class CanvasFacePlayer : MonoBehaviour
{
    [SerializeField] Transform canvas;
    [SerializeField] Transform player;
    
    private void Start()
    {
        player = gameManager.instance.player.transform;
    }
    void Update()
    {
        if (player == null || canvas == null)
            return;

        Vector3 dir = transform.position - player.position;
        canvas.rotation = Quaternion.LookRotation(dir);
    }
}
