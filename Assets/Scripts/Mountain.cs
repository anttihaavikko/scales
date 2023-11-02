using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Mountain : GameMode
{
    [SerializeField] private Deck deck;

    public override void Setup()
    {
        var shuffled = deck.Cards.RandomOrder().ToList();

        var rows = 4;
        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < row + 1; col++)
            {
                if(row == 3) shuffled[index].Flip();
                shuffled[index].transform.position = new Vector3(-row * 0.6f + col * 1.2f, rows - row * 0.8f);

                ApplyCover(shuffled, shuffled[index], index - row - 1, row);
                ApplyCover(shuffled, shuffled[index], index - row, row);
                
                index++;
            }
        }
    }

    private void ApplyCover(IReadOnlyList<Card> list, Card cur, int index, int row)
    {
        if (index < 0) return;
        
        var r = GetRow(index);
        if (r == row - 1)
        {
            list[index].AddCover(cur);
        }
    }

    private static int GetRow(int index)
    {
        return Mathf.FloorToInt((-1 + Mathf.Sqrt(1 + 8 * index)) / 2);
    }
    
    public override void Select()
    {
        var selected = deck.Cards.Where(c => c.IsSelected).ToList();
        var sum = selected.Sum(c => c.Number);
        if (sum == 10)
        {
            deck.Kill(selected);
        }
        
        deck.Cards.ToList().ForEach(c =>
        {
            if (!c.IsCovered)
            {
                c.Flip();
            }
        });
    }
}

public abstract class GameMode : MonoBehaviour
{
    public abstract void Setup();
    public abstract void Select();
}