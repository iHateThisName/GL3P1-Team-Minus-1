using UnityEngine;

public class UnderwaterFogController : MonoBehaviour {
    [SerializeField] private Material fogMaterial;
    [SerializeField] private Transform player;

    [SerializeField] private bool isUnderWater;

    private void FixedUpdate() {
        if (!fogMaterial || !player) return;
        isUnderWater = GameManager.Instance.PlayerMovement.isUnderWater;

        fogMaterial.SetVector("PlayerPosition", player.position);
        fogMaterial.SetFloat("IsUnderwater", this.isUnderWater ? 1 : 0);
    }
}
