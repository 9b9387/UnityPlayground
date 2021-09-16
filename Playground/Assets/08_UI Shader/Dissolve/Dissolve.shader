Shader "Owlet/2D Unlit/Dissolve" 
{
    Properties
    {
        _EageColor("Eage Color", Color) = (1, 1, 1, 1)
        _EageWidth("Eage Width", Range(0, 10)) = 5
        _NoiseScale("Noise Scale", Float) = 30
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
            Name "Owlet 2D Dissolve"
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
            #include "Assets/Shaders/SimpleNoise.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 uv: TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float4 _EageColor;
            float _EageWidth;
            float _NoiseScale;
            CBUFFER_END
            
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

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);

                // generate noise, define in Assets/Shaders/SimpleNoise.hlsl
                float noise = SimpleNoise(input.uv, _NoiseScale);

                // remap sin time -1,1 to 0,1
                float remap_sintime = (_SinTime.w + 1) * 0.5;

                // calculate eage color
                float step_noise_inner = step(noise, remap_sintime);
                float step_noise_outer = step(noise, remap_sintime + _EageWidth * 0.01f);
                float3 eage_color = (step_noise_outer - step_noise_inner) * _EageColor.rgb;

                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 color = SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, input.uv.xy);
                // add eage color
                color.rgb = color.rgb * step_noise_inner + eage_color.rgb;

                // calculate alpha
                color.a = step_noise_outer * color.a;
				return color;
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}