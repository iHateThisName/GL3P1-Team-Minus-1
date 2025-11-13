using Assets.Scripts.Singleton;
using System.Collections;
using TMPro;
using UnityEngine;

public class TransitionController : Singleton<TransitionController> {

    [SerializeField] private Animator transitionAnimator;
    public bool isFadedOut { private set; get; } = false;

    private readonly struct AnimatorParameters {
        public const string FadeOutTrigger = "StartFadeOutTransitionTrigger";
        public const string FadeInTrigger = "StartFadeInTransitionTrigger";
        public const string FadeOutState = "FadeOut";
        public const string FadeInState = "FadeIn";
    }
    [ContextMenu("Start Fade Transition")]
    public void StartFadeTransition() {
        StartCoroutine(StartFadeTransitionCoroutine());
    }

    public IEnumerator FadeOutCoroutine() {
        transitionAnimator.SetTrigger(AnimatorParameters.FadeOutTrigger);
        yield return StartCoroutine(WaitForAnimationToFinish(AnimatorParameters.FadeOutState));
    }
    public IEnumerator FadeInCoroutine() {
        transitionAnimator.SetTrigger(AnimatorParameters.FadeInTrigger);
        yield return StartCoroutine(WaitForAnimationToFinish(AnimatorParameters.FadeInState));
    }

    public void FadeIn() => transitionAnimator.SetTrigger(AnimatorParameters.FadeInTrigger);
    private IEnumerator StartFadeTransitionCoroutine() {
        if (transitionAnimator == null) {
            Debug.LogError("Transition Animator is not assigned in TransitionController.");
            yield break;
        }

        // Start fade out
        transitionAnimator.SetTrigger(AnimatorParameters.FadeOutTrigger);
        yield return StartCoroutine(WaitForAnimationToFinish(AnimatorParameters.FadeOutState));

        // Fade out complete, wait briefly before fading in
        yield return new WaitForSecondsRealtime(0.5f);

        // Start fade in
        transitionAnimator.SetTrigger(AnimatorParameters.FadeInTrigger);
        yield return WaitForAnimationToFinish(AnimatorParameters.FadeInState);
    }

    private IEnumerator WaitForAnimationToFinish(string stateName) {
        if (transitionAnimator == null) {
            Debug.LogError("Transition Animator is not assigned in TransitionController.");
            yield break;
        }

        yield return new WaitUntil(() => {
            AnimatorStateInfo stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f;
        });
    }

    public IEnumerator FadeTextInCoroutine(TMP_Text text, float seconds) {
        // Fade in the text over v seconds
        text.alpha = text.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < seconds) {
            text.alpha = Mathf.Clamp01(elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeTextOutCoroutine(TMP_Text text, float seconds) {
        // Fade out the text over v seconds
        text.alpha = text.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < seconds) {
            text.alpha = 1 - Mathf.Clamp01(elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
