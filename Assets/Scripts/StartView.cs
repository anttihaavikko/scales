using UnityEngine;

public class StartView : MonoBehaviour
{
    public void Play()
    {
        State.Instance.Reset();
    }
}