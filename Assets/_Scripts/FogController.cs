using Assets.Scripts.Singleton;
using UnityEngine;
using UnityEngine.VFX;

public class FogController : Singleton<FogController> {
    [Header("References")]
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private Transform player;

    [SerializeField] private bool playEffect = false;

    private const string fogPositionProperty = "FogPosition";
    private const string fogClearUpCollider = "ColliderPos";
    private const string fogPlayBoolProperty = "Play";

    private void Start() {
        vfx.SetBool(fogPlayBoolProperty, playEffect);
    }

    [ContextMenu("Toggle Fog Spawn Effect")]
    public void ToggleFogEffect() {
        playEffect = !playEffect;
        vfx.SetBool(fogPlayBoolProperty, playEffect);
    }

    public void DisableFogEffect() {
        playEffect = false;
        vfx.SetBool(fogPlayBoolProperty, playEffect);
    }

    public void EnableFogEffect() {
        playEffect = true;
        vfx.SetBool(fogPlayBoolProperty, playEffect);
    }

    private void FixedUpdate() {
        if (vfx == null || player == null) return;
        vfx.SetVector3(fogPositionProperty, player.position);
        vfx.SetVector3(fogClearUpCollider, player.position);
    }
}
