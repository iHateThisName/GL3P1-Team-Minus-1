using Assets.Scripts.Singleton;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScreenDialouge : Singleton<ScreenDialouge> {

    [SerializeField] private Transform DialougeScreen;
    [SerializeField] private TMP_Text dialogText;

    private bool isVisable = false;
    private float typingSpeed = 0.02f;
    public float speedUpFactor = 1f;

    public bool isPlayingDialogLine = false;

    void Start() {
        this.DialougeScreen.gameObject.SetActive(isVisable);
    }

    private void SetDialogVisablity(bool newValue) {
        if (newValue != this.isVisable) {
            this.isVisable = newValue;
            this.DialougeScreen.gameObject.SetActive(this.isVisable);
        }
    }

    private IEnumerator printChar(string line) {
        this.isPlayingDialogLine = true;

        foreach (char x in line.ToCharArray()) {
            this.dialogText.text += x;
            yield return new WaitForSeconds(this.typingSpeed * this.speedUpFactor);
        }

        this.isPlayingDialogLine = false;
    }

    public IEnumerator ShowDialougeScreen(string line) {
        if (!this.isVisable) {
            SetDialogVisablity(true);
        }

        this.dialogText.text = "";
        yield return StartCoroutine(printChar(line));
    }

    public void CloseDialougeScreen() {
        this.dialogText.text = "";
        SetDialogVisablity(false);

    }
}
