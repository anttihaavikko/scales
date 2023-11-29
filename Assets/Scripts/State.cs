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
    private readonly List<CardData> opponentCards = new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10 }.Select(num => new CardData(num)).ToList();
    private readonly List<Skill> skills = new();
    private MessageHistory messageHistory;
    private List<HistoryMessage> messages;
    private string previousLevel;

    public int Level { get; private set; }
    public long Score { get; set; }
    public int Strikes { get; set; }
    public int MaxStrikes { get; set; } = 3;
    public int LevelMulti => Level + 1;
    public int HeldMulti { get; set; } = 1;
    public SkillIcons SkillIcons { get; set; }

    public IEnumerable<CardData> Cards => cards;
    public IEnumerable<CardData> OpponentCards => opponentCards;
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
        var val = skills.Any(s => s.effect == skill);
        if (val && SkillIcons) SkillIcons.Trigger(skill);
        return val;
    }

    public bool Has(Effect skill, int value)
    {
        var val = skills.Any(s => s.effect == skill && s.value == value);
        if (val && SkillIcons) SkillIcons.Trigger(skill, value);
        return val;
    }

    public bool HasTrifecta()
    {
        return Has(Effect.UkkoMine) && Has(Effect.UkkoPowerPlant) && Has(Effect.UkkoTower);
    }

    public int GetCount(Effect skill)
    {
        var val = skills.Count(s => s.effect == skill);
        if (val > 0 && SkillIcons) SkillIcons.Trigger(skill);
        return val;
    }
    
    public int GetCount(Effect skill, int value)
    {
        var val = skills.Count(s => s.effect == skill && s.value == value);
        if (val > 0 && SkillIcons) SkillIcons.Trigger(skill, value);
        return val;
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

    public void AddForOpponent(CardData card)
    {
        if (card.modifier == CardModifier.None)
        {
            opponentCards.Add(card);
            return;
        }

        var target = opponentCards.Where(c => c.CanBeModified).ToList().Random();
        opponentCards.Remove(target);
        target.Modify(card);
        opponentCards.Add(target);
    }

    public void Add(CardData card)
    {
        cards.Add(card);
    }

    private string GetNextLevel()
    {
        if (Level == 1) return "Scale";
        if (Level == 2) return "Uno";
        if (Level == 3) return "Fish";
        return new List<string> { "Mountain", "Scale", "Uno", "Fish" }.Where(lvl => lvl != previousLevel).ToList().Random();
    }

    public void NextLevel()
    {
        Level++;
        previousLevel = GetNextLevel();
        SceneChanger.Instance.ChangeScene(previousLevel);
    }

    public CardData GetCard(Guid id)
    {
        return cards.FirstOrDefault(c => c.id == id);
    }

    public bool Has(Guid id)
    {
        return GetCard(id) != default;
    }

    public void RoundEnded(long score)
    {
        Score = score;
        SceneChanger.Instance.ChangeScene("Reward");
    }

    public void Reset()
    {
        cards.Clear();
        cards.AddRange(new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10 }.Select(num => new CardData(num)).ToList());
        opponentCards.Clear();
        opponentCards.AddRange(new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10 }.Select(num => new CardData(num)).ToList());
        Level = 0;
        previousLevel = null;
        HeldMulti = 1;
        skills.Clear();
        Strikes = 0;
        MaxStrikes = 3;
        messageHistory = default;
        messages = default;

        AudioManager.Instance.TargetPitch = 1f;
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Mountain" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }
}