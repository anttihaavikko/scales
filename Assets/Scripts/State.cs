using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using UnityEngine;

public class State : Manager<State>
{
    private readonly List<CardData> cards = new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10 }.Select(num => new CardData(num)).ToList();
    private readonly List<Skill> skills = new();
    private MessageHistory messageHistory;
    private List<HistoryMessage> messages;

    public int Level { get; private set; }
    public int Score { get; private set; }
    public int Strikes { get; set; }
    public int MaxStrikes { get; set; } = 3;

    public int LevelMulti => Level + 1;

    public IEnumerable<CardData> Cards => cards;
    public IEnumerable<Skill> Skills => skills;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public List<HistoryMessage> GetMessages()
    {
        if (messageHistory == default) messageHistory = new MessageHistory();
        if (messages == default) messages = new List<HistoryMessage>();
        messages.Add(messageHistory.Get());
        return messages;
    }

    public bool Has(Effect skill)
    {
        return skills.Any(s => s.effect == skill);
    }

    public int GetCount(Effect skill)
    {
        return skills.Count(s => s.effect == skill);
    }

    public void Add(Skill skill)
    {
        skills.Add(skill);
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
        if (DevKey.Down(KeyCode.Alpha4)) SceneChanger.Instance.ChangeScene("Fish");
    }

    public void Add(CardData card)
    {
        cards.Add(card);
    }

    public void NextLevel()
    {
        Level++;
        var scene = new List<string> { "Mountain", "Scale", "Uno", "Fish" }.Random();
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

    public void RoundEnded(int score)
    {
        Score = score;
        SceneChanger.Instance.ChangeScene("Reward");
    }
}