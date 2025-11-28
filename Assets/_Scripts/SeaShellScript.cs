using UnityEngine;

public class SeaShellScript : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private int damage;

    private bool inRange = false;

    [SerializeField]
    private AudioSource closeSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    public void CloseShell()
    {
        anim.SetTrigger("Close");
    }

    public void CheckDistance()
    {
        if(inRange)
        {
            GameManager.Instance.BreathingScript.TakeDamage(damage);

            GameManager.Instance.PlayerMovement.GetRigidbody().AddForce(transform.forward * 25f, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Not in range");
        }
        closeSound.Play();
    }
}
