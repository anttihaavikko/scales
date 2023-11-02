Shader "Custom/Pixelate with Palette"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Amount ("Amount", Float) = 0
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
			float _Amount;
			float4 _Colors[64];
			int _ColorCount = 64;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 gridUv = round(i.uv * float(_Amount)) / float(_Amount);
				float4 color = tex2D(_MainTex, gridUv);

				float maxDistance = 999;
				float4 best = float4(0, 0, 0, 1);
				
				for(int i = 0; i < _ColorCount; i++)
				{
					float dist = distance(color.rgb, _Colors[i]);
					const bool closer = dist < maxDistance;
					maxDistance = closer ? dist : maxDistance;
					best = closer ? _Colors[i] : best;
				}
				
				return best;
			}
			ENDCG
		}
	}
}
