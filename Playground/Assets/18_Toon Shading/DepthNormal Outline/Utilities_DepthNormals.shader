Shader "Owlet/Utilities/DepthNormals"
{
    SubShader
    {
        Tags {
            "RenderType" = "Opaque" 
            "IgnoreProjector" = "True" 
            "RenderPipeline" = "UniversalPipeline" 
            "ShaderModel"="2.0"
        }

        Pass
        {
            Name "DepthNormals"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float3 normal           : NORMAL;
                float4 texcoord         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex       : SV_POSITION;
                float4 nz           : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.vertex = TransformWorldToHClip(positionWS);

                output.nz.xyz = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, input.normal));
                output.nz.w = -(TransformWorldToView(positionWS).z * _ProjectionParams.w);

                return output;
            }
            // copy from UnityCG.cginc
            inline float2 EncodeViewNormalStereo(float3 n)
            {
                float kScale = 1.7777;
                float2 enc;
                enc = n.xy / (n.z+1);
                enc /= kScale;
                enc = enc*0.5+0.5;
                return enc;
            }

            // copy from UnityCG.cginc
            inline float2 EncodeFloatRG(float v)
            {
                float2 kEncodeMul = float2(1.0, 255.0);
                float kEncodeBit = 1.0/255.0;
                float2 enc = kEncodeMul * v;
                enc = frac (enc);
                enc.x -= enc.y * kEncodeBit;
                return enc;
            }

            // copy from UnityCG.cginc
            inline float4 EncodeDepthNormal(float depth, float3 normal)
            {
                float4 enc;
                enc.xy = EncodeViewNormalStereo (normal);
                enc.zw = EncodeFloatRG (depth);
                return enc;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return EncodeDepthNormal(input.nz.w, input.nz.xyz);
            }
            ENDHLSL
        }
    }

    FallBack Off
    CustomEditor ""
}
