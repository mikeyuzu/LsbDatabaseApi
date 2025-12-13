using LsbDatabaseApi.@struct;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class ZilartMission
    {
        public static MessageParam GetMessageZilart(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();
            if (QuestMission.GetLevelCapMessage(database, charaInfo, 75, ref message))
            {
                return message;
            }

            var zilartMission = database.GetMissionInfo(charaInfo.CharaId, MissionId.ZILART);
            message.missionKind = MissionKind.MissionZilart;
            message.missionType = ((MissionZilart)zilartMission.Current).ToString();
            message.missionPhase = zilartMission.StatusLower.ToString();
            var optionString = $"Mission[{(int)MissionId.ZILART}][{(int)zilartMission.Current}]Option";
            var option = database.GetVarNum(charaInfo.CharaId, optionString);
            if (option > 0)
            {
                message.missionPhase = "option" + option.ToString();
            }
            switch ((MissionZilart)zilartMission.Current)
            {
                // 新たなる世界
                case MissionZilart.THE_NEW_FRONTIER:
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.AIRSHIP_PASS_FOR_KAZHAM))
                    {
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "AIRSHIP_PASS_FOR_KAZHAM";
                        message.missionPhase = "0";
                        return message;
                    }
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.NORG:
                            break;
                        case ZoneId.SEA_SERPENT_GROTTO:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.NORG.ToString();
                            message.missionPhase = ZoneId.SEA_SERPENT_GROTTO.ToString();
                            return message;
                        case ZoneId.YUHTUNGA_JUNGLE:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.SEA_SERPENT_GROTTO.ToString();
                            message.missionPhase = ZoneId.YUHTUNGA_JUNGLE.ToString();
                            return message;
                        case ZoneId.KAZHAM:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.YUHTUNGA_JUNGLE.ToString();
                            message.missionPhase = ZoneId.KAZHAM.ToString();
                            return message;
                        case ZoneId.KAZHAM_JEUNO_AIRSHIP:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.KAZHAM.ToString();
                            message.missionPhase = ZoneId.KAZHAM_JEUNO_AIRSHIP.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.KAZHAM_JEUNO_AIRSHIP.ToString();
                            message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                            return message;
                    }
                    
                    break;
                // ウガレピ寺院
                case MissionZilart.THE_TEMPLE_OF_UGGALEPIH:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.SACRIFICIAL_CHAMBER:
                            break;
                        case ZoneId.DEN_OF_RANCOR:
                            // TODO　再度動作確認が必要。
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.SACRIFICIAL_CHAMBER.ToString();
                            if (database.HasItem(charaInfo.CharaId, ItemId.RANCOR_FLAME))
                            {
                                message.missionPhase = "RANCOR_FLAME";
                                return message;
                            }
                            if (database.HasItem(charaInfo.CharaId, ItemId.UNLIT_LANTERN))
                            {
                                message.missionPhase = "UNLIT_LANTERN";
                                return message;
                            }
                            message.missionPhase = "0";
                            return message;
                        case ZoneId.TEMPLE_OF_UGGALEPIH:
                            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PAINTBRUSH_OF_SOULS))
                            {
                                message.missionKind = MissionKind.Quest;
                                message.missionType = "PAINTBRUSH_OF_SOULS";
                                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.OLD_RUSTY_KEY))
                                {
                                    message.missionPhase = "2";
                                    return message;
                                }
                                if (database.HasItem(charaInfo.CharaId, ItemId.UGGALEPIH_KEY))
                                {
                                    message.missionPhase = "1";
                                    return message;
                                }
                                if (charaInfo.Coordinates == "(F-7)")
                                {
                                    message.missionPhase = "2";
                                    return message;
                                }
                                message.missionPhase = "0";
                                return message;
                            }
                            if (charaInfo.MapId == 1)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.YHOATOR_JUNGLE.ToString();
                                message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                return message;
                            }
                            if (charaInfo.MapId == 2)
                            {
                                if (charaInfo.Coordinates == "(I-10)")
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    return message;
                                }
                                if (charaInfo.Coordinates == "(I-7)" || charaInfo.Coordinates == "(I-8)")
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                    message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString() + "_2";
                                    return message;
                                }
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.DEN_OF_RANCOR.ToString();
                            message.missionPhase = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                            return message;
                        case ZoneId.YHOATOR_JUNGLE:
                            if (charaInfo.PreZoneId == ZoneId.TEMPLE_OF_UGGALEPIH && charaInfo.PreCoordinates == "(F-5)")
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString() + "_2";
                                return message;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.TEMPLE_OF_UGGALEPIH.ToString();
                            message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.YHOATOR_JUNGLE.ToString();
                            message.missionPhase = ZoneId.YUHTUNGA_JUNGLE.ToString();
                            return message;
                    }
                    break;
                // 古代石碑巡礼
                case MissionZilart.HEADSTONE_PILGRIMAGE:
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.FIRE_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.YUHTUNGA_JUNGLE:
                                break;
                            case ZoneId.IFRITS_CAULDRON:
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                    message.missionPhase = ZoneId.IFRITS_CAULDRON.ToString() + "_2_7";
                                    return message;
                                }
                                if (charaInfo.MapId == 4)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                    message.missionPhase = ZoneId.IFRITS_CAULDRON.ToString() + "_4_7";
                                    return message;
                                }
                                if (charaInfo.MapId == 5)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                    message.missionPhase = ZoneId.IFRITS_CAULDRON.ToString() + "_5_2";
                                    return message;
                                }
                                if (charaInfo.MapId == 16)
                                {
                                    if (charaInfo.PreZoneId == ZoneId.IFRITS_CAULDRON && charaInfo.PreMapId == 2
                                        || charaInfo.PreZoneId == ZoneId.IFRITS_CAULDRON && charaInfo.PreMapId == 17)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                        message.missionPhase = ZoneId.IFRITS_CAULDRON.ToString() + "_7_8";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                    message.missionPhase = ZoneId.IFRITS_CAULDRON.ToString() + "_7_5";
                                    return message;
                                }
                                if (charaInfo.MapId == 17)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                    message.missionPhase = ZoneId.YUHTUNGA_JUNGLE.ToString();
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.IFRITS_CAULDRON.ToString();
                                message.missionPhase = ZoneId.YHOATOR_JUNGLE.ToString();
                                return message;
                        }
                        message.missionPhase = "FIRE_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.EARTH_FRAGMENT))
                    {
                        if (!database.HasZoneId(charaInfo.CharaId, ZoneId.BASTOK_MINES))
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.BASTOK_MINES.ToString();
                            message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.QUICKSAND_CAVES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                message.missionPhase = ZoneId.QUICKSAND_CAVES.ToString();
                                return message;
                            case ZoneId.WESTERN_ALTEPA_DESERT:
                                if (charaInfo.PreZoneId == ZoneId.QUICKSAND_CAVES && charaInfo.PreMapId == 3)
                                {
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                return message;
                            case ZoneId.EASTERN_ALTEPA_DESERT:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                message.missionPhase = ZoneId.EASTERN_ALTEPA_DESERT.ToString();
                                return message;
                            case ZoneId.KORROLOKA_TUNNEL:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.KORROLOKA_TUNNEL.ToString();
                                    message.missionPhase = ZoneId.KORROLOKA_TUNNEL.ToString() + "_1_2";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EASTERN_ALTEPA_DESERT.ToString();
                                message.missionPhase = ZoneId.KORROLOKA_TUNNEL.ToString();
                                return message;
                            case ZoneId.ZERUHN_MINES:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KORROLOKA_TUNNEL.ToString();
                                message.missionPhase = ZoneId.ZERUHN_MINES.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ZERUHN_MINES.ToString();
                                message.missionPhase = ZoneId.BASTOK_MINES.ToString();
                                return message;
                        }

                        message.missionPhase = "EARTH_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.WATER_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.ORDELLES_CAVES:
                                if (charaInfo.MapId == 1)
                                {
                                    if (charaInfo.PreZoneId == ZoneId.LA_THEINE_PLATEAU
                                        || charaInfo.PreZoneId == ZoneId.ORDELLES_CAVES && charaInfo.PreMapId == 15 && charaInfo.PreCoordinates == "(I-6)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                        message.missionPhase = ZoneId.ORDELLES_CAVES.ToString() + "_1_4";
                                        return message;
                                    }
                                    if (charaInfo.PreZoneId == ZoneId.ORDELLES_CAVES && charaInfo.PreMapId == 15)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                        message.missionPhase = ZoneId.ORDELLES_CAVES.ToString() + "_1_4_2";
                                        return message;
                                    }
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.LA_THEINE_PLATEAU.ToString();
                                    message.missionPhase = ZoneId.ORDELLES_CAVES.ToString();
                                    return message;
                                }
                                if (charaInfo.MapId == 15)
                                {
                                    if (charaInfo.PreMapId == 0 && charaInfo.PreCoordinates == "(A-1)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                        message.missionPhase = ZoneId.ORDELLES_CAVES.ToString() + "_4_2";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                    message.missionPhase = ZoneId.ORDELLES_CAVES.ToString() + "_4_1";
                                    return message;
                                }
                                break;
                            default:
                                if (charaInfo.ZoneId == ZoneId.LA_THEINE_PLATEAU && charaInfo.PreZoneId == ZoneId.ORDELLES_CAVES && charaInfo.PreMapId == 2)
                                {
                                    break;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ORDELLES_CAVES.ToString();
                                message.missionPhase = ZoneId.LA_THEINE_PLATEAU.ToString();
                                return message;
                        }
                        message.missionPhase = "WATER_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.WIND_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CAPE_TERIGGAN:
                                break;
                            case ZoneId.KUFTAL_TUNNEL:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.KUFTAL_TUNNEL.ToString();
                                    message.missionPhase = ZoneId.KUFTAL_TUNNEL.ToString() + "_1_2";
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CAPE_TERIGGAN.ToString();
                                message.missionPhase = ZoneId.KUFTAL_TUNNEL.ToString();
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.KUFTAL_TUNNEL.ToString();
                                message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                return message;
                        }
                        message.missionPhase = "WIND_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ICE_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CLOISTER_OF_FROST:
                                break;
                            case ZoneId.FEIYIN:
                                if (charaInfo.MapId == 15)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CLOISTER_OF_FROST.ToString();
                                    message.missionPhase = ZoneId.FEIYIN.ToString();
                                    return message;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FEIYIN.ToString();
                                message.missionPhase = ZoneId.FEIYIN.ToString() + "_1_2";
                                return message;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FEIYIN.ToString();
                                message.missionPhase = ZoneId.BEAUCEDINE_GLACIER.ToString();
                                return message;
                        }
                        message.missionPhase = "ICE_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIGHTNING_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.BEHEMOTHS_DOMINION:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BEHEMOTHS_DOMINION.ToString();
                                message.missionPhase = ZoneId.QUFIM_ISLAND.ToString();
                                return message;
                        }
                        message.missionPhase = "LIGHTNING_FRAGMENT";
                        return message;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIGHT_FRAGMENT))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_SANCTUARY_OF_ZITAH:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_SANCTUARY_OF_ZITAH.ToString();
                                message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                return message;
                        }
                        message.missionPhase = "LIGHT_FRAGMENT";
                        return message;
                    }
                    break;
                // 流砂洞を越えて
                case MissionZilart.THROUGH_THE_QUICKSAND_CAVES:
                    if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.OUTLANDS, (int)QuestOutlands.OPEN_SESAME))
                    {
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "OPEN_SESAME";
                        if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.OUTLANDS, (int)QuestOutlands.OPEN_SESAME))
                        {
                            message.missionPhase = "0";
                            return message;
                        }
                        if (!database.HasItem(charaInfo.CharaId, ItemId.TREMORSTONE))
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.CLOISTER_OF_TREMORS:
                                    break;
                                case ZoneId.QUICKSAND_CAVES:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CLOISTER_OF_TREMORS.ToString();
                                    message.missionPhase = ZoneId.QUICKSAND_CAVES.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                    message.missionPhase = ZoneId.EASTERN_ALTEPA_DESERT.ToString();
                                    return message;
                            }
                            message.missionPhase = "TREMORSTONE";
                            return message;
                        }
                        if (!database.HasItem(charaInfo.CharaId, ItemId.METEORITE))
                        {
                            message.missionPhase = "METEORITE";
                            return message;
                        }
                        message.missionPhase = "1";
                        return message;
                    }
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.CHAMBER_OF_ORACLES:
                            break;
                        case ZoneId.QUICKSAND_CAVES:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.CHAMBER_OF_ORACLES.ToString();
                            message.missionPhase = ZoneId.QUICKSAND_CAVES.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                            message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString() + "_2";
                            return message;
                    }
                    break;
                // 宣託の間
                case MissionZilart.THE_CHAMBER_OF_ORACLES:
                    if (zilartMission.StatusLower < 255)
                    {
                        message.missionPhase = "0";
                        return message;
                    }
                    break;
                // デルクフの塔再び
                case MissionZilart.RETURN_TO_DELKFUTTS_TOWER:
                    if (zilartMission.StatusLower == 0)
                    {
                        if (option == 3)
                        {
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.STELLAR_FULCRUM:
                                    break;
                                case ZoneId.UPPER_DELKFUTTS_TOWER:
                                    if (charaInfo.MapId == 6)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.UPPER_DELKFUTTS_TOWER.ToString() + "_6_1";
                                        return message;
                                    }
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.UPPER_DELKFUTTS_TOWER.ToString() + "_1_2";
                                        return message;
                                    }
                                    if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.UPPER_DELKFUTTS_TOWER.ToString() + "_2_3";
                                        return message;
                                    }
                                    break;
                                case ZoneId.LOWER_DELKFUTTS_TOWER:
                                    if (charaInfo.MapId == 1)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                        message.missionPhase = ZoneId.LOWER_DELKFUTTS_TOWER.ToString() + "_2_1";
                                        return message;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                    return message;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.LOWER_DELKFUTTS_TOWER.ToString();
                                    message.missionPhase = ZoneId.QUFIM_ISLAND.ToString();
                                    return message;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.STELLAR_FULCRUM.ToString();
                            message.missionPhase = ZoneId.UPPER_DELKFUTTS_TOWER.ToString();
                            return message;
                        }
                    }
                    if (zilartMission.StatusLower == 1)
                    {
                        message.missionPhase = "1";
                    }
                    break;
                // 聖地ジ・タ～滅びの神殿
                case MissionZilart.THE_TEMPLE_OF_DESOLATION:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.HALL_OF_THE_GODS:
                            break;
                        case ZoneId.ROMAEVE:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.HALL_OF_THE_GODS.ToString();
                            message.missionPhase = ZoneId.ROMAEVE.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.ROMAEVE.ToString();
                            message.missionPhase = ZoneId.THE_SANCTUARY_OF_ZITAH.ToString();
                            return message;
                    }
                    break;
                // ミスラとクリスタル
                case MissionZilart.THE_MITHRA_AND_THE_CRYSTAL:
                    if (zilartMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.RABAO:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.RABAO.ToString();
                                message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString();
                                return message;
                        }
                    }
                    if (zilartMission.StatusLower == 1)
                    {
                        if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.SCRAP_OF_PAPYRUS))
                        {
                            message.missionPhase += "end";
                            return message;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.QUICKSAND_CAVES:
                                if (charaInfo.MapId == 4)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                    message.missionPhase = ZoneId.QUICKSAND_CAVES.ToString() + "_4_7";
                                    return message;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.QUICKSAND_CAVES.ToString();
                                message.missionPhase = ZoneId.WESTERN_ALTEPA_DESERT.ToString() + "_2";
                                return message;
                        }
                    }
                    break;
                // アーク・ガーディアン
                case MissionZilart.ARK_ANGELS:
                    if (zilartMission.StatusLower == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_SHRINE_OF_RUAVITAU:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_SHRINE_OF_RUAVITAU.ToString();
                                message.missionPhase = ZoneId.RUAUN_GARDENS.ToString();
                                return message;
                        }
                    }
                    if (zilartMission.StatusLower == 1)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SHARD_OF_COWARDICE))
                        {
                            message.missionPhase = "SHARD_OF_COWARDICE";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SHARD_OF_ENVY))
                        {
                            message.missionPhase = "SHARD_OF_ENVY";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SHARD_OF_APATHY))
                        {
                            message.missionPhase = "SHARD_OF_APATHY";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SHARD_OF_ARROGANCE))
                        {
                            message.missionPhase = "SHARD_OF_ARROGANCE";
                            return message;
                        }
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SHARD_OF_RAGE))
                        {
                            message.missionPhase = "SHARD_OF_RAGE";
                            return message;
                        }
                    }
                    break;
                // 宿星の座
                case MissionZilart.THE_CELESTIAL_NEXUS:
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.THE_CELESTIAL_NEXUS:
                            break;
                        case ZoneId.THE_SHRINE_OF_RUAVITAU:
                            if (charaInfo.MapId == 1)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_SHRINE_OF_RUAVITAU.ToString();
                                message.missionPhase = ZoneId.THE_SHRINE_OF_RUAVITAU.ToString() + "_2_1";
                                return message;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.THE_CELESTIAL_NEXUS.ToString();
                            message.missionPhase = ZoneId.THE_SHRINE_OF_RUAVITAU.ToString();
                            return message;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.THE_SHRINE_OF_RUAVITAU.ToString();
                            message.missionPhase = ZoneId.RUAUN_GARDENS.ToString() + "_2";
                            return message;
                    }
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
