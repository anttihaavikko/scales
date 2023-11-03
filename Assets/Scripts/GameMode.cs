using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    [SerializeField] protected Deck deck;
    [SerializeField] protected Hand hand;
    [SerializeField] protected List<Slot> slots;

    private IEnumerable<Card> AllCards => deck.Cards.Concat(hand ? hand.Cards : new List<Card>());

    private void Start()
    {
        slots.ForEach(slot => slot.click += SlotClicked);
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

    public abstract int GetJokerValue();
    protected abstract void Combine(Card first, Card second);
    public abstract void Setup();
    public abstract void Select(Card card);
    public abstract void DropToSlot(Card card, Slot slot);
    public abstract bool CanCombine(Card first, Card second);
    public abstract void RightClick(Card card);
}