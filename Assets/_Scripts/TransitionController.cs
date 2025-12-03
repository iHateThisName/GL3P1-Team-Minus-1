using Assets.Scripts.Singleton;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : Singleton<TransitionController> {

    [SerializeField] private Animator transitionAnimator;
    public bool isFadedOut { private set; get; } = false;

    public readonly struct AnimatorParameters {
        public const string FadeOutTrigger = "StartFadeOutTransitionTrigger";
        public const string FadeInTrigger = "StartFadeInTransitionTrigger";
        public const string FadeInBlackAndWhiteTrigger = "BlackWhiteFadeInTrigger";
        public const string FadeOutBlackAndWhiteTrigger = "BlackWhiteFadeOutTrigger";

        public const string FadeOutState = "FadeOut";
        public const string FadeInState = "FadeIn";
        public const string BlackWhiteFadeInState = "BlackWhiteFadeIn";
        public const string BlackWhiteFadeOutState = "BlackWhiteFadeOut";
    }
    [ContextMenu("Start Fade Transition")]
    public void StartFadeTransition() {
        StartCoroutine(StartFadeTransitionCoroutine());
    }

    public IEnumerator FadeOutCoroutine() {
        transitionAnimator.SetTrigger(AnimatorParameters.FadeOutTrigger);
        yield return StartCoroutine(WaitForAnimationToFinish(AnimatorParameters.FadeOutState));
    }

    private IEnumerator TransitionCoroutine(string trigger, string state) {
        // the parameters should be a field value from AnimatorParameters
        transitionAnimator.SetTrigger(trigger);
        yield return StartCoroutine(WaitForAnimationToFinish(trigger));
    }

    public void RollCredits() => StartCoroutine(RollCreditsCorutine());

    private IEnumerator RollCreditsCorutine() {
        transitionAnimator.SetTrigger(AnimatorParameters.FadeOutBlackAndWhiteTrigger);
        yield return new WaitForSecondsRealtime(3f);
        GameSceneManager.Instance.UnloadeScene(EnumScene.GameScene);
        GameSceneManager.Instance.LoadScene(EnumScene.MainMenu);
        yield return new WaitForSecondsRealtime(3f);
        this.transitionAnimator.SetTrigger(AnimatorParameters.FadeInBlackAndWhiteTrigger);

        //yield return StartCoroutine(TransitionCoroutine(AnimatorParameters.FadeOutBlackAndWhiteTrigger, AnimatorParameters.BlackWhiteFadeOutState));
        //GameSceneManager.Instance.UnloadeScene(EnumScene.GameScene);
        //yield return StartCoroutine(GameSceneManager.Instance.LoadSceneCoroutine(EnumScene.GameScene, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        //yield return StartCoroutine(TransitionCoroutine(AnimatorParameters.FadeInBlackAndWhiteTrigger, AnimatorParameters.BlackWhiteFadeInState));
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
        float elapsedTime = 0f;

        while (elapsedTime < seconds) {
            text.alpha = Mathf.Clamp01(elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeImageInCoroutine(Image image, float seconds) {
        // Fade in the text over v seconds
        float elapsedTime = 0f;

        while (elapsedTime < seconds) {
            float alpha = Mathf.Clamp01(elapsedTime / seconds);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
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

    public IEnumerator FadeImageOutCoroutine(Image image, float seconds) {
        // Fade out the text over v seconds
        float startAlpha = image.color.a;
        float elapsedTime = 0f;
        while (elapsedTime < seconds) {
            float alpha = startAlpha * (1 - Mathf.Clamp01(elapsedTime / seconds));
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
    }
}
