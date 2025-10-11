using Assets.Scripts.Singleton;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public PlayerMovement PlayerMovement;
    public Transform PlayerInteractTransform;
}
