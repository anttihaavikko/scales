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
        if (amount == 5 && State.Instance.Has(Effect.Fiver)) return;
        
        amount = amount > 0 ? 
            Mathf.Clamp(amount - State.Instance.GetCount(Effect.Shield), 0, State.Instance.MaxStrikes) : 
            amount;

        if (amount == 0) return;
        
        State.Instance.Strikes = Mathf.Clamp(State.Instance.Strikes + amount, 0, State.Instance.MaxStrikes);

        var i = 0;
        if (amount > 0)
        {
            strikes.Skip(State.Instance.Strikes - amount).Take(amount).ToList().ForEach(s =>
            {
                this.StartCoroutine(s.Fill, i * 0.1f);
                i++;
            });
            return;
        }
        
        strikes.Skip(State.Instance.Strikes).ToList().ForEach(s => s.Clear());
    }
}