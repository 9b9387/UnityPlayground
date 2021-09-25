Shader "Owlet/2D Unlit/Ghost" 
{
    Properties
    {
        [PerRendererData]_MainTex("Texture2D", 2D) = "white" {}
        _GhostAmount("Ghost Amount", Range(0, 1)) = 0.22
        _GhostColor("Ghost Color", Color) = (1, 0, 0, 0)
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
            Name "Owlet 2D Ghost"
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
            float _GhostAmount;
            float4 _GhostColor;
            CBUFFER_END
            
            // Object and Global properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

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

                half2 uv = input.uv.xy;
                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 color = SAMPLE_TEXTURE2D(unity_texture.tex, unity_texture.samplerstate, uv);
                half3 final = ((color.r + color.g + color.b) / 6 + 0.5) * _GhostColor.rgb;

                half amount = -2 + _GhostAmount * 4; //remap 0,1 -> -1,1

                half alpha = saturate(uv.y * 2 + amount) * color.a * _GhostColor.a;
				return half4(final.rgb, alpha);
            } 
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
}