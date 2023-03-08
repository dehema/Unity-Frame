Shader "Unlit/Flash"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("_NoiseTex",2D) = "white"{}
        _FlashOffset("_FlashOffset",float) = 1
        _NoisePower("_NoisePower",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
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
                float2 uv_1 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _MainTex_ST;
            float _NoisePower;
            float _FlashOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_1 = TRANSFORM_TEX(v.uv_1, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                i.uv_1 += _FlashOffset;
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 NoiseCol = tex2D(_NoiseTex,i.uv_1);
                NoiseCol.a = NoiseCol.r;
                NoiseCol *= col*_NoisePower;
                col+=NoiseCol;
                return col;
            }
            ENDCG
        }
    }
}
