using AnttiStarterKit.Extensions;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private GameObject ledger;
    [SerializeField] private Transform staff;
    [SerializeField] private Pulsater pulsater;
    [SerializeField] private GameObject sharp, flat;

    public void Show(int value, bool s, bool f)
    {
        var line = value % 13;
        ledger.SetActive(line is 0 or 12);
        var t = transform;
        t.localPosition = t.localPosition.WhereY(-0.2f + 0.1f * line);
        staff.localPosition = staff.localPosition.WhereY(value < 7 ? 2.2f : -2.2f);
        pulsater.Pulsate();
        sharp.SetActive(s);
        flat.SetActive(f);
    }
}