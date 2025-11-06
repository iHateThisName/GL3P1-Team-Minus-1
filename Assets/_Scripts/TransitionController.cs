using Assets.Scripts.Singleton;
using System.Collections;
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
}
