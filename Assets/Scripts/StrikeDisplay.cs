using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class StrikeDisplay : MonoBehaviour
{
    [SerializeField] private StrikeBox prefab;

    private readonly List<StrikeBox> strikes = new();
    
    private int cur;
    private int max;

    private void Start()
    {
        AddMax(3);   
    }

    public void AddMax(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var strike = Instantiate(prefab, transform);
            strikes.Add(strike);
            this.StartCoroutine(() => strike.Show(), 0.1f * i);
            max++;
        }
    }

    public void AddStrikes(int amount)
    {
        cur = Mathf.Min(cur + amount, max);
        
        if (amount > 0)
        {
            strikes.Skip(cur - amount).Take(amount).ToList().ForEach(s => s.Fill());
            return;
        }
        
        strikes.Skip(cur).ToList().ForEach(s => s.Clear());
    }
}