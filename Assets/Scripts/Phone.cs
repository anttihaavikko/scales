using System;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Phone : MonoBehaviour
{
    [SerializeField] private Transform phone;
    [SerializeField] private Transform onPos, offPos;
    [SerializeField] private PhoneMessage prefab, datePrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private Reward reward;
    
    private bool loaded;

    private void Start()
    {
        phone.position = offPos.position;
    }

    private void Update()
    {
        if(DevKey.Down(KeyCode.K)) Show();
    }

    public void Show()
    {
        if (!loaded)
        {
            loaded = true;

            reward.PhoneScore();

            var messages = State.Instance.GetMessages();
            var day = messages.Count(m => m.isFirst) - 1;
            messages.ForEach(msg =>
            {
                if (msg.isFirst)
                {
                    var date = Instantiate(datePrefab, container);
                    date.Show(GetDayLabel(day));
                    day--;
                }
                var message = Instantiate(prefab, container);
                message.Show(msg.message);
            });
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }

        this.StartCoroutine(() => scrollView.normalizedPosition = new Vector2(0, 0), 0.1f);
        
        Tweener.MoveToBounceOut(phone, onPos.position, 0.3f);
    }

    private string GetDayLabel(int day)
    {
        if (day == 0) return "TODAY";
        if (day == 1) return "1 DAY AGO";
        return $"{day} DAYS AGO";
    }

    public void Hide()
    {
        Tweener.MoveToBounceOut(phone, offPos.position, 0.3f);
    }

    public void Scroll(int dir)
    {
        var step = scrollView.viewport.rect.height / scrollView.content.rect.height * 0.5f;
        scrollView.normalizedPosition += Vector2.up * dir * step;
    }
}