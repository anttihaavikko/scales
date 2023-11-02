using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

namespace AnttiStarterKit.Visuals
{
    public class PixelatePaletteAssigner : MonoBehaviour
    {
        [SerializeField] private ColorCollection colors;
        [SerializeField] private Material material;
        
        private static readonly int Colors = Shader.PropertyToID("_Colors");
        private static readonly int ColorCount = Shader.PropertyToID("_ColorCount");

        private void Awake()
        {
            material.SetColorArray(Colors, colors.ToList());
            material.SetInt(ColorCount, colors.Count);
        }
    }
}