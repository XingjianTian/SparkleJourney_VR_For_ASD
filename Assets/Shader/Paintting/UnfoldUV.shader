Shader "Unlit/UnfoldUV"
{
    Properties
    {
        _MainColor("MainColor",Color) = (1,0,0,1)
        _BrushSize("BrushSize",Float) = 1
        _EnablePaintting("EnablePaintting",int) = 0
        _CurMousePos("CurrentMousePos",Vector) = (0,0,0,0)
        _PrevMousePos("PrevMousePos",Vector) = (0,0,0,0)

        _PrevTex("PrevTex",2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            fixed4 _MainColor;
            float _BrushSize;
            int _EnablePaintting;
            float4 _CurMousePos;
            float4 _PrevMousePos;

            sampler2D _PrevTex;

            struct v2f
            {
                float4 pos: SV_POSITION;
                float4 worldPos: TEXCOORD0;
                fixed2 uv: texcoord1;
            };


            v2f vert (appdata_base v)
            {
                float4 uvPoint = float4(0,0,0.3,1);
                uvPoint.xy = float2(1,_ProjectionParams.x) * (v.texcoord.xy * 2 - 1);
                v2f o;
                o.pos = uvPoint;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            bool IsPointInLine(float3 curMousePos,float3 prevMousePos,float3 pWorldPos,float brushSize)
            {
                float3 brushDir = normalize(curMousePos - prevMousePos);
                float3 pre2pWorldDir = pWorldPos - prevMousePos;
                float mapDistance = dot(brushDir,pre2pWorldDir);
                if (mapDistance < 0)
                {
                    return false;
                }
                float cur2prevDistance = length(curMousePos-prevMousePos);
                if (mapDistance > cur2prevDistance)
                {
                    return false;
                }
                float3 lineComparePoint = mapDistance / cur2prevDistance * (curMousePos - prevMousePos) + prevMousePos;
                if (distance(pWorldPos,lineComparePoint) < brushSize)
                {
                    return true;
                }
                return false;
            }

            bool IsInTheLine(float4 prePos,float4 curPos,float4 pieWPos,float brushSize)
            {
                float3 moveDir = normalize(curPos.xyz - prePos.xyz);
                float3 pieDir = pieWPos.xyz - prePos.xyz;
                float dis = dot(moveDir,pieDir);
                float gapLength = length(curPos.xyz - prePos.xyz);
                if (dis < 0)
                {
                    return false;
                }
                if (dis > gapLength)
                {
                    return false;
                }
                float3 linePoint = (curPos.xyz - prePos.xyz) * ( dis/gapLength) + prePos.xyz;
                if (distance(pieWPos.xyz,linePoint) <= brushSize)
                {
                    return true;
                }
                return false;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // if (_EnablePaintting && IsPointInLine(_CurMousePos.xyz,_PrevMousePos.xyz,i.worldPos,_BrushSize))
                if (_EnablePaintting && IsInTheLine(_PrevMousePos,_CurMousePos,i.worldPos,_BrushSize))
                {
                    return _MainColor;
                }
                if (_EnablePaintting && distance(_CurMousePos.xyz,i.worldPos) < _BrushSize)
                {
                    return _MainColor;
                }
                return fixed4(tex2D(_PrevTex,i.uv).rgb,1);
            }
            ENDCG
        }
    }
}
