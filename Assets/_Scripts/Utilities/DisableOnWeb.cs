using UnityEngine;

public class DisableOnWeb : MonoBehaviour {
    private void Awake() {
#if UNITY_WEBGL
        this.gameObject.SetActive(false);
#endif
    }
}
