using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dragon : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D snout;
    [SerializeField] private Transform head;

    private Vector3 start;
    
    private static readonly int HopAnim = Animator.StringToHash("Hop");
    private static readonly int FlapAnim = Animator.StringToHash("Flap");
    private static readonly int FlapTwiceAnim = Animator.StringToHash("DoubleFlap");

    private void Start()
    {
        start = head.transform.position;
        Invoke(nameof(WingFlaps), 5f);
    }

    private void WingFlaps()
    {
        Flap();
        Invoke(nameof(WingFlaps), Random.Range(1f, 5f));
    }

    private void Update()
    {
        if(DevKey.Down(KeyCode.H)) Hop();
        if (DevKey.Down(KeyCode.T)) Nudge();
    }

    public void Nudge()
    {
        snout.AddForce(Vector3.zero.RandomOffset(100f));
        var x = Random.Range(-1f, 1f) * 0.5f;
        var y = Random.Range(-1f, 1f) * 0.3f;
        var pos = start + x * Vector3.right + Vector3.up * y;
        Tweener.MoveToBounceOut(head, pos, 0.2f);
    }

    public void Acknowledge()
    {
        if (Random.value < 0.1f)
        {
            Hop();
            return;
        }
        
        Nudge();
    }

    public void Hop()
    {
        Tweener.MoveToBounceOut(head, start, 0.4f);
        anim.SetTrigger(HopAnim);
    }

    public void DoubleFlap()
    {
        anim.SetTrigger(FlapTwiceAnim);
    }

    public void Flap()
    {
        anim.SetTrigger(FlapAnim);
    }
}