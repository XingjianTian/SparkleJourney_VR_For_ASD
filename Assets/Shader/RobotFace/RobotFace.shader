Shader "Unlit/RobotFace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeThreshold ("边缘区域宽度", Range(0, 0.5)) = 0.05
        _ColorThreshold ("黑色判定阈值", Range(0, 1)) = 0.1
        _BaseColor ("基础颜色", Color) = (1,1,1,1)
        
        // 玻璃质感属性
        _GlassTint ("玻璃颜色", Color) = (0.8,0.9,1.0,1)
        _ReflectionIntensity ("反射强度", Range(0, 1)) = 0.4
        _SpecularPower ("高光强度", Range(1, 100)) = 50
        _SpecularIntensity ("高光亮度", Range(0, 2)) = 1
        _FresnelPower ("菲涅耳效果强度", Range(0, 5)) = 2.5
        _RimColor ("边缘光颜色", Color) = (0.5,0.7,1.0,1)
        _RimPower ("边缘光强度", Range(0, 8)) = 3.0
        _GlossAmount ("光泽度", Range(0, 1)) = 0.5
        _BumpMap ("法线贴图", 2D) = "bump" {}
        _BumpAmount ("法线强度", Range(0, 1)) = 0.3
        _Smoothness ("平滑度", Range(0, 1)) = 0.8
        _Metallic ("金属感", Range(0, 1)) = 0.2
        
        // 白色裁剪参数
        _WhiteClipThreshold ("白色裁剪阈值", Range(0.5, 1)) = 0.85
        [Toggle] _InvertClipping ("反转裁剪", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        // 已移除透明度混合
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 worldNormal : TEXCOORD4;
                float3 worldTangent : TEXCOORD5;
                float3 worldBinormal : TEXCOORD6;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EdgeThreshold;
            float _ColorThreshold;
            float4 _BaseColor;
            
            // 玻璃效果参数
            float4 _GlassTint;
            float _ReflectionIntensity;
            float _SpecularPower;
            float _SpecularIntensity;
            float _FresnelPower;
            float4 _RimColor;
            float _RimPower;
            float _GlossAmount;
            sampler2D _BumpMap;
            float _BumpAmount;
            float _Smoothness;
            float _Metallic;
            
            // 裁剪参数
            float _WhiteClipThreshold;
            float _InvertClipping;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // 世界空间位置和法线
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                // 计算切线空间转换矩阵，用于法线贴图
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;
                
                // 视角方向
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // 计算简化的环境反射
            float3 GetEnvironmentReflection(float3 worldPos, float3 worldNormal, float3 viewDir, float roughness)
            {
                // 简单的基于高度的天空渐变色
                float3 skyColor = lerp(float3(0.5, 0.7, 1.0), float3(0.2, 0.3, 0.5), 0.5 * worldNormal.y + 0.5);
                
                // 计算反射向量
                float3 reflectDir = reflect(-viewDir, worldNormal);
                
                // 添加一些假的环境光与反射色混合
                float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb * 1.5;
                
                // 基于粗糙度调整反射强度
                float reflectivity = 1.0 - roughness * 0.5;
                
                return lerp(ambientLight, skyColor, reflectivity);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 上下翻转UV坐标
                float2 flippedUV = float2(i.uv.x, 1.0 - i.uv.y);
                
                // 使用翻转后的UV采样贴图
                fixed4 col = tex2D(_MainTex, flippedUV);
                
                // 检查是否为白色区域，如果是则裁剪
                float brightness = max(max(col.r, col.g), col.b);
                bool isWhite = brightness > _WhiteClipThreshold;
                
                // 根据_InvertClipping决定是裁剪白色还是非白色
                if (_InvertClipping < 0.5 ? isWhite : !isWhite) {
                    clip(-1); // 丢弃该片段
                    return fixed4(0, 0, 0, 0);
                }
                
                // 使用材质面板参数
                float edgeThreshold = _EdgeThreshold;
                float colorThreshold = _ColorThreshold;
                
                // 检查像素是否在边缘区域内
                bool isInEdgeRegion = flippedUV.x < edgeThreshold || 
                                     flippedUV.x > (1.0 - edgeThreshold) || 
                                     flippedUV.y < edgeThreshold || 
                                     flippedUV.y > (1.0 - edgeThreshold);
                
                // 只替换边缘区域内的黑色像素为白色
                if (isInEdgeRegion && col.r <= colorThreshold && col.g <= colorThreshold && col.b <= colorThreshold) {
                    col.rgb = fixed3(1, 1, 1); // 设为白色
                }
                
                // 将贴图颜色与基础颜色叠加
                fixed4 baseColor = col * _BaseColor;
                
                // ---- 玻璃效果计算 ----
                
                // 采样法线贴图并应用强度
                float3 normalMap = UnpackNormal(tex2D(_BumpMap, flippedUV));
                normalMap.xy *= _BumpAmount;
                normalMap = normalize(normalMap);
                
                // 构建切线空间到世界空间的转换矩阵
                float3x3 tangentToWorld = float3x3(
                    i.worldTangent,
                    i.worldBinormal,
                    i.worldNormal
                );
                
                // 将法线从切线空间转换到世界空间
                float3 worldNormal = mul(normalMap, tangentToWorld);
                worldNormal = normalize(worldNormal);
                
                // 计算菲涅耳效应 (视角与法线夹角决定的反射率)
                float nDotV = saturate(dot(i.viewDir, worldNormal));
                float fresnel = pow(1.0 - nDotV, _FresnelPower);
                
                // 获取环境反射
                float roughness = 1.0 - _Smoothness;
                float3 envReflection = GetEnvironmentReflection(i.worldPos, worldNormal, i.viewDir, roughness);
                
                // 计算简单的反射
                float3 reflectionColor = envReflection * _GlassTint.rgb;
                
                // 获取主光源方向（通常是方向光，如太阳）
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                // 计算高光反射
                float3 halfVector = normalize(lightDir + i.viewDir);
                float nDotH = saturate(dot(worldNormal, halfVector));
                float specular = pow(nDotH, _SpecularPower) * _SpecularIntensity;
                
                // 计算边缘光
                float rim = 1.0 - nDotV;
                float rimFactor = pow(rim, _RimPower);
                
                // 混合所有效果
                fixed4 finalColor = baseColor;
                
                // 添加环境反射和玻璃色调
                finalColor.rgb = lerp(finalColor.rgb, reflectionColor, _ReflectionIntensity * 0.5);
                
                // 添加菲涅耳反射
                finalColor.rgb = lerp(finalColor.rgb, reflectionColor, fresnel * _ReflectionIntensity);
                
                // 添加高光
                finalColor.rgb += specular * _GlassTint.rgb;
                
                // 添加边缘光
                finalColor.rgb += rimFactor * _RimColor.rgb;
                
                // 设置为完全不透明
                finalColor.a = 1.0;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
