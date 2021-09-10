namespace StatisticsAnalysisTool.Enumerations
{
    public enum EventCodes
    {
        Unused = 0,
        Leave,
        JoinFinished, // <- UserInfo,
        Move,
        Teleport,
        ChangeEquipment,
        HealthUpdate,
        EnergyUpdate,
        DamageShieldUpdate,
        CraftingFocusUpdate,
        ActiveSpellEffectsUpdate,
        ResetCooldowns,
        Attack,
        CastStart,
        ChannelingUpdate,
        CastCancel,
        CastTimeUpdate,
        CastFinished,
        CastSpell,
        CastHit,
        CastHits,
        ChannelingEnded,
        AttackBuilding,
        InventoryPutItem, //  map[0:652 1:6 2:[118 -97 114 112 -46 84 -60 75 -103 -93 -3 -29 118 -125 -50 96] 3:17 252:23] (0: ObjectId, 2: UserId)
        InventoryDeleteItem, // map[0:754 1:48 252:24] (0: ObjectId) 
        NewCharacter,
        NewEquipmentItem, //  map[0:657 1:2036 2:1 4:28169331 5:Apolo540 6:3 7:90000000 8:[] 9:[0] 252:26] (0: ObjectId, 1: ItemId, 2: Amount, 4: Avarage Market Price, 5: CrafterName)
        NewSimpleItem, //  map[0:505 1:7006 2:1 3:true 4:29033970 252:27] (0: ObjectId, 1: ItemId, 2: Amount)
        NewFurnitureItem,
        NewJournalItem,
        NewLaborerItem,
        NewSimpleHarvestableObject,
        NewSimpleHarvestableObjectList,
        NewHarvestableObject,
        NewSilverObject,
        NewBuilding,
        HarvestableChangeState,
        MobChangeState,
        FactionBuildingInfo,
        CraftBuildingInfo,
        RepairBuildingInfo,
        MeldBuildingInfo,
        ConstructionSiteInfo,
        PlayerBuildingInfo,
        FarmBuildingInfo,
        TutorialBuildingInfo,
        LaborerObjectInfo,
        LaborerObjectJobInfo,
        MarketPlaceBuildingInfo,
        HarvestStart,
        HarvestCancel,
        HarvestFinished,
        TakeSilver,
        ActionOnBuildingStart,
        ActionOnBuildingCancel,
        ActionOnBuildingFinished,
        ItemRerollQualityStart,
        ItemRerollQualityCancel,
        ItemRerollQualityFinished,
        InstallResourceStart,
        InstallResourceCancel,
        InstallResourceFinished,
        CraftItemFinished,
        LogoutCancel,
        ChatMessage,
        ChatSay,
        ChatWhisper,
        ChatMuted,
        PlayEmote,
        StopEmote,
        SystemMessage,
        UtilityTextMessage,
        UpdateMoney,
        UpdateFame,
        UpdateLearningPoints,
        UpdateReSpecPoints,
        UpdateCurrency,
        UpdateFactionStanding,
        Respawn,
        ServerDebugLog,
        CharacterEquipmentChanged,
        RegenerationHealthChanged,
        RegenerationEnergyChanged,
        RegenerationMountHealthChanged,
        RegenerationCraftingChanged,
        RegenerationHealthEnergyComboChanged,
        RegenerationPlayerComboChanged,
        DurabilityChanged,
        NewLoot,
        AttachItemContainer, //  map[0:78 1:[-99 -50 125 -49 86 0 -115 74 -74 67 9 101 -87 -71 -66 -10] 3:[0 0 0 0 0 0 656 657] 4:8 252:89] (0: ObjectId, 3: ItemId[])
        DetachItemContainer, //  map[0:[-95 72 -77 -75 -70 34 127 73 -114 -96 28 8 75 -107 -106 125] 252:90]
        InvalidateItemContainer,
        LockItemContainer,
        GuildUpdate,
        GuildPlayerUpdated,
        InvitedToGuild,
        GuildMemberWorldUpdate,
        UpdateMatchDetails,
        ObjectEvent,
        NewMonolithObject,
        NewSiegeCampObject,
        NewOrbObject,
        NewCastleObject,
        NewSpellEffectArea,
        UpdateSpellEffectArea,
        NewChainSpell,
        UpdateChainSpell,
        NewTreasureChest,
        StartMatch,
        StartTerritoryMatchInfos,
        StartArenaMatchInfos,
        EndTerritoryMatch,
        EndArenaMatch,
        MatchUpdate,
        ActiveMatchUpdate,
        NewMob,
        DebugAggroInfo,
        DebugVariablesInfo,
        DebugReputationInfo,
        DebugDiminishingReturnInfo,
        DebugSmartClusterQueueInfo,
        ClaimOrbStart,
        ClaimOrbFinished,
        ClaimOrbCancel,
        OrbUpdate,
        OrbClaimed,
        NewWarCampObject,
        NewMatchLootChestObject,
        NewArenaExit,
        GuildMemberTerritoryUpdate,
        InvitedMercenaryToMatch,
        ClusterInfoUpdate,
        ForcedMovement,
        ForcedMovementCancel,
        CharacterStats,
        CharacterStatsKillHistory,
        CharacterStatsDeathHistory,
        GuildStats,
        KillHistoryDetails,
        FullAchievementInfo,
        FinishedAchievement,
        AchievementProgressInfo,
        FullAchievementProgressInfo,
        FullTrackedAchievementInfo,
        FullAutoLearnAchievementInfo,
        QuestGiverQuestOffered,
        QuestGiverDebugInfo,
        ConsoleEvent,
        TimeSync,
        ChangeAvatar,
        ChangeMountSkin,
        GameEvent,
        KilledPlayer,
        Died,
        KnockedDown,
        MatchPlayerJoinedEvent,
        MatchPlayerStatsEvent,
        MatchPlayerStatsCompleteEvent,
        MatchTimeLineEventEvent,
        MatchPlayerMainGearStatsEvent,
        MatchPlayerChangedAvatarEvent,
        InvitationPlayerTrade,
        PlayerTradeStart,
        PlayerTradeCancel,
        PlayerTradeUpdate,
        PlayerTradeFinished,
        PlayerTradeAcceptChange,
        MiniMapPing,
        MarketPlaceNotification,
        DuellingChallengePlayer,
        NewDuellingPost,
        DuelStarted,
        DuelEnded,
        DuelDenied,
        DuelLeftArea,
        DuelReEnteredArea,
        NewRealEstate,
        MiniMapOwnedBuildingsPositions,
        RealEstateListUpdate,
        GuildLogoUpdate,
        GuildLogoChanged,
        PlaceableObjectPlace,
        PlaceableObjectPlaceCancel,
        FurnitureObjectBuffProviderInfo,
        FurnitureObjectCheatProviderInfo,
        FarmableObjectInfo,
        NewUnreadMails,
        Unknown187,
        GuildLogoObjectUpdate,
        StartLogout,
        NewChatChannels,
        JoinedChatChannel,
        LeftChatChannel,
        RemovedChatChannel,
        AccessStatus,
        Mounted,
        MountStart,
        MountCancel,
        NewTravelpoint,
        NewIslandAccessPoint,
        NewExit,
        UpdateHome,
        UpdateChatSettings,
        ResurrectionOffer,
        ResurrectionReply,
        LootEquipmentChanged,
        UpdateUnlockedGuildLogos,
        UpdateUnlockedAvatars,
        UpdateUnlockedAvatarRings,
        UpdateUnlockedBuildings,
        NewIslandManagement,
        NewTeleportStone,
        Cloak,
        PartyInvitation,
        PartyJoined,
        PartyDisbanded,
        PartyPlayerJoined,
        PartyChangedOrder,
        PartyPlayerLeft,
        PartyLeaderChanged,
        PartyLootSettingChangedPlayer,
        PartySilverGained,
        PartyPlayerUpdated,
        PartyInvitationPlayerBusy,
        PartyMarkedObjectsUpdated,
        PartyOnClusterPartyJoined,
        PartySetRoleFlag,
        SpellCooldownUpdate,
        NewHellgate,
        NewHellgateExit,
        NewExpeditionExit,
        NewExpeditionNarrator,
        ExitEnterStart,
        ExitEnterCancel,
        ExitEnterFinished,
        HellClusterTimeUpdate,
        NewQuestGiverObject,
        FullQuestInfo,
        QuestProgressInfo,
        QuestGiverInfoForPlayer,
        FullExpeditionInfo,
        ExpeditionQuestProgressInfo,
        InvitedToExpedition,
        ExpeditionRegistrationInfo,
        EnteringExpeditionStart,
        EnteringExpeditionCancel,
        RewardGranted,
        ArenaRegistrationInfo,
        EnteringArenaStart,
        EnteringArenaCancel,
        EnteringArenaLockStart,
        EnteringArenaLockCancel,
        InvitedToArenaMatch,
        PlayerCounts,
        Unknown254,
        Unknown255,
        Unknown256,
        OtherGrabbedLoot, // map[0:4584 1:rxdc 2:Yoopo04 3:true 4:460 5:6 252:257] (0: ObjectId, 1: LootedBody, 2: Looter, 4: ItemId, 5: Quantity)
        Unknown258,
        InCombatStateUpdate = 259, // <- 1 = true; player hits enemy | 2 = true; enemy hits player
        Unknown260,
        SiegeCampClaimStart = 261,
        SiegeCampClaimCancel,
        SiegeCampClaimFinished,
        SiegeCampScheduleResult,
        TreasureChestUsingStart,
        TreasureChestUsingFinished,
        TreasureChestUsingCancel,
        TreasureChestUsingOpeningComplete,
        TreasureChestForceCloseInventory,
        PremiumChanged,
        PremiumExtended,
        PremiumLifeTimeRewardGained,
        LaborerGotUpgraded,
        JournalGotFull,
        JournalFillError,
        FriendRequest,
        FriendRequestInfos,
        FriendInfos,
        FriendRequestAnswered,
        FriendOnlineStatus,
        FriendRequestCanceled,
        FriendRemoved,
        FriendUpdated,
        PartyLootItems,
        PartyLootItemsRemoved,
        ReputationUpdate,
        DefenseUnitAttackBegin,
        DefenseUnitAttackEnd,
        DefenseUnitAttackDamage,
        UnrestrictedPvpZoneUpdate,
        ReputationImplicationUpdate,
        NewMountObject,
        MountHealthUpdate,
        MountCooldownUpdate,
        NewExpeditionAgent,
        NewExpeditionCheckPoint,
        ExpeditionStartent,
        Voteent,
        Ratingent,
        NewArenaAgent,
        BoostFarmable,
        UseFunction,
        NewPortalEntrance,
        NewPortalExit,
        NewRandomDungeonExit,
        WaitingQueueUpdate,
        PlayerMovementRateUpdate,
        ObserveStart,
        MinimapZergs,
        MinimapSmartClusterZergs,
        PaymentTransactions,
        PerformanceStatsUpdate,
        OverloadModeUpdate,
        DebugDrawEvent,
        RecordCameraMove,
        RecordStart,
        TerritoryClaimStart,
        TerritoryClaimCancel,
        TerritoryClaimFinished,
        TerritoryScheduleResult,
        UpdateAccountState,
        StartDeterministicRoam,
        GuildFullAccessTagsUpdated,
        GuildAccessTagUpdated,
        GvgSeasonUpdate,
        GvgSeasonCheatCommand,
        SeasonPointsByKillingBooster,
        FishingStart,
        FishingCast,
        FishingCatch,
        FishingFinished,
        FishingCancel,
        NewFloatObject,
        NewFishingZoneObject,
        FishingMiniGame,
        SteamAchievementCompleted,
        UpdatePuppet,
        ChangeFlaggingFinished,
        NewOutpostObject,
        OutpostUpdate,
        OutpostClaimed,
        OutpostReward,
        OverChargeEnd,
        OverChargeStatus,
        PartyFinderFullUpdate,
        PartyFinderUpdate,
        PartyFinderApplicantsUpdate,
        PartyFinderEquipmentSnapshot,
        PartyFinderJoinRequestDeclined,
        NewUnlockedPersonalSeasonRewards,
        PersonalSeasonPointsGained,
        EasyAntiCheatMessageToClient,
        MatchLootChestOpeningStart,
        MatchLootChestOpeningFinished,
        MatchLootChestOpeningCancel,
        NotifyCrystalMatchReward,
        CrystalRealmFeedback,
        NewLocationMarker,
        NewTutorialBlocker,
        NewTileSwitch,
        NewInformationProvider,
        NewDynamicGuildLogo,
        TutorialUpdate,
        TriggerHintBox,
        RandomDungeonPositionInfo,
        NewLootChest = 364,
        UpdateLootChest = 365, // <- Mob Info: MOB_KEEPER_UNPROVEN_MALE_VETERAN ... and more
        LootChestOpened = 366,
        NewShrine, // [0:24 1:[-148 109] 2:180 3:KEEPER_SHRINE_SILVER_STANDARD 4:SHRINE_KPR_SILVER_STANDARD 5:1 6:637517907902977990 252:370]
        UpdateShrine,
        MutePlayerUpdate,
        ShopTileUpdate,
        ShopUpdate,
        EasyAntiCheatKick,
        UnlockVanityUnlock,
        AvatarUnlocked,
        CustomizationChanged,
        BaseVaultInfo,
        GuildVaultInfo,
        BankVaultInfo,
        RecoveryVaultPlayerInfo,
        RecoveryVaultGuildInfo,
        UpdateWardrobe,
        CastlePhaseChanged,
        GuildAccountLogEvent,
        NewHideoutObject,
        NewHideoutManagement,
        NewHideoutExit,
        InitHideoutAttackStart,
        InitHideoutAttackCancel,
        InitHideoutAttackFinished,
        HideoutManagementUpdate,
        IpChanged,
        SmartClusterQueueUpdateInfo,
        SmartClusterQueueActiveInfo,
        SmartClusterQueueKickWarning,
        SmartClusterQueueInvite,
        ReceivedSeasonPoints = 399,
        TerritoryBonusLelUpdate,
        OpenWorldAttackScheduleStart,
        OpenWorldAttackScheduleFinished,
        OpenWorldAttackScheduleCancel,
        OpenWorldAttackConquerStart,
        OpenWorldAttackConquerFinished,
        OpenWorldAttackConquerCancel,
        OpenWorldAttackConquerStatus,
        OpenWorldAttackStart,
        OpenWorldAttackEnd,
        NewRandomResourceBlocker,
        NewHomeObject,
        HideoutObjectUpdate,
        UpdateInfamy,
        Unknown408,
        Unknown409,
        Unknown410,
        Unknown411,
        Unknown412,
        Unknown413,
        Unknown414,
        Unknown415,
        Unknown416,
        Unknown417,
        Unknown418,
        Unknown419,
        Unknown420,
        Unknown421,
        Unknown422
    }
}