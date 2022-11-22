Shader "NovelEditor/BlurEffect"
{
        Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength("Strength",float) =5
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Strength;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col += tex2D(_MainTex, i.uv+float2(-_Strength,_Strength));
                col += tex2D(_MainTex, i.uv+float2(0,_Strength));
                col += tex2D(_MainTex, i.uv+float2(_Strength,_Strength));
                col += tex2D(_MainTex, i.uv+float2(-_Strength,0));
                col += tex2D(_MainTex, i.uv+float2(_Strength,0));
                col += tex2D(_MainTex, i.uv+float2(-_Strength,-_Strength));
                col += tex2D(_MainTex, i.uv+float2(0,-_Strength));
                col += tex2D(_MainTex, i.uv+float2(_Strength,-_Strength));
                col.r/=9;
                col.g/=9;
                col.b/=9;
                return col;
            }
            ENDCG
        }
    }
}
