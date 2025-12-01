using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class MineScript : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private Transform lookAtPlayer;

    [SerializeField]
    private VisualEffect explosionEffect;

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

            VisualEffect obj = Instantiate(explosionEffect);
            obj.transform.position = transform.position;

            Destroy(this.gameObject);
        }
    }
}
