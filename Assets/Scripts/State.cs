using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using UnityEngine;

public class State : Manager<State>
{
    private readonly List<CardData> cards = new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10, 0, 0, 0, 0 }.Select(num => new CardData(num)).ToList();

    public int Level { get; private set; } = 0;

    public IEnumerable<CardData> Cards => cards;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (DevKey.Down(KeyCode.N))
        {
            NextLevel();
        }
        
        if (DevKey.Down(KeyCode.M)) SceneChanger.Instance.ChangeScene("Reward");
        if (DevKey.Down(KeyCode.Alpha1)) SceneChanger.Instance.ChangeScene("Mountain");
        if (DevKey.Down(KeyCode.Alpha2)) SceneChanger.Instance.ChangeScene("Scale");
        if (DevKey.Down(KeyCode.Alpha3)) SceneChanger.Instance.ChangeScene("Uno");
    }

    public void Add(CardData card)
    {
        cards.Add(card);
    }

    public void NextLevel()
    {
        Level++;
        var scene = new List<string> { "Mountain", "Scale", "Uno" }.Random();
        SceneChanger.Instance.ChangeScene(scene);
    }

    public CardData GetCard(Guid id)
    {
        return cards.FirstOrDefault(c => c.id == id);
    }

    public bool Has(Guid id)
    {
        return GetCard(id) != default;
    }
}