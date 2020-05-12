﻿using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using BearMyBanner.Settings;
using BearMyBanner.Wrapper;
using System.Collections.Generic;

namespace BearMyBanner
{
    public class Main : MBSubModuleBase
    {

        public const string ModuleFolderName = "BearMyShield";
        public const string ModName = "Bear my Shield";

        public static List<(string content, bool isError)> LoadingMessages = new List<(string, bool)>();

        private IBMBSettings _settings;
        private IPolybianConfig _polybianConfig;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {

                LoadingMessages.Add(("Loaded Bear my Shield", false));
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            try
            {
                _polybianConfig = PolybianConfig.Instance;
                _settings = BMBSettings.Instance;

                foreach ((string content, bool isError) message in LoadingMessages)
                {
                    PrintWhileLoading(message.content, message.isError);
                }
                LoadingMessages.Clear();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
            try
            {
                if (_settings == null)
                {
                    throw new InvalidOperationException("Settings were not initialized");
                }

                if (Mission.Current.CombatType == Mission.MissionCombatType.Combat)
                {
                    MissionType missionType = mission.GetMissionType();
                    switch (missionType)
                    {
                        case MissionType.FieldBattle:
                            mission.AddMissionBehaviour(new BattleBannerAssignBehaviour(_settings, _polybianConfig, missionType));
                            break;
                        case MissionType.Siege:
                            if (_settings.AllowSieges) mission.AddMissionBehaviour(new BattleBannerAssignBehaviour(_settings, _polybianConfig, missionType));
                            break;
                        case MissionType.Hideout:
                            if (_settings.AllowHideouts) mission.AddMissionBehaviour(new BattleBannerAssignBehaviour(_settings, _polybianConfig, missionType));
                            break;
                        case MissionType.CustomBattle:
                            mission.AddMissionBehaviour(new CustomBattleBannerBehaviour(_settings));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static void PrintInMessageLog(string message)
        {
            PrintInMessageLog(message, TaleWorlds.Library.Color.White);
        }

        public static void PrintInMessageLog(string message, uint color)
        {
            PrintInMessageLog(message, TaleWorlds.Library.Color.FromUint(color));
        }

        public static void PrintInMessageLog(string message, TaleWorlds.Library.Color color)
        {
            if (BMBSettings.Instance.ShowMessages)
            {
                if (BMBSettings.Instance.WhiteMessages) color = TaleWorlds.Library.Color.White;
                InformationManager.DisplayMessage(new InformationMessage(message, color));
            }
        }

        public static void LogError(Exception ex)
        {
            try
            {
                PrintInMessageLog("BMS Error: " + ex.Message);
            }
            catch (Exception) //If there's an exception because settings were not loaded
            {
                InformationManager.DisplayMessage(new InformationMessage("BMS Error loading settings"));
            }
        }

        public static void PrintWhileLoading(string message, bool isError)
        {
            Color color = isError ? new Color(1f, 0f, 0f, 1f) : Color.FromUint(4282569842U);
            InformationManager.DisplayMessage(new InformationMessage(message, color));
        }
    }
}
