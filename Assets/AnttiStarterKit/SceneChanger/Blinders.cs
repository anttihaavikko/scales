using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Blinders : MonoBehaviour
{
    public Transform left, right;
    public bool startsOpen, openAtStart = true;

    private readonly float duration = 0.3f;
    private bool isOpen;

    // Start is called before the first frame update
    private void Start()
    {
        isOpen = startsOpen;

        if (startsOpen) return;

        left.transform.localScale = new Vector3(1f, 5f, 1f);
        right.transform.localScale = new Vector3(1f, 5f, 1f);

        if (openAtStart)
            Invoke("Open", 0.5f);
    }

    public void Close()
    {
        if (!isOpen) return;

        Tweener.Instance.ScaleTo(left, new Vector3(1f, 2f, 1f), duration, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(right, new Vector3(1f, 2f, 1f), duration, 0f, TweenEasings.BounceEaseOut);

        if (AudioManager.Instance)
        {
            // AudioManager.Instance.PlayEffectAt(29, Vector3.zero, 1.804f);
            // AudioManager.Instance.PlayEffectAt(9, Vector3.zero, 1.094f);
        }

        Invoke("Clang", duration * 0.9f);

        isOpen = false;
    }

    public void Open()
    {
        Tweener.Instance.ScaleTo(left, new Vector3(0f, 2f, 1f), duration, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(right, new Vector3(0f, 2f, 1f), duration, 0f, TweenEasings.BounceEaseOut);

        if (AudioManager.Instance)
        {
            // AudioManager.Instance.PlayEffectAt(29, Vector3.zero, 1.804f);
            // AudioManager.Instance.PlayEffectAt(9, Vector3.zero, 1.094f);
        }

        isOpen = true;
    }

    public float GetDuration()
    {
        return duration;
    }

    private void Clang()
    {
        if (AudioManager.Instance)
        {
            //AudioManager.Instance.PlayEffectAt(11, Vector3.zero, 1.193f);
            //AudioManager.Instance.PlayEffectAt(19, Vector3.zero, 1.159f);
            //AudioManager.Instance.PlayEffectAt(21, Vector3.zero, 1.322f);
            //AudioManager.Instance.PlayEffectAt(22, Vector3.zero, 1.284f);
            //AudioManager.Instance.PlayEffectAt(15, Vector3.zero, 1.511f);
            //AudioManager.Instance.PlayEffectAt(3, Vector3.zero, 1.605f);
        }
    }
}