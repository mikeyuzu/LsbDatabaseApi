using LsbDatabaseApi.@struct;
using MySql.Data.MySqlClient;
using static LsbDatabaseApi.MessageParam;

namespace LsbDatabaseApi.mission
{
    internal partial class TutorialMission
    {
        /// <summary>
        /// チュートリアルミッション中か確認する
        /// </summary>
        /// <param name="database"></param>
        /// <param name="charaInfo"></param>
        /// <returns></returns>
        public static bool IsTutorialMission(DatabaseApi database, CharaInfo charaInfo)
        {
            bool result = false;

            // varパラメータ取得
            int TutorialProgress = database.GetVarNum(charaInfo.CharaId, "TutorialProgress");

            if (TutorialProgress > 0)
            {
                // チュートリアル進行中
                result = true;
            }

            return result;
        }

        /// <summary>
        /// チュートリアルメッセージを取得する
        /// </summary>
        /// <param name="charaInfo"></param>
        /// <returns></returns>
        public static MessageParam GetMessageTutorial(DatabaseApi database, CharaInfo charaInfo)
        {
            var message = new MessageParam();

            if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.CONQUEST_PROMOTION_VOUCHER))
            {
                // コンクェスト奨励特別券を持っている
                message.missionKind = MissionKind.Tutorial;
                message.missionType = database.GetNation(charaInfo.CharaId).ToString();
                message.missionPhase = "CONQUEST_PROMOTION_VOUCHER";
                return message;
            }

            // 冒険者優待券の状態フラグ
            bool isAdventurerCoupon = false;

            int TutorialProgress = database.GetVarNum(charaInfo.CharaId, "TutorialProgress");
            // チュートリアル前の冒険者優待券の対応中か確認する
            if (TutorialProgress == 1)
            {
                // アイテムの情報を取得する
                isAdventurerCoupon = database.HasItem(charaInfo.CharaId, ItemId.ADVENTURER_COUPON);
            }

            if (isAdventurerCoupon)
            {
                // 冒険者優待券の誘導
                message.missionKind = MissionKind.Coupon;
                message.missionType = charaInfo.ZoneId.ToString();
            }
            else
            {
                // チュートリアル中
                message.missionKind = MissionKind.Tutorial;
                message.missionType = database.GetNation(charaInfo.CharaId).ToString();
                message.missionPhase = TutorialProgress.ToString();
                if (TutorialProgress == 1)
                {
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            break;
                        case NationId.BASTOK:
                            break;
                        case NationId.WINDURST:
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.WINDURST_WATERS:
                                    if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.WINDURST_WATERS.ToString();
                                        message.missionPhase = ZoneId.WINDURST_WATERS.ToString() + charaInfo.MapId.ToString();
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                        message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                    }
                                    break;
                                case ZoneId.WINDURST_WALLS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                    message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                    break;
                                case ZoneId.PORT_WINDURST:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                    message.missionPhase = ZoneId.PORT_WINDURST.ToString();
                                    break;
                                default:
                                    break;
                            }
                            return message;
                        default:
                            break;
                    }
                }
                else if (TutorialProgress == 2)
                {
                    // シグネットを受けているか
                    var isEffect = database.IsEffect(charaInfo.CharaId, EffectId.EFFECT_SIGNET);
                    if (isEffect)
                    {
                        message.missionPhase += "end";
                    }
                }
                else if (TutorialProgress == 3)
                {
                    // 食事済みか確認
                    var isEffect = database.IsEffect(charaInfo.CharaId, EffectId.EFFECT_FOOD);
                    if (isEffect)
                    {
                        message.missionPhase += "end";
                    }
                }
                else if (TutorialProgress == 4)
                {
                    // 武器スキルが5以上か確認
                    var skillList = database.GetSkillList(charaInfo.CharaId);
                    for (var i = (int)SkillId.HAND_TO_HAND; i < (int)SkillId.MAX; i++)
                    {
                        if (skillList.TryGetValue(i, out var skill))
                        {
                            if (skill.value >= 50)
                            {
                                message.missionPhase += "end";
                                return message;
                            }
                        }
                    }
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            break;
                        case NationId.BASTOK:
                            break;
                        case NationId.WINDURST:
                            switch (charaInfo.ZoneId)
                            {
                                case ZoneId.WINDURST_WATERS:
                                    if (charaInfo.MapId == 2)
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.PORT_WINDURST.ToString();
                                        message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                    }
                                    else
                                    {
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                        message.missionPhase = ZoneId.WINDURST_WATERS.ToString();
                                    }
                                    break;
                                case ZoneId.WINDURST_WALLS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WINDURST_WOODS.ToString();
                                    message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                    break;
                                case ZoneId.PORT_WINDURST:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.WEST_SARUTABARUTA.ToString();
                                    message.missionPhase = ZoneId.PORT_WINDURST.ToString();
                                    break;
                                case ZoneId.WINDURST_WOODS:
                                    message.missionKind = MissionKind.Area;
                                    message.missionType = ZoneId.EAST_SARUTABARUTA.ToString();
                                    message.missionPhase = ZoneId.WINDURST_WOODS.ToString();
                                    break;
                                default:
                                    break;
                            }
                            return message;
                        default:
                            break;
                    }
                }
                else if (TutorialProgress == 7)
                {
                    // レベル4以上か
                    var stats = database.GetStatus(charaInfo.CharaId);
                    var job = database.GetJob(charaInfo.CharaId);
                    if (job.level[stats.mjob - 1] >= 4)
                    {
                        message.missionPhase += "end";
                    }
                }
                else if (TutorialProgress >= 8)
                {
                    bool isTrust = false;
                    switch (database.GetNation(charaInfo.CharaId))
                    {
                        case NationId.SANDORIA:
                            isTrust = database.HasQuestComplete(charaInfo.CharaId, QuestId.SANDORIA, (int)QuestSandoria.TRUST_SANDORIA);
                            break;
                        case NationId.BASTOK:
                            isTrust = database.HasQuestComplete(charaInfo.CharaId, QuestId.BASTOK, (int)QuestBastok.TRUST_BASTOK);
                            break;
                        case NationId.WINDURST:
                            isTrust = database.HasQuestComplete(charaInfo.CharaId, QuestId.WINDURST, (int)QuestWindurst.TRUST_WINDURST);
                            break;
                        default:
                            break;
                    }
                    if (!isTrust)
                    {
                        // フェイスクエストがまだの時
                        message.missionKind = MissionKind.Trust;
                        message.missionType = database.GetNation(charaInfo.CharaId).ToString();
                        // レベル5未満か
                        var stats = database.GetStatus(charaInfo.CharaId);
                        var job = database.GetJob(charaInfo.CharaId);
                        if (job.level[stats.mjob - 1] < 5)
                        {
                            message.missionPhase = "1";
                            return message;
                        }

                        // 魔法学会のカードを所持しているか
                        bool isCard = false;
                        switch (database.GetNation(charaInfo.CharaId))
                        {
                            case NationId.SANDORIA:
                                isCard = database.HasKeyItem(charaInfo.CharaId, KeyItemId.RED_INSTITUTE_CARD);
                                break;
                            case NationId.BASTOK:
                                isCard = database.HasKeyItem(charaInfo.CharaId, KeyItemId.BLUE_INSTITUTE_CARD);
                                break;
                            case NationId.WINDURST:
                                isCard = database.HasKeyItem(charaInfo.CharaId, KeyItemId.GREEN_INSTITUTE_CARD);
                                break;
                            default:
                                break;
                        }

                        if (!isCard)
                        {
                            message.missionPhase = "2";
                            return message;
                        }

                        // フェイスを取得しているか
                        var magic = database.GetCharaMagic(charaInfo.CharaId);
                        bool isTrustMagic = false;
                        switch (database.GetNation(charaInfo.CharaId))
                        {
                            case NationId.SANDORIA:
                                isTrustMagic = magic.IsMagic((int)MagicId.EXCENMILLE);
                                break;
                            case NationId.BASTOK:
                                isTrustMagic = magic.IsMagic((int)MagicId.NAJI);
                                break;
                            case NationId.WINDURST:
                                isTrustMagic = magic.IsMagic((int)MagicId.KUPIPI);
                                break;
                            default:
                                break;
                        }

                        if (!isTrustMagic)
                        {
                            switch (database.GetNation(charaInfo.CharaId))
                            {
                                case NationId.SANDORIA:
                                    break;
                                case NationId.BASTOK:
                                    break;
                                case NationId.WINDURST:
                                    switch (charaInfo.ZoneId)
                                    {
                                        case ZoneId.WINDURST_WALLS:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.HEAVENS_TOWER.ToString();
                                            message.missionPhase = ZoneId.WINDURST_WALLS.ToString();
                                            return message;
                                        case ZoneId.WINDURST_WOODS:
                                            message.missionKind = MissionKind.Area;
                                            message.missionType = ZoneId.WINDURST_WALLS.ToString();
                                            message.missionPhase = ZoneId.WINDURST_WOODS.ToString();
                                            return message;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }

                            message.missionPhase = "3";
                            return message;
                        }

                        int FirstTrustVal = 0;
                        switch (database.GetNation(charaInfo.CharaId))
                        {
                            case NationId.SANDORIA:
                                FirstTrustVal = database.GetVarNum(charaInfo.CharaId, "SandoriaFirstTrust");
                                break;
                            case NationId.BASTOK:
                                FirstTrustVal = database.GetVarNum(charaInfo.CharaId, "BastokFirstTrust");
                                break;
                            case NationId.WINDURST:
                                FirstTrustVal = database.GetVarNum(charaInfo.CharaId, "WindurstFirstTrust");
                                break;
                            default:
                                break;
                        }

                        if (FirstTrustVal == 1)
                        {
                            message.missionPhase = "4";
                            return message;
                        }
                        else if (FirstTrustVal > 1)
                        {
                            message.missionPhase = "4end";
                            return message;
                        }
                    }
                    else if (TutorialProgress == 8)
                    {
                        switch (database.GetNation(charaInfo.CharaId))
                        {
                            case NationId.SANDORIA:
                                break;
                            case NationId.BASTOK:
                                break;
                            case NationId.WINDURST:
                                switch (charaInfo.ZoneId)
                                {
                                    case ZoneId.WINDURST_WOODS:
                                    case ZoneId.WINDURST_WALLS:
                                    case ZoneId.HEAVENS_TOWER:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.EAST_SARUTABARUTA.ToString();
                                        message.missionPhase = ZoneId.WINDURST_WOODS.ToString();
                                        return message;
                                    case ZoneId.EAST_SARUTABARUTA:
                                        message.missionKind = MissionKind.Area;
                                        message.missionType = ZoneId.TAHRONGI_CANYON.ToString();
                                        message.missionPhase = ZoneId.EAST_SARUTABARUTA.ToString();
                                        return message;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (TutorialProgress == 10)
                    {
                        // レベル10以上か
                        var stats = database.GetStatus(charaInfo.CharaId);
                        var job = database.GetJob(charaInfo.CharaId);
                        if (job.level[stats.mjob - 1] >= 10)
                        {
                            message.missionPhase += "end";
                        }
                    }
                    else if (TutorialProgress == 11)
                    {
                        // ゲートクリスタルの場合、入手しているか確認
                        switch (database.GetNation(charaInfo.CharaId))
                        {
                            case NationId.SANDORIA:
                                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.HOLLA_GATE_CRYSTAL))
                                {
                                    message.missionPhase += "end";
                                }
                                break;
                            case NationId.BASTOK:
                                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.DEM_GATE_CRYSTAL))
                                {
                                    message.missionPhase += "end";
                                }
                                break;
                            case NationId.WINDURST:
                                if (database.HasKeyItem(charaInfo.CharaId, KeyItemId.MEA_GATE_CRYSTAL))
                                {
                                    message.missionPhase += "end";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return message;
        }
    }
}
