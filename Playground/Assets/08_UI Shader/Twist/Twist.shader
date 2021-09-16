Shader "Owlet/2D Unlit/Twist" 
{
    Properties
    {
        _TwistTex("Twist Tex", 2D) = "black"{}
		_TwistMaskTex("Twist Mask Tex", 2D) = "black"{}
		_TwistScale("Twist Scale", float) = 0
		_TwistScrollingX("Twist Scrolling X", Float) = 0
		_TwistScrollingY("Twist Scrolling Y", Float) = 0
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
            Name "Owlet 2D Twist"
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
            float4 _TwistTex_TexelSize;
            float4 _TwistMaskTex_TexelSize;
            float _TwistScrollingX;
            float _TwistScrollingY;
            float _TwistScale;
            CBUFFER_END
            
            // Object and Global properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_TwistTex);
            SAMPLER(sampler_TwistTex);
            TEXTURE2D(_TwistMaskTex);
            SAMPLER(sampler_TwistMaskTex);

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

                half2 twist_uv = input.uv.xy + half2(_TwistScrollingX, _TwistScrollingY) * _Time.y;
                UnityTexture2D twist_texture = UnityBuildTexture2DStructNoScale(_TwistTex);
                half4 twist_color = SAMPLE_TEXTURE2D(twist_texture.tex, twist_texture.samplerstate, twist_uv);
                half twist = twist_color.r * _TwistScale;

                UnityTexture2D mask_texture = UnityBuildTexture2DStructNoScale(_TwistMaskTex);
                half4 mask_color = SAMPLE_TEXTURE2D(mask_texture.tex, mask_texture.samplerstate, input.uv.xy);
                half mask = mask_color.r;

                half2 main_uv = input.uv + twist + mask;
                UnityTexture2D main_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 main_color = SAMPLE_TEXTURE2D(main_texture.tex, main_texture.samplerstate, main_uv);

				return main_color;
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}