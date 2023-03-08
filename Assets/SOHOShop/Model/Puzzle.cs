using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle
{
    public string name { get; set; }
    public string puzzle_img { get; set; }
    public string reward_img { get; set; }
    // 碎片总数
    public int sum_count { get; set; }
    // 已经获得的碎片数
    public double claim_count { get; set; }
    // 单次奖励数量
    public double reward_count { get; set; }
}
