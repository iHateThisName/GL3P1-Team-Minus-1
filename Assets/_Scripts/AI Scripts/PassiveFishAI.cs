using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveFishAI : BaseAI {

    [Header("Fish")]
    [SerializeField] private List<Transform> potentialTargets = new List<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (this.potentialTargets.Count == 0) {
            Debug.LogWarning("No potential targets assigned to PassiveFishAI.");
        }
        StartCoroutine(ChangeTarget());
    }

    private IEnumerator ChangeTarget() {
        int i = 0;

        while (true) {
            // Set current target
            SetTarget(potentialTargets[i]);

            yield return new WaitForSecondsRealtime(3f);
            // Wait until the fish reaches this target
            while (!IsReachedEndOfPath) {
                yield return new WaitForSecondsRealtime(0.5f); // check every half second
            }

            // Move to next target (wrap around)
            i = (i + 1) % potentialTargets.Count;
        }
    }
}
