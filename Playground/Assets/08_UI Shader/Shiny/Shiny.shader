// Shiny.shader
// Author: shihongyang@weile.com

Shader "Owlet/2D Unlit/Shiny"
{
    Properties
    {
        [PerRendererData]_MainTex("Texture2D", 2D) = "white" {}
        _ShinySpeed("Shiny Speed", Range(-2, 2)) = 0.5
        _ShinyWidth("Shiny Width", Range(1, 10)) = 5.83
        _ShinyAngle("Shiny Angle", Range(-1, 1)) = 0.33
        _ShinyColor("Shiny Color", Color) = (1, 1, 1, 0.5)
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
            Name "Owlet 2D Shiny"
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
                float4 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 uv: TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _ShinySpeed;
            float _ShinyWidth;
            float _ShinyAngle;
            float4 _ShinyColor;
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
                output.uv.xyzw = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);

                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 col = SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, input.uv.xy);

				float x = input.uv.x + input.uv.y * _ShinyAngle;
				float v = sin(x - _Time.w * _ShinySpeed);
				v = smoothstep(1 - _ShinyWidth / 1000, 1.0, v);
				float3 target = v * _ShinyColor.xyz * _ShinyColor.a + col.rgb;
				col.rgb = lerp(col.rgb, target * col.a , _ShinyColor.a);
				return half4(target, col.a);
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}