// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BattleTransitions"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Color("Screen Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 0
		[MaterialToggle] _Distort("Distort", Float) = 0
		_Fade("Fade", Range(0, 1)) = 0
	}

		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

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
					float2 uv1 : TEXCOORD1;
					float2 screenuv : TEXCOORD2;
					float4 vertex : SV_POSITION;
				};

				float4 _MainTex_TexelSize;

				v2f simplevert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv1 = v.uv;
					o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;

					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv1.y = 1 - o.uv1.y;
					#endif

					return o;
				}

				sampler2D _TransitionTex;
				int _Distort;
				float _Fade;

				sampler2D _MainTex;
				float _Cutoff;
				fixed4 _Color;
				uniform sampler2D _GlobalDisplaceTex;

				fixed4 simplefrag(v2f i) : SV_Target
				{
					if (i.uv.x < _Cutoff)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 simplefragopen(v2f i) : SV_Target
				{
					if (0.5 - abs(i.uv.y - 0.5) < abs(_Cutoff) * 0.5)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 simpleTexture(v2f i) : SV_Target
				{
					fixed4 transit = tex2D(_TransitionTex, i.uv);

					if (transit.b < _Cutoff)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 disp = tex2D(_GlobalDisplaceTex, i.screenuv);

					fixed4 transit = tex2D(_TransitionTex, i.uv1);

					fixed2 direction = float2(0,0);
					if(_Distort)
						direction = normalize(float2((transit.r - 0.5) * 2, (transit.g - 0.5) * 2));

					fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

					direction = normalize(float2((disp.r - 0.5) * 2, (disp.g - 0.5) * 2));
					float4 color = tex2D(_MainTex, i.uv + 0.01 * direction * disp.a);

					if (transit.b < _Cutoff)
						return color = lerp(color, _Color, _Fade);

					return color;
				}					
				ENDCG
			}
		}
}
