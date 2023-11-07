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

    private bool descending;
    private bool hasTurn;
    private bool dodged;

    private Slot Pile => slots[0];
    private int EmptyValue => descending ? int.MaxValue : int.MinValue;
    private int PileValue => Pile.TopCard ? Pile.TopCard.Number : EmptyValue;

    private Dragon helper => isPlayer ? dragon : opponent.dragon;

    public override void Setup()
    {
        hasTurn = isPlayer;
        this.StartCoroutine(() => hand.Fill(), 0.5f);

        if (isPlayer)
        {
            Invoke(nameof(ShowIntro), 1f);
        }
    }
    
    private void ShowIntro()
    {
        dragon.Tutorial.Show(TutorialMessage.UnoInfo);
    }

    private void Update()
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
        var isSame = card.Number == PileValue;
        
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
        
        if (hand.IsEmpty)
        {
            continueButton.Show();
            return;
        }
        
        if (isSame)
        {
            helper.Tutorial.Mark(TutorialMessage.UnoSame);
            helper.Tutorial.Show(TutorialMessage.UnoChoice);
            HandleSame();
            return;
        }
        
        EndTurn();
    }
    
    private void RoundEnded()
    {
        State.Instance.RoundEnded(scoreDisplay.Total);
    }

    private void HandleSame()
    {
        if (isPlayer)
        {
            sameOptions.Show();
            return;
        }
        
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
                takeButton.Show();
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
        takeButton.Hide();
        hand.Add(Pile.Cards);
        Pile.Clear();
        Flip();

        if (isPlayer && hand.Cards.ToList().Count >= 8 && !dodged)
        {
            dodged = true;
            dragon.HopTo(dragon.transform.position.WhereY(-0.5f));
        }
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
        
        TakePile();
        helper.Tutorial.Show(TutorialMessage.UnoTake);
        opponent.dragon.Hop();
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