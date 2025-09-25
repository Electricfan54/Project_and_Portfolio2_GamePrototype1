using UnityEngine;

public class CanvasFacePlayer : MonoBehaviour
{
    [SerializeField] RectTransform canvas;
    [SerializeField] Transform player;
    [SerializeField] EnemyAI enemyScript;
    [SerializeField] GameObject statusEffectIcon;
    
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

        if (enemyScript.hasStatusEffect)
            statusEffectIcon.SetActive(true);
        else if (statusEffectIcon.activeSelf)
            statusEffectIcon.SetActive(false);

    }
}