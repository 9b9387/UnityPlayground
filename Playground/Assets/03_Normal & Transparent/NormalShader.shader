Shader "Custom/NormalShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Unlit 
        #pragma target 3.0

        // 无光照的光照模型，直接返回Albedo颜色
        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(s.Albedo, 1);
        }

        struct Input
        {
            float3 worldNormal;
        };

        // 法线值赋值给Albedo
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = UnityObjectToWorldNormal(IN.worldNormal) * 0.5 + 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
