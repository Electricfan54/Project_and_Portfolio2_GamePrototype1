using UnityEngine;

public class CanvasFacePlayer : MonoBehaviour
{
    [SerializeField] RectTransform canvas;
    [SerializeField] Transform player;
    
    private void Start()
    {
        player = gameManager.instance.player.transform;
    }
    void Update()
    {
        if (player == null || canvas == null)
            return;

        Vector3 dir = transform.position - new Vector3(player.position.x, transform.position.y, player.position.z);

        canvas.rotation = Quaternion.LookRotation(dir);
    }
}