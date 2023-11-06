Shader "CustomSprites/OutlinedSprite"
{
    Properties{
        _Color ("Main Color", Color) = (1,1,1,1)
        _Width ("Outline Width", Float) = 0.05
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
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
        ZTest Off

        Pass{

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _Speed;
            float _Width;

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

            fixed4 frag(v2f i) : SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                // return float4(_Color.rgb, col.a);
                fixed4 outline = tex2D(_MainTex, fixed2(i.uv.x - _Width, i.uv.y)) +
                    tex2D(_MainTex, fixed2(i.uv.x + _Width, i.uv.y)) +
                        tex2D(_MainTex, fixed2(i.uv.x, i.uv.y - _Width)) +
                            tex2D(_MainTex, fixed2(i.uv.x, i.uv.y + _Width));
                return outline * (1 - col.a) + float4(col.r * i.color.r, col.g * i.color.g, col.b * i.color.b, col.a);
                return float4(col.r * i.color.r, col.g * i.color.g, col.b * i.color.b, col.a);
            }

            ENDCG
        }
    }
}
