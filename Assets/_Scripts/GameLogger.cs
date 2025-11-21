using System.Threading.Tasks;
using UnityEngine;
using Xasu.HighLevel;

public class GameLogger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await Task.Yield();
        CompletableTracker.Instance.Initialized("Breathe In", CompletableTracker.CompletableType.Game);
    }

    private void OnDestroy()
    {
        CompletableTracker.Instance.Completed("Breathe In", CompletableTracker.CompletableType.Game);
    }
}
