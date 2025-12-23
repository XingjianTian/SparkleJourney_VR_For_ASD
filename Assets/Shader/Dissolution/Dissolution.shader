Shader "Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveTex("DissolveTex", 2D) = "white" {}
        _DissolveThreshold("DissolveThreshold", Range(0, 1)) = 0
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0, 0.1)) = 0.01 // 添加溶解边缘宽度属性
        _LightColor0 ("Light Color", Color) = (1,1,1,1)
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
            #include "Lighting.cginc" // 引入光照计算所需的函数和变量

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DissolveTex;
            fixed _DissolveThreshold;
            fixed _DissolveEdgeWidth; // 添加溶解边缘宽度属性

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 dissolveCol = tex2D(_DissolveTex, i.uv);

                // 使用 smoothstep 函数来平滑溶解边缘
                fixed edge = smoothstep(_DissolveThreshold - _DissolveEdgeWidth, _DissolveThreshold, dissolveCol.r);

                // 根据平滑后的边缘值裁剪片段
                clip(edge - 0.5);

                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算光照
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diffuse = saturate(dot(worldNormal, lightDir));

                // 结合光照和材质颜色
                col.rgb *= diffuse * _LightColor0.rgb;

                return col;
            }
            ENDCG
        }
    }
}