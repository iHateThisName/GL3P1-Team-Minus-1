using UnityEngine;

public class FogScript : MonoBehaviour {
    void LateUpdate() {
        if (transform.position.y > 0) {
            RenderSettings.fog = false;
        } else if (transform.position.y < 0) {
            RenderSettings.fog = true;
        }
    }
}
