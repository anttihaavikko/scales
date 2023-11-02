using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.Rendering;

public class Draggable : MonoBehaviour
{
    public Action hidePreview, click, dropCancelled, pick;
    public Action<Draggable> preview, dropped;
    
    [SerializeField] private LayerMask dropMask, blockMask;
    [SerializeField] private bool lockAfterDrop = true;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private int normalSortOrder, dragSortOrder;

    public bool CanDrag { get; set; } = true;
    public bool DropLocked { get; set; }

    private Camera cam;
    private bool dragging;
    private Vector3 offset;
    private Vector3 start;
    private int layerId;

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
        // layerId = go.layer;
        // go.layer = 0;
        
        pick?.Invoke();
        
        // AudioManager.Instance.PlayEffectAt(0, start, 1f);

        SetSortOrder(dragSortOrder);
    }

    private void SetSortOrder(int order)
    {
        if (sortingGroup)
        {
            sortingGroup.sortingOrder = order;
        }
    }

    public void NormalizeSortOrder()
    {
        SetSortOrder(normalSortOrder);
    }

    private void OnMouseUp()
    {
        if (Vector3.Distance(transform.position, start) < 0.01f)
        {
            click?.Invoke();   
        }
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = GetMousePos() + offset;
            InvokePreview();
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    public void Return()
    {
        Tweener.MoveToBounceOut(transform, start, 0.3f);
    }

    private void InvokePreview()
    {
        preview?.Invoke(this);
        // var rounded = GetRoundedPos();
        // if (CanDrop(rounded))
        // {
        //     preview?.Invoke(this);
        //     return;
        // }
        //
        // hidePreview?.Invoke();
    }

    private void StopDrag()
    {
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

        if (CanDrop(pos))
        {
            // transform.position = pos;
            dropped?.Invoke(this);
            // enabled = !lockAfterDrop;
            // gameObject.layer = layerId;
            
            return;
        }
        
        Tweener.MoveToBounceOut(transform, start, 0.3f);
        this.StartCoroutine(() =>
        {
            gameObject.layer = layerId;
        }, 0.3f);
        dropCancelled?.Invoke();
    }

    private bool CanDrop(Vector2 pos)
    {
        return true;
        
        if (DropLocked) return false;
        
        var allowed = Physics2D.OverlapCircle(pos, 0.1f, dropMask);
        var blocked = Physics2D.OverlapCircle(pos, 0.1f, blockMask);

        return allowed && !blocked;
    }

    private Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
    }
}