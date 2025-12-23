Shader "Custom/AdvancedNeonOutline"
{
    Properties
    {
        [HDR] _MainColor ("Main Color", Color) = (1, 0.3, 0.8, 1)
        [HDR] _SecondaryColor ("Secondary Color", Color) = (0.3, 0.8, 1, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.3)) = 0.05
        _OutlineIntensity ("Outline Intensity", Range(0, 20)) = 8.0
        _BreathSpeed ("Breath Speed", Range(0.1, 5)) = 1.2
        _ColorShiftSpeed ("Color Shift Speed", Range(0, 3)) = 1.0
        _GlowFalloff ("Glow Falloff", Range(1, 10)) = 4.0
        _UseWorldSpace ("Use World Space", Range(0, 1)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };
            
            float4 _MainColor;
            float4 _SecondaryColor;
            float _OutlineWidth;
            float _OutlineIntensity;
            float _BreathSpeed;
            float _ColorShiftSpeed;
            float _GlowFalloff;
            float _UseWorldSpace;
            
            // 屏幕空间边缘检测
            float GetEdgeFactor(v2f i)
            {
                // 计算法线差异
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float NdotV = 1 - saturate(dot(normal, viewDir));
                
                // 更精确的边缘计算
                float edge = pow(NdotV, _GlowFalloff);
                edge = smoothstep(1 - _OutlineWidth, 1.0, edge);
                
                return edge;
            }
            
            // 动态色彩变化
            float3 GetDynamicColor(float time, float3 worldPos)
            {
                // 基础颜色混合
                float colorMix = (sin(time * _ColorShiftSpeed * 0.7) + 1) * 0.5;
                float3 baseColor = lerp(_MainColor.rgb, _SecondaryColor.rgb, colorMix);
                
                // 添加彩虹色波动
                float hue = time * _ColorShiftSpeed * 0.2;
                float3 rainbow = saturate(float3(
                    sin(hue) * 0.5 + 0.5,
                    sin(hue + 2.094) * 0.5 + 0.5,
                    sin(hue + 4.188) * 0.5 + 0.5
                ));
                
                // 世界空间位置影响
                float worldEffect = _UseWorldSpace * (sin(worldPos.x + worldPos.z + time) * 0.5 + 0.5);
                return lerp(baseColor, rainbow, worldEffect * 0.3);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算精确边缘
                float edge = GetEdgeFactor(i);
                
                // 呼吸效果
                float breath = (sin(_Time.y * _BreathSpeed) * 0.5 + 0.5) * 0.5 + 0.5;
                
                // 动态颜色
                float3 dynamicColor = GetDynamicColor(_Time.y, i.worldPos * _UseWorldSpace);
                
                // 最终发光效果
                float glow = edge * _OutlineIntensity * breath;
                float4 finalColor = float4(dynamicColor * glow, glow * 0.9);
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}