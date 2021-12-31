Shader "Playground/01 ObjectPosition"
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
                float3 normalOS : NORMAL;
            };

            // TRS变换矩阵
            float4x4 TransfromTRS(float4 translate, float4 rotation, float4 scale)
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

                return float4x4(cosY * cosZ * scale.x, -cosY * sinZ * scale.y, sinY * scale.z, translate.x,
                                (cosX * sinZ + sinX * sinY * cosZ) * scale.x, (cosX * cosZ - sinX * sinY * sinZ) * scale.y, -sinX * cosY * scale.z, translate.y,
                                (sinX * sinZ - cosX * sinY * cosZ) * scale.x, (sinX * cosZ + cosX * sinY * sinZ) * scale.y, cosX * cosY * scale.z, translate.z,
                                0.0, 0.0, 0.0, 1.0);
            }

            // 顶点函数
            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);

                output.positionCS = mul(TransfromTRS(_Translate, _Rotation, _Scale), input.positionOS);
                output.normalOS = input.normalOS;
                return output;
            }

            // 表面着色器函数
            half4 frag(Varyings input) : SV_TARGET
            {
                // lambert lighting
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(input.normalOS, mainLight.direction));
                half3 diffuse = (mainLight.color * NdotL) * 0.5 + 0.5;
                return half4(diffuse * _Color.xyz, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor ""
}
