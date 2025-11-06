using UnityEngine;

public class SharkDamagePlayer : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.BreathingScript.TakeDamage(damage);

            GameManager.Instance.PlayerMovement.GetRigidbody().AddForce(transform.forward * 25f, ForceMode.Impulse);
        }
    }
}
