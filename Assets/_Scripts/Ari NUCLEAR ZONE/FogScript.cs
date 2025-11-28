using UnityEngine;

public class FogScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 0) RenderSettings.fog = false;
        else if (transform.position.y < 0) RenderSettings.fog = true;
    }
}
