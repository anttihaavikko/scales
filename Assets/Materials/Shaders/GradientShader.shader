Shader "GradientShader"
{
    Properties
    {
        _Color1 ("First Color", Color) = (1,1,1,1)
        _Color2 ("Second Color", Color) = (0,0,0,1)
        _Segments ("Segment count", Int) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag               
            #include "UnityCG.cginc"

            fixed4 _Color1;
            fixed4 _Color2;
            int _Segments;

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

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	float pos = i.uv.y;

            	if(_Segments > 0) {
            		pos = floor(_Segments * i.uv.y) / _Segments;
            	}

                return lerp(_Color1, _Color2, pos);
            }
            ENDCG
        }

    }   
}