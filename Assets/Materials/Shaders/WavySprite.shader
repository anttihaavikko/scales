// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CustomSprites/WavySprite"
{
    Properties{
        _Speed ("Speed", Float) = 1
        _Offset ("Offset", Float) = 0
        _Amount ("Move Amount", Float) = 1
        _Sway ("Sway Amount", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader{
        Tags{ 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
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

            float _Speed;
            float _Offset;
            float _Amount;
            float _Sway;

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

            float2 diff(fixed2 uv, float2 world) {
                float phase = sin((_Time + _Offset) * 100.0 * _Speed + world.x * 0.5 + world.y * 0.7);
                
                float xdiff = phase * uv.y * 0.02 * _Amount;
                float ydiff = phase * uv.y * 0.005 * uv.x * _Sway;

                return float2(xdiff, ydiff);
            }

            v2f vert(appdata v){
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                float2 world = mul(unity_ObjectToWorld, v.vertex).xy;

                float2 d = diff(v.uv, world);

                o.position = UnityObjectToClipPos(v.vertex + float4(d.x, d.y , 0, 0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;
                return col;
            }

            ENDCG
        }
    }
}
