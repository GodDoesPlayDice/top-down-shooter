using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProgressionManager : MonoBehaviour, Loadable
{
    public ProgressionHolder progressionHolder;

    private SaveLoadController saveLoadController;
    private JsonConverter jsonConverter;

    public int _loadCost = 5;
    public int loadCost => _loadCost;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (saveLoadController == null)
        {
            jsonConverter = new JsonConverter();
            saveLoadController = new SaveLoadController(jsonConverter);
        }
    }

    public void LoadFromSaveFile()
    {
        var saveData = saveLoadController.LoadProgression();
        progressionHolder.moneyCount = saveData.money;
        progressionHolder.SetPurchasedUpgrades(saveData.upgrades);
        progressionHolder.SetSelectedUpgrades(saveData.selected);
        progressionHolder.topScore = saveData.topScore;
        progressionHolder.SetPurchasedPlayerUpgrades(saveData.playerUpgrades);
        progressionHolder.exp = saveData.exp;
    }

    public void WriteToSaveFile()
    {
        Debug.Log("ProgressionManager, topScore: " + progressionHolder.topScore);
        saveLoadController.SaveProgression(progressionHolder.moneyCount,
            progressionHolder.GetPurchasedUpgradesId(),
            progressionHolder.GetSelectedIds(),
            progressionHolder.topScore,
            progressionHolder.GetPurchasedPlayerUpgradesId(),
            progressionHolder.exp);
    }

    public void Load(Action onLoad)
    {
        LoadFromSaveFile();
        onLoad();
    }

    public void DeleteSaveData()
    {
        saveLoadController.DeleteSaveFile();
        progressionHolder.ClearAll();
        LoadFromSaveFile();
    }
}
