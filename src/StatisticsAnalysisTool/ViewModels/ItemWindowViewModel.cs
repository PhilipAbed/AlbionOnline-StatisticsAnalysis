﻿using log4net;
using StatisticsAnalysisTool.Common;
using StatisticsAnalysisTool.Common.UserSettings;
using StatisticsAnalysisTool.Exceptions;
using StatisticsAnalysisTool.Models;
using StatisticsAnalysisTool.Models.ItemsJsonModel;
using StatisticsAnalysisTool.Models.ItemWindowModel;
using StatisticsAnalysisTool.Models.TranslationModel;
using StatisticsAnalysisTool.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace StatisticsAnalysisTool.ViewModels;

public class ItemWindowViewModel : INotifyPropertyChanged
{
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly ItemWindow _itemWindow;
    private Item _item;
    private string _titleName;
    private string _itemTierLevel;
    private BitmapImage _icon;
    private Visibility _errorBarVisibility;
    private ItemWindowTranslation _translation = new();
    private bool _refreshSpin;
    private bool _isAutoUpdateActive;
    private readonly Timer _timer = new();
    private double _taskProgressbarMinimum;
    private double _taskProgressbarMaximum = 100;
    private double _taskProgressbarValue;
    private bool _isTaskProgressbarIndeterminate;
    private XmlLanguage _itemListViewLanguage = XmlLanguage.GetLanguage(LanguageController.CurrentCultureInfo.ToString());
    private double _refreshRateInMilliseconds = 10;
    private RequiredJournal _requiredJournal;
    private EssentialCraftingValuesTemplate _essentialCraftingValues;
    private ObservableCollection<RequiredResource> _requiredResources = new();
    private Visibility _requiredJournalVisibility = Visibility.Collapsed;
    private Visibility _craftingTabVisibility = Visibility.Collapsed;
    private string _craftingNotes;
    private List<MarketResponse> _currentItemPrices = new();
    private ExtraItemInformation _extraItemInformation = new();
    private string _errorBarText;
    private List<QualityStruct> _qualities = new();
    private QualityStruct _qualitiesSelection;
    private ObservableCollection<CityFilterObject> _locationFilters = new();
    private ObservableCollection<ItemPricesObject> _mainTabItemPrices = new();

    private CraftingCalculation _craftingCalculation = new()
    {
        AuctionsHouseTax = 0.0d,
        CraftingTax = 0.0d,
        PossibleItemCrafting = 0.0d,
        SetupFee = 0.0d,
        TotalCosts = 0.0,
        TotalJournalCosts = 0.0d,
        TotalItemSells = 0.0d,
        TotalJournalSells = 0.0d,
        TotalResourceCosts = 0.0d,
        GrandTotal = 0.0d
    };

    public enum Error
    {
        NoPrices,
        NoItemInfo,
        GeneralError,
        ToManyRequests
    }

    public ItemWindowViewModel(ItemWindow itemWindow, Item item)
    {
        _itemWindow = itemWindow;

        ErrorBarVisibility = Visibility.Hidden;
        //SetDefaultQualityIfNoOneChecked();

        Item = item;

        Translation = new ItemWindowTranslation();
        _ = InitAsync(item);

        ItemListViewLanguage = XmlLanguage.GetLanguage(LanguageController.CurrentCultureInfo.ToString());
    }

    #region Inits

    private async Task InitAsync(Item item)
    {
        IsTaskProgressbarIndeterminate = true;
        Icon = null;
        TitleName = "-";
        ItemTierLevel = string.Empty;

        Item = item;

        if (item == null)
        {
            SetErrorValues(Error.NoItemInfo);
            return;
        }

        InitCityFiltering();
        InitQualityFiltering();
        InitExtraItemInformation();
        await InitCraftingTabAsync();

        if (Application.Current.Dispatcher == null)
        {
            SetErrorValues(Error.GeneralError);
            return;
        }

        ChangeHeaderValues(item);
        ChangeWindowValuesAsync(item);

        await InitTimerAsync();
        IsAutoUpdateActive = true;

        IsTaskProgressbarIndeterminate = false;
    }

    private void InitCityFiltering()
    {
        var locationFilters = new List<CityFilterObject>
        {
            new (MarketLocation.ThetfordMarket, Locations.GetParameterName(Location.Thetford), true),
            new (MarketLocation.FortSterlingMarket, Locations.GetParameterName(Location.FortSterling), true),
            new (MarketLocation.LymhurstMarket, Locations.GetParameterName(Location.Lymhurst), true),
            new (MarketLocation.BridgewatchMarket, Locations.GetParameterName(Location.Bridgewatch), true),
            new (MarketLocation.MartlockMarket, Locations.GetParameterName(Location.Martlock), true),
            new (MarketLocation.BrecilienMarket, Locations.GetParameterName(Location.Brecilien), true),
            new (MarketLocation.ArthursRest, Locations.GetParameterName(Location.ArthursRest), true),
            new (MarketLocation.MerlynsRest, Locations.GetParameterName(Location.MerlynsRest), true),
            new (MarketLocation.MorganasRest, Locations.GetParameterName(Location.MorganasRest), true),
            new (MarketLocation.BlackMarket, Locations.GetParameterName(Location.BlackMarket), true),
            new (MarketLocation.ForestCross, Locations.GetParameterName(Location.ForestCross), true),
            new (MarketLocation.SwampCross, Locations.GetParameterName(Location.SwampCross), true),
            new (MarketLocation.SteppeCross, Locations.GetParameterName(Location.SteppeCross), true),
            new (MarketLocation.HighlandCross, Locations.GetParameterName(Location.HighlandCross), true),
            new (MarketLocation.MountainCross, Locations.GetParameterName(Location.MountainCross), true)
        };

        LocationFilters = new ObservableCollection<CityFilterObject>(locationFilters.OrderBy(x => x.Name));

        AddLocationFiltersEvents();
    }

    private void InitQualityFiltering()
    {
        Qualities.Add(new QualityStruct() { Name = LanguageController.Translation("NORMAL"), Quality = 1 });
        Qualities.Add(new QualityStruct() { Name = LanguageController.Translation("GOOD"), Quality = 2 });
        Qualities.Add(new QualityStruct() { Name = LanguageController.Translation("OUTSTANDING"), Quality = 3 });
        Qualities.Add(new QualityStruct() { Name = LanguageController.Translation("EXCELLENT"), Quality = 4 });
        Qualities.Add(new QualityStruct() { Name = LanguageController.Translation("MASTERPIECE"), Quality = 5 });

        if (Qualities != null)
        {
            QualitiesSelection = Qualities.FirstOrDefault();
        }
    }

    private void InitExtraItemInformation()
    {
        switch (Item?.FullItemInformation)
        {
            case Weapon weapon:
                ExtraItemInformation.ShopCategory = weapon.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = weapon.ShopSubCategory1;
                ExtraItemInformation.CanBeOvercharged = weapon.CanBeOvercharged.SetYesOrNo();
                ExtraItemInformation.Durability = weapon.Durability;
                ExtraItemInformation.ShowInMarketPlace = weapon.ShowInMarketPlace.SetYesOrNo();
                ExtraItemInformation.Weight = weapon.Weight;
                break;
            case HideoutItem hideoutItem:
                ExtraItemInformation.ShopCategory = hideoutItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = hideoutItem.ShopSubCategory1;
                ExtraItemInformation.Weight = hideoutItem.Weight;
                break;
            case FarmableItem farmableItem:
                ExtraItemInformation.ShopCategory = farmableItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = farmableItem.ShopSubCategory1;
                ExtraItemInformation.ShowInMarketPlace = farmableItem.ShowInMarketPlace.SetYesOrNo();
                ExtraItemInformation.Weight = farmableItem.Weight;
                break;
            case SimpleItem simpleItem:
                ExtraItemInformation.ShopCategory = simpleItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = simpleItem.ShopSubCategory1;
                ExtraItemInformation.Weight = simpleItem.Weight;
                break;
            case ConsumableItem consumableItem:
                ExtraItemInformation.ShopCategory = consumableItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = consumableItem.ShopSubCategory1;
                ExtraItemInformation.Weight = consumableItem.Weight;
                break;
            case ConsumableFromInventoryItem consumableFromInventoryItem:
                ExtraItemInformation.ShopCategory = consumableFromInventoryItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = consumableFromInventoryItem.ShopSubCategory1;
                ExtraItemInformation.Weight = consumableFromInventoryItem.Weight;
                break;
            case EquipmentItem equipmentItem:
                ExtraItemInformation.ShopCategory = equipmentItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = equipmentItem.ShopSubCategory1;
                ExtraItemInformation.CanBeOvercharged = equipmentItem.CanBeOvercharged.SetYesOrNo();
                ExtraItemInformation.Durability = equipmentItem.Durability;
                ExtraItemInformation.ShowInMarketPlace = equipmentItem.ShowInMarketPlace.SetYesOrNo();
                ExtraItemInformation.Weight = equipmentItem.Weight;
                break;
            case Mount mount:
                ExtraItemInformation.ShopCategory = mount.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = mount.ShopSubCategory1;
                ExtraItemInformation.Durability = mount.Durability;
                ExtraItemInformation.ShowInMarketPlace = mount.ShowInMarketPlace.SetYesOrNo();
                ExtraItemInformation.Weight = mount.Weight;
                break;
            case FurnitureItem furnitureItem:
                ExtraItemInformation.ShopCategory = furnitureItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = furnitureItem.ShopSubCategory1;
                ExtraItemInformation.Durability = furnitureItem.Durability;
                ExtraItemInformation.ShowInMarketPlace = furnitureItem.ShowInMarketPlace.SetYesOrNo();
                ExtraItemInformation.Weight = furnitureItem.Weight;
                break;
            case JournalItem journalItem:
                ExtraItemInformation.ShopCategory = journalItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = journalItem.ShopSubCategory1;
                ExtraItemInformation.Weight = journalItem.Weight;
                break;
            case LabourerContract labourerContract:
                ExtraItemInformation.ShopCategory = labourerContract.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = labourerContract.ShopSubCategory1;
                ExtraItemInformation.Weight = labourerContract.Weight;
                break;
            case CrystalLeagueItem crystalLeagueItem:
                ExtraItemInformation.ShopCategory = crystalLeagueItem.ShopCategory;
                ExtraItemInformation.ShopSubCategory1 = crystalLeagueItem.ShopSubCategory1;
                ExtraItemInformation.Weight = crystalLeagueItem.Weight;
                break;
        }
    }

    #endregion

    private async void ChangeWindowValuesAsync(Item item)
    {
        var localizedName = ItemController.LocalizedName(item?.LocalizedNames, null, item?.UniqueName);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _itemWindow.Icon = item?.Icon;
            _itemWindow.Title = $"{localizedName} (T{item?.Tier})";
        });
    }

    private void ChangeHeaderValues(Item item)
    {
        var localizedName = ItemController.LocalizedName(item?.LocalizedNames, null, item?.UniqueName);

        Icon = item?.Icon;
        TitleName = localizedName;
        ItemTierLevel = item?.Tier != -1 && item?.Level != -1 ? $"T{item?.Tier}.{item?.Level}" : string.Empty;
    }

    #region Timer

    private async Task InitTimerAsync()
    {
        await UpdateMarketPricesAsync();
        UpdateMainTabItemPrices(null, null);

        _timer.Interval = SettingsController.CurrentSettings.RefreshRate;
        _timer.Elapsed += UpdateInterval;
        _timer.Elapsed += UpdateMarketPricesAsync;
        _timer.Elapsed += UpdateMainTabItemPrices;
    }

    private void UpdateInterval(object sender, EventArgs e)
    {
        if (Math.Abs(_refreshRateInMilliseconds - SettingsController.CurrentSettings.RefreshRate) <= 0)
        {
            return;
        }

        _refreshRateInMilliseconds = SettingsController.CurrentSettings.RefreshRate;
        _timer.Interval = _refreshRateInMilliseconds;
    }

    public void AutoUpdateSwitcher()
    {
        IsAutoUpdateActive = !IsAutoUpdateActive;
    }

    #endregion

    #region Crafting tab

    private async Task InitCraftingTabAsync()
    {
        var areResourcesAvailable = false;

        switch (Item?.FullItemInformation)
        {
            case Weapon weapon when weapon.CraftingRequirements?.FirstOrDefault()?.CraftResource?.Count > 0:
            case EquipmentItem equipmentItem when equipmentItem.CraftingRequirements?.FirstOrDefault()?.CraftResource?.Count > 0:
            case Mount mount when mount.CraftingRequirements?.FirstOrDefault()?.CraftResource?.Count > 0:
            case ConsumableItem consumableItem when consumableItem.CraftingRequirements?.FirstOrDefault()?.CraftResource?.Count > 0:
                areResourcesAvailable = true;
                break;
        }

        if (areResourcesAvailable)
        {
            CraftingTabVisibility = Visibility.Visible;

            EssentialCraftingValues = new EssentialCraftingValuesTemplate(this, CurrentItemPrices, Item.UniqueName);
            SetJournalInfo();
            await SetRequiredResourcesAsync();
            CraftingNotes = CraftingTabController.GetNote(Item.UniqueName);
        }
    }

    private void SetJournalInfo()
    {
        var craftingJournalType = Item?.FullItemInformation switch
        {
            Weapon weapon => CraftingController.GetCraftingJournalItem(Item.Tier, weapon.CraftingJournalType),
            EquipmentItem equipmentItem => CraftingController.GetCraftingJournalItem(Item.Tier, equipmentItem.CraftingJournalType),
            _ => null
        };

        if (craftingJournalType == null)
        {
            return;
        }

        RequiredJournalVisibility = Visibility.Visible;

        var fullItemInformation = ItemController.GetItemByUniqueName(ItemController.GetGeneralJournalName(craftingJournalType.UniqueName))?.FullItemInformation;

        RequiredJournal = new RequiredJournal(this)
        {
            UniqueName = craftingJournalType.UniqueName,
            CostsPerJournal = 0,
            CraftingResourceName = craftingJournalType.LocalizedName,
            Icon = craftingJournalType.Icon,
            Weight = ItemController.GetWeight(fullItemInformation),
            RequiredJournalAmount = CraftingController.GetRequiredJournalAmount(Item, CraftingCalculation.PossibleItemCrafting),
            SellPricePerJournal = 0
        };
    }

    private async Task SetRequiredResourcesAsync()
    {
        var currentItemEnchantmentLevel = Item.Level;
        List<CraftingRequirements> craftingRequirements = null;

        var enchantments = Item?.FullItemInformation switch
        {
            EquipmentItem equipmentItem => equipmentItem.Enchantments,
            ConsumableItem consumableItem => consumableItem.Enchantments,
            _ => null
        };

        var enchantment = enchantments?.Enchantment?.FirstOrDefault(x => x.EnchantmentLevelInteger == currentItemEnchantmentLevel);

        if (enchantment != null)
        {
            craftingRequirements = enchantment.CraftingRequirements;
        }

        if (craftingRequirements == null)
        {
            craftingRequirements = Item?.FullItemInformation switch
            {
                Weapon weapon => weapon.CraftingRequirements,
                EquipmentItem equipmentItem => equipmentItem.CraftingRequirements,
                Mount mount => mount.CraftingRequirements,
                ConsumableItem consumableItem => consumableItem.CraftingRequirements,
                _ => null
            };
        }

        if (craftingRequirements?.FirstOrDefault()?.CraftResource == null)
        {
            return;
        }

        if (int.TryParse(craftingRequirements.FirstOrDefault()?.AmountCrafted, out var amountCrafted))
        {
            EssentialCraftingValues.AmountCrafted = amountCrafted;
        }

        await foreach (var craftResource in craftingRequirements
                           .SelectMany(x => x.CraftResource).ToList().GroupBy(x => x.UniqueName).Select(grp => grp.FirstOrDefault()).ToAsyncEnumerable().ConfigureAwait(false))
        {
            var item = GetSuitableResourceItem(craftResource.UniqueName);
            var craftingQuantity = (long)Math.Round(item?.UniqueName?.ToUpper().Contains("ARTEFACT") ?? false
            ? CraftingCalculation.PossibleItemCrafting
                : EssentialCraftingValues.CraftingItemQuantity, MidpointRounding.ToPositiveInfinity);

            RequiredResources.Add(new RequiredResource(this)
            {
                CraftingResourceName = item?.LocalizedName,
                UniqueName = item?.UniqueName,
                OneProductionAmount = craftResource.Count,
                Icon = item?.Icon,
                ResourceCost = 0,
                Weight = ItemController.GetWeight(item?.FullItemInformation),
                CraftingQuantity = craftingQuantity,
                IsArtifactResource = item?.UniqueName?.ToUpper().Contains("ARTEFACT") ?? false,
                IsTomeOfInsightResource = item?.UniqueName?.ToUpper().Contains("SKILLBOOK_STANDARD") ?? false,
                IsAvalonianEnergy = item?.UniqueName?.ToUpper().Contains("QUESTITEM_TOKEN_AVALON") ?? false
            });
        }
    }

    private Item GetSuitableResourceItem(string uniqueName)
    {
        var suitableUniqueName = $"{uniqueName}_LEVEL{Item.Level}@{Item.Level}";
        return ItemController.GetItemByUniqueName(suitableUniqueName) ?? ItemController.GetItemByUniqueName(uniqueName);
    }

    public void UpdateCraftingCalculationTab()
    {
        if (EssentialCraftingValues == null || CraftingCalculation == null)
        {
            return;
        }

        // PossibleItem crafting
        var possibleItemCrafting = EssentialCraftingValues.CraftingItemQuantity / 100d * EssentialCraftingValues.CraftingBonus * ((EssentialCraftingValues.IsCraftingWithFocus)
            ? ((23.1d / 100d) + 1d) : 1d);

        // Crafting quantity
        if (RequiredResources?.Count > 0)
        {
            foreach (var requiredResource in RequiredResources.ToList())
            {
                if (requiredResource.IsArtifactResource || requiredResource.IsTomeOfInsightResource || requiredResource.IsAvalonianEnergy)
                {
                    requiredResource.CraftingQuantity = (long)Math.Round(possibleItemCrafting, MidpointRounding.ToNegativeInfinity);
                    continue;
                }

                requiredResource.CraftingQuantity = EssentialCraftingValues.CraftingItemQuantity;
            }
        }

        CraftingCalculation.PossibleItemCrafting = Math.Round(possibleItemCrafting, MidpointRounding.ToNegativeInfinity);

        // Crafting (Usage) tax
        CraftingCalculation.CraftingTax = CraftingController.GetCraftingTax(EssentialCraftingValues.UsageFeePerHundredFood, Item, CraftingCalculation.PossibleItemCrafting);

        // Setup fee
        CraftingCalculation.SetupFee = CraftingController.GetSetupFeeCalculation(EssentialCraftingValues.CraftingItemQuantity, EssentialCraftingValues.SetupFee, EssentialCraftingValues.SellPricePerItem);

        // Auctions house tax
        CraftingCalculation.AuctionsHouseTax =
            EssentialCraftingValues.SellPricePerItem * Convert.ToInt64(EssentialCraftingValues.CraftingItemQuantity) / 100 * Convert.ToInt64(EssentialCraftingValues.AuctionHouseTax);

        // Total resource costs
        CraftingCalculation.TotalResourceCosts = RequiredResources?.Sum(x => x.TotalCost) ?? 0;

        // Other costs
        CraftingCalculation.OtherCosts = EssentialCraftingValues.OtherCosts;

        // Total item sells
        CraftingCalculation.TotalItemSells = EssentialCraftingValues.SellPricePerItem * (CraftingCalculation.PossibleItemCrafting * EssentialCraftingValues.AmountCrafted);

        if (RequiredJournal != null)
        {
            // Required journal amount
            RequiredJournal.RequiredJournalAmount = CraftingController.GetRequiredJournalAmount(Item, Math.Round(possibleItemCrafting, MidpointRounding.ToNegativeInfinity));

            // Total journal costs
            CraftingCalculation.TotalJournalCosts = RequiredJournal.CostsPerJournal * RequiredJournal.RequiredJournalAmount;

            // Total journal sells
            CraftingCalculation.TotalJournalSells = RequiredJournal.RequiredJournalAmount * RequiredJournal.SellPricePerJournal;
        }

        // Amount crafted
        CraftingCalculation.AmountCrafted = EssentialCraftingValues.AmountCrafted;

        // Weight
        var requiredResourcesWeights = RequiredResources?.Sum(x => x.TotalWeight) ?? 0;
        var possibleItemCraftingWeights = CraftingCalculation?.PossibleItemCrafting * ItemController.GetWeight(Item?.FullItemInformation) ?? 0;

        if (CraftingCalculation != null)
        {
            CraftingCalculation.TotalResourcesWeight = requiredResourcesWeights;
            CraftingCalculation.TotalRequiredJournalWeight = RequiredJournal?.TotalWeight ?? 0;
            CraftingCalculation.TotalUnfinishedCraftingWeight = CraftingCalculation.TotalResourcesWeight + CraftingCalculation.TotalRequiredJournalWeight;

            CraftingCalculation.TotalCraftedItemWeight = possibleItemCraftingWeights;
            CraftingCalculation.TotalFinishedCraftingWeight = CraftingCalculation.TotalCraftedItemWeight;
        }
    }

    #endregion Crafting tab

    #region Error methods

    private void SetErrorValues(Error error)
    {
        switch (error)
        {
            case Error.NoItemInfo:
                Icon = new BitmapImage(new Uri(@"pack://application:,,,/"
                                               + Assembly.GetExecutingAssembly().GetName().Name + ";component/"
                                               + "Resources/Trash.png", UriKind.Absolute));
                SetLoadingImageToError();
                SetErrorBar(Visibility.Visible, LanguageController.Translation("ERROR_NO_ITEM_INFO"));
                return;

            case Error.NoPrices:
                SetLoadingImageToError();
                SetErrorBar(Visibility.Visible, LanguageController.Translation("ERROR_PRICES_CAN_NOT_BE_LOADED"));
                return;

            case Error.GeneralError:
                SetLoadingImageToError();
                SetErrorBar(Visibility.Visible, LanguageController.Translation("ERROR_GENERAL_ERROR"));
                return;

            case Error.ToManyRequests:
                SetLoadingImageToError();
                SetErrorBar(Visibility.Visible, LanguageController.Translation("TOO_MANY_REQUESTS_CLOSE_WINDOWS_OR_WAIT"));
                return;

            default:
                SetLoadingImageToError();
                SetErrorBar(Visibility.Visible, LanguageController.Translation("ERROR_GENERAL_ERROR"));
                return;
        }
    }

    private void ErrorBarReset()
    {
        IsTaskProgressbarIndeterminate = false;
        SetErrorBar(Visibility.Hidden, string.Empty);
    }

    private void SetLoadingImageToError()
    {
        IsTaskProgressbarIndeterminate = true;
    }

    private void SetErrorBar(Visibility visibility, string errorMessage)
    {
        ErrorBarText = errorMessage;
        ErrorBarVisibility = visibility;
    }

    #endregion

    #region Prices

    public async void UpdateMarketPricesAsync(object sender, ElapsedEventArgs e)
    {
        await UpdateMarketPricesAsync();
    }

    public async Task UpdateMarketPricesAsync()
    {
        try
        {
            var marketResponses = await ApiController.GetCityItemPricesFromJsonAsync(Item?.UniqueName);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CurrentItemPrices = marketResponses;
            });
            ErrorBarReset();
        }
        catch (TooManyRequestsException ex)
        {
            SetErrorValues(Error.ToManyRequests);
            Log.Warn(nameof(UpdateMarketPricesAsync), ex);
        }
    }

    //public async Task GetItemPricesInRealMoneyAsync()
    //{
    //    if (CurrentCityPrices == null)
    //    {
    //        return;
    //    }

    //    var realMoneyMarketObject = new List<MarketQualityObject>();

    //    var filteredCityPrices = GetFilteredCityPrices(ShowBlackZoneOutpostsChecked, ShowVillagesChecked, true, true, true);
    //    foreach (var stat in filteredCityPrices)
    //    {
    //        if (realMoneyMarketObject.Exists(x => x.Location == stat.City))
    //        {
    //            var marketQualityObject = realMoneyMarketObject.Find(x => x.LocationName == stat.City);
    //            await SetRealMoneyQualityStat(stat, marketQualityObject);
    //        }
    //        else
    //        {
    //            var marketQualityObject = new MarketQualityObject { Location = stat.City };
    //            await SetRealMoneyQualityStat(stat, marketQualityObject);
    //            realMoneyMarketObject.Add(marketQualityObject);
    //        }
    //    }
    //    RealMoneyPriceList = realMoneyMarketObject;
    //}

    //public void SetQualityPriceStatsOnListView()
    //{
    //    if (CurrentCityPrices == null)
    //    {
    //        return;
    //    }

    //    var filteredCityPrices = GetFilteredCityPrices(ShowBlackZoneOutpostsChecked, ShowVillagesChecked, true, true, true);
    //    var marketQualityObjectList = new List<MarketQualityObject>();

    //    foreach (var stat in filteredCityPrices)
    //    {
    //        if (marketQualityObjectList.Exists(x => x.Location == stat.City))
    //        {
    //            var marketQualityObject = marketQualityObjectList.Find(x => x.LocationName == stat.City);
    //            SetQualityStat(stat, ref marketQualityObject);
    //        }
    //        else
    //        {
    //            var marketQualityObject = new MarketQualityObject { Location = stat.City };
    //            SetQualityStat(stat, ref marketQualityObject);
    //            marketQualityObjectList.Add(marketQualityObject);
    //        }
    //    }

    //    AllQualityPricesList = marketQualityObjectList;
    //}

    //private async Task SetRealMoneyQualityStat(MarketResponse marketResponse, MarketQualityObject marketQualityObject)
    //{
    //    if (marketQualityObject == null)
    //        return;

    //    if (_currentGoldPrice == null)
    //    {
    //        var getGoldPricesObjectList = await ApiController.GetGoldPricesFromJsonAsync(null, 1);
    //        _currentGoldPrice = getGoldPricesObjectList?.FirstOrDefault();
    //    }

    //    switch (ItemController.GetQuality(marketResponse.QualityLevel))
    //    {
    //        case ItemQuality.Normal:
    //            marketQualityObject.SellPriceMinNormalStringInRalMoney =
    //                Converter.GoldToDollar(marketResponse.SellPriceMin, _currentGoldPrice?.Price ?? 0);
    //            marketQualityObject.SellPriceMinNormalDate = marketResponse.SellPriceMinDate;
    //            return;

    //        case ItemQuality.Good:
    //            marketQualityObject.SellPriceMinGoodStringInRalMoney =
    //                Converter.GoldToDollar(marketResponse.SellPriceMin, _currentGoldPrice?.Price ?? 0);
    //            marketQualityObject.SellPriceMinGoodDate = marketResponse.SellPriceMinDate;
    //            return;

    //        case ItemQuality.Outstanding:
    //            marketQualityObject.SellPriceMinOutstandingStringInRalMoney =
    //                Converter.GoldToDollar(marketResponse.SellPriceMin, _currentGoldPrice?.Price ?? 0);
    //            marketQualityObject.SellPriceMinOutstandingDate = marketResponse.SellPriceMinDate;
    //            return;

    //        case ItemQuality.Excellent:
    //            marketQualityObject.SellPriceMinExcellentStringInRalMoney =
    //                Converter.GoldToDollar(marketResponse.SellPriceMin, _currentGoldPrice?.Price ?? 0);
    //            marketQualityObject.SellPriceMinExcellentDate = marketResponse.SellPriceMinDate;
    //            return;

    //        case ItemQuality.Masterpiece:
    //            marketQualityObject.SellPriceMinMasterpieceStringInRalMoney =
    //                Converter.GoldToDollar(marketResponse.SellPriceMin, _currentGoldPrice?.Price ?? 0);
    //            marketQualityObject.SellPriceMinMasterpieceDate = marketResponse.SellPriceMinDate;
    //            return;
    //    }
    //}

    // TODO: Without try catch

    private static void FindBestPrice(IReadOnlyCollection<ItemPricesObject> list)
    {
        if (list == null || list.Count == 0)
            return;

        for (var i = 1; i <= 5; i++)
        {
            var max = GetMaxPrice(list, i);

            var itemPricesObjectBuyPriceMax = list
                .Where(x => x.Visibility == Visibility.Visible && x.MarketResponse.QualityLevel == i).FirstOrDefault(s => s.MarketResponse?.BuyPriceMax == max);
            if (itemPricesObjectBuyPriceMax != null)
            {
                itemPricesObjectBuyPriceMax.IsBestBuyMaxPrice = true;
            }

            var min = GetMinPrice(list, i);

            var itemPricesObjectSellPriceMin = list
                .Where(x => x.Visibility == Visibility.Visible && x.MarketResponse.QualityLevel == i).FirstOrDefault(s => s.MarketResponse?.SellPriceMin == min);
            if (itemPricesObjectSellPriceMin != null)
            {
                itemPricesObjectSellPriceMin.IsBestSellMinPrice = true;
            }
        }
    }

    private static ulong GetMaxPrice(IEnumerable<ItemPricesObject> list, int quality)
    {
        var max = ulong.MinValue;
        foreach (var type in list.Where(x => x.MarketResponse.QualityLevel == quality))
        {
            if (type.MarketResponse.BuyPriceMax == 0)
                continue;

            if (type.MarketResponse.BuyPriceMax > max)
                max = type.MarketResponse.BuyPriceMax;
        }

        return max;
    }

    private static ulong GetMinPrice(IEnumerable<ItemPricesObject> list, int quality)
    {
        var min = ulong.MaxValue;
        foreach (var type in list.Where(x => x.MarketResponse.QualityLevel == quality))
        {
            if (type.MarketResponse.SellPriceMin == 0)
                continue;

            if (type.MarketResponse.SellPriceMin < min)
                min = type.MarketResponse.SellPriceMin;
        }

        return min;
    }

    //private void SetAveragePricesString()
    //{
    //    var cityPrices = GetFilteredCityPrices(false, false, true, false);

    //    var sellPriceMin = new List<ulong>();
    //    var sellPriceMax = new List<ulong>();
    //    var buyPriceMin = new List<ulong>();
    //    var buyPriceMax = new List<ulong>();

    //    foreach (var price in cityPrices ?? new List<MarketResponse>())
    //    {
    //        if (price.SellPriceMin != 0) sellPriceMin.Add(price.SellPriceMin);

    //        if (price.SellPriceMax != 0) sellPriceMax.Add(price.SellPriceMax);

    //        if (price.BuyPriceMin != 0) buyPriceMin.Add(price.BuyPriceMin);

    //        if (price.BuyPriceMax != 0) buyPriceMax.Add(price.BuyPriceMax);
    //    }

    //    var sellPriceMinAverage = Average(sellPriceMin.ToArray());
    //    var sellPriceMaxAverage = Average(sellPriceMax.ToArray());
    //    var buyPriceMinAverage = Average(buyPriceMin.ToArray());
    //    var buyPriceMaxAverage = Average(buyPriceMax.ToArray());

    //    AveragePrices = $"{string.Format(LanguageController.CurrentCultureInfo, "{0:n0}", sellPriceMinAverage)}  |  " +
    //                    $"{string.Format(LanguageController.CurrentCultureInfo, "{0:n0}", sellPriceMaxAverage)}  |  " +
    //                    $"{string.Format(LanguageController.CurrentCultureInfo, "{0:n0}", buyPriceMinAverage)}  |  " +
    //                    $"{string.Format(LanguageController.CurrentCultureInfo, "{0:n0}", buyPriceMaxAverage)}";
    //}

    #endregion Prices

    #region Main tab

    private void UpdateMainTabItemPrices(object sender, ElapsedEventArgs e)
    {
        UpdateMainTabItemPricesAsync();
    }

    private async void UpdateMainTabItemPricesAsync()
    {
        var currentItemPrices = CurrentItemPrices?.Select(x => new ItemPricesObject(x)).ToList();
        await UpdateMainTabItemPricesObjectsAsync(currentItemPrices);
        SetItemPricesObjectVisibility(MainTabItemPrices);
    }

    private void SetItemPricesObjectVisibility(ObservableCollection<ItemPricesObject> prices)
    {
        foreach (var currentItemPricesObject in prices?.ToList() ?? new List<ItemPricesObject>())
        {
            if (GetMainTabCheckedLocations().Contains(currentItemPricesObject.MarketLocation) && currentItemPricesObject.MarketResponse.QualityLevel == QualitiesSelection.Quality)
            {
                currentItemPricesObject.Visibility = Visibility.Visible;
            }
            else
            {
                currentItemPricesObject.Visibility = Visibility.Collapsed;
            }
        }

        FindBestPrice(prices?.Where(x => GetMainTabCheckedLocations().Contains(x.MarketLocation)).ToList());
    }

    private async Task UpdateMainTabItemPricesObjectsAsync(List<ItemPricesObject> newPrices)
    {
        foreach (var newItemPricesObject in newPrices)
        {
            var currentItemPricesObject = MainTabItemPrices?.FirstOrDefault(x => x.MarketLocation == newItemPricesObject.MarketLocation && x.MarketResponse.QualityLevel == newItemPricesObject.MarketResponse.QualityLevel);

            if (currentItemPricesObject == null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MainTabItemPrices?.Add(newItemPricesObject);
                });
            }

            if (newItemPricesObject?.MarketResponse?.SellPriceMinDate < currentItemPricesObject?.MarketResponse?.SellPriceMinDate)
            {
                currentItemPricesObject.MarketResponse.SellPriceMin = newItemPricesObject.MarketResponse.SellPriceMin;
                currentItemPricesObject.MarketResponse.SellPriceMinDate = newItemPricesObject.MarketResponse.SellPriceMinDate;
            }

            if (newItemPricesObject?.MarketResponse?.SellPriceMaxDate < currentItemPricesObject?.MarketResponse?.SellPriceMaxDate)
            {
                currentItemPricesObject.MarketResponse.SellPriceMax = newItemPricesObject.MarketResponse.SellPriceMax;
                currentItemPricesObject.MarketResponse.SellPriceMaxDate = newItemPricesObject.MarketResponse.SellPriceMaxDate;
            }

            if (newItemPricesObject?.MarketResponse?.BuyPriceMinDate < currentItemPricesObject?.MarketResponse?.BuyPriceMinDate)
            {
                currentItemPricesObject.MarketResponse.BuyPriceMin = newItemPricesObject.MarketResponse.BuyPriceMin;
                currentItemPricesObject.MarketResponse.BuyPriceMinDate = newItemPricesObject.MarketResponse.BuyPriceMinDate;
            }

            if (newItemPricesObject?.MarketResponse?.BuyPriceMaxDate < currentItemPricesObject?.MarketResponse?.BuyPriceMaxDate)
            {
                currentItemPricesObject.MarketResponse.BuyPriceMax = newItemPricesObject.MarketResponse.BuyPriceMax;
                currentItemPricesObject.MarketResponse.BuyPriceMaxDate = newItemPricesObject.MarketResponse.BuyPriceMaxDate;
            }
        }
    }

    private List<MarketLocation> GetMainTabCheckedLocations()
    {
        return LocationFilters?.Where(x => x?.IsChecked == true).Select(x => x.Location).ToList() ?? new List<MarketLocation>();
    }

    private void AddLocationFiltersEvents()
    {
        foreach (var cityFilterObject in LocationFilters)
        {
            cityFilterObject.OnCheckedChanged += UpdateMainTabItemPricesAsync;
        }
    }

    #endregion

    #region Bindings

    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            OnPropertyChanged();
        }
    }

    public string TitleName
    {
        get => _titleName;
        set
        {
            _titleName = value;
            OnPropertyChanged();
        }
    }

    public string ItemTierLevel
    {
        get => _itemTierLevel;
        set
        {
            _itemTierLevel = value;
            OnPropertyChanged();
        }
    }

    public BitmapImage Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            OnPropertyChanged();
        }
    }

    public bool RefreshSpin
    {
        get => _refreshSpin;
        set
        {
            _refreshSpin = value;
            OnPropertyChanged();
        }
    }

    public XmlLanguage ItemListViewLanguage
    {
        get => _itemListViewLanguage;
        set
        {
            _itemListViewLanguage = value;
            OnPropertyChanged();
        }
    }

    public List<QualityStruct> Qualities
    {
        get => _qualities;
        set
        {
            _qualities = value;
            OnPropertyChanged();
        }
    }

    public QualityStruct QualitiesSelection
    {
        get => _qualitiesSelection;
        set
        {
            _qualitiesSelection = value;
            SettingsController.CurrentSettings.ItemWindowQualitiesSelection = _qualitiesSelection.Quality;
            UpdateMainTabItemPrices(null, null);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CityFilterObject> LocationFilters
    {
        get => _locationFilters;
        set
        {
            _locationFilters = value;
            OnPropertyChanged();
        }
    }

    public RequiredJournal RequiredJournal
    {
        get => _requiredJournal;
        set
        {
            _requiredJournal = value;
            OnPropertyChanged();
        }
    }

    public CraftingCalculation CraftingCalculation
    {
        get => _craftingCalculation;
        set
        {
            _craftingCalculation = value;
            OnPropertyChanged();
        }
    }

    public EssentialCraftingValuesTemplate EssentialCraftingValues
    {
        get => _essentialCraftingValues;
        set
        {
            _essentialCraftingValues = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<RequiredResource> RequiredResources
    {
        get => _requiredResources;
        set
        {
            _requiredResources = value;
            OnPropertyChanged();
        }
    }

    public string CraftingNotes
    {
        get => _craftingNotes;
        set
        {
            _craftingNotes = value;
            OnPropertyChanged();
        }
    }

    public Visibility RequiredJournalVisibility
    {
        get => _requiredJournalVisibility;
        set
        {
            _requiredJournalVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility CraftingTabVisibility
    {
        get => _craftingTabVisibility;
        set
        {
            _craftingTabVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility ErrorBarVisibility
    {
        get => _errorBarVisibility;
        set
        {
            _errorBarVisibility = value;
            OnPropertyChanged();
        }
    }

    public string ErrorBarText
    {
        get => _errorBarText;
        set
        {
            _errorBarText = value;
            OnPropertyChanged();
        }
    }

    public List<MarketResponse> CurrentItemPrices
    {
        get => _currentItemPrices;
        set
        {
            _currentItemPrices = value;
            if (EssentialCraftingValues != null)
            {
                EssentialCraftingValues.CurrentCityPrices = value;
            }

            OnPropertyChanged();
        }
    }

    public ObservableCollection<ItemPricesObject> MainTabItemPrices
    {
        get => _mainTabItemPrices;
        set
        {
            _mainTabItemPrices = value;
            OnPropertyChanged();
        }
    }

    public bool IsAutoUpdateActive
    {
        get => _isAutoUpdateActive;
        set
        {
            _isAutoUpdateActive = value;

            _timer.Enabled = _isAutoUpdateActive;
            RefreshSpin = _isAutoUpdateActive;
            OnPropertyChanged();
        }
    }

    public double TaskProgressbarMinimum
    {
        get => _taskProgressbarMinimum;
        set
        {
            _taskProgressbarMinimum = value;
            OnPropertyChanged();
        }
    }

    public double TaskProgressbarMaximum
    {
        get => _taskProgressbarMaximum;
        set
        {
            _taskProgressbarMaximum = value;
            OnPropertyChanged();
        }
    }

    public double TaskProgressbarValue
    {
        get => _taskProgressbarValue;
        set
        {
            _taskProgressbarValue = value;
            OnPropertyChanged();
        }
    }

    public bool IsTaskProgressbarIndeterminate
    {
        get => _isTaskProgressbarIndeterminate;
        set
        {
            _isTaskProgressbarIndeterminate = value;
            OnPropertyChanged();
        }
    }

    public ExtraItemInformation ExtraItemInformation
    {
        get => _extraItemInformation;
        set
        {
            _extraItemInformation = value;
            OnPropertyChanged();
        }
    }

    public ItemWindowTranslation Translation
    {
        get => _translation;
        set
        {
            _translation = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Helper

    public ulong Sum(params ulong[] values)
    {
        return values.Aggregate(0UL, (current, t) => current + t);
    }

    public ulong Average(params ulong[] values)
    {
        if (values.Length == 0) return 0;

        var sum = Sum(values);
        var result = sum / (ulong)values.Length;
        return result;
    }

    public struct QualityStruct
    {
        public string Name { get; set; }
        public int Quality { get; set; }
    }

    //private List<int> GetQualities()
    //{
    //    var qualities = new List<int>();

    //    if (NormalQualityChecked)
    //    {
    //        qualities.Add(1);
    //    }

    //    if (GoodQualityChecked)
    //    {
    //        qualities.Add(2);
    //    }

    //    if (OutstandingQualityChecked)
    //    {
    //        qualities.Add(3);
    //    }

    //    if (ExcellentQualityChecked)
    //    {
    //        qualities.Add(4);
    //    }

    //    if (MasterpieceQualityChecked)
    //    {
    //        qualities.Add(5);
    //    }

    //    return qualities;
    //}

    //private void SetDefaultQualityIfNoOneChecked()
    //{
    //    if (!NormalQualityChecked && !GoodQualityChecked && !OutstandingQualityChecked && !ExcellentQualityChecked && !MasterpieceQualityChecked)
    //    {
    //        NormalQualityChecked = true;
    //    }
    //}

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}