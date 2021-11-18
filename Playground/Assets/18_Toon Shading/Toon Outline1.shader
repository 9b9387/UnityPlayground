Shader "Demo/Toon/Outline1"
{
    Properties
    {
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        [Space]
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Float) = 0.0
        _OutlineDepthOffset("Outline Depth Offset", Range(0, 1)) = 0.0
        _CameraDistanceImpact("Outline Camera Distance Impact", Range(0, 1)) = 0.5
        [Toggle(USE_PRECALCULATED_OUTLINE_NORMALS)] _PrecalculateNormals("Use UV1 normals", Float) = 0
    }

    SubShader
    {
        Tags {
            "RenderType" = "Opaque" 
            "IgnoreProjector" = "True" 
            "RenderPipeline" = "UniversalPipeline" 
            "ShaderModel"="2.0"
        }
        LOD 100

        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 vertex       : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.vertex = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                return _BaseColor;
            }
            ENDHLSL
        }
        Pass
        {
            Name "Outline"
            Tags{"LightMode" = "SRPDefaultUnlit"}

            Cull Front

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half4 _OutlineColor;
            half _OutlineWidth;
            half _OutlineDepthOffset;
            half _CameraDistanceImpact;
            CBUFFER_END

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #pragma shader_feature USE_PRECALCULATED_OUTLINE_NORMALS
            #pragma multi_compile_fog

            struct VertexInput
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
#ifdef USE_PRECALCULATED_OUTLINE_NORMALS
                float3 smoothNormalOS : TEXCOORD1;
#endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                
                float fogCoord : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            VertexOutput VertexProgram(VertexInput v)
            {
                VertexOutput o;

                UNITY_SETUP_INSTANCE_ID(v);

                o = (VertexOutput)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 clipPosition = TransformObjectToHClip(v.position);
                const float3 clipNormal = mul((float3x3)UNITY_MATRIX_VP, mul((float3x3)UNITY_MATRIX_M, v.normal));
                const half outlineWidth = _OutlineWidth;
                const half cameraDistanceImpact = lerp(clipPosition.w, 4.0, _CameraDistanceImpact);
                const float2 aspectRatio = float2(_ScreenParams.x / _ScreenParams.y, 1);
                const float2 offset = normalize(clipNormal.xy) / aspectRatio * outlineWidth * cameraDistanceImpact * 0.005;
                clipPosition.xy += offset;
                const half outlineDepthOffset = _OutlineDepthOffset;

                #if UNITY_REVERSED_Z
                clipPosition.z -= outlineDepthOffset * 0.1;
                #else
                clipPosition.z += outlineDepthOffset * 0.1 * (1.0 - UNITY_NEAR_CLIP_VALUE);
                #endif
                
                o.position = clipPosition;
                o.normal = clipNormal;

                o.fogCoord = ComputeFogFactor(o.position.z);

                return o;
            }

            half4 FragmentProgram(VertexOutput i) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color = _OutlineColor;
                color.rgb = MixFog(color.rgb, i.fogCoord);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor ""
}
