using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomJumper : MonoBehaviour
{
    [SerializeField] private Dragon dragon;

    private void Start()
    {
        Invoke(nameof(Jump), Random.Range(1f, 10f));
        Invoke(nameof(Nudge), Random.Range(1f, 5f));
    }

    private void Jump()
    {
        dragon.Hop();
        Invoke(nameof(Jump), Random.Range(1f, 10f));
    }

    private void Nudge()
    {
        dragon.Nudge();
        Invoke(nameof(Nudge), Random.Range(1f, 5f));
    }
}