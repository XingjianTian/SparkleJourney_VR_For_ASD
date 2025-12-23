Shader "Unlit/ShowNavPath"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor("MainColor",Color) = (0,1,0,1)
        _Speed("Speed",Float) = 1
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



            struct v2f
            {
                float4 pos: SV_POSITION;
                fixed4 uv: TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MainColor;
            float _Speed;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex) + float2(-_Time.y * _Speed,0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex,i.uv.xy);
                clip(color.a-0.1);
                // if (color.a == 0)
                // {
                //     clip(-1);
                // }
                return _MainColor;
            }
            ENDCG
        }
    }
}
