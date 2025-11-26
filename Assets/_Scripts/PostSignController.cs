using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PostSignController : Interactable {

    [SerializeField] private Image promtImage;
    private bool isPlayerInRange = false;

    [Header("Signe Varibals")]
    [SerializeField] private bool is200Enabled = false;
    [SerializeField] private bool is1KmEnabled = false, is4KmEnabled = false, is5KmEnabled = false, is6KmEnabled = false;

    [Header("Signe Refrences")]
    [SerializeField] private Transform TwoHundredMeterSign;
    [SerializeField] private Transform km1Sign, km4Sign, km5Sign, km6Sign;
    [SerializeField] private CinemachineCamera cameraPostSign;

    [Header("Flower References")]
    [SerializeField] private Transform TwoHundredFlower;
    [SerializeField] private Transform OneKmFlower, FourKmFlower, FiveKmFlower, SixKmFlower;

    [Header("Light References")]
    [SerializeField] private Transform TwoHundredLight;
    [SerializeField] private Transform OneKmLight, FourKmLight, FiveKmLight, SixKmLight;

    private const float flowerScaleHoverValue = 0.8f;
    private const float flowerScaleDefaultValue = 0.2f;

    private bool isInteracted = false;
    private CheckPointManager.EnumCheckPoint currentHoverCheckPoint = CheckPointManager.EnumCheckPoint.None;

    [Header("Player Actions")]
    [SerializeField] private InputActionReference moveActionReference;

    public override int GetValueAmount() => 0;

    public override void Interact() {
        if (!this.isPlayerInRange) return;

        // Toggle interaction state
        this.isInteracted = !this.isInteracted;

        UpdateCamera();

        if (this.isInteracted) {


            List<CheckPointManager.EnumCheckPoint> available = CheckPointManager.Instance.avaiableCheckPoints
                .Where(x => x != CheckPointManager.EnumCheckPoint.None && x != CheckPointManager.EnumCheckPoint.Store && x != CheckPointManager.EnumCheckPoint.RespawnCheckpoint)
                .ToList();

            available.Sort();

            if (available.Count > 0) {
                OnHoverChange(available[0]);
            }
            GameManager.Instance.IsPlayerMovementEnabled = false;
        } else {
            GameManager.Instance.IsPlayerMovementEnabled = true;
            if (this.currentHoverCheckPoint != CheckPointManager.EnumCheckPoint.None) {
                // Teleport to selected checkpoint
                CheckPointManager.Instance.SetCurrentCheckPoint(this.currentHoverCheckPoint);
                CheckPointManager.Instance.UseCheckpoint();
            }

            ResetAllHoverInteractions();
        }
        Debug.Log("Interacted with Post Sign.");
    }


    private void UpdateCamera() {
        Camera mainCamera = Camera.main;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (this.isInteracted) {
            mainCamera.cullingMask &= ~(1 << playerLayer); // Hide player layer
            this.cameraPostSign.Priority = 1;
        } else {
            mainCamera.cullingMask |= (1 << playerLayer); // Show player layer
            this.cameraPostSign.Priority = -1;
        }
    }

    private IEnumerator Start() {

        ResetAllHoverInteractions();
        UpdateSignVisibility();
        UpdateCamera();
        return null;
    }

    private void OnEnable() {
        CheckPointManager.Instance.OnAvaiableCheckPointAdded += UpdateSignVisibility;
        this.moveActionReference.action.performed += OnMoveAction;
    }

    private void OnMoveAction(InputAction.CallbackContext context) {
        if (!this.isInteracted) return; // only process move input when interacted
        if (context.performed) {
            Vector2 moveInput = context.ReadValue<Vector2>();

            // Check if moving left or right
            if (moveInput.x != 0) {
                this.isInteracted = !this.isInteracted;
                UpdateCamera();

                if (this.isInteracted) {
                    OnHoverChange(CheckPointManager.EnumCheckPoint.DawnCheckPoint);
                    GameManager.Instance.IsPlayerMovementEnabled = false;
                } else {
                    GameManager.Instance.IsPlayerMovementEnabled = true;
                    ResetAllHoverInteractions();
                }
            } else if (moveInput.y != 0) {

                CheckPointManager.EnumCheckPoint nextTargetCheckPoint = this.currentHoverCheckPoint;

                List<CheckPointManager.EnumCheckPoint> available = CheckPointManager.Instance.avaiableCheckPoints
                    .Where(x => x != CheckPointManager.EnumCheckPoint.None && x != CheckPointManager.EnumCheckPoint.Store && x != CheckPointManager.EnumCheckPoint.RespawnCheckpoint)
                    .ToList();

                available.Sort();

                if (available.Count() <= 1) {
                    // only one available, no change
                    return;
                }
                int availableIndex = available.IndexOf(this.currentHoverCheckPoint);


                if (moveInput.y > 0) {
                    // Moving Up

                    if (availableIndex == 0) {
                        // wrap to last
                        nextTargetCheckPoint = available[available.Count - 1];
                    } else {
                        // go to previous
                        nextTargetCheckPoint = available[availableIndex - 1];
                    }
                } else if (moveInput.y < 0) {
                    // Moving Down

                    if (availableIndex == available.IndexOf(available[available.Count - 1])) {
                        // Is the last elment, wrap to first
                        nextTargetCheckPoint = available[0];
                    } else {
                        // wrap to next
                        nextTargetCheckPoint = available[availableIndex + 1];
                    }
                }

                if (nextTargetCheckPoint == this.currentHoverCheckPoint) return; // no change

                Debug.Log("Changing Hover from " + this.currentHoverCheckPoint.ToString() + " to " + nextTargetCheckPoint.ToString());
                OnHoverChange(nextTargetCheckPoint);
            }
        }
    }



    private void OnDisable() {
        CheckPointManager.Instance.OnAvaiableCheckPointAdded -= UpdateSignVisibility;
    }

    private void UpdateSignVisibility() {
        // update variables based on available checkpoints
        this.is200Enabled = CheckPointManager.Instance.avaiableCheckPoints.Contains(CheckPointManager.EnumCheckPoint.DawnCheckPoint);
        this.is1KmEnabled = CheckPointManager.Instance.avaiableCheckPoints.Contains(CheckPointManager.EnumCheckPoint.TwilightCheckPoint);
        this.is4KmEnabled = CheckPointManager.Instance.avaiableCheckPoints.Contains(CheckPointManager.EnumCheckPoint.LeftMidnightCheckPoint);
        this.is5KmEnabled = CheckPointManager.Instance.avaiableCheckPoints.Contains(CheckPointManager.EnumCheckPoint.MiddleMidnightCheckPoint);
        this.is6KmEnabled = CheckPointManager.Instance.avaiableCheckPoints.Contains(CheckPointManager.EnumCheckPoint.RightMidnightCheckPoint);

        // set sign visibility based on variables
        //this.TwoHundredMeterSign.gameObject.SetActive(this.is200Enabled);
        //this.km1Sign.gameObject.SetActive(this.is1KmEnabled);
        //this.km4Sign.gameObject.SetActive(this.is4KmEnabled);
        //this.km5Sign.gameObject.SetActive(this.is5KmEnabled);
        //this.km6Sign.gameObject.SetActive(this.is6KmEnabled);

        // set flower visibility based on variables
        this.TwoHundredFlower.gameObject.SetActive(this.is200Enabled);
        this.OneKmFlower.gameObject.SetActive(this.is1KmEnabled);
        this.FourKmFlower.gameObject.SetActive(this.is4KmEnabled);
        this.FiveKmFlower.gameObject.SetActive(this.is5KmEnabled);
        this.SixKmFlower.gameObject.SetActive(this.is6KmEnabled);
    }

    private void OnHoverChange(CheckPointManager.EnumCheckPoint enumCheckPoint) {

        if (this.currentHoverCheckPoint != CheckPointManager.EnumCheckPoint.None) {
            // reset previous hover interaction
            ResetHoverInteraction(this.currentHoverCheckPoint);
        }

        switch (enumCheckPoint) {
            case CheckPointManager.EnumCheckPoint.DawnCheckPoint:
                TwoHundredFlower.localScale = new Vector3(flowerScaleHoverValue, flowerScaleHoverValue, flowerScaleHoverValue);
                TwoHundredLight.gameObject.SetActive(true);
                break;
            case CheckPointManager.EnumCheckPoint.TwilightCheckPoint:
                OneKmFlower.localScale = new Vector3(flowerScaleHoverValue, flowerScaleHoverValue, flowerScaleHoverValue);
                OneKmLight.gameObject.SetActive(true);
                break;
            case CheckPointManager.EnumCheckPoint.LeftMidnightCheckPoint:
                FourKmFlower.localScale = new Vector3(flowerScaleHoverValue, flowerScaleHoverValue, flowerScaleHoverValue);
                FourKmLight.gameObject.SetActive(true);
                break;
            case CheckPointManager.EnumCheckPoint.MiddleMidnightCheckPoint:
                FiveKmFlower.localScale = new Vector3(flowerScaleHoverValue, flowerScaleHoverValue, flowerScaleHoverValue);
                FiveKmLight.gameObject.SetActive(true);
                break;
            case CheckPointManager.EnumCheckPoint.RightMidnightCheckPoint:
                SixKmFlower.localScale = new Vector3(flowerScaleHoverValue, flowerScaleHoverValue, flowerScaleHoverValue);
                SixKmLight.gameObject.SetActive(true);
                break;
        }

        this.currentHoverCheckPoint = enumCheckPoint;
    }

    private void ResetHoverInteraction(CheckPointManager.EnumCheckPoint enumCheckPoint) {

        switch (enumCheckPoint) {
            case CheckPointManager.EnumCheckPoint.DawnCheckPoint:
                TwoHundredFlower.localScale = new Vector3(flowerScaleDefaultValue, flowerScaleDefaultValue, flowerScaleDefaultValue);
                TwoHundredLight.gameObject.SetActive(false);
                break;
            case CheckPointManager.EnumCheckPoint.TwilightCheckPoint:
                OneKmFlower.localScale = new Vector3(flowerScaleDefaultValue, flowerScaleDefaultValue, flowerScaleDefaultValue);
                OneKmLight.gameObject.SetActive(false);
                break;
            case CheckPointManager.EnumCheckPoint.LeftMidnightCheckPoint:
                FourKmFlower.localScale = new Vector3(flowerScaleDefaultValue, flowerScaleDefaultValue, flowerScaleDefaultValue);
                FourKmLight.gameObject.SetActive(false);
                break;
            case CheckPointManager.EnumCheckPoint.MiddleMidnightCheckPoint:
                FiveKmFlower.localScale = new Vector3(flowerScaleDefaultValue, flowerScaleDefaultValue, flowerScaleDefaultValue);
                FiveKmLight.gameObject.SetActive(false);
                break;
            case CheckPointManager.EnumCheckPoint.RightMidnightCheckPoint:
                SixKmFlower.localScale = new Vector3(flowerScaleDefaultValue, flowerScaleDefaultValue, flowerScaleDefaultValue);
                SixKmLight.gameObject.SetActive(false);
                break;
        }
    }

    private void ResetAllHoverInteractions() {
        ResetHoverInteraction(CheckPointManager.EnumCheckPoint.DawnCheckPoint);
        ResetHoverInteraction(CheckPointManager.EnumCheckPoint.TwilightCheckPoint);
        ResetHoverInteraction(CheckPointManager.EnumCheckPoint.LeftMidnightCheckPoint);
        ResetHoverInteraction(CheckPointManager.EnumCheckPoint.MiddleMidnightCheckPoint);
        ResetHoverInteraction(CheckPointManager.EnumCheckPoint.RightMidnightCheckPoint);
        this.currentHoverCheckPoint = CheckPointManager.EnumCheckPoint.None;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            this.isPlayerInRange = true;

            // shortly fade in the prompt
            StartCoroutine(TransitionController.Instance.FadeImageInCoroutine(this.promtImage, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerHead")) {
            this.isPlayerInRange = false;

            // shortly fade out the prompt
            StartCoroutine(TransitionController.Instance.FadeImageOutCoroutine(this.promtImage, 0.5f));
        }
    }
}
