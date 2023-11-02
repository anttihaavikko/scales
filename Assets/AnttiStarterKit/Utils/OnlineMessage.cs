using System;
using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace AnttiStarterKit.Utils
{
    public class OnlineMessage : MonoBehaviour
    {
        [SerializeField] private Appearer appearer;
        [SerializeField] private List<TMP_Text> texts;
        [SerializeField] private string messageUrl;
        [SerializeField] private float showDelay;
        [SerializeField] private bool hidesOnClick;
        
        private bool shown;

        private void Start()
        {
            StartCoroutine(Load());
        }

        private void Update()
        {
            if (shown && hidesOnClick && Input.anyKeyDown)
            {
                appearer.Hide();
            }
        }

        private IEnumerator Load() {
            var www = UnityWebRequest.Get(messageUrl);
            www.certificateHandler = new MyCertificateHandler();
            yield return www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error)) yield break;
            Show(www.downloadHandler.text);
        }

        private void Show(string message)
        {
            var prev = PlayerPrefs.GetString("PrevMOTD", "");
            if (message == prev) return;
            texts.ForEach(t => t.text = message);
            PlayerPrefs.SetString("PrevMOTD", message);
            Invoke(nameof(ShowWithDelay), showDelay);
        }

        private void ShowWithDelay()
        {
            shown = true;
            appearer.Show();
        }
    }
    
    internal class MyCertificateHandler : CertificateHandler
    {
        // Encoded RSAPublicKey
        private static readonly string PUB_KEY = "";


        /// <summary>
        /// Validate the Certificate Against the Amazon public Cert
        /// </summary>
        /// <param name="certificateData">Certifcate to validate</param>
        /// <returns></returns>
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}