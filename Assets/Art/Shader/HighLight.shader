Shader "Unlit/HighLight"
{
    Properties
    {
        _LightTex ("Texture", 2D) = "white" {}
        _NoiseTex("_NoiseTex",2D) = "white"{}
        _Speed("_Speed",float) = 1
        _MainCol("_MainCol",color) = (0,0,0,0)
        _NoiseCol("_NoiseCol",color) = (0,0,0,0)
        _AlphaOffset("_AlphaOffset",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque"
                "Queue" = "Transparent" }
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

            sampler2D _LightTex;
            float4 _LightTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _Speed;
            float _AlphaOffset;
            float4 _MainCol;
            float4 _NoiseCol;


           

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _LightTex);
                o.uv_1 = TRANSFORM_TEX(v.uv_1, _NoiseTex);
                return o;
            }

             float2 Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation)
            {
                //rotation matrix
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);

                //center rotation matrix
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix*2 - 1;

                //multiply the UVs by the rotation matrix
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;
                float2 OutUV = UV;
                return OutUV;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 outuv = Unity_Rotate_Radians_float(i.uv_1,float2(0.5,0.5),_Time.x*_Speed);
                // sample the texture
                float4 NoiseCol = tex2D(_NoiseTex,i.uv_1);
                NoiseCol.a = NoiseCol.r;
                float4 col = tex2D(_LightTex, outuv);
                col*=NoiseCol;
                //NoiseCol *=2;
                NoiseCol*=_MainCol;
                NoiseCol+=col;
                NoiseCol.a*=_AlphaOffset;
                return NoiseCol;
            }
            ENDCG
        }
    }
}
