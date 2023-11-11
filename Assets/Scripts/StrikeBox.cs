using AnttiStarterKit.Animations;
using UnityEngine;

public class StrikeBox : MonoBehaviour
{
    [SerializeField] private Appearer box, cross;

    public void Show(bool filled = false)
    {
        box.Show();
        if (filled) Fill();
    }

    public void Fill()
    {
        cross.Show();
    }

    public void Clear()
    {
        cross.Hide();
    }
}