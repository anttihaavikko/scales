using UnityEngine;

public abstract class Markable : MonoBehaviour
{
    public abstract void Mark(bool state, bool dark);
    public abstract bool AcceptsCard(Card card);
}