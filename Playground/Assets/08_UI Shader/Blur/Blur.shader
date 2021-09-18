Shader "Owlet/2D Unlit/Blur" 
{
    Properties
    {
        _BlurAmount("Blur Amount", Range(0, 5)) = 2
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
            Name "Owlet 2D Blur"
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
            half _BlurAmount;
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
                half amount = _BlurAmount * 0.01;

                half4 color = offset_sample(input.uv, -amount, 0);
                color += offset_sample(input.uv, amount, 0);
                color += offset_sample(input.uv, 0, -amount);
                color += offset_sample(input.uv, 0, amount);
                color += offset_sample(input.uv, -amount, amount);
                color += offset_sample(input.uv, amount, amount);
                color += offset_sample(input.uv, amount, -amount);
                color += offset_sample(input.uv, -amount, -amount);
                
				return color /= 8;
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}