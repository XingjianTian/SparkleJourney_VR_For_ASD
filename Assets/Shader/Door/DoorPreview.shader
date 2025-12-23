Shader "Unlit/DoorPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XOffset("XOffset",Range(-0.3,0.3)) = 0
        _YOffset("YOffset",Range(-0.3,0.3)) = 0
        _DissolveTex("Dissolve Texture", 2D) = "white" {} // 添加溶解纹理属性
        _DissolveThreshold("Dissolve Amount", Range(0, 1)) = 0 // 添加溶解进度属性
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0, 0.1)) = 0.01 // 添加溶解边缘宽度属性
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

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 dissolveUV : TEXCOORD1; // 添加溶解纹理坐标
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _XOffset;
            float _YOffset;
            sampler2D _DissolveTex; // 添加溶解纹理
            float4 _DissolveTex_ST; // 添加溶解纹理坐标变换
            float _DissolveThreshold; // 添加溶解进度
            float _DissolveEdgeWidth; // 添加溶解边缘宽度

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex) * 0.4 + fixed2(0.3 + _XOffset,0.3 + _YOffset);
                o.dissolveUV = TRANSFORM_TEX(v.texcoord, _DissolveTex); // 传递溶解纹理坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed dissolveValue = tex2D(_DissolveTex, i.dissolveUV).r; // 获取溶解纹理的灰度值

                // 使用平滑步函数来平滑溶解边缘
                fixed edge = smoothstep(_DissolveThreshold - _DissolveEdgeWidth, _DissolveThreshold, dissolveValue);

                // 根据溶解进度裁剪片段
                clip(edge - 0.5);

                return fixed4(col.rgb, 1);
            }
            ENDCG
        }
    }
}