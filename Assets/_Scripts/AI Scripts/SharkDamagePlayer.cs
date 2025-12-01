using System.Collections;
using UnityEngine;

public class SharkDamagePlayer : MonoBehaviour {

    [SerializeField] private int damage;
    [SerializeField] private Animator animationAnimator;

    private const string BiteTrigger = "BiteTrigger";

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            StartCoroutine(BiteAnimationCoroutine());
        }
    }

    private IEnumerator BiteAnimationCoroutine() {
        animationAnimator.SetTrigger(BiteTrigger);
        yield return new WaitForSeconds(0.25f);
        GameManager.Instance.BreathingScript.TakeDamage(damage);
        GameManager.Instance.PlayerMovement.GetRigidbody().AddForce(transform.forward * 25f, ForceMode.Impulse);
    }
}
