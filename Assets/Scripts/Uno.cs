using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

public class Uno : GameMode
{
    [SerializeField] private Transform arrow;
    [SerializeField] private Uno opponent;
    [SerializeField] private bool isPlayer;
    [SerializeField] private Transform turnIndicator, turnSpot;
    [SerializeField] private GameObject sameOptions, takeButton;

    private bool descending;
    private bool hasTurn;

    private Slot Pile => slots[0];
    private int EmptyValue => descending ? int.MaxValue : int.MinValue;
    private int PileValue => Pile.TopCard ? Pile.TopCard.Number : EmptyValue;

    public override void Setup()
    {
        hasTurn = isPlayer;
        this.StartCoroutine(() => hand.Fill(), 0.5f);
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
        var isSame = card.Number == PileValue;
        
        card.transform.SetParent(null);
        card.Flatten();
        card.SetDepth(Pile.TopCard, 1);
        card.Lock();
        hand.Remove(card);
        card.ChangeSelection(false);
        Pile.Add(card);
        card.MoveTo(Pile.GetPosition(), isPlayer ? 1f : 0.5f);
        
        if (hand.HasRoom) hand.Draw();
        
        if (hand.IsEmpty)
        {
            State.Instance.RoundEnded(scoreDisplay.Total);
            return;
        }
        
        if (isSame)
        {
            HandleSame();
            return;
        }
        
        EndTurn();
    }

    private void HandleSame()
    {
        if (isPlayer)
        {
            sameOptions.SetActive(true);
            return;
        }
        
        Flip();
    }

    public void Discard()
    {
        deck.Kill(Pile.Cards);
        Pile.Clear();
        sameOptions.SetActive(false);
        EndTurn();
    }

    public void Flip()
    {
        sameOptions.SetActive(false);
        descending = !descending;
        arrow.localScale = new Vector3(descending ? -1 : 1, 1, 1);
        opponent.descending = descending;
        EndTurn();
    }

    private void StartTurn()
    {
        Tweener.MoveToBounceOut(turnIndicator, turnSpot.position, 0.2f);

        hasTurn = true;
        if (!isPlayer)
        {
            Invoke(nameof(DoMove), 0.5f);
            return;
        }

        if (!GetOptions().Any())
        {
            this.StartCoroutine(() => takeButton.SetActive(true), 1f);
        }
    }

    private void EndTurn()
    {
        hasTurn = false;
        opponent.StartTurn();
    }

    public void TakePile()
    {
        takeButton.SetActive(false);
        hand.Add(Pile.Cards);
        Pile.Clear();
        Flip();
    }

    private void DoMove()
    {
        var options = GetOptions();
        if (options.Any())
        {
            var pick = options.Random();
            pick.MoveTo(pick.transform.position + Vector3.down * 0.5f);
            this.StartCoroutine(() => Play(pick), 0.5f);
            return;
        }
        
        TakePile();
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
    
    public override int GetJokerValue()
    {
        return slots.Sum(s => s.TopCard && !s.TopCard.IsJoker ? s.TopCard.Number : 0) + hand.Cards.Where(c => !c.IsJoker).Sum(c => c.Number);
    }
}