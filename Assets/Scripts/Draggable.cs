using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.Rendering;

public class Draggable : MonoBehaviour
{
    public Action hidePreview, click, dropCancelled, pick;
    public Action<List<Card>> preview;
    public Action<List<Collider2D>> droppedOn;

    [SerializeField] private LayerMask dropMask, blockMask;
    [SerializeField] private bool lockAfterDrop = true;
    [SerializeField] private SortingGroup sortingGroup;

    public bool CanDrag { get; set; } = true;
    public bool DropLocked { get; set; }

    private Camera cam;
    private bool dragging;
    private Vector3 offset;
    private Vector3 start;
    private int layerId;
    private int normalLayer;

    public bool IsDragging => dragging;

    public Vector3 ReturnPos => start;

    public Vector3 Offset => offset;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (!CanDrag || !enabled) return;
        
        var go = gameObject;
        dragging = true;
        start = transform.position;
        offset = start - GetMousePos();
        layerId = go.layer;
        // go.layer = 0;

        SetSortOrder("Picked");
        
        pick?.Invoke();
        
        // AudioManager.Instance.PlayEffectAt(0, start, 1f);
    }

    public void SetSortOrder(string layer)
    {
        if (sortingGroup)
        {
            sortingGroup.sortingLayerName = layer;
        }
    }


    private void OnMouseUp()
    {
        SetSortOrder("Default");
        hidePreview?.Invoke();
        
        if (Vector3.Distance(transform.position, start) < 0.1f)
        {
            click?.Invoke();   
        }
    }

    private void Update()
    {
        if (dragging && Input.GetMouseButtonUp(0))
        {
            StopDrag();
            return;
        }
        
        if (dragging)
        {
            transform.position = GetMousePos() + offset;
            InvokePreview();
        }
    }

    public void Return()
    {
        Tweener.MoveToBounceOut(transform, start, 0.3f);
    }

    private void InvokePreview()
    {
        var hits = TryDrop(transform.position).Select(h => h.GetComponent<Card>()).Where(h => h).ToList();
        preview?.Invoke(hits);
    }

    private void StopDrag()
    {
        hidePreview?.Invoke();
        var rounded = GetRoundedPos();
        DropOn(rounded);
    }

    public Vector2 GetRoundedPos()
    {
        var p = transform.position;
        return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
    }

    private void DropOn(Vector2 pos)
    {
        dragging = false;

        var hits = TryDrop(pos);
        if (hits.Any())
        {
            droppedOn?.Invoke(hits.ToList());
            return;
        }

        CancelDrop();
    }

    public void CancelDrop()
    {
        SetSortOrder("Picked");
        Tweener.MoveToBounceOut(transform, start, 0.1f);
        this.StartCoroutine(() =>
        {
            gameObject.layer = layerId;
            SetSortOrder("Default");
            dropCancelled?.Invoke();
        }, 0.1f);
    }

    private Collider2D[] TryDrop(Vector2 pos)
    {
        gameObject.layer++;
        var hits = Physics2D.OverlapBoxAll(pos, new Vector2(1f, 1.5f), 0, dropMask);
        gameObject.layer--;
        return hits;
    }

    private Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
    }
}