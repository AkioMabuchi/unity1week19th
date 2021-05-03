using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionQuery
{
    public QueryType type;
    public QueryElement element;
    public int value;
    public bool isPlayer;
}

public enum QueryType
{
    Nothing,
    Guard,
    MagicReflector,
    RecoveryBlock,
    SlashAttack,
    MagicAttack,
    Damage,
    UsePotion,
    Debuff,
    Recovery,
    Enchant,
}

public enum QueryElement
{
    Normal,
    Fire,
    Thunder,
    Ice
}

[Serializable]
public class QueryInformation
{
    public int card1;
    public int card2;
    public FighterSword sword;
    public FighterCondition condition;
}
public class ActionQueryModel : SingletonMonoBehaviour<ActionQueryModel>
{
    private readonly int[] _conditionBurned = {0, 0};
    private readonly int[] _conditionParalysis = {0, 0};
    private readonly int[] _conditionFrozen = {0, 0};
    public List<ActionQuery> GenerateActionQueries(QueryInformation playerQueryInformation, QueryInformation opponentQueryInformation)
    {
        List<ActionQuery> queries = new List<ActionQuery>();

        int[] opposite = {1, 0};
        int[][] cards =
        {
            new[] {playerQueryInformation.card1, playerQueryInformation.card2},
            new[] {opponentQueryInformation.card1, opponentQueryInformation.card2}
        };

        FighterSword[] swords = {playerQueryInformation.sword, opponentQueryInformation.sword};
        FighterCondition[] conditions = {playerQueryInformation.condition, opponentQueryInformation.condition};

        bool[] hasUsedSlashAttack = new bool[2];
        bool[] hasUsedMagicAttack = new bool[2];
        bool[] hasUsedFireEnchant = new bool[2];
        bool[] hasUsedThunderEnchant = new bool[2];
        bool[] hasUsedIceEnchant = new bool[2];
        bool[] hasUsedFireAttack = new bool[2];
        bool[] hasUsedThunderAttack = new bool[2];
        bool[] hasUsedIceAttack = new bool[2];
        bool[] hasUsedGuard = new bool[2];
        bool[] hasUsedMagicReflector = new bool[2];
        bool[] hasUsedRecoveryBlock = new bool[2];
        bool[] hasUsedRecovery = new bool[2];

        int[] powerOfSlashAttack = new int[2];
        int[] powerOfFireSlashAttack = new int[2];
        int[] powerOfThunderSlashAttack = new int[2];
        int[] powerOfIceSlashAttack = new int[2];
        int[] powerOfFireMagicAttack = new int[2];
        int[] powerOfThunderMagicAttack = new int[2];
        int[] powerOfIceMagicAttack = new int[2];
        int[] valueOfRecovery = new int[2];

        FighterSword[] currentSword = new FighterSword[2];
        
        for (int i = 0; i < 2; i++)
        {
            hasUsedSlashAttack[i] = false;
            hasUsedMagicAttack[i] = false;
            hasUsedFireEnchant[i] = false;
            hasUsedThunderEnchant[i] = false;
            hasUsedIceEnchant[i] = false;
            hasUsedFireAttack[i] = false;
            hasUsedThunderAttack[i] = false;
            hasUsedIceAttack[i] = false;
            hasUsedGuard[i] = false;
            hasUsedMagicReflector[i] = false;
            hasUsedRecovery[i] = false;

            powerOfSlashAttack[i] = 0;
            powerOfFireSlashAttack[i] = 0;
            powerOfThunderSlashAttack[i] = 0;
            powerOfIceSlashAttack[i] = 0;
            powerOfFireMagicAttack[i] = 0;
            powerOfThunderMagicAttack[i] = 0;
            powerOfIceMagicAttack[i] = 0;

            currentSword[i] = swords[i];

            if (conditions[i] == FighterCondition.Burned)
            {
                powerOfSlashAttack[i] = -1;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                switch (cards[i][j])
                {
                    case 0: // 斬撃
                        hasUsedSlashAttack[i] = true;
                        powerOfSlashAttack[i] += 1;
                        break;
                    case 1: // 炎エンチャント
                        hasUsedFireEnchant[i] = true;
                        break;
                    case 2: // 雷エンチャント
                        hasUsedThunderEnchant[i] = true;
                        break;
                    case 3: // 氷エンチャント
                        hasUsedIceEnchant[i] = true;
                        break;
                    case 4: // 火炎斬
                        hasUsedSlashAttack[i] = true;
                        hasUsedFireAttack[i] = true;
                        powerOfFireSlashAttack[i] += 2;
                        break;
                    case 5: // 雷撃斬
                        hasUsedSlashAttack[i] = true;
                        hasUsedThunderAttack[i] = true;
                        powerOfThunderSlashAttack[i] += 2;
                        break;
                    case 6: // 氷雪斬
                        hasUsedSlashAttack[i] = true;
                        hasUsedIceAttack[i] = true;
                        powerOfIceSlashAttack[i] += 2;
                        break;
                    case 7: // 防御
                        hasUsedGuard[i] = true;
                        break;
                    case 8: // 二段斬り
                        hasUsedSlashAttack[i] = true;
                        powerOfSlashAttack[i] += 2;
                        break;
                    case 9: // 奥義
                        hasUsedSlashAttack[i] = true;
                        powerOfSlashAttack[i] += 3;
                        break;
                    case 10: // マジックリフレクター
                        hasUsedMagicReflector[i] = true;
                        break;
                    case 11: // ポーションA
                        hasUsedRecovery[i] = true;
                        valueOfRecovery[i] += 1;
                        break;
                    case 12: // ポーションB
                        hasUsedRecovery[i] = true;
                        valueOfRecovery[i] += 2;
                        break;
                    case 13: // ポーションC
                        hasUsedRecovery[i] = true;
                        valueOfRecovery[i] += 3;
                        break;
                    case 14: // 回復封じ
                        hasUsedRecoveryBlock[i] = true;
                        break;
                    case 15: // フレイム
                        hasUsedMagicAttack[i] = true;
                        hasUsedFireAttack[i] = true;
                        powerOfFireMagicAttack[i] += 3;
                        break;
                    case 16: // サンダー
                        hasUsedMagicAttack[i] = true;
                        hasUsedThunderAttack[i] = true;
                        powerOfThunderMagicAttack[i] += 3;
                        break;
                    case 17: // ブリザード
                        hasUsedMagicAttack[i] = true;
                        hasUsedIceAttack[i] = true;
                        powerOfIceMagicAttack[i] += 3;
                        break;
                }
            }
        }

        for (int phase = 0; phase < 10; phase++)
        {
            for (int i = 0; i < 2; i++)
            {
                QueryElement element = QueryElement.Normal;
                switch (phase)
                {
                    case 1: // 優先エンチャント
                        bool isPriorityEnchant = false;
                        if (hasUsedFireEnchant[i] && hasUsedFireAttack[i] && currentSword[i] != FighterSword.Fire)
                        {
                            isPriorityEnchant = true;
                            hasUsedFireEnchant[i] = false;
                            element = QueryElement.Fire;
                        }
                        else if (hasUsedThunderEnchant[i] && hasUsedThunderAttack[i] &&
                                 currentSword[i] != FighterSword.Thunder)
                        {
                            isPriorityEnchant = true;
                            hasUsedThunderEnchant[i] = false;
                            element = QueryElement.Thunder;
                        }
                        else if (hasUsedIceEnchant[i] && hasUsedIceAttack[i] && currentSword[i] != FighterSword.Ice)
                        {
                            isPriorityEnchant = true;
                            hasUsedIceEnchant[i] = false;
                            element = QueryElement.Ice;
                        }

                        if (isPriorityEnchant)
                        {
                            queries.Add(new ActionQuery
                            {
                                type = QueryType.Enchant,
                                isPlayer = i == 0,
                                element = element
                            });
                            switch (element)
                            {
                                case QueryElement.Fire:
                                    currentSword[i] = FighterSword.Fire;
                                    break;
                                case QueryElement.Thunder:
                                    currentSword[i] = FighterSword.Thunder;
                                    break;
                                case QueryElement.Ice:
                                    currentSword[i] = FighterSword.Ice;
                                    break;
                            }
                        }
                        
                        break;
                    case 2: // 斬撃フェーズ
                        if (hasUsedSlashAttack[i])
                        {
                            if (powerOfFireSlashAttack[i] > 0 && currentSword[i] == FighterSword.Fire)
                            {
                                powerOfSlashAttack[i] += powerOfFireSlashAttack[i];
                                element = QueryElement.Fire;
                            }

                            if (powerOfThunderSlashAttack[i] > 0 && currentSword[i] == FighterSword.Thunder)
                            {
                                powerOfSlashAttack[i] += powerOfThunderSlashAttack[i];
                                element = QueryElement.Thunder;
                            }

                            if (powerOfIceSlashAttack[i] > 0 && currentSword[i] == FighterSword.Ice)
                            {
                                powerOfSlashAttack[i] += powerOfIceSlashAttack[i];
                                element = QueryElement.Ice;
                            }

                            queries.Add(new ActionQuery
                            {
                                type = QueryType.SlashAttack,
                                isPlayer = i == 0
                            });

                            if (hasUsedGuard[opposite[i]])
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Guard,
                                    isPlayer =  i == 1
                                });
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Damage,
                                    isPlayer = i == 1,
                                    value = 0
                                });
                                element = QueryElement.Normal;
                            }
                            else
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Damage,
                                    isPlayer = i == 1,
                                    value = powerOfSlashAttack[i]
                                });
                            }

                            switch (element)
                            {
                                case QueryElement.Fire:
                                    _conditionBurned[opposite[i]] = 3;
                                    break;
                                case QueryElement.Thunder:
                                    _conditionParalysis[opposite[i]] = 2;
                                    break;
                                case QueryElement.Ice:
                                    _conditionFrozen[opposite[i]] = 1;
                                    break;
                            }
                        }
                        break;
                    
                    case 3: // 魔法攻撃フェーズ
                        if (hasUsedMagicAttack[i])
                        {
                            int powerOfMagicAttack = 0;
                            if (powerOfFireMagicAttack[i] > 0 && currentSword[i] == FighterSword.Fire)
                            {
                                powerOfMagicAttack += powerOfFireMagicAttack[i];
                                element = QueryElement.Fire;
                            }

                            if (powerOfThunderMagicAttack[i] > 0 && currentSword[i] == FighterSword.Thunder)
                            {
                                powerOfMagicAttack += powerOfThunderMagicAttack[i];
                                element = QueryElement.Thunder;
                            }

                            if (powerOfIceMagicAttack[i] > 0 && currentSword[i] == FighterSword.Ice)
                            {
                                powerOfMagicAttack += powerOfIceMagicAttack[i];
                                element = QueryElement.Ice;
                            }

                            if (powerOfMagicAttack > 0)
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.MagicAttack,
                                    isPlayer = i == 0
                                });

                                if (hasUsedMagicReflector[opposite[i]])
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.MagicReflector,
                                        isPlayer = i == 1
                                    });
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Damage,
                                        isPlayer = i == 0,
                                        value = powerOfMagicAttack
                                    });
                                    switch (element)
                                    {
                                        case QueryElement.Fire:
                                            _conditionBurned[i] = 3;
                                            break;
                                        case QueryElement.Thunder:
                                            _conditionParalysis[i] = 2;
                                            break;
                                        case QueryElement.Ice:
                                            _conditionFrozen[i] = 1;
                                            break;
                                    }
                                }
                                else
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Damage,
                                        isPlayer = i == 1,
                                        value = powerOfMagicAttack
                                    });
                                    switch (element)
                                    {
                                        case QueryElement.Fire:
                                            _conditionBurned[opposite[i]] = 3;
                                            break;
                                        case QueryElement.Thunder:
                                            _conditionParalysis[opposite[i]] = 2;
                                            break;
                                        case QueryElement.Ice:
                                            _conditionFrozen[opposite[i]] = 1;
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    
                    case 4: // 回復フェーズ
                        if (hasUsedRecovery[i])
                        {
                            queries.Add(new ActionQuery
                            {
                                type = QueryType.UsePotion,
                                isPlayer = i == 0
                            });

                            if (hasUsedRecoveryBlock[opposite[i]])
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.RecoveryBlock,
                                    isPlayer = i == 1
                                });
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Recovery,
                                    isPlayer = i == 0,
                                    value = 0
                                });
                            }
                            else
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Recovery,
                                    isPlayer = i == 0,
                                    value = valueOfRecovery[i]
                                });
                            }
                        }
                        break;
                    
                    case 5: // エンチャント
                        bool isFinishEnchant = false;
                        if (hasUsedFireEnchant[i])
                        {
                            element = QueryElement.Fire;
                        }
                        else if (hasUsedThunderEnchant[i])
                        {
                            element = QueryElement.Thunder;
                        }
                        else if (hasUsedIceEnchant[i])
                        {
                            element = QueryElement.Ice;
                        }
                        else
                        {
                            if (hasUsedFireAttack[i] && currentSword[i] == FighterSword.Fire)
                            {
                                isFinishEnchant = true;
                            }
                            
                            if (hasUsedThunderAttack[i] && currentSword[i] == FighterSword.Thunder)
                            {
                                isFinishEnchant = true;
                            }

                            if (hasUsedIceAttack[i] && currentSword[i] == FighterSword.Ice)
                            {
                                isFinishEnchant = true;
                            }
                        }

                        if (element == QueryElement.Normal)
                        {
                            if (isFinishEnchant)
                            {
                                queries.Add(new ActionQuery
                                {
                                    type = QueryType.Enchant,
                                    isPlayer = i == 0,
                                    element = QueryElement.Normal
                                });
                            }
                        }
                        else
                        {
                            queries.Add(new ActionQuery
                            {
                                type = QueryType.Enchant,
                                isPlayer = i == 0,
                                element = element
                            });
                        }
                        break;
                    case 6: // 状態異常フェーズ
                        switch (conditions[i])
                        {
                            case FighterCondition.Fine:
                                if (_conditionBurned[i] > 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Fire
                                    });
                                }
                                if (_conditionParalysis[i] > 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Thunder
                                    });
                                }
                                if (_conditionFrozen[i] > 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Ice
                                    });
                                }

                                break;
                            case FighterCondition.Burned:
                                _conditionBurned[i]--;
                                if (_conditionBurned[i] <= 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Normal
                                    });
                                }

                                break;
                            case FighterCondition.Paralysis:
                                _conditionParalysis[i]--;
                                if (_conditionParalysis[i] <= 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Normal
                                    });
                                }

                                break;
                            case FighterCondition.Frozen:
                                _conditionFrozen[i]--;
                                if (_conditionFrozen[i] <= 0)
                                {
                                    queries.Add(new ActionQuery
                                    {
                                        type = QueryType.Debuff,
                                        isPlayer = i == 0,
                                        element = QueryElement.Normal
                                    });
                                }

                                break;
                        }
                        break;
                }
            }
        }
        
        if (queries.Count == 0)
        {
            queries.Add(new ActionQuery
            {
                type = QueryType.Nothing
            });
        }

        return queries;
    }
}
