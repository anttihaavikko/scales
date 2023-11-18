using System;
using UnityEngine;

public class OrthographicMatcher : MonoBehaviour
{
    [SerializeField] private Camera target, self;

    private void Update()
    {
        self.orthographicSize = target.orthographicSize;
    }
}