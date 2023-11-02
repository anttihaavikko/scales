using Cinemachine;
using AnttiStarterKit.Extensions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnttiStarterKit.Visuals
{
    public class EffectCamera : MonoBehaviour
    {
        public Transform cameraRig;
        public Volume ppVolume;
        public Material colorSplitMaterial;

        public float returnSpeed = 0.6f;
        public float shakeChance = 0.3f;
        public float timeResumeSpeed = 1f;
        
        private float chromaAmount;
        private float defaultLensDistortion;
        private float bulgeAmount;
        private float bulgeSpeed;
        private float chromaSpeed = 1f;
        private float colorAmount, colorSpeed = 1f;
        private float shakeAmount, shakeTime;
        private float totalShakeTime;

        private Vector3 originalPos;

        private ChromaticAberration ca;
        private LensDistortion ld;
        private ColorAdjustments cg;
        private static readonly int Amount = Shader.PropertyToID("_Amount");
        
        private static EffectCamera instance = null;
        public static EffectCamera Instance {
            get { return instance; }
        }

        private void Awake()
        {
            if (instance != null && instance != this) {
                Destroy (this.gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            InitPostProcessing();
            originalPos = cameraRig.position;
        }

        private void InitPostProcessing()
        {
            if (!ppVolume) return;
            
            ppVolume.profile.TryGet(out ca);
            ppVolume.profile.TryGet(out ld);
            ppVolume.profile.TryGet(out cg);

            bulgeAmount = defaultLensDistortion = ld.intensity.value;
        }

        private void Update()
        {
            HandlePostProcessing();
            HandleShake();
            
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * timeResumeSpeed);
        }

        private void HandlePostProcessing()
        {
            if (!ppVolume) return;
            
            chromaAmount = Mathf.MoveTowards(chromaAmount, 0, Time.deltaTime * chromaSpeed);
            ca.intensity.value = chromaAmount * 0.7f * 3f;
            
            if(colorSplitMaterial)
                colorSplitMaterial.SetFloat(Amount, chromaAmount * 0.02f);

            bulgeAmount = Mathf.MoveTowards(bulgeAmount, defaultLensDistortion, Time.deltaTime * bulgeSpeed);
            ld.intensity.value = bulgeAmount;

            colorAmount = Mathf.MoveTowards(colorAmount, 0, Time.deltaTime * colorSpeed * 0.2f);
            cg.saturation.value = Mathf.Lerp(0f, 50f, colorAmount);
            cg.contrast.value = Mathf.Lerp(0f, 100f, colorAmount);
        }

        private void HandleShake()
        {
            if (shakeTime > 0f)
            {
                if (Random.value < shakeChance)
                    return;

                var mod = Mathf.SmoothStep(0f, 1f, shakeTime / totalShakeTime);
                shakeTime -= Time.deltaTime;

                var diff = new Vector3(Random.Range(-shakeAmount, shakeAmount) * mod,
                    Random.Range(-shakeAmount, shakeAmount) * mod, 0);

                var rot = Quaternion.Euler(0, 0, Random.Range(-shakeAmount, shakeAmount) * mod * 1.5f);

                cameraRig.position = originalPos + diff * 0.075f;
                cameraRig.rotation = rot;
            }
            else
            {
                var p = Vector3.MoveTowards(cameraRig.transform.position, originalPos, Time.deltaTime * returnSpeed);
                var r = Quaternion.RotateTowards(cameraRig.transform.rotation, Quaternion.identity, Time.deltaTime);
                cameraRig.position = p;
                cameraRig.rotation = r;
            }
        }

        public void Chromate(float amount, float speed) {
            chromaAmount = amount;
            chromaSpeed = speed;
        }

        public void Shake(float amount, float time) {
            shakeAmount = amount;
            shakeTime = time;
            totalShakeTime = time;
        }

        public void Bulge(float amount, float speed)
        {
            bulgeAmount = amount;
            bulgeSpeed = speed;
        }

        public void Decolor(float amount, float speed)
        {
            colorAmount = amount;
            colorSpeed = speed;
        }

        public void BaseEffect(float mod = 1f) {
            Shake(2.5f * mod, 0.8f * mod);
            Chromate(0.5f * mod, 0.5f * mod);
            Bulge(defaultLensDistortion + 1f * mod, 1f * mod);
            Decolor(0.5f * mod, 3f * mod);
        }

        public static void Effect(float mod = 1f)
        {
            Instance.BaseEffect(mod);
        }

        public void TimeStop(int frames = 1)
        {
            Time.timeScale = 0f;
            this.StartCoroutine(() => Time.timeScale = 1f, frames / 60f);
        }
    }
}