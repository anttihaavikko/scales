using System;
using System.Linq;
using UnityEngine;

namespace Leaderboards
{
    public class Leaderboards : MonoBehaviour
    {
        public ScoreRow rowPrefab;

        private ScoreManager scoreManager;
        private int page;
        
        private void Start()
        {
            scoreManager = GetComponent<ScoreManager>();
            scoreManager.onLoaded += ScoresLoaded;
            scoreManager.LoadLeaderBoards(page);
        }

        private void Update()
        {
            DebugPagination();
        }

        private void DebugPagination()
        {
            if (!Application.isEditor) return;
            if (Input.GetKeyDown(KeyCode.A)) ChangePage(-1);
            if (Input.GetKeyDown(KeyCode.D)) ChangePage(1);
        }

        private void ScoresLoaded()
        {
            var data = scoreManager.GetData();

            for (var i = 0; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                Destroy(go);
            }

            data.scores.ToList().ForEach(entry =>
            {
                var row = Instantiate(rowPrefab, transform);
                row.Setup(entry.position + ". " + entry.name, entry.score, entry.locale);
            });
        }

        public void ChangePage(int direction)
        {
            if (page + direction < 0 || direction > 0 && scoreManager.EndReached) return;
            page = Mathf.Max(page + direction, 0);
            scoreManager.CancelLeaderboards();
            scoreManager.LoadLeaderBoards(page);
        }
    }
}
