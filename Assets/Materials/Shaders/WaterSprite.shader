Shader "CustomSprites/WaterSprite"
{
    Properties{
        _Color ("Main Color", Color) = (1,1,1,1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
        _OffsetX ("Offset X", Float) = 0
        _OffsetY ("Offset X", Float) = 0
        
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _DisplaceTex ("Displace", 2D) = "white" {}
    }

    SubShader{
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

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off
        Cull off

        Pass{

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            sampler2D _NoiseTex;
            float _OffsetX;
            float _OffsetY;
            sampler2D _DisplaceTex;

            float _Speed;

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v){
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float2 rotateUV(float2 uv, float2 pivot, float rotation) {
                float sine = sin(rotation);
                float cosine = cos(rotation);

                uv -= pivot;
                uv.x = uv.x * cosine - uv.y * sine;
                uv.y = uv.x * sine + uv.y * cosine;
                uv += pivot;

                return uv;
            }

            float2 scaleUv(float2 uv, float amount)
            {
                return float2(uv.x * amount - (amount - 1) * 0.5, uv.y * amount - (amount - 1) * 0.5);
            }
            
			fixed4 frag(v2f i) : SV_TARGET {
			    fixed4 noise = tex2D(_NoiseTex, i.uv * 3 * 0.5 + float4(_Time.x * 0.3, 0, 0, 0));
			    fixed4 second = tex2D(_NoiseTex, i.uv * 3 * 0.09 + float4(-_Time.x * 0.01, _Time.x * 0.01, 0, 0));
			    float2 uv = i.uv.xy * 10 + noise * 0.06 + second * 0.5 + float4(_OffsetX, _OffsetY, 0, 0);
			    fixed4 displace = tex2D(_DisplaceTex, scaleUv(i.uv, 4)/* + float2(-_OffsetX * 0.06, -_OffsetY * 0.06)*/);
			    fixed2 direction = normalize(float2((displace.r - 0.5) * 10, (displace.g - 0.5) * 10));
                fixed4 col = tex2D(_MainTex, uv + direction);
                return float4(_Color.r, _Color.g, _Color.b, col.a * _Color.a);
            }

            ENDCG
        }
    }
}
