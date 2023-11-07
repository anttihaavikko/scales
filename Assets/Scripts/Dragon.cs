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
    [SerializeField] private SpeechBubble speechBubble;

    private Vector3 start;

    private static readonly int HopAnim = Animator.StringToHash("Hop");
    private static readonly int FlapAnim = Animator.StringToHash("Flap");
    private static readonly int FlapTwiceAnim = Animator.StringToHash("DoubleFlap");
    private static readonly int SitAnim = Animator.StringToHash("Sit");

    public Tutorial<TutorialMessage> Tutorial { get; private set; }

    private void Awake()
    {
        Tutorial = new Tutorial<TutorialMessage>("ScaleTutorials");
        Tutorial.onShow += ShowTutorial;
    }

    private void Start()
    {
        start = head.transform.position;
        Invoke(nameof(WingFlaps), 5f);
        
        speechBubble.onWord += Speak;
        speechBubble.onHide += () => Nudge();
    }

    private string GetTutorialMessage(TutorialMessage message)
    {
        return message switch
        {
            TutorialMessage.Intro => "Combine cards that (total up to 10) to remove them. You can also store any cards on (empty spots).",
            TutorialMessage.Minus => "It's time to do some (subtractions) now. How's your (minus game)?",
            TutorialMessage.BigScore => "Oh yeah! The (more cards) you use, (bigger) the (score) you're awarded...",
            TutorialMessage.ScaleIntro => "Now you need to (balance) these (scales) by loading the same amount of weight on (both sides).",
            TutorialMessage.Overloaded => "Try to (avoid) letting the (scales tip) too much to one side as that will (reset) your (multiplier).",
            TutorialMessage.ExtraWeights => "Notice that (extra weight) over there! It might make this a wee bit (trickier).",
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };
    }

    private void ShowTutorial(TutorialMessage message)
    {
        speechBubble.Show(GetTutorialMessage(message));
    }

    private void WingFlaps()
    {
        Flap();
        Invoke(nameof(WingFlaps), Random.Range(1f, 5f));
    }

    private void Update()
    {
        if (DevKey.Down(KeyCode.H)) Hop();
        if (DevKey.Down(KeyCode.T)) Nudge();
        if (DevKey.Down(KeyCode.S)) Sit();
        if (DevKey.Down(KeyCode.D)) Tutorial.Clear();
    }

    private void Speak()
    {
        Nudge(Random.value);
    }

    public void Nudge(float amount = 1f)
    {
        snout.AddForce(Vector3.zero.RandomOffset(100f * amount));
        var x = Random.Range(-1f, 1f) * 0.5f * amount;
        var y = Random.Range(-1f, 1f) * 0.3f * amount;
        var pos = start + x * Vector3.right + Vector3.up * y;
        Tweener.MoveToBounceOut(head, pos, 0.3f);
    }

    public void Acknowledge(bool hop)
    {
        if (hop)
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

    public void HopTo(Vector3 pos)
    {
        Tweener.MoveToQuad(transform, pos, 5f / 6f * 0.5f);
        // Tweener.MoveToBounceOut(head, start, 0.4f);
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

    public void Sit()
    {
        anim.SetTrigger(SitAnim);
    }
}

public enum TutorialMessage
{
    Intro,
    Minus,
    BigScore,
    ScaleIntro,
    Overloaded,
    ExtraWeights
}