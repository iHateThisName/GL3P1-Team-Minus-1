using UnityEngine;

public class MineScript : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private Transform lookAtPlayer;

    private void Update()
    {
        lookAtPlayer.LookAt(GameManager.Instance.PlayerInteractTransform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.BreathingScript.TakeDamage(damage);

            GameManager.Instance.PlayerMovement.GetRigidbody().AddForce(lookAtPlayer.forward * 25f, ForceMode.Impulse);

            Destroy(this.gameObject);
        }
    }
}
