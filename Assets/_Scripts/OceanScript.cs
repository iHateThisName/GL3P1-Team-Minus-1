using System.Collections.Generic;
using UnityEngine;

public class OceanScript : MonoBehaviour {

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private BreathingScript breatheScript;
    [SerializeField] private GameObject breatheSlider;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.isUnderWater = true;
            this.breatheScript.enabled = true;
            breatheSlider.SetActive(true);

            List<ShopItemData> upgrades = ShopItemLookUp.Instance.GetAllPlayerUppgrades();

            foreach (ShopItemData item in upgrades)
            {
                if(item.ItemType == EnumItemSprite.suitUppgradeTier1)
                {
                    breatheScript.intendedOxygen = 1000f;
                    breatheScript.oxygenAmount = breatheScript.intendedOxygen;
                    breatheScript.oxygenSlider.maxValue = breatheScript.intendedOxygen;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.playerMovement.isUnderWater = false;
            this.breatheScript.DisableBreathing();
            this.breatheScript.enabled = false;
            breatheSlider.SetActive(false);
        }
    }
}
