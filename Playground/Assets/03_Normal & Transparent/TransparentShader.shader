// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Unlit alpha:auto
        #pragma target 3.0

        // 无光照的光照模型，直接返回Albedo颜色
        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(s.Albedo, s.Alpha);
        }

        struct Input
        {
            float3 worldNormal;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = (UnityObjectToWorldNormal(IN.worldNormal) * 0.5 + 0.5) * _Color;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
