using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour {
    [SerializeField] private Button selectionButton;
    private void Start() {
        EventSystem.current.SetSelectedGameObject(selectionButton.gameObject);
    }

}
