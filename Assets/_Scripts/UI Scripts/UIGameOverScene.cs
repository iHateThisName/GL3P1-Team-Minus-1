using UnityEngine;

public class UIGameOverScene : MonoBehaviour
{
    public void LoadScene(string name)
    {
        GameSceneManager.Instance.LoadScene(name);
    }
}
