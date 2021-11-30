Shader "Custom/TransformShader"
{
    Properties
    {
        // 基础色
        _Color ("BaseColor", Color) = (1.0, 1.0, 1.0, 1.0)
        // 位移量
        _Translate("Translate", Vector) = (0.0, 0.0, 0.0, 1.0)
        // 缩放量
        _Scale("Scale", Vector) = (1.0, 1.0, 1.0, 1.0)
        // 旋转量
        _Rotation("Rotation", Vector) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "IgnoreProjector" = "True" 
            "RenderPipeline" = "UniversalPipeline" 
            "ShaderModel"="2.0"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _Translate;
            float4 _Scale;
            float4 _Rotation;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : NORMAL;
            };

            // 位移矩阵
            float4x4 Translate(float4 translate)
            {
                return float4x4(1.0, 0.0, 0.0, translate.x,
                                0.0, 1.0, 0.0, translate.y,
                                0.0, 0.0, 1.0, translate.z,
                                0.0, 0.0, 0.0, 1.0);
            }

            // 缩放矩阵
            float4x4 Scale(float4 scale)
            {
                return float4x4(scale.x, 0.0, 0.0, 0.0,
                                0.0, scale.y, 0.0, 0.0,
                                0.0, 0.0, scale.z, 0.0,
                                0.0, 0.0, 0.0, 1.0);
            }

            // 旋转矩阵
            float4x4 Rotation(float4 rotation)
            {
                float radX = radians(rotation.x);
                float radY = radians(rotation.y);
                float radZ = radians(rotation.z);

                float sinX = sin(radX);
                float cosX = cos(radX);
                float sinY = sin(radY);
                float cosY = cos(radY);
                float sinZ = sin(radZ);
                float cosZ = cos(radZ);

                return float4x4(cosY * cosZ, -cosY * sinZ, sinY, 0.0,
                                cosX * sinZ + sinX * sinY * cosZ, cosX * cosZ - sinX * sinY * sinZ, -sinX * cosY, 0.0,
                                sinX * sinZ - cosX * sinY * cosZ, sinX * cosZ + cosX * sinY * sinZ, cosX * cosY, 0.0,
                                0.0, 0.0, 0.0, 1.0);
            }

            // 顶点函数
            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);

                float4 vectex = input.positionOS;
                // 先缩放、后旋转、再平移
                // 应用缩放
                vectex = mul(Scale(_Scale), vectex);
                // 应用旋转
                vectex = mul(Rotation(_Rotation), vectex);
                // 应用位移
                vectex = mul(Translate(_Translate), vectex);
                // MVP
                output.positionCS = TransformObjectToHClip(vectex.xyz);
                // Normal
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            // 表面着色器函数
            half4 frag(Varyings input) : SV_TARGET
            {
                // lambert lighting
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(input.normalWS, mainLight.direction));
                half3 diffuse = (mainLight.color * NdotL) * 0.5 + 0.5;
                return half4(diffuse * _Color, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor ""
}