Shader "Custom/GlassShader"
{
    Properties
    {
        _Color ("颜色", Color) = (1,1,1,1) // 玻璃的基础颜色
        _MainTex ("主纹理 (RGB)", 2D) = "white" {} // 玻璃的纹理贴图
        _Glossiness ("光滑度", Range(0,1)) = 0.5 // 控制玻璃表面的光滑程度
        _Metallic ("金属度", Range(0,1)) = 0.0 // 控制玻璃的金属感
        _RefractionIndex ("折射率", Range(1, 2)) = 1.5 // 控制光线的折射效果
        _ReflectionStrength ("反射强度", Range(0,1)) = 0.5 // 控制反射效果的强度
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" } // 设置渲染队列为透明
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" } // 使用URP的前向渲染模式

            Blend SrcAlpha OneMinusSrcAlpha // 设置透明混合模式
            ZWrite Off // 关闭深度写入，确保透明物体正确渲染
            Cull Back // 只渲染物体的背面

            HLSLPROGRAM
            #pragma vertex vert // 顶点着色器
            #pragma fragment frag // 片段着色器
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" // 引入URP的核心库

            struct appdata
            {
                float4 vertex : POSITION; // 顶点位置
                float2 uv : TEXCOORD0; // 纹理坐标
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; // 传递纹理坐标
                float4 pos : SV_POSITION; // 裁剪空间中的顶点位置
                float3 worldPos : TEXCOORD1; // 世界空间中的顶点位置
                float3 worldNormal : TEXCOORD2; // 世界空间中的法线
            };

            sampler2D _MainTex; // 主纹理
            float4 _Color; // 颜色属性
            float _Glossiness; // 光滑度属性
            float _Metallic; // 金属度属性
            float _RefractionIndex; // 折射率属性
            float _ReflectionStrength; // 反射强度属性

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex); // 将顶点位置转换到裁剪空间
                o.uv = v.uv; // 传递纹理坐标
                o.worldPos = TransformObjectToWorld(v.vertex); // 将顶点位置转换到世界空间
                o.worldNormal = TransformObjectToWorldNormal(v.vertex); // 将法线转换到世界空间
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos); // 计算视线方向
                float3 worldNormal = normalize(i.worldNormal); // 归一化法线

                // 折射计算
                float3 refracted = refract(-worldViewDir, worldNormal, 1.0 / _RefractionIndex); // 根据折射率计算折射方向
                float4 refractionColor = tex2D(_MainTex, i.uv + refracted.xy); // 采样折射后的颜色

                // 反射计算
                float3 reflected = reflect(-worldViewDir, worldNormal); // 计算反射方向
                float4 reflectionColor = tex2D(_MainTex, i.uv + reflected.xy); // 采样反射后的颜色

                // 混合反射和折射效果
                float4 finalColor = lerp(refractionColor, reflectionColor, _ReflectionStrength) * _Color; // 根据反射强度混合颜色

                // 应用金属度和光滑度
                finalColor.rgb *= _Metallic; // 调整金属感
                finalColor.a = _Glossiness; // 调整透明度（光滑度）

                return finalColor; // 返回最终颜色
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse" // 回退到默认的漫反射Shader
}