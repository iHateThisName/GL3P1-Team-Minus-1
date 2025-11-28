using Assets.Scripts.Singleton;
using UnityEngine;

public class AudioManager : PersistenSingleton<AudioManager>
{
    public AudioSource underWaterSound;
    public AudioSource endgameSound;

    public AudioSource breatheInSound;
    public AudioSource breatheOutSound;

    public AudioSource bubbleSound;

    public AudioSource shopEnterAndExit;
    public AudioSource shopPurchaseSound;
    public AudioSource sellTreasureSound;
}
