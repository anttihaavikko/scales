// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShiningSprite"
{
	Properties
	{
		_MainTex("Albdeo", 2D) = "white" {}
		_ShineTex("Shine", 2D) = "white" {}
		_ShineColor("Shine Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_ShineSpeed("Shine Speed", Float) = 1
		_ShineDistance("Shine Distance", Float) = 2
		_PosMod("Position Modifier", Float) = 0.1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"DisableBatching" = "True"
		}

		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
				float3 wpos : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
				o.color = v.color;
				return o;
			}

			float2 safemul(float4x4 M, float4 v)
			{
				float2 r;

				r.x = dot(M._m00_m01_m02, v);
				r.y = dot(M._m10_m11_m12, v);

				return r;
			}

			sampler2D _MainTex;
			sampler2D _ShineTex;
			float4 _ShineColor;
			float _ShineSpeed;
			float _ShineDistance;
			float _PosMod;

			float saw(float t, float a) {
				return 2 * (t/a - floor(1/2 + t/a));
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv) * i.color;
				//  + i.wpos.x * 0.1 + i.wpos.y * 0.1
				float offset = saw(_Time.w * _ShineSpeed + i.wpos.x * _PosMod + i.wpos.y * _PosMod, _ShineDistance);
				offset *= _ShineDistance;
				// offset = (offset - _ShineDistance/2) * _ShineDistance;
				// return float4(offset, 0, 0, 1);

				if(offset < 0) {
					offset = 0;
				}

				float4 shine = tex2D(_ShineTex, i.uv + float2(0, (offset - 0.5f) * 2));
				color = color + shine * shine.a * _ShineColor * _ShineColor.a;
				return color;
			}
			ENDCG
		}
	}

	Fallback "Sprites/Default"
}