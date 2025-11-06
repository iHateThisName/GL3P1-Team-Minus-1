using UnityEngine;

public class SeaShellScript : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    public void CloseShell()
    {
        anim.SetTrigger("Close");
    }
}
