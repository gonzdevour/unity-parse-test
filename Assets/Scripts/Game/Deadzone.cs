using UnityEngine;

public class Deadzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Inst?.OnPlayerDead();
        }
    }
}
