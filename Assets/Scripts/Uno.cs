using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using TMPro;
using UnityEngine;

public class Uno : GameMode
{
    [SerializeField] private Transform arrow;
    [SerializeField] private Uno opponent;
    [SerializeField] private bool isPlayer;
    [SerializeField] private Transform turnIndicator, turnSpot;
    [SerializeField] private Appearer sameOptions, takeButton;
    [SerializeField] private NoteTrack noteTrack;
    [SerializeField] private TMP_Text tickDisplay;
    [SerializeField] private Appearer tickLine;
    [SerializeField] private Pulsater tickPulsater;

    private bool descending;
    private bool hasTurn;
    private bool dodged;
    private int ticks;

    private Slot Pile => slots[0];
    private int EmptyValue => descending ? int.MaxValue : int.MinValue;
    private int PileValue => Pile.TopCard ? Pile.TopCard.Number : EmptyValue;

    private Dragon helper => isPlayer ? dragon : opponent.dragon;

    public override void Setup()
    {
        hasTurn = isPlayer;
        this.StartCoroutine(() => hand.Fill(), 0.5f);
    }

    private void LateUpdate()
    {
        if (DevKey.Down(KeyCode.J))
        {
            dragon.HopTo(dragon.transform.position.WhereY(-0.4f));
        }
    }

    public override void Select(Card card)
    {
        if (!hasTurn) return;
        var state = card.IsSelected;
        DeselectAll();
        card.ChangeSelection(state);
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        if (!hasTurn || !CanPlay(card.Number))
        {
            card.ReturnToPrevious();
            return;
        }

        Play(card);
    }

    private bool CanPlay(int val)
    {
        var cur = PileValue;
        return !descending && val > cur || descending && val < cur || val == cur;
    }

    private void Play(Card card)
    {
        var isSame = card.IsTrueJoker || card.Number == PileValue;
        
        dragon.Nudge();
        
        card.transform.SetParent(null);
        card.Flatten();
        card.SetDepth(Pile.TopCard, 1);
        card.Lock();
        hand.Remove(card);
        card.ChangeSelection(false);
        Pile.Add(card);
        card.MoveTo(Pile.GetPosition(), isPlayer ? 1f : 0.5f);

        if (noteTrack)
        {
            noteTrack.Add(card.Number, isSame && descending, isSame && !descending);
        }

        if (hand.HasRoom) hand.Draw();

        if (hand.HasRoom)
        {
            helper.Tutorial.Show(TutorialMessage.UnoWinner);
        }

        if (isPlayer)
        {
            Score(card);
            AfterPlay(card);
        }
        
        if (TryEnd())
        {
            return;
        }
        
        if (isSame)
        {
            if(isPlayer) scoreDisplay.AddMulti();
            helper.Tutorial.Mark(TutorialMessage.UnoSame);
            helper.Tutorial.Show(TutorialMessage.UnoChoice);
            HandleSame();
            return;
        }
        
        EndTurn();
    }

    private bool TryEnd()
    {
        if (!hand.IsEmpty && ticks < 5 && opponent.ticks < 5) return false;
        HopOrSit();
        opponent.HopOrSit();
        continueButton.Show();
        return true;
    }

    private void HopOrSit()
    {
        if (ticks < opponent.ticks)
        {
            dragon.Sit();
            return;
        }

        if (isPlayer || ticks > opponent.ticks)
        {
            dragon.Hop();
        }
    }
    
    private void RoundEnded()
    {
        State.Instance.RoundEnded(scoreDisplay.Total);
    }

    private void HandleSame()
    {
        Flip();
    }

    public void Discard()
    {
        deck.Kill(Pile.Cards);
        Pile.Clear();
        sameOptions.Hide();
        EndTurn();
    }

    public void Flip()
    {
        sameOptions.Hide();
        descending = !descending;
        arrow.localScale = new Vector3(descending ? -1 : 1, 1, 1);
        opponent.descending = descending;
        EndTurn();
        helper.Tutorial.Show(TutorialMessage.UnoFlip);
    }

    private void StartTurn()
    {
        Tweener.MoveToBounceOut(turnIndicator, turnSpot.position, 0.2f);

        if (isPlayer && GetOptions().Any(c => c.Number == PileValue))
        {
            helper.Tutorial.Show(TutorialMessage.UnoSame);
        }

        hasTurn = true;
        if (!isPlayer)
        {
            Invoke(nameof(DoMove), 0.5f);
            return;
        }

        if (!GetOptions().Any())
        {
            this.StartCoroutine(() =>
            {
                helper.Tutorial.Show(TutorialMessage.UnoTake);
                helper.Tutorial.Show(TutorialMessage.UnoFlipped);
                // takeButton.Show();
                opponent.AddTick();
                opponent.dragon.Hop();
            }, 1f);
        }
    }

    private void EndTurn()
    {
        hasTurn = false;
        opponent.StartTurn();
    }

    public void TakePile()
    {
        if (isPlayer)
        {
            scoreDisplay.ResetMulti();
        }
        
        takeButton.Hide();
        hand.Add(Pile.Cards);
        Pile.Clear();
        Flip();

        if (isPlayer && hand.Cards.ToList().Count >= 8 && !dodged)
        {
            dodged = true;
            dragon.HopTo(dragon.transform.position.WhereY(-0.5f));
        }

        var max = Mathf.Max(hand.Cards.ToList().Count, opponent.hand.Cards.ToList().Count);
        var ratio = Screen.width * 1f / Screen.height;
        var sample = 16f / 10f;
        var limit = 10 / sample * ratio;
        // cam.orthographicSize = 5 * max / limit;
        Tweener.ZoomTo(cam, Mathf.Clamp(5 * max / limit, 5, 12), 1f);
    }

    private void DoMove()
    {
        var options = GetOptions();
        if (options.Any())
        {
            var pick = options.OrderBy(o => Mathf.Abs(PileValue - o.Number)).First();
            pick.MoveTo(pick.transform.position + Vector3.down * 0.5f);
            dragon.Nudge();
            this.StartCoroutine(() => Play(pick), 0.5f);
            return;
        }
        
        // TakePile();
        helper.Tutorial.Show(TutorialMessage.UnoTake);
        opponent.dragon.Hop();
        opponent.AddTick();
    }

    private List<Card> GetOptions()
    {
        return hand.Cards.Where(c => CanPlay(c.Number)).ToList();
    }

    protected override void Combine(Card first, Card second)
    {
        Play(first);
    }

    public override bool CanCombine(Card first, Card second)
    {
        return false;
    }

    public override void RightClick(Card card)
    {
        DropToSlot(card, Pile);
    }

    public override bool CanPlay(Card card)
    {
        return !opponent.hand.IsEmpty && (card.IsTrueJoker || CanPlay(card.Number));
    }

    public override int AddStrikes()
    {
        var total = Mathf.Max(opponent.ticks - ticks, 0);
        if(opponent.ticks == 0) Perfect();
        strikeDisplay.AddStrikes(total);
        return total;
    }

    public override void PlayInstant(Card card)
    {
        var p = card.transform.position;
        
        card.Pop();
        hand.Remove(card);
        
        if (card.Is(CardType.Recall))
        {
            scoreDisplay.ResetMulti();
            hand.Draw();
            hand.Draw();
        }
        
        PlayGenericInstant(card);
        
        if (card.Is(CardType.Averager))
        {
            var list = GetVisibleCards().ToList();
            var sum = list.Average(c => c.Number);
            list.ForEach(c => c.ChangeNumber((int)sum));
        }
        
        hand.Draw();
        TryEnd();
        card.gameObject.SetActive(false);
    }

    public override IReadOnlyCollection<Card> GetVisibleCards()
    {
        var list = hand.Cards.ToList();
        list.AddRange(opponent.hand.Cards);
        list.AddRange(slots.Select(s => s.TopCard));
        return list.Where(c => c).ToList();
    }

    public override int GetHandSize()
    {
        return hand.Cards.ToList().Count;
    }

    public override int GetTrueJokerValue()
    {
        var values = GetVisibleCards()
            .Where(c => !c.IsTrueJoker && !c.IsJoker)
            .Select(c => c.Number)
            .Concat(new []{ 0 }).ToArray();
        return Mathf.Clamp(PileValue, Mathf.Min(values), Mathf.Max(values));
    }

    protected override void ReSelect()
    {
    }

    private void Score(Card card)
    {
        var tickMulti = Mathf.Max(ticks - opponent.ticks, 0) + 1;
        var total = card.ScoreValue * tickMulti * State.Instance.LevelMulti;
        ShowScore(total, tickMulti, Pile.transform.position);
        scoreDisplay.Add(total * tickMulti);
    }

    private void AddTick()
    {
        ticks++;
        tickPulsater.Pulsate();
        
        if(!isPlayer) Shake(0.3f);

        if (ticks == 5)
        {
            tickLine.Show();
            TryEnd();
            return;
        }
        
        tickDisplay.text = new string('I', ticks);
        Flip();
    }
}