using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOPuzzleManager : MonoBehaviour
{
    public static SOHOPuzzleManager instance;

    public Puzzle[] puzzleShopGroup;    // 碎片

    public Dictionary<string, double> puzzle_multi_group;


    private void Awake()
    {
        instance = this;
    }

    public void initPuzzle()
    {
        string[] puzzles = SaveDataManager.GetStringArray(SOHOShopConst.sv_Puzzle);
        if (puzzles.Length == 0)
        {
            puzzleShopGroup = SOHOShopDataManager.instance.shopJson.puzzle_shop_group;
        }
        else
        {
            puzzleShopGroup = new Puzzle[puzzles.Length];
            for (int i = 0; i < puzzles.Length; i++)
            {
                puzzleShopGroup[i] = JsonMapper.ToObject<Puzzle>(puzzles[i]);
            }
        }

        puzzle_multi_group = SOHOShopDataManager.instance.shopJson.puzzle_multi_group;
    }

    /// <summary>
    /// 获取比例未达到99%的碎片
    /// </summary>
    /// <returns></returns>
    public Puzzle getUnachievedPuzzle()
    {
        List<Puzzle> unachievedPuzzles = new List<Puzzle>();
        if (puzzleShopGroup == null)
        {
            initPuzzle();
        }
        foreach (Puzzle item in puzzleShopGroup)
        {
            if (item.claim_count * 100 / item.sum_count < 99)
            {
                unachievedPuzzles.Add(item);
            }
        }
        Puzzle result = unachievedPuzzles.Count == 0 ? null : unachievedPuzzles[Random.Range(0, unachievedPuzzles.Count)];
        if (result != null)
        {
            // 获得的碎片数量 = multi * 碎片总数
            result.reward_count = getPuzzleMulti(result) * result.sum_count;
        }
        return result;
    }

    public double getPuzzleMulti(Puzzle puzzle)
    {
        double achievePercent = puzzle.claim_count / puzzle.sum_count;
        foreach (string key in puzzle_multi_group.Keys)
        {
            if (achievePercent <= float.Parse(key))
            {
                return puzzle_multi_group[key];
            }
        }
        return puzzle_multi_group["1"];
    }

    /// <summary>
    /// 增加碎片
    /// </summary>
    /// <param name="puzzle"></param>
    public void addPuzzle(Puzzle puzzle)
    {
        if (puzzleShopGroup.Length == 0)
        {
            initPuzzle();
        }
        foreach (Puzzle item in puzzleShopGroup)
        {
            if (item.name == puzzle.name)
            {
                item.claim_count += puzzle.reward_count;
                break;
            }
        }
        savePuzzle();
    }

    private void savePuzzle()
    {
        List<string> strings = new List<string>();
        foreach (Puzzle item in puzzleShopGroup)
        {
            strings.Add(JsonMapper.ToJson(item));
        }
        SaveDataManager.SetStringArray(SOHOShopConst.sv_Puzzle, strings.ToArray());
    }
}
