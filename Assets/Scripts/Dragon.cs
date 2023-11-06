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

    public Tutorial<TutorialMessage> Tutorial { get; private set; }

    private void Start()
    {
        Tutorial = new Tutorial<TutorialMessage>("ScaleTutorials");
        Tutorial.onShow += ShowTutorial;
        
        start = head.transform.position;
        Invoke(nameof(WingFlaps), 5f);
        
        speechBubble.onWord += Speak;
        speechBubble.onHide += () => Nudge();
        Invoke(nameof(ShowIntro), 1f);
    }

    private string GetTutorialMessage(TutorialMessage message)
    {
        return message switch
        {
            TutorialMessage.Intro => "Combine cards that (total up to 10) to remove them. You can also place any cards on (empty spots).",
            TutorialMessage.Minus => "It's time to do some (subtractions) now. How's your (minus game)?",
            TutorialMessage.BigScore => "Oh yeah! The (more cards) you use, (bigger) the (score) you're awarded...",
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };
    }

    private void ShowTutorial(TutorialMessage message)
    {
        speechBubble.Show(GetTutorialMessage(message));
    }

    private void ShowIntro()
    {
        Tutorial.Show(TutorialMessage.Intro);
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
        Tweener.MoveToBounceOut(head, pos, 0.2f);
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

    public void DoubleFlap()
    {
        anim.SetTrigger(FlapTwiceAnim);
    }

    public void Flap()
    {
        anim.SetTrigger(FlapAnim);
    }
}

public enum TutorialMessage
{
    Intro,
    Minus,
    BigScore
}