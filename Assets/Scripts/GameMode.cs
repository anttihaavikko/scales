using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    public abstract void Setup();
    public abstract void Select(Card card);
    public abstract void DropToSlot(Card card, Slot slot);
    public abstract bool TryCombine(Card first, Card second);
    public abstract bool CanCombine(Card first, Card second);
    public abstract void RightClick(Card card);
}