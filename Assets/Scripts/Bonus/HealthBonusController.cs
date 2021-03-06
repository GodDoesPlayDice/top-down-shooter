using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBonusController : AbstractBonusController
{
    public float amount = 20f;

    private PlayerController playerController;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    public override bool OnPickUp()
    {
        return playerController.AddEnergy(amount);
    }

    public override bool CanPickUp()
    {
        return playerController.CanAddEnergy();
    }

    public override string GetPickupText()
    {
        string localizedString = localizationTableHolder.currentTable.GetEntry(pickupString).GetLocalizedString();
        return "+" + amount + " " + localizedString;
    }
}
