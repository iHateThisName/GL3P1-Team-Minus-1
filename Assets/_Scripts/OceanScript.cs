using System.Collections.Generic;
using UnityEngine;

public class OceanScript : MonoBehaviour {

    [SerializeField] private GameObject breatheSlider;

    [SerializeField] private BreathingScript breathingScript;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerMovement.isUnderWater = true;
            breathingScript.enabled = true;
            breatheSlider.SetActive(true);

            List<ShopItemData> upgrades = ShopItemLookUp.Instance.GetAllPlayerUppgrades();

            foreach (ShopItemData item in upgrades)
            {
                if(item.ItemType == EnumItemSprite.suitUppgradeTier1)
                {
                    breathingScript.intendedOxygen = 1000f;
                    breathingScript.oxygenAmount = breathingScript.intendedOxygen;
                }
            }
            breathingScript.oxygenSlider.maxValue = breathingScript.intendedOxygen;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.PlayerMovement.isUnderWater = false;
            breathingScript.DisableBreathing();
            breathingScript.enabled = false;
            breatheSlider.SetActive(false);
        }
    }
}
