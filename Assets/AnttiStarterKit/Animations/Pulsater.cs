using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsater : MonoBehaviour
{
    public float amount = 0.1f;
    public float speed = 1f;

    private float pos = -1f;

    private Vector3 targetSize;

    private void Awake()
    {
        targetSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(pos >= 0f)
        {
            pos = Mathf.MoveTowards(pos, 1f, Time.deltaTime * speed);
            var stepped = Mathf.SmoothStep(0f, 1f, pos);
            var size = Mathf.Sin(Mathf.PI * stepped) * amount + 1f;
            transform.localScale = size * targetSize;
        }
    }

    public void Pulsate()
    {
        pos = 0f;
    }
}
