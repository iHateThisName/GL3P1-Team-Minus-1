using UnityEngine;

public class SlowDown : MonoBehaviour {
    [SerializeField]
    private Transform invisibleWall;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            invisibleWall.gameObject.SetActive(true);
            GameManager.Instance.SlowDownPlayer();
        }
    }
}
