using LsbDatabaseApi.Controllers;
using LsbDatabaseApi.@struct;
using MySql.Data.MySqlClient;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class QuestMission
    {
        /// <summary>
        /// サポートジョブクエストメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetSupportJobMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            var isSupport = database.HasQuestComplete(charaInfo.CharaId, QuestId.OTHER_AREAS, (int)OtherAreas.THE_OLD_LADY);
            if (!isSupport)
            {
                // レベル18以上か
                var stats = database.GetStatus(charaInfo.CharaId);
                var job = database.GetJob(charaInfo.CharaId);
                if (job.level[stats.mjob - 1] < 18)
                {
                    message.missionKind = MissionKind.Support;
                    message.missionType = "Mhaura";
                    message.missionPhase = "1";
                    return true;
                }
                var value = database.GetVarNum(charaInfo.CharaId, "VeraOldLadyVar");
                switch (value)
                {
                    case 0:
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MHAURA:
                                break;
                            case ZoneId.BUBURIMU_PENINSULA:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MHAURA.ToString();
                                message.missionPhase = ZoneId.BUBURIMU_PENINSULA.ToString();
                                return true;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BUBURIMU_PENINSULA.ToString();
                                message.missionPhase = ZoneId.TAHRONGI_CANYON.ToString();
                                return true;
                        }

                        message.missionKind = MissionKind.Support;
                        message.missionType = "Mhaura";
                        message.missionPhase = "2";
                        return true;
                    case 1:
                        message.missionKind = MissionKind.Support;
                        message.missionType = "Mhaura";
                        if (database.HasItem(charaInfo.CharaId, ItemId.WILD_RABBIT_TAIL))
                        {
                            message.missionPhase = "4";
                        }
                        else
                        {
                            message.missionPhase = "3";
                        }
                        return true;
                    case 2:
                        message.missionKind = MissionKind.Support;
                        message.missionType = "Mhaura";
                        if (database.HasItem(charaInfo.CharaId, ItemId.CUP_OF_DHALMEL_SALIVA))
                        {
                            message.missionPhase = "6";
                        }
                        else
                        {
                            message.missionPhase = "5";
                        }
                        return true;
                    case 3:
                        message.missionKind = MissionKind.Support;
                        message.missionType = "Mhaura";
                        if (database.HasItem(charaInfo.CharaId, ItemId.BLOODY_ROBE))
                        {
                            message.missionPhase = "8";
                        }
                        else
                        {
                            message.missionPhase = "7";
                        }
                        return true;
                    default:
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// サンドリアフェイスクエストメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetTrustSandoriaMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.SANDORIA, (int)QuestSandoria.TRUST_SANDORIA))
            {
                message.missionKind = MissionKind.Trust;
                message.missionType = "SANDORIA";
                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_INSTITUTE_CARD))
                {
                    message.missionPhase = "2";
                    return true;
                }
                var magic = database.GetCharaMagic(charaInfo.CharaId);
                if (!magic.IsMagic((int)MagicId.EXCENMILLE))
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.NORTHERN_SAN_DORIA:
                            break;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.NORTHERN_SAN_DORIA.ToString();
                            message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA.ToString();
                            return true;
                    }
                    message.missionPhase = "3";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// バストゥークフェイスクエストメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetTrustBastokMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.BASTOK, (int)QuestBastok.TRUST_BASTOK))
            {
                message.missionKind = MissionKind.Trust;
                message.missionType = "BASTOK";
                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLUE_INSTITUTE_CARD))
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.PORT_BASTOK:
                            break;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.PORT_BASTOK.ToString();
                            message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                            return true;
                    }
                    message.missionPhase = "2";
                    return true;
                }
                var magic = database.GetCharaMagic(charaInfo.CharaId);
                if (!magic.IsMagic((int)MagicId.NAJI))
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.METALWORKS:
                            break;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.METALWORKS.ToString();
                            message.missionPhase = ZoneId.BASTOK_MARKETS.ToString();
                            return true;
                    }
                    message.missionPhase = "3";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// チョコボ取得クエストメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetChocobosWoundsMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.CHOCOBOS_WOUNDS))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = "CHOCOBOS_WOUNDS";
                // レベル20以上か
                var stats = database.GetStatus(charaInfo.CharaId);
                var job = database.GetJob(charaInfo.CharaId);
                if (job.level[stats.mjob - 1] < 20)
                {
                    message.missionPhase = "level";
                    return true;
                }
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.CHOCOBOS_WOUNDS))
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.UPPER_JEUNO:
                            break;
                        case ZoneId.LOWER_JEUNO:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.UPPER_JEUNO.ToString();
                            message.missionPhase = ZoneId.LOWER_JEUNO.ToString();
                            return true;
                        case ZoneId.PORT_JEUNO:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.LOWER_JEUNO.ToString();
                            message.missionPhase = ZoneId.PORT_JEUNO.ToString();
                            return true;
                        case ZoneId.SAUROMUGUE_CHAMPAIGN:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.PORT_JEUNO.ToString();
                            message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN.ToString();
                            return true;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.SAUROMUGUE_CHAMPAIGN.ToString();
                            message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                            return true;
                    }
                    message.missionPhase = "0";
                    return true;
                }
                var varProgString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.CHOCOBOS_WOUNDS}]Prog";
                var value = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = value.ToString();
                if (value == 1)
                {
                    var num = database.GetItemCount(charaInfo.CharaId, ItemId.CLUMP_OF_GAUSEBIT_WILDGRASS);
                    if (num < 4)
                    {
                        message.missionPhase = "0_CLUMP_OF_GAUSEBIT_WILDGRASS";
                    }
                    return true;
                }
                if (value > 1)
                {
                    var varTimerString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.CHOCOBOS_WOUNDS}]Timer";
                    var unixTimestamp = database.GetVarNum(charaInfo.CharaId, varTimerString);
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                    DateTimeOffset jstTime = dateTimeOffset.ToOffset(TimeSpan.FromHours(9));
                    message.missionParam = value;
                    message.missionPhase = $"{jstTime:yyyy-MM-dd HH:mm:ss}";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// フルスピードアヘッドメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetFullSpeedAheadMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.FULL_SPEED_AHEAD))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestJeuno.FULL_SPEED_AHEAD.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.FULL_SPEED_AHEAD))
                {
                    message.missionPhase = "0";
                    return true;
                }
                var foodNum = database.GetVarNum(charaInfo.CharaId, "[QUEST]FullSpeedAhead");
                if (charaInfo.ZoneId == ZoneId.UPPER_JEUNO && foodNum > 3)
                {
                    message.missionPhase = "2";
                }
                else
                {
                    message.missionPhase = "1";
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// レベルキャップ開放メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="checkCap"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetLevelCapMessage(DatabaseApi database, CharaInfo charaInfo, int checkCap, ref MessageParam message)
        {
            var job = database.GetJob(charaInfo.CharaId);
            int levelMax = 0;
            for (int i = 0; i < job.level.Count; i++)
            {
                if (job.level[i] > levelMax)
                {
                    levelMax = job.level[i];
                }
            }
            if (checkCap <= 50)
            {
                return false;
            }
            if (levelMax == 50)
            {
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.IN_DEFIANT_CHALLENGE))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestJeuno.IN_DEFIANT_CHALLENGE.ToString();
                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.IN_DEFIANT_CHALLENGE))
                    {
                        message.missionPhase = "0";
                        return true;
                    }
                    if (!database.HasItem(charaInfo.CharaId, ItemId.PIECE_OF_ANCIENT_PAPYRUS))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_ELDIEME_NECROPOLIS:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_ELDIEME_NECROPOLIS.ToString();
                                message.missionPhase = ZoneId.BATALLIA_DOWNS.ToString();
                                return true;
                        }
                        message.missionPhase = "PIECE_OF_ANCIENT_PAPYRUS";
                        return true;
                    }
                    if (!database.HasItem(charaInfo.CharaId, ItemId.CLUMP_OF_EXORAY_MOLD))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CRAWLERS_NEST:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CRAWLERS_NEST.ToString();
                                message.missionPhase = ZoneId.ROLANBERRY_FIELDS.ToString();
                                return true;
                        }
                        message.missionPhase = "CLUMP_OF_EXORAY_MOLD";
                        return true;
                    }
                    if (!database.HasItem(charaInfo.CharaId, ItemId.CHUNK_OF_BOMB_COAL))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GARLAIGE_CITADEL:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GARLAIGE_CITADEL.ToString();
                                message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN.ToString();
                                return true;
                        }
                        message.missionPhase = "CHUNK_OF_BOMB_COAL";
                        return true;
                    }
                    message.missionPhase = "1";
                    return true;
                }
            }
            if (checkCap <= 55)
            {
                return false;
            }
            if (levelMax == 55)
            {
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.ATOP_THE_HIGHEST_MOUNTAINS))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestJeuno.ATOP_THE_HIGHEST_MOUNTAINS.ToString();
                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.ATOP_THE_HIGHEST_MOUNTAINS))
                    {
                        message.missionPhase = "0";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ROUND_FRIGICITE))
                    {
                        message.missionPhase = "ROUND_FRIGICITE";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.SQUARE_FRIGICITE))
                    {
                        message.missionPhase = "SQUARE_FRIGICITE";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.TRIANGULAR_FRIGICITE))
                    {
                        message.missionPhase = "TRIANGULAR_FRIGICITE";
                        return true;
                    }
                    message.missionPhase = "1";
                    return true;
                }
            }
            if (checkCap <= 60)
            {
                return false;
            }
            if (levelMax == 60)
            {
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.WHENCE_BLOWS_THE_WIND))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestJeuno.WHENCE_BLOWS_THE_WIND.ToString();
                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.WHENCE_BLOWS_THE_WIND))
                    {
                        message.missionPhase = "0";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.ORCISH_CREST))
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.CRIMSON_ORB))
                        {
                            message.missionType = "CRIMSON_ORB";
                            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.WHITE_ORB))
                            {
                                message.missionPhase = "WHITE_ORB";
                                return true;
                            }
                            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.PINK_ORB))
                            {
                                message.missionPhase = "PINK_ORB";
                                return true;
                            }
                            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_ORB))
                            {
                                message.missionPhase = "RED_ORB";
                                return true;
                            }
                            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLOOD_ORB))
                            {
                                message.missionPhase = "BLOOD_ORB";
                                return true;
                            }
                            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.CURSED_ORB))
                            {
                                message.missionPhase = "CURSED_ORB";
                                return true;
                            }
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.MONASTIC_CAVERN:
                                    if (charaInfo.PreZoneId == ZoneId.DAVOI && charaInfo.PreCoordinates == "(H-11)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.DAVOI.ToString();
                                        message.missionPhase = ZoneId.MONASTIC_CAVERN.ToString();
                                        return true;
                                    }
                                    break;
                                case ZoneId.DAVOI:
                                    if (charaInfo.PreZoneId == ZoneId.MONASTIC_CAVERN && charaInfo.PreCoordinates == "(H-8)")
                                    {
                                        break;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.MONASTIC_CAVERN.ToString();
                                    message.missionPhase = ZoneId.DAVOI.ToString() + "_2";
                                    return true;
                                default:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.DAVOI.ToString();
                                    message.missionPhase = ZoneId.JUGNER_FOREST.ToString();
                                    return true;
                            }
                            var value = database.GetVarNum(charaInfo.CharaId, "HQuest[CrimsonOrb]Prog");
                            message.missionPhase = value.ToString();
                            return true;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MONASTIC_CAVERN:
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MONASTIC_CAVERN.ToString();
                                message.missionPhase = ZoneId.DAVOI.ToString() + "_3";
                                return true;
                        }
                        message.missionPhase = "ORCISH_CREST";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.QUADAV_CREST))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.QULUN_DOME:
                                break;
                            case ZoneId.BEADEAUX:
                                if (charaInfo.MapId == 1)
                                {
                                    if (charaInfo.PreMapId == 15 && charaInfo.PreCoordinates == "(F-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.BEADEAUX.ToString();
                                        message.missionPhase = ZoneId.BEADEAUX.ToString() + "_1_2_2";
                                        return true;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BEADEAUX.ToString();
                                    message.missionPhase = ZoneId.BEADEAUX.ToString() + "_1_2";
                                    return true;
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    if (charaInfo.PreMapId == 1 && charaInfo.PreCoordinates == "(J-6)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.BEADEAUX.ToString();
                                        message.missionPhase = ZoneId.BEADEAUX.ToString() + "_2_1_2";
                                        return true;
                                    }
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.BEADEAUX.ToString();
                                    message.missionPhase = ZoneId.BEADEAUX.ToString() + "_2_1";
                                    return true;
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BEADEAUX.ToString();
                                message.missionPhase = ZoneId.PASHHOW_MARSHLANDS.ToString();
                                return true;
                        }
                        message.missionPhase = "QUADAV_CREST";
                        return true;
                    }
                    if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.YAGUDO_CREST))
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.CASTLE_OZTROJA:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_2_3";
                                    return true;
                                }
                                else if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_3_4";
                                    return true;
                                }
                                else if (charaInfo.MapId == 3)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_4_5";
                                    return true;
                                }
                                else if (charaInfo.MapId == 4)
                                {
                                    if (charaInfo.Coordinates == "(G-7)" || charaInfo.Coordinates == "(H-7)"
                                        || charaInfo.Coordinates == "(G-8)" || charaInfo.Coordinates == "(H-8)"
                                        || charaInfo.Coordinates == "(G-9)" || charaInfo.Coordinates == "(H-9)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_5_5";
                                        return true;
                                    }
                                    break;
                                }
                                else if (charaInfo.MapId == 5)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                    message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_6_2";
                                    return true;
                                }
                                else if (charaInfo.MapId == 15)
                                {
                                    if (charaInfo.PreMapId == 1
                                        && charaInfo.PreCoordinates == "(I-8)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_0_1";
                                        return true;
                                    }
                                    if (charaInfo.PreMapId == 0
                                        && charaInfo.PreCoordinates == "(A-1)")
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString() + "_1_6";
                                        return true;
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.ALTAR_ROOM.ToString();
                                        message.missionPhase = ZoneId.CASTLE_OZTROJA.ToString();
                                        return true;
                                    }
                                }
                                break;
                            default:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CASTLE_OZTROJA.ToString();
                                message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS.ToString();
                                return true;
                        }
                        message.missionPhase = "YAGUDO_CREST";
                        return true;
                    }
                    message.missionPhase = "1";
                    return true;
                }
            }
            if (checkCap <= 65)
            {
                return false;
            }
            if (levelMax == 65)
            {
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.RIDING_ON_THE_CLOUDS))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestJeuno.RIDING_ON_THE_CLOUDS.ToString();
                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.RIDING_ON_THE_CLOUDS))
                    {
                        message.missionPhase = "0";
                        return true;
                    }
                    var npcSandoriaString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.RIDING_ON_THE_CLOUDS}]npcSandoria";
                    var npcSandoria = database.GetVarNum(charaInfo.CharaId, npcSandoriaString);
                    if (npcSandoria < 8)
                    {
                        message.missionPhase = "SANDORIA" + npcSandoria.ToString();
                        return true;
                    }
                    var npcBastokString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.RIDING_ON_THE_CLOUDS}]npcBastok";
                    var npcBastok = database.GetVarNum(charaInfo.CharaId, npcBastokString);
                    if (npcBastok < 8)
                    {
                        message.missionPhase = "BASTOK" + npcBastok.ToString();
                        return true;
                    }
                    var npcWindurstString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.RIDING_ON_THE_CLOUDS}]npcWindurst";
                    var npcWindurst = database.GetVarNum(charaInfo.CharaId, npcWindurstString);
                    if (npcWindurst < 8)
                    {
                        message.missionPhase = "WINDURST" + npcWindurst.ToString();
                        return true;
                    }
                    var npcOhterString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.RIDING_ON_THE_CLOUDS}]npcOtherlands";
                    var npcOhter = database.GetVarNum(charaInfo.CharaId, npcOhterString);
                    if (npcOhter < 8)
                    {
                        message.missionPhase = "OTHER" + npcOhter.ToString();
                        return true;
                    }
                    message.missionPhase = "1";
                    return true;
                }
            }
            if (checkCap <= 70)
            {
                return false;
            }
            if (levelMax == 70)
            {
                if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.SHATTERING_STARS))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestJeuno.SHATTERING_STARS.ToString();
                    if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.JEUNO, (int)QuestJeuno.SHATTERING_STARS))
                    {
                        message.missionPhase = "0";
                        return true;
                    }
                    var progString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.SHATTERING_STARS}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, progString);
                    if (prog == 2)
                    {
                        message.missionPhase = "3";
                        return true;
                    }
                    var valueString = $"Quest[{(int)QuestId.JEUNO}][{(int)QuestJeuno.SHATTERING_STARS}]tradedTestimony";
                    var value = database.GetVarNum(charaInfo.CharaId, valueString);
                    if (value == 0)
                    {
                        message.missionPhase = "1";
                        return true;
                    }
                    else if (value == 1)
                    {
                        message.missionPhase = "2";
                        return true;
                    }
                }
            }
            if (levelMax == 75)
            {
                if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.LIMIT_BREAKER))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = "LIMIT_BREAKER";
                    message.missionPhase = "0";
                    return true;
                }
            }
            if (checkCap <= 75)
            {
                return false;
            }
            if (levelMax == 75)
            {
            }
            if (checkCap <= 80)
            {
                return false;
            }
            if (levelMax == 80)
            {
            }
            if (checkCap <= 85)
            {
                return false;
            }
            if (levelMax == 85)
            {
            }
            if (checkCap <= 90)
            {
                return false;
            }
            if (levelMax == 90)
            {
            }
            if (checkCap <= 95)
            {
                return false;
            }
            if (levelMax == 95)
            {
            }

            return false;
        }

        /// <summary>
        /// デュナミスクエストメッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetDynamisMessage(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_BATTLE_STANDARD))
            {
                return false;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.VIAL_OF_SHROUDED_SAND))
            {
                var value = database.GetVarNum(charaInfo.CharaId, "Dynamis_Status");
                if (value <= 1)
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = "DYNAMIS";
                    message.missionPhase = value.ToString();
                    return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PRISMATIC_HOURGLASS))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = "DYNAMIS";
                message.missionPhase = "2";
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_COMMAND_SCEPTER))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_SAN_DORIA:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_SAN_DORIA_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_SAN_DORIA_1";
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_EYEGLASS))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_BASTOK:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_BASTOK_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_BASTOK_1";
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_LANTERN))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_WINDURST:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_WINDURST_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_WINDURST_1";
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_TACTICAL_MAP))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_JEUNO:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_JEUNO_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_JEUNO_1";
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_INSIGNIA))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_BEAUCEDINE:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_BEAUCEDINE_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_BEAUCEDINE_1";
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.HYDRA_CORPS_BATTLE_STANDARD))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.DYNAMIS_XARCABARD:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_XARCABARD_2";
                        return true;
                    default:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = "DYNAMIS";
                        message.missionPhase = "DYNAMIS_XARCABARD_1";
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア1メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria1Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.CLAWS_OF_THE_GRIFFON))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.GIFTS_OF_THE_GRIFFON))
            {
                // Q2 オーク軍団掃討作戦
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.CLAWS_OF_THE_GRIFFON.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.CLAWS_OF_THE_GRIFFON))
                {
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                var progString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.CLAWS_OF_THE_GRIFFON}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, progString);
                message.missionPhase = prog.ToString();

                if (prog == 1)
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.JUGNER_FOREST_S:
                            return true;
                        case ZoneId.EAST_RONFAURE_S:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.JUGNER_FOREST_S.ToString();
                            message.missionPhase = ZoneId.EAST_RONFAURE_S.ToString();
                            return true;
                        default:
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.EAST_RONFAURE_S))
                            {
                            message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }                            
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                            message.missionPhase = ZoneId.SOUTHERN_SAN_DORIA_S.ToString();
                            return true;
                    }
                }

                return true;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.STEAMED_RAMS) || true)
            {
                // Q1 少年たちの贈り物
                var progString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.GIFTS_OF_THE_GRIFFON}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, progString);
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.GIFTS_OF_THE_GRIFFON.ToString();
                message.missionPhase = prog.ToString();

                if (prog == 2)
                {
                    var optionString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.GIFTS_OF_THE_GRIFFON}]Option";
                    var option = database.GetVarNum(charaInfo.CharaId, optionString);
                    if (database.HasItem(charaInfo.CharaId, ItemId.PLUME_DOR))
                    {
                        if ((option & 1) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_1";
                        }
                        else if ((option & 2) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_2";
                        }
                        else if ((option & 4) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_4";
                        }
                        else if ((option & 8) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_8";
                        }
                        else if ((option & 16) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_16";
                        }
                        else if ((option & 32) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_32";
                        }
                        else if ((option & 64) == 0)
                        {
                            message.missionPhase = ItemId.PLUME_DOR.ToString() + "_64";
                        }
                    }
                    else if (option == 127)
                    {
                        message.missionPhase = ItemId.PLUME_DOR.ToString() + "_end";
                    }
                }
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_RECOMMENDATION_LETTER))
            {
                // 紅の紹介状
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.GARLAIGE_CITADEL_S:
                        if (charaInfo.MapId == 1)
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.GARLAIGE_CITADEL_S.ToString();
                            message.missionPhase = ZoneId.GARLAIGE_CITADEL_S.ToString() + "_1_2";
                            return true;
                        }
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                        message.missionPhase = "ACCEPT";
                        return true;
                    case ZoneId.SAUROMUGUE_CHAMPAIGN_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.GARLAIGE_CITADEL_S.ToString();
                        message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                        return true;
                    case ZoneId.ROLANBERRY_FIELDS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                        message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        return true;
                    case ZoneId.BATALLIA_DOWNS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                        return true;
                    default:
                        return true;
                }
            }
            // 王立騎士団入団試験
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.STEAMED_RAMS))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.SOUTHERN_SAN_DORIA_S:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                        message.missionPhase = "0";
                        return true;
                    case ZoneId.EAST_RONFAURE_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.SOUTHERN_SAN_DORIA_S.ToString();
                        message.missionPhase = ZoneId.EAST_RONFAURE_S.ToString();
                        return true;
                    case ZoneId.JUGNER_FOREST_S:
                        if (charaInfo.PreZoneId == ZoneId.BATALLIA_DOWNS_S)
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.VUNKERL_INLET_S.ToString();
                            message.missionPhase = ZoneId.JUGNER_FOREST_S.ToString();
                            return true;
                        }
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                        message.missionPhase = ZoneId.JUGNER_FOREST_S.ToString();
                        return true;
                    case ZoneId.VUNKERL_INLET_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.JUGNER_FOREST_S.ToString();
                        message.missionPhase = ZoneId.VUNKERL_INLET_S.ToString();
                        return true;
                    case ZoneId.BATALLIA_DOWNS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.JUGNER_FOREST_S.ToString();
                        message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                        return true;
                    case ZoneId.ROLANBERRY_FIELDS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.BATALLIA_DOWNS_S.ToString();
                        message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        return true;
                    case ZoneId.SAUROMUGUE_CHAMPAIGN_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                        return true;
                    case ZoneId.GARLAIGE_CITADEL_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                        message.missionPhase = ZoneId.GARLAIGE_CITADEL_S.ToString();
                        return true;
                    default:
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.CHARRED_PROPELLER))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "CHARRED_PROPELLER";
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PIECE_OF_SHATTERED_LUMBER))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "PIECE_OF_SHATTERED_LUMBER";
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.OXIDIZED_PLATE))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "OXIDIZED_PLATE";
                return true;
            }
            message.missionKind = MissionKind.Quest;
            message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
            message.missionPhase = "1";
            return true;
        }

        /// <summary>
        /// アルタナ連続クエストバストゥーク1メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok1Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.FIRES_OF_DISCONTENT))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BETTER_PART_OF_VALOR))
            {
                // Q2 静かなる警鐘
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.FIRES_OF_DISCONTENT.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.FIRES_OF_DISCONTENT))
                {
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                var prog = database.GetVarNum(charaInfo.CharaId, "FiresOfDiscProg");
                message.missionPhase = prog.ToString();

                if (prog == 3)
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.GRAUBERG_S:
                            return true;
                        default:
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.GRAUBERG_S))
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GRAUBERG_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            return true;
                    }
                }

                return true;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_FIGHTING_FOURTH) || true)
            {
                // Q1 沈黙の契約 胎動
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.BETTER_PART_OF_VALOR.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BETTER_PART_OF_VALOR))
                {
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                var prog = database.GetVarNum(charaInfo.CharaId, "BetterPartOfValProg");
                message.missionPhase = prog.ToString();

                if (prog == 1)
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.NORTH_GUSTABERG_S:
                            return true;
                        default:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.NORTH_GUSTABERG_S.ToString();
                            message.missionPhase = ZoneId.BASTOK_MARKETS_S.ToString();
                            return true;
                    }
                }
                else if (prog == 3)
                {
                    if (!database.HasItem(charaInfo.CharaId, ItemId.GNOLE_CLAW))
                    {
                        message.missionPhase = ItemId.GNOLE_CLAW.ToString();
                        return true;
                    }
                }
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLUE_RECOMMENDATION_LETTER))
            {
                // 蒼の紹介状
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.THE_ELDIEME_NECROPOLIS_S:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.THE_FIGHTING_FOURTH.ToString();
                        message.missionPhase = "ACCEPT";
                        return true;
                    case ZoneId.BATALLIA_DOWNS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString();
                        message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                        return true;
                    default:
                        return true;
                }
            }
            // 第四共和軍団入団試験
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_FIGHTING_FOURTH))
            {
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.BASTOK_MARKETS_S:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.THE_FIGHTING_FOURTH.ToString();
                        message.missionPhase = "0";
                        return true;
                    case ZoneId.NORTH_GUSTABERG_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.BASTOK_MARKETS_S.ToString();
                        message.missionPhase = ZoneId.NORTH_GUSTABERG_S.ToString();
                        return true;
                    case ZoneId.GRAUBERG_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.NORTH_GUSTABERG_S.ToString();
                        message.missionPhase = ZoneId.GRAUBERG_S.ToString();
                        return true;
                    case ZoneId.PASHHOW_MARSHLANDS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.GRAUBERG_S.ToString();
                        message.missionPhase = ZoneId.PASHHOW_MARSHLANDS_S.ToString();
                        return true;
                    case ZoneId.ROLANBERRY_FIELDS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.PASHHOW_MARSHLANDS_S.ToString();
                        message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        return true;
                    case ZoneId.BATALLIA_DOWNS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                        return true;
                    case ZoneId.THE_ELDIEME_NECROPOLIS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.BATALLIA_DOWNS_S.ToString();
                        message.missionPhase = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString();
                        return true;
                    default:
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.CHARRED_PROPELLER))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "CHARRED_PROPELLER";
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.PIECE_OF_SHATTERED_LUMBER))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "PIECE_OF_SHATTERED_LUMBER";
                return true;
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.OXIDIZED_PLATE))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
                message.missionPhase = "OXIDIZED_PLATE";
                return true;
            }
            message.missionKind = MissionKind.Quest;
            message.missionType = QuestCrystalWar.STEAMED_RAMS.ToString();
            message.missionPhase = "1";
            return true;
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス1メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst1Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_TIGRESS_STRIKES))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_TIGRESS_STIRS))
            {
                // Q2 禍つ闇、襲来
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.THE_TIGRESS_STRIKES.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_TIGRESS_STRIKES))
                {
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                var TigressStrikesProg = database.GetVarNum(charaInfo.CharaId, "TigressStrikesProg");
                message.missionPhase = TigressStrikesProg.ToString();

                if (TigressStrikesProg == 0)
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.FORT_KARUGO_NARUGO_S:
                            if (charaInfo.MapId == 1)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString() + "_1_2";
                            }
                            return true;
                        case ZoneId.WEST_SARUTABARUTA_S:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                            message.missionPhase = ZoneId.WEST_SARUTABARUTA_S.ToString();
                            return true;
                        default:
                            message.missionKind = MissionKind.Area;
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.FORT_KARUGO_NARUGO_S))
                            {
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.WEST_SARUTABARUTA_S))
                            {
                                message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                            message.missionPhase = ZoneId.WINDURST_WATERS_S.ToString();
                            return true;
                    }
                }
                else if (TigressStrikesProg == 1)
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.FORT_KARUGO_NARUGO_S:
                            if (charaInfo.MapId == 2)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString() + "_2_1";
                            }
                            return true;
                        default:
                            return true;
                    }
                }
                else if (TigressStrikesProg == 2)
                {
                    var WarLynxKilled = database.GetVarNum(charaInfo.CharaId, "WarLynxKilled");
                    if (WarLynxKilled > 0)
                    {
                        message.missionPhase += "_kill";
                        return true;
                    }
                }
                
                return true;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.SNAKE_ON_THE_PLAINS))
            {
                // Q1 牙持つ乙女
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.THE_TIGRESS_STIRS.ToString();
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.THE_TIGRESS_STIRS))
                {
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.SMALL_STARFRUIT))
                {
                    message.missionPhase = "0";
                    return true;
                }

                switch (charaInfo.ZoneId)
                {
                    case ZoneId.WEST_SARUTABARUTA_S:
                        message.missionPhase = KeyItemId.SMALL_STARFRUIT.ToString();
                        return true;
                    default:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                        message.missionPhase = ZoneId.WINDURST_WATERS_S.ToString();
                        return true;
                }
            }
            if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.GREEN_RECOMMENDATION_LETTER))
            {
                // 翠の紹介状
                switch (charaInfo.ZoneId)
                {
                    case ZoneId.CRAWLERS_NEST_S:
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.SNAKE_ON_THE_PLAINS.ToString();
                        message.missionPhase = "ACCEPT";
                        return true;
                    case ZoneId.ROLANBERRY_FIELDS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.CRAWLERS_NEST_S.ToString();
                        message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        return true;
                    case ZoneId.BATALLIA_DOWNS_S:
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                        message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                        return true;
                    default:
                        return true;
                }
            }
            // コブラ傭兵団入団試験
            var GREEN_R_LETTER_USED = database.GetVarNum(charaInfo.CharaId, "GREEN_R_LETTER_USED");
            if (GREEN_R_LETTER_USED == 1)
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.SNAKE_ON_THE_PLAINS.ToString();
                var SEALED_DOORS = database.GetVarNum(charaInfo.CharaId, "SEALED_DOORS");
                if ((SEALED_DOORS & 2) == 0)
                {
                    message.missionPhase = "SEALED_DOORS_2";
                    return true;
                }
                else if ((SEALED_DOORS & 1) == 0)
                {
                    message.missionPhase = "SEALED_DOORS_1";
                    return true;
                }
                else if ((SEALED_DOORS & 4) == 0)
                {
                    message.missionPhase = "SEALED_DOORS_4";
                    return true;
                }
                else
                {
                    message.missionPhase = GREEN_R_LETTER_USED.ToString();
                    return true;
                }
            }
            switch (charaInfo.ZoneId)
            {
                case ZoneId.WINDURST_WATERS_S:
                    if (!database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.SNAKE_ON_THE_PLAINS))
                    {
                        message.missionKind = MissionKind.Quest;
                        message.missionType = QuestCrystalWar.SNAKE_ON_THE_PLAINS.ToString();
                        message.missionPhase = GREEN_R_LETTER_USED.ToString();
                        return true;
                    }
                    break;
                case ZoneId.WEST_SARUTABARUTA_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.WINDURST_WATERS_S.ToString();
                    message.missionPhase = ZoneId.WEST_SARUTABARUTA_S.ToString();
                    return true;
                case ZoneId.FORT_KARUGO_NARUGO_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                    message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                    return true;
                case ZoneId.MERIPHATAUD_MOUNTAINS_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                    message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS_S.ToString();
                    return true;
                case ZoneId.SAUROMUGUE_CHAMPAIGN_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.MERIPHATAUD_MOUNTAINS_S.ToString();
                    message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                    return true;
                case ZoneId.ROLANBERRY_FIELDS_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                    message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                    return true;
                case ZoneId.CRAWLERS_NEST_S:
                    message.missionKind = MissionKind.Area;
                    message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                    message.missionPhase = ZoneId.CRAWLERS_NEST_S.ToString();
                    return true;
                default:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア2メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria2Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.WRATH_OF_THE_GRIFFON))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BOY_AND_THE_BEAST))
            {
                // Q4 ちいさな勝利、ひとつの決意
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.WRATH_OF_THE_GRIFFON))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.WRATH_OF_THE_GRIFFON.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.WRATH_OF_THE_GRIFFON.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.WRATH_OF_THE_GRIFFON}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            else
            {
                // Q3 巨人偵察作戦II（ツー）
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.BOY_AND_THE_BEAST.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.BOY_AND_THE_BEAST}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();

                if (prog == 2 && database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BOY_AND_THE_BEAST))
                {
                    message.missionPhase = "ACCEPT";
                }

                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストバストゥーク2メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok2Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BURDEN_OF_SUSPICION))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.LIGHT_IN_THE_DARKNESS))
            {
                // Q4 新たなる猜疑
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.BURDEN_OF_SUSPICION))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.BURDEN_OF_SUSPICION.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.BURDEN_OF_SUSPICION.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.BURDEN_OF_SUSPICION}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();

                    if (prog == 1)
                    {
                        if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.THE_ELDIEME_NECROPOLIS_S)
                            && charaInfo.ZoneId != ZoneId.THE_ELDIEME_NECROPOLIS_S)
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                            message.missionPhase = "SURVIVAL";
                            return true;
                        }
                        if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.ROLANBERRY_FIELDS_S)
                            && (charaInfo.ZoneId != ZoneId.THE_ELDIEME_NECROPOLIS_S
                            && charaInfo.ZoneId != ZoneId.ROLANBERRY_FIELDS_S))
                        {
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                            message.missionPhase = "SURVIVAL";
                            return true;
                        }
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.THE_ELDIEME_NECROPOLIS_S:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString();
                                    message.missionPhase = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString() + "_1_2";
                                    return true;
                                }
                                if (charaInfo.MapId == 2)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString();
                                    message.missionPhase = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString() + "_2_3";
                                    return true;
                                }
                                break;
                            case ZoneId.BATALLIA_DOWNS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.THE_ELDIEME_NECROPOLIS_S.ToString();
                                message.missionPhase = ZoneId.BATALLIA_DOWNS_S.ToString();
                                return true;
                            case ZoneId.ROLANBERRY_FIELDS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.BATALLIA_DOWNS_S.ToString();
                                message.missionPhase = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                                return true;
                            case ZoneId.PASHHOW_MARSHLANDS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.ROLANBERRY_FIELDS_S.ToString();
                                message.missionPhase = ZoneId.PASHHOW_MARSHLANDS_S.ToString();
                                return true;
                            case ZoneId.GRAUBERG_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.PASHHOW_MARSHLANDS_S.ToString();
                                message.missionPhase = ZoneId.GRAUBERG_S.ToString();
                                return true;
                            case ZoneId.NORTH_GUSTABERG_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GRAUBERG_S.ToString();
                                message.missionPhase = ZoneId.NORTH_GUSTABERG_S.ToString();
                                return true;
                            case ZoneId.BASTOK_MARKETS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.NORTH_GUSTABERG_S.ToString();
                                message.missionPhase = ZoneId.BASTOK_MARKETS_S.ToString();
                                return true;
                            default:
                                return true;
                        }

                    }

                    return true;
                }
            }
            // Q3 解明への灯
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.LIGHT_IN_THE_DARKNESS))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.LIGHT_IN_THE_DARKNESS.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.LIGHT_IN_THE_DARKNESS.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.LIGHT_IN_THE_DARKNESS}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス2メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst2Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.A_MANIFEST_PROBLEM))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.KNOT_QUITE_THERE))
            {
                // Q4 降臨、異貌の徒
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.A_MANIFEST_PROBLEM))
                {
                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.FORT_KARUGO_NARUGO_S:
                            if (charaInfo.MapId == 1)
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString() + "_1_2";
                                return true;
                            }
                            message.missionKind = MissionKind.Quest;
                            message.missionType = QuestCrystalWar.A_MANIFEST_PROBLEM.ToString();
                            message.missionPhase = "ACCEPT";
                            return true;
                        case ZoneId.WEST_SARUTABARUTA_S:
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                            message.missionPhase = ZoneId.WEST_SARUTABARUTA_S.ToString();
                            return true;
                        default:
                            message.missionKind = MissionKind.Area;
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.FORT_KARUGO_NARUGO_S))
                            {
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.WEST_SARUTABARUTA_S))
                            {
                                message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                            message.missionPhase = ZoneId.WINDURST_WATERS_S.ToString();
                            return true;
                    }
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.A_MANIFEST_PROBLEM.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.A_MANIFEST_PROBLEM}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();

                    if (prog == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.MERIPHATAUD_MOUNTAINS_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.CASTLE_OZTROJA_S.ToString();
                                message.missionPhase = ZoneId.MERIPHATAUD_MOUNTAINS_S.ToString();
                                return true;
                            case ZoneId.FORT_KARUGO_NARUGO_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.MERIPHATAUD_MOUNTAINS_S.ToString();
                                message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                return true;
                            case ZoneId.WEST_SARUTABARUTA_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                return true;
                            default:
                                message.missionKind = MissionKind.Area;
                                if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.MERIPHATAUD_MOUNTAINS_S))
                                {
                                    message.missionType = ZoneId.MERIPHATAUD_MOUNTAINS_S.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return true;
                                }
                                if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.FORT_KARUGO_NARUGO_S))
                                {
                                    message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return true;
                                }
                                if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.WEST_SARUTABARUTA_S))
                                {
                                    message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return true;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                message.missionPhase = ZoneId.WINDURST_WATERS_S.ToString();
                                return true;
                        }
                    }
                    else if (prog == 1)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.FORT_KARUGO_NARUGO_S:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                    message.missionPhase = ZoneId.FORT_KARUGO_NARUGO_S.ToString() + "_1_2";
                                }
                                return true;
                            case ZoneId.WEST_SARUTABARUTA_S:
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                message.missionPhase = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                return true;
                            default:
                                message.missionKind = MissionKind.Area;
                                if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.FORT_KARUGO_NARUGO_S))
                                {
                                    message.missionType = ZoneId.FORT_KARUGO_NARUGO_S.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return true;
                                }
                                if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.WEST_SARUTABARUTA_S))
                                {
                                    message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                    message.missionPhase = "SURVIVAL";
                                    return true;
                                }
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.WEST_SARUTABARUTA_S.ToString();
                                message.missionPhase = ZoneId.WINDURST_WATERS_S.ToString();
                                return true;
                        }
                    }

                    return true;
                }
            }
            // Q3 憂国の使者
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.KNOT_QUITE_THERE))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.KNOT_QUITE_THERE.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.KNOT_QUITE_THERE.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.KNOT_QUITE_THERE}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();

                if (prog == 1 && !database.HasItem(charaInfo.CharaId, ItemId.ONE_HUNDRED_EIGHT_KNOT_QUIPU))
                {
                    message.missionPhase = ItemId.ONE_HUNDRED_EIGHT_KNOT_QUIPU.ToString();
                }
                else if (prog == 2)
                {
                    if ((charaInfo.ZoneId != ZoneId.EAST_RONFAURE_S
                        || charaInfo.ZoneId != ZoneId.SOUTHERN_SAN_DORIA_S)
                        && database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.EAST_RONFAURE_S))
                    {
                        message.missionKind = MissionKind.Area;
                        message.missionType = ZoneId.EAST_RONFAURE_S.ToString();
                        message.missionPhase = "SURVIVAL";
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア3メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria3Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.IN_A_HAZE_OF_GLORY))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.PERILS_OF_THE_GRIFFON))
            {
                // Q6 それぞれの死地へ
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.IN_A_HAZE_OF_GLORY))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.IN_A_HAZE_OF_GLORY.ToString();
                    message.missionPhase = "ACCEPT";

                    switch (charaInfo.ZoneId)
                    {
                        case ZoneId.GARLAIGE_CITADEL_S:
                            return true;
                        default:
                            if (database.IsSurvivalOpen(charaInfo.CharaId, SurvivalId.GARLAIGE_CITADEL_S))
                            {
                                message.missionKind = MissionKind.Area;
                                message.missionType = ZoneId.GARLAIGE_CITADEL_S.ToString();
                                message.missionPhase = "SURVIVAL";
                                return true;
                            }
                            message.missionKind = MissionKind.Area;
                            message.missionType = ZoneId.GARLAIGE_CITADEL_S.ToString();
                            message.missionPhase = ZoneId.SAUROMUGUE_CHAMPAIGN_S.ToString();
                            return true;
                    }
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.IN_A_HAZE_OF_GLORY.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.IN_A_HAZE_OF_GLORY}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();

                    if (prog == 0)
                    {
                        switch (charaInfo.ZoneId)
                        {
                            case ZoneId.GARLAIGE_CITADEL_S:
                                if (charaInfo.MapId == 1)
                                {
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.GARLAIGE_CITADEL_S.ToString();
                                    message.missionPhase = ZoneId.GARLAIGE_CITADEL_S.ToString() + "_1_2";
                                }
                                return true;
                            default:
                                return true;
                        }
                    }
                    else if (prog == 1)
                    {
                        if (!database.HasKeyItem(charaInfo.CharaId, KeyItemId.NUMBER_EIGHT_SHELTER_KEY)
                            && charaInfo.ZoneId != ZoneId.GHOYUS_REVERIE)
                        {
                            message.missionPhase = KeyItemId.NUMBER_EIGHT_SHELTER_KEY.ToString();
                        }
                    }
                    
                    return true;
                }
            }
            // Q5 羽撃け、鷲獅子
            if (database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.PERILS_OF_THE_GRIFFON))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.PERILS_OF_THE_GRIFFON.ToString();
                message.missionPhase = "ACCEPT";

                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.ORCISH_WARMACHINE_BODY))
                {
                    message.missionPhase = KeyItemId.ORCISH_WARMACHINE_BODY.ToString();
                }

                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.PERILS_OF_THE_GRIFFON.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.PERILS_OF_THE_GRIFFON}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストバストゥークサンドリア3メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok3Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.FIRE_IN_THE_HOLE))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.STORM_ON_THE_HORIZON))
            {
                // Q6 隠滅の炎
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.FIRE_IN_THE_HOLE))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.FIRE_IN_THE_HOLE.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.FIRE_IN_THE_HOLE.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.FIRE_IN_THE_HOLE}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q5 騒乱の行方
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.STORM_ON_THE_HORIZON))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STORM_ON_THE_HORIZON.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.STORM_ON_THE_HORIZON.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.STORM_ON_THE_HORIZON}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス3メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst3Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.A_FEAST_FOR_GNATS))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.WHEN_ONE_MAN_IS_NOT_ENOUGH))
            {
                // Q6 淑女たちの饗宴
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.A_FEAST_FOR_GNATS))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.A_FEAST_FOR_GNATS.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.A_FEAST_FOR_GNATS.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.A_FEAST_FOR_GNATS}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q5 勃発、ミスラ大戦
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.WHEN_ONE_MAN_IS_NOT_ENOUGH))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.WHEN_ONE_MAN_IS_NOT_ENOUGH.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.WHEN_ONE_MAN_IS_NOT_ENOUGH.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.WHEN_ONE_MAN_IS_NOT_ENOUGH}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア4メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria4Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                // Q8
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q8}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q7
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q7}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストバストゥーク4メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok4Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                // Q8
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q8}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q7
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q7}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス4メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst4Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                // Q8
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q8))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q8.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q8}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q7
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q7))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q7.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q7}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア5メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria5Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                // Q10
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q10}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q9
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q9}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストバストゥーク5メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok5Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                // Q10
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q10}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q9
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q9}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス5メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst5Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                // Q10
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q10))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q10.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q10}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q9
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q9))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q9.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q9}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストサンドリア6メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestSandoria6Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                // Q12
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q12}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q11
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q11}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストバストゥーク6メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestBastok6Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                // Q12
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q12}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q11
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q11}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }

        /// <summary>
        /// アルタナ連続クエストウィンダス6メッセージ取得
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool GetWOTGQuestWindurst6Message(DatabaseApi database, CharaInfo charaInfo, ref MessageParam message)
        {
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
            {
                return false;
            }
            if (database.HasQuestComplete(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                // Q12
                if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q12))
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    message.missionPhase = "ACCEPT";
                    return true;
                }
                else
                {
                    message.missionKind = MissionKind.Quest;
                    message.missionType = QuestCrystalWar.Q12.ToString();
                    var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q12}]Prog";
                    var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                    message.missionPhase = prog.ToString();
                    return true;
                }
            }
            // Q11
            if (!database.HasQuestCurrent(charaInfo.CharaId, QuestId.CRYSTAL_WAR, (int)QuestCrystalWar.Q11))
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                message.missionPhase = "ACCEPT";
                return true;
            }
            else
            {
                message.missionKind = MissionKind.Quest;
                message.missionType = QuestCrystalWar.Q11.ToString();
                var varProgString = $"Quest[{(int)QuestId.CRYSTAL_WAR}][{(int)QuestCrystalWar.Q11}]Prog";
                var prog = database.GetVarNum(charaInfo.CharaId, varProgString);
                message.missionPhase = prog.ToString();
                return true;
            }
        }
    }
}
