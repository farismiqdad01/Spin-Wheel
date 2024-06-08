using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    private List<string> rewards = new List<string>();

    public void AddReward(string reward)
    {
        rewards.Add(reward);
    }

    public List<string> GetRewards()
    {
        return new List<string>(rewards);
    }
}