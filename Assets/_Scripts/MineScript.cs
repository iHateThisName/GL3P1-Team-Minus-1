using UnityEngine;

public class MineScript : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private void Update()
    {
        transform.LookAt(GameManager.Instance.PlayerInteractTransform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.BreathingScript.TakeDamage(damage);

            GameManager.Instance.PlayerMovement.GetRigidbody().AddForce(transform.forward * 25f, ForceMode.Impulse);

            Destroy(this.gameObject);
        }
    }
}
