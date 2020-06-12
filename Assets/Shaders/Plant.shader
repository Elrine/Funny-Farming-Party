Shader "Custom/Plant"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        float _GrowPercent;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color;
            clip(_GrowPercent - IN.uv_MainTex.y);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
