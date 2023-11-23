using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class StrikeDisplay : MonoBehaviour
{
    [SerializeField] private StrikeBox prefab;

    public Action onEnd;

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

    public void AddStrikes(int amount, bool ignoreShields = false)
    {
        if (amount == 5 && State.Instance.Has(Effect.Fiver)) return;

        var mod = ignoreShields ? 0 : 1;
        amount = amount > 0 ? 
            Mathf.Clamp(amount - State.Instance.GetCount(Effect.Shield) * mod, 0, State.Instance.MaxStrikes) : 
            amount;

        if (amount == 0) return;
        
        State.Instance.Strikes = Mathf.Clamp(State.Instance.Strikes + amount, 0, State.Instance.MaxStrikes);
        
        if(State.Instance.Strikes >= State.Instance.MaxStrikes) onEnd?.Invoke();

        var i = 0;
        if (amount > 0)
        {
            strikes.Skip(State.Instance.Strikes - amount).Take(amount).ToList().ForEach(s =>
            {
                this.StartCoroutine(s.FillAndShake, i * 0.1f);
                i++;
            });
            return;
        }
        
        strikes.Skip(State.Instance.Strikes).ToList().ForEach(s => s.Clear());
    }
}