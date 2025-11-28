using UnityEngine;

public class FogScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 0)
        { 
            RenderSettings.fog = false;
            Debug.Log("Turning off fog");
        }
        else if (transform.position.y < 0)
        {
            RenderSettings.fog = true;
            Debug.Log("Turning on fog");
        }
    }
}
