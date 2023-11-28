using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboards
{
    public class ScoreRow : MonoBehaviour
    {
        public TMP_Text namePart, scorePart;
        public RawImage flag;
    
        public void Setup(string nam, string sco, string locale)
        {
            namePart.text = nam;
            scorePart.text = ulong.Parse(sco).AsScore();
            FlagManager.SetFlag(flag, locale);
        }
    }
}
