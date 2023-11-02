Shader "Custom/ColorSplit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Amount ("Amount", Float) = 0
		_RedOffset ("Red offset", Vector) = (1,-1,0,0) 
		_GreenOffset ("Green offset", Vector) = (-1,0,0,0) 
		_BlueOffset ("Blue offset", Vector) = (0,1,0,0) 
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

	        float2 _RedOffset;
	        float2 _GreenOffset;
	        float2 _BlueOffset;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 coords = i.uv;
	           
	            //Red Channel
	            float4 red = tex2D(_MainTex, coords + _RedOffset * _Amount);
	            //Green Channel
	            float4 green = tex2D(_MainTex, coords + _GreenOffset * _Amount);
	            //Blue Channel
	            float4 blue = tex2D(_MainTex, coords + _BlueOffset * _Amount);
	           
	            float4 finalColor = float4(red.r, green.g, blue.b, 1.0f);
	            return finalColor;
			}
			ENDCG
		}
	}
}
