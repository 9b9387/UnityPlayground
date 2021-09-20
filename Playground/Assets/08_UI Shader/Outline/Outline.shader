Shader "Owlet/2D Unlit/Outline" 
{
    Properties
    {
        [PerRendererData]_MainTex("Texture2D", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 0)
        _OutlineWidth("Outline Width", Range(0, 2)) = 1.86
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Owlet 2D Outline"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM

            // Pragmas
            #pragma vertex vert
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv: TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;
            CBUFFER_END
            
            // Object and Global properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = input.uv;
                return output;
            }

            half4 offset_sample(half2 uv, half offset_x, half offset_y)
            {
                half2 sample_uv = uv;
                sample_uv.x += offset_x;
                sample_uv.y += offset_y;

                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                return SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, sample_uv);
            }

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);

                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 color = SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, input.uv);
                half4 final = half4(color.rgb * color.a, color.a) + (1 - color.a) * _OutlineColor;

                half width = _OutlineWidth * 0.01;

                half alpha = offset_sample(input.uv, -width, 0).a;
                alpha += offset_sample(input.uv, width, 0).a;
                alpha += offset_sample(input.uv, 0, -width).a;
                alpha += offset_sample(input.uv, 0, width).a;
                alpha += offset_sample(input.uv, -width, width).a;
                alpha += offset_sample(input.uv, width, width).a;
                alpha += offset_sample(input.uv, width, -width).a;
                alpha += offset_sample(input.uv, -width, -width).a;
                
				return half4(final.rgb, saturate(alpha * final.a));
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}