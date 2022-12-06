﻿using StatisticsAnalysisTool.Common;
using StatisticsAnalysisTool.Common.UserSettings;
using StatisticsAnalysisTool.Properties;
using StatisticsAnalysisTool.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StatisticsAnalysisTool.Models.BindingModel;

public class ItemWindowMainTabBindings : INotifyPropertyChanged
{
    private readonly ItemWindowViewModel _itemWindowViewModel;
    private List<QualityStruct> _qualities = new();
    private QualityStruct _qualitiesSelection;
    private ObservableCollection<MainTabLocationFilterObject> _locationFilters;
    private ObservableCollection<ItemPricesObject> _itemPrices = new();

    public ItemWindowMainTabBindings(ItemWindowViewModel itemWindowViewModel)
    {
        _itemWindowViewModel = itemWindowViewModel;
        InitMainTabLocationFiltering();
    }

    private void InitMainTabLocationFiltering()
    {
        var locationFilters = new List<MainTabLocationFilterObject>
        {
            new (MarketLocation.CaerleonMarket, Locations.GetParameterName(Location.Caerleon), true),
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

        foreach (var itemWindowMainTabLocationFilter in SettingsController.CurrentSettings.ItemWindowMainTabLocationFilters)
        {
            var filter = locationFilters.FirstOrDefault(x => x.Location == itemWindowMainTabLocationFilter?.Location);
            if (filter != null)
            {
                filter.IsChecked = itemWindowMainTabLocationFilter.IsChecked;
            }
        }

        LocationFilters = new ObservableCollection<MainTabLocationFilterObject>(locationFilters.OrderBy(x => x.Name));

        AddLocationFiltersEvents();
    }

    private void AddLocationFiltersEvents()
    {
        foreach (var cityFilterObject in LocationFilters)
        {
            cityFilterObject.OnCheckedChanged += _itemWindowViewModel.UpdateMainTabItemPricesAsync;
        }
    }

    #region Bindings

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
            _itemWindowViewModel.UpdateMainTabItemPrices(null, null);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<MainTabLocationFilterObject> LocationFilters
    {
        get => _locationFilters;
        set
        {
            _locationFilters = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ItemPricesObject> ItemPrices
    {
        get => _itemPrices;
        set
        {
            _itemPrices = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public struct QualityStruct
    {
        public string Name { get; set; }
        public int Quality { get; set; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}