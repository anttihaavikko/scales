using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class StrikeDisplay : MonoBehaviour
{
    [SerializeField] private StrikeBox prefab;

    private readonly List<StrikeBox> strikes = new();

    private void Start()
    {
        Debug.Log($"Strikes {State.Instance.Strikes}/{State.Instance.MaxStrikes}");
        AddMax(State.Instance.MaxStrikes);
    }

    public void AddMax(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var strike = Instantiate(prefab, transform);
            strikes.Add(strike);
            var filled = State.Instance.Strikes > i;
            this.StartCoroutine(() => strike.Show(filled), 0.1f * i);
        }
    }

    public void AddStrikes(int amount)
    {
        if (amount == 0) return;
        
        State.Instance.Strikes = Mathf.Clamp(State.Instance.Strikes + amount, 0, State.Instance.MaxStrikes);
        
        if (amount > 0)
        {
            strikes.Skip(State.Instance.Strikes - amount).Take(amount).ToList().ForEach(s => s.Fill());
            return;
        }
        
        strikes.Skip(State.Instance.Strikes).ToList().ForEach(s => s.Clear());
    }
}