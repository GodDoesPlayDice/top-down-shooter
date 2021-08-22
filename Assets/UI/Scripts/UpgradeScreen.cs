using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UpgradeScreen : MonoBehaviour
{
    public MainUIController manager;
    public VisualTreeAsset upgradeElement;
    //public FloatVariable moneyCount;

    public ProgressionManager progressionManager;
    public ProgressionHolder progressionHolder;

    public LocalizationTableHolder localizer;

    private VisualElement rootEl;
    private ScrollView scrollEl;

    //private AbstractUpgrade[] equipped = new AbstractUpgrade[2];
    private int equippedCount;
    private const int MAX_WEAPON_COUNT = 2;

    //private AbstractUpgrade[] upgrades;

    
    private void Awake()
    {
        //upgrades = Resources.LoadAll<AbstractUpgrade>("UpgradesSO");
        progressionManager.Init();
        progressionManager.LoadFromSaveFile();
        equippedCount = progressionHolder.GetSelected().Count();
    }

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        scrollEl = rootEl.Q<ScrollView>("UpgradesScroll");
        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => manager.ToTitleScreen());
        rootEl.Q("StartGame").RegisterCallback<ClickEvent>(e => manager.StartGame());

#if UNITY_STANDALONE || UNITY_EDITOR
        scrollEl.horizontalScrollerVisibility = ScrollerVisibility.Auto;
#endif

        Draw();
    }

    private void Draw()
    {
        ClearUpgradeScroll();
        rootEl.Q<Label>("MoneyLabel").text = progressionHolder.moneyCount.ToString();
        FillUpgradeScroll();
    }

    private void ClearUpgradeScroll()
    {
        // TODO: clear callback on buttons?
        scrollEl.Clear();
    }

    private void FillUpgradeScroll()
    {
        var sortedUIList = CreateListForUI().OrderBy(e => e.isRoot ? 0 : 1);
        //Debug.Log("SortedLength " + sortedUIList.Count());
        foreach (AbstractUpgrade upgrade in sortedUIList)
        {
            var upgradeEl = BuildUpgradeEl(upgrade);
            scrollEl.Add(upgradeEl);
        }
    }

    // TODO: Move to separate class/interface UpgradesUIProvider
    private List<AbstractUpgrade> CreateListForUI()
    {
        List<AbstractUpgrade> result = new List<AbstractUpgrade>();
        foreach(AbstractUpgrade upgrade in progressionHolder.GetAllUpgrades().Where(e => e.isRoot))
        {
            //Debug.Log("Root: " + upgrade);
            result.Add(getFirstNotBought(upgrade));
        }

        return result;
    }

    private AbstractUpgrade getFirstNotBought(AbstractUpgrade first) {
        AbstractUpgrade current = first;
        while (progressionHolder.IsPurchased(current) && current.children.Length > 0)
        {
            current = current.children[0];
        }
        return current;
    }

    private UpgradeVisualElement BuildUpgradeEl(AbstractUpgrade upgrade)
    {
        var tree = upgradeElement.CloneTree();
        var upgradeEl = tree.Q<UpgradeVisualElement>();

        upgradeEl.SetLocalizer(localizer);

        //upgradeEl.SetIcon(upgrade.image);
        upgradeEl.SetName(upgrade.upgradeName);
        upgradeEl.SetDescription(upgrade.description);
        upgradeEl.SetCost(upgrade.cost);
        upgradeEl.SetIcon(upgrade.image);

        upgradeEl.SetUpgrade(upgrade.upgradeType == UpgradeType.WEAPON_UPGRADE);
        if (upgrade.upgradeType == UpgradeType.WEAPON_UPGRADE)
        {
            int currentTier = ((WeaponUpgrade)upgrade).tier - (progressionHolder.IsPurchased(upgrade) ? 0 : 1);
            int maxTier = GetMaxTier((WeaponUpgrade)upgrade);
            upgradeEl.SetProgressValue(currentTier, maxTier);
        }
        upgradeEl.SetUpgradeButtonCallback(e => TryToPurchase(upgrade));

        bool selectable = IsSelectable(upgrade);
        upgradeEl.SetSelectable(selectable);
        if (selectable) {
            upgradeEl.SetSelected(IsSelected(upgrade));
            upgradeEl.SetEquipButtonCallback(e => ToggleEquip(upgrade));
        }
        return upgradeEl;
    }

    private bool IsSelectable(AbstractUpgrade upgrade)
    {
        return IsSelected(upgrade) || (equippedCount < MAX_WEAPON_COUNT && upgrade.upgradeType != UpgradeType.WEAPON_UNLOCK && progressionHolder.IsSelectable(upgrade));
    }

    private bool IsSelected(AbstractUpgrade upgrade)
    {
        return progressionHolder.IsSelectedRoot(upgrade);
    }

    private int GetMaxTier(WeaponUpgrade upgrade)
    {
        WeaponUpgrade current = upgrade;
        while (current.children.Length > 0)
        {
            current = (WeaponUpgrade) current.children[0];
        }
        return current.tier;
    }

    private void TryToPurchase(AbstractUpgrade upgrade)
    {
        Purchase(upgrade);
    }

    private bool Purchase(AbstractUpgrade upgrade)
    {
        bool result = false;
        if (!progressionHolder.IsPurchased(upgrade) && progressionHolder.moneyCount >= upgrade.cost)
        {
            progressionHolder.AddPurchasedUpgrade(upgrade);
            progressionHolder.moneyCount -= upgrade.cost;
            progressionManager.WriteToSaveFile();
            Draw();
            result = true;
        }
        return result;
    }

    private void ToggleEquip(AbstractUpgrade upgrade)
    {
        if (progressionHolder.IsSelectedRoot(upgrade))
        {
            equippedCount--;
            progressionHolder.RemoveSelectedByUpgrade(upgrade);
        } else if (equippedCount < MAX_WEAPON_COUNT)
        {
            equippedCount++;
            progressionHolder.AddSelectedByUpgrade(upgrade);
        } else
        {
            return;
        }
        progressionManager.WriteToSaveFile();
        Draw();

        /*
        if (equipped.Contains(upgrade))
        {
            if (equipped[0] == upgrade)
            {
                equipped[0] = equipped[1];
                
            }
            equipped[1] = null;
        } else if (equipped[0] == null)
        {
            equipped[0] = upgrade;
        } else if (equipped[1] == null)
        {
            equipped[1] = upgrade;
        } else
        {
            return;
        }
        
        */
    }
}
