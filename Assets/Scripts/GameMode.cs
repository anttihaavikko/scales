using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    [SerializeField] protected Deck deck;
    [SerializeField] protected Hand hand;
    [SerializeField] protected List<Slot> slots;
    [SerializeField] protected Camera cam;
    [SerializeField] protected ScoreDisplay scoreDisplay;
    [SerializeField] protected Dragon dragon;
    [SerializeField] protected Appearer continueButton;

    private IEnumerable<Card> AllCards => deck.Cards.Concat(hand ? hand.Cards : new List<Card>());

    private void Start()
    {
        slots.ForEach(slot => slot.click += SlotClicked);

        if (scoreDisplay)
        {
            scoreDisplay.Set(State.Instance.Score);
        }
    }
    
    public void ToRewards()
    {
        State.Instance.RoundEnded(scoreDisplay.Total);
    }

    protected void DeselectAll()
    {
        AllCards.Where(c => c.IsSelected).ToList().ForEach(c => c.ChangeSelection(false));
    }
    
    private void SlotClicked(Slot slot)
    {
        var card = AllCards.FirstOrDefault(c => c.IsSelected);

        if (slot.Accepts && card)
        {
            DropToSlot(card, slot);
        }
    }

    public bool TryCombine(Card first, Card second)
    {
        if (!CanCombine(first, second)) return false;
        Combine(first, second);
        return true;
    }
    
    protected static void ShowScore(int total, int multi, Vector3 p)
    {
        var multiText = $"<color=#CDE7B0><size=5>x{multi}</size></color>";
        if (multi > 1)
        {
            EffectManager.AddTextPopup(multiText, p + Vector3.down * 0.3f + Vector3.right * 0.3f, 1);
        }
        EffectManager.AddTextPopup($"{total}", p);
    }

    public abstract int GetJokerValue();
    protected abstract void Combine(Card first, Card second);
    public abstract void Setup();
    public abstract void Select(Card card);
    public abstract void DropToSlot(Card card, Slot slot);
    public abstract bool CanCombine(Card first, Card second);
    public abstract void RightClick(Card card);
    public abstract bool CanPlay(Card card);
}