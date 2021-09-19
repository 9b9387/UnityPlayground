Shader "Owlet/2D Unlit/Hologram" 
{
    Properties
    {
        _HologramColor("Hologram Color", Color) = (0.1769758, 0.7235747, 0.9150943, 1)
        _HologramSpeed("Hologram Speed", Float) = 0.5
        _HologramFrequency("Hologram Frequency", Float) = 1
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
            Name "Owlet 2D Hologram"
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
            float4 _HologramColor;
            float _HologramSpeed;
            float _HologramFrequency;
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

                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv.xy = input.uv.xy;
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);

                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 color = SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, input.uv.xy);
                half grey = (color.r + color.g + color.b) / 3;
                half alpha = grey * color.a * _HologramColor.a;
                    
                half offset = 0.3 + fmod((input.uv.xy + _Time.y * _HologramSpeed.xx).y * _HologramFrequency, 1) * 0.7;
				return half4(_HologramColor.rgb, alpha * offset);
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}