Shader "Custom/Black and White"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Threshold ("Amount", Float) = 1
		_Dark("Dark", Color) = (0, 0, 0, 1)
		_Light("Light", Color) = (1, 1, 1, 1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags{ 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
		
		Stencil {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
        }
		
		ZWrite off
        Cull off
        ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Threshold;
			float4 _Dark;
			float4 _Light;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float4 white = float4(1, 1, 1, 1);
				return distance(color.rgb, white.rgb) < _Threshold ? _Light : _Dark;
			}
			ENDCG
		}
	}
}
