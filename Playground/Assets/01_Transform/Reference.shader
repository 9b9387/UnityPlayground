Shader "Playground/01 Reference"
{
    Properties
    {
        // 基础色
        _Color ("BaseColor", Color) = (1.0, 1.0, 1.0, 1.0)
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
        Cull Off
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
            float4 _CameraWorldPosition;
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

            // 顶点函数
            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 positionVS = TransformWorldToView(positionWS);
                float4 positionCS = TransformWViewToHClip(float4(positionVS, 1));

                output.positionCS = positionCS;
                output.normalOS = TransformObjectToWorldNormal(input.normalOS);
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
