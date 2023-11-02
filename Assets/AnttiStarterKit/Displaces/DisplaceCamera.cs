using UnityEngine;

namespace AnttiStarterKit.Displaces
{
    public class DisplaceCamera : MonoBehaviour
    {
        public Material worldDisplaceMaterial;
        
        private Camera cam;
        private RenderTexture texture;
        private static readonly int DisplaceTex = Shader.PropertyToID("_DisplaceTex");
        // private static readonly int Flip = Shader.PropertyToID("_Flip");

        private void Awake()
        {
            cam = GetComponent<Camera>();
            var pw = Screen.width;
            var ph = Screen.height;
            var ratio = 1f * pw / ph;
            // cam.rect = new Rect(0, 0, 1f, 1f / ratio);
            texture = new RenderTexture(pw, ph, 16);
            cam.targetTexture = texture;
            worldDisplaceMaterial.SetTexture(DisplaceTex, texture);
            // worldDisplaceMaterial.SetFloat(Flip, Application.platform == RuntimePlatform.WebGLPlayer ? 0 : 1);
        }
    }
}
