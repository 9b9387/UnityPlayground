Shader "Playground/01 View Transform"
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

            float4x4 _CameraInfo;
            float4x4 _MyViewMatrix;
            float4x4 _MyOrthoMatrix;
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

            //TRS变换矩阵
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

            float4x4 Get_Ortho_Matrix(float l, float r, float b, float t, float n, float f)
            {
                float4x4 translation = float4x4(
                    1, 0, 0, -(l + r) / 2,
                    0, 1, 0, -(b + t) / 2,
                    0, 0, 1, -(f + n) / 2,
                    0, 0, 0, 1
                );

                float4x4 scale = float4x4(
                    2 / (r - l), 0, 0, 0,
                    0, 2 / (t - b), 0, 0,
                    0, 0, 2 / (n - f), 0,
                    0, 0, 0, 1
                );

                return scale * translation;
            }

            float4x4 Get_ViewPort_Matrix()
            {
                float width = _ScreenParams.x;
                float height = _ScreenParams.y;
                return float4x4(
                    width / 2, 0, 0, width / 2,
                    0, height / 2, 0, height / 2,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                );
            }

            // 计算视图矩阵
            float4x4 Get_LookAt_Matrix(float4x4 cameraInfo)
            {
                float4x4 translation = float4x4(
                    1, 0, 0, -cameraInfo[3][0],
                    0, 1, 0, -cameraInfo[3][1],
                    0, 0, 1, -cameraInfo[3][2],
                    0, 0, 0, 1
                );
                float4x4 rotation = float4x4(
                    cameraInfo[0][0], cameraInfo[0][1], cameraInfo[0][2], 0,
                    cameraInfo[1][0], cameraInfo[1][1], cameraInfo[1][2], 0,
                    -cameraInfo[2][0], -cameraInfo[2][1], -cameraInfo[2][2], 0,
                    0, 0, 0, 1
                );

                return mul(rotation, translation);
            }

            // 顶点函数
            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);

                float4 positionWS = mul(TransfromTRS(_Translate, _Rotation, _Scale), input.positionOS);
                // positionWS = mul(Get_LookAt_Matrix(_CameraInfo), positionWS);
                positionWS = mul(_MyViewMatrix, positionWS);
                positionWS = mul(_MyOrthoMatrix, float4(positionWS.xyz, 1));
                
                // positionWS = mul(UNITY_MATRIX_V, positionWS);
                // positionWS = mul(Get_Ortho_Matrix(0, 1.92, 0, 1.08, 0.1, 100), positionWS);
                // positionWS = mul(Get_ViewPort_Matrix(), positionWS);
                // mul(Get_ViewPort_Matrix(), 
                // mul(Get_Ortho_Matrix(0, _ScreenParams.x, 0, _ScreenParams.y, 0.1, 100), 
                // output.positionCS = mul(Get_LookAt_Matrix(cameraPositionWS, targetPositionWS), positionWS);
                output.positionCS = positionWS;
                // output.positionCS = TransformObjectToHClip(input.positionOS);
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
