Shader "Custom/Pixelate"
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

			fixed4 frag (v2f i) : SV_Target
			{
				const float2 gridUv = round(i.uv * float(_Amount)) / float(_Amount);
				float4 color = tex2D(_MainTex, gridUv);

				const float distanceToBlack = distance(color.rgb, float4(0, 0, 0, 1));
				const float distanceToWhite = distance(color.rgb, float4(1, 1, 1, 1));
				const float distanceToGray = distance(color.rgb, float4(0.5, 0.5, 0.5, 1));

				if(distanceToGray < distanceToWhite && distanceToGray < distanceToBlack)
				{
					return color;
				}

				return distanceToBlack < distanceToWhite ? float4(0, 0, 0, 1) : float4(1, 1, 1, 1);
			}
			ENDCG
		}
	}
}
