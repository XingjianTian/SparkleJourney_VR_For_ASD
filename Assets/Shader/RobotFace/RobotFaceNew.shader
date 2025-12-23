Shader "Unlit/RobotFaceNew"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeThreshold ("边缘区域宽度", Range(0, 0.5)) = 0.05
        _ColorThreshold ("黑色判定阈值", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            float _EdgeThreshold;
            float _ColorThreshold;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                 // 使用材质面板参数
                float edgeThreshold = _EdgeThreshold;
                float colorThreshold = _ColorThreshold;

                float2 flippedUV = float2(i.uv.x, 1.0 - i.uv.y);
                // sample the texture
                fixed4 col = tex2D(_MainTex, flippedUV);
                bool isInEdgeRegion = flippedUV.x < edgeThreshold ||
                                     flippedUV.x > (1.0 - edgeThreshold) ||
                                     flippedUV.y < edgeThreshold ||
                                     flippedUV.y > (1.0 - edgeThreshold);
                 if (isInEdgeRegion)
                 {
                     return 0;
                 }
                 if (col.r != 0)
                 {
                     return 0;
                 }
                return 1- col;
            }
            ENDCG
        }
    }
}
