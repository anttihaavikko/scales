using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.Utils.DevMenu
{
    public class DevMenu : MonoBehaviour
    {
        [SerializeField] private Button prefab;
        [SerializeField] private GameObject wrap;

        private bool state;

        private void Start()
        {
            wrap.SetActive(false);
        }

        public void Setup(IEnumerable<DevMenuOption> options)
        {
            options.ToList().ForEach(o =>
            {
                var button = Instantiate(prefab, wrap.transform);
                button.GetComponentInChildren<TMP_Text>().text = o.Title;
                button.onClick.AddListener(() => o.Action.Invoke());
            });
        }

        private void Update()
        {
            if (!DevKey.Down(KeyCode.Tab)) return;
            state = !state;
            wrap.SetActive(state);
        }
    }

    public class DevMenuOption
    {
        public string Title { get; }
        public Action Action { get; }

        public DevMenuOption(string name, Action action)
        {
            Title = name;
            Action = action;
        }
    }
}