Shader "Custom/ExactColorSwap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OriginalColor ("Original Color", Color) = (1,1,1,1)
        _SecondaryOriginalColor ("Secondary Original Color", Color) = (0.18,0.31,0.09,1)
        _ReplacementColor ("Replacement Color", Color) = (1,0,0,1)
        _Tolerance ("Tolerance", Range(0, 0.5)) = 0.18
        _SecondaryTolerance ("Secondary Tolerance", Range(0, 0.5)) = 0.12
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _OriginalColor;
            float4 _SecondaryOriginalColor;
            float4 _ReplacementColor;
            float _Tolerance;
            float _SecondaryTolerance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
               half4 col = tex2D(_MainTex, i.uv) * i.color;

               if (col.a <= 0)
               {
                    return half4(0, 0, 0, 0);
               }

               float primaryDistance = distance(col.rgb, _OriginalColor.rgb);
               float secondaryDistance = distance(col.rgb, _SecondaryOriginalColor.rgb);

               if (primaryDistance <= _Tolerance || secondaryDistance <= _SecondaryTolerance)
               {
                    float originalBrightness = max(dot(_OriginalColor.rgb, float3(0.299, 0.587, 0.114)), 0.001);
                    float pixelBrightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    float toneMultiplier = pixelBrightness / originalBrightness;
                    return half4(saturate(_ReplacementColor.rgb * toneMultiplier), col.a);
               }

               return col;
            }

            ENDCG
        }
    }

}
