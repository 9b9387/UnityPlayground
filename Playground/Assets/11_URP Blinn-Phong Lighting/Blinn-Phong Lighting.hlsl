#ifndef BLINNPHONG_LIGHT_INCLUDED
#define BLINNPHONG_LIGHT_INCLUDED

#if defined(SHADERGRAPH_PREVIEW)
#else
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#endif

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float ShadowAtten)
{
#if defined(SHADERGRAPH_PREVIEW)
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    ShadowAtten = 1;
#else
    // 定义在Shadows.hlsl文件内
    // 需要开启 _MAIN_LIGHT_SHADOWS_CASCADE
    // float4 TransformWorldToShadowCoord(float3 positionWS)
    // {
    //     #ifdef _MAIN_LIGHT_SHADOWS_CASCADE
    //     half cascadeIndex = ComputeCascadeIndex(positionWS);
    //     #else
    //     half cascadeIndex = 0;
    //     #endif

    //     float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1.0));

    //     return float4(shadowCoord.xyz, cascadeIndex);
    // }
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    // 定义在Lighting.hlsh文件内
    // MainLightRealtimeShadow用来计算光照衰减
    // 需要开启 _MAIN_LIGHT_SHADOWS
    // Light GetMainLight(float4 shadowCoord)
    // {
    //     Light light = GetMainLight();
    //     light.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
    //     return light;
    // }
    Light mainLight = GetMainLight(shadowCoord);
    // 返回光源方向
    Direction = mainLight.direction;
    // 返回光源颜色
    Color = mainLight.color;
    // 暂时不考虑阴影衰减
    ShadowAtten = 1.0h;
#endif
}

void Lambert_float(float3 LightColor, float3 Direction, float3 WorldNormal, out float3 Out)
{
#if defined(SHADERGRAPH_PREVIEW)
    Out = 0;
#else
    Out = LightingLambert(LightColor, Direction, WorldNormal);
#endif
}

void DirectSpecular_float(float Smoothness, float3 Direction, float3 WorldNormal, float3 WorldView, out float3 Out)
{
    float4 White = 1;

#if defined(SHADERGRAPH_PREVIEW)
    Out = 0;
#else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    Out = LightingSpecular(White, Direction, WorldNormal, WorldView, White, Smoothness);
#endif
}

#endif