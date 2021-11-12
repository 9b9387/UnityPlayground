Shader "Owlet/Procedural/Noise Glitch"
{
    Properties
    {
        _GlitchAmount("GlitchAmount", Range(0, 10)) = 0.9
        _GlitchSize("GlitchSize", Range(0, 10)) = 1
        _BaseColor("Color", Color) = (1, 1, 1, 1)
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
            Name "Unlit"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Procedural.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _GlitchAmount;
                float _GlitchSize;
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
            half rand2(half2 seed, half offset) {
                return (frac(sin(dot(seed * floor(50 + (_Time % 1.0) * 12.), half2(127.1, 311.7))) * 43758.5453123) + offset) % 1.0;
            }
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half2 uvGlitch = input.uv;
				uvGlitch.y -= 0.5;
				half lineNoise = pow(rand2(floor(uvGlitch * half2(24., 19.) * _GlitchSize) * 4.0, 500), 3.0) * _GlitchAmount
					* pow(rand2(floor(uvGlitch * half2(38., 14.) * _GlitchSize) * 4.0, 500), 3.0);
				uvGlitch = input.uv + half2(lineNoise * 0.02 * rand2(half2(2.0, 1), 500), 0);

                return _BaseColor * Rectangle(uvGlitch, 0.7, 0.7);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor ""
}