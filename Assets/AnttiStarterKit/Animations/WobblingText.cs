using System;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class WobblingText : MonoBehaviour
    {
        [SerializeField] private float amount = 0.01f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float topModifier = 1f;
        [SerializeField] private float bottomModifier = 1f;
        
        private float baseOffset;
        private TMP_Text textField;

        public int EndIndex { get; set; } = -1;

        public void Clone(WobblingText wobble)
        {
            amount = wobble.amount;
            speed = wobble.speed;
        }

        private void Awake()
        {
            baseOffset = Random.value * 0.2f;
            textField = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            textField.ForceMeshUpdate();
        
            var mesh = textField.mesh;
            var verts = mesh.vertices;
            var realPos = 0;
            var styleStarted = false;

            for (var i = 0; i < textField.text.Length; i++)
            {
                if (EndIndex >= 0 && realPos >= EndIndex) break;
                
                var current = textField.text.Substring(i, 1);
                if (current == "<")
                {
                    styleStarted = true;
                    continue;
                }

                if (styleStarted)
                {
                    if (current == ">")
                    {
                        styleStarted = false;
                    }
                    continue;
                }

                OffsetCharacter(realPos, textField.textInfo.characterInfo[realPos], ref verts);
                realPos++;
            }

            mesh.vertices = verts;

            if (textField.canvasRenderer)
            {
                textField.canvasRenderer.SetMesh(mesh);
            }
        }
        
        private void OffsetCharacter(int index, TMP_CharacterInfo info, ref Vector3[] verts)
        {
            var offset = Vector3.zero.WhereY(Mathf.Sin(Time.time * speed + index * 0.5f + baseOffset) * amount);
            verts[info.vertexIndex] += offset * bottomModifier;
            verts[info.vertexIndex + 1] += offset * topModifier;
            verts[info.vertexIndex + 2] += offset * topModifier;
            verts[info.vertexIndex + 3] += offset * bottomModifier;
        }
    }
}