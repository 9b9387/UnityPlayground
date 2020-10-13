Shader "Custom/TransformShaderTRS"
{
    Properties
    {
        // 基础色
        _Color ("BaseColor", Color) = (1.0, 1.0, 1.0, 1.0)
        // 位移量
        _Translate("Translate", Vector) = (0.0, 0.0, 0.0, 1.0)
        // 缩放量
        _Scale("Scale", Vector) = (1.0, 1.0, 1.0, 1.0)
        // 旋转量
        _Rotation("Rotation", Vector) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        // 引用appdata_full结构体
        #include "UnityCG.cginc"

        float4 _Color;
        float4 _Translate;
        float4 _Scale;
        float4 _Rotation;

        struct Input
        {
            fixed4  color : COLOR;
        };

        // 变换矩阵
        float4x4 TransfromTRS(float4 translate, float4 rotation, float4 scale)
        {
            float radX = radians(rotation.x);
            float radY = radians(rotation.y);
            float radZ = radians(rotation.z);

            float sinX = sin(radX);
            float cosX = cos(radX);
            float sinY = sin(radY);
            float cosY = cos(radY);
            float sinZ = sin(radZ);
            float cosZ = cos(radZ);

            return float4x4(cosY * cosZ * scale.x, -cosY * sinZ * scale.y, sinY * scale.z, translate.x,
                            (cosX * sinZ + sinX * sinY * cosZ) * scale.x, (cosX * cosZ - sinX * sinY * sinZ) * scale.y, -sinX * cosY * scale.z, translate.y,
                            (sinX * sinZ - cosX * sinY * cosZ) * scale.x, (sinX * cosZ + cosX * sinY * sinZ) * scale.y, cosX * cosY * scale.z, translate.z,
                            0.0, 0.0, 0.0, 1.0);
        }

        // 顶点函数
        void vert(inout appdata_full v)
        {
            v.color = _Color;
            // 应用顶点变换
            v.vertex = mul(TransfromTRS(_Translate, _Rotation, _Scale), v.vertex);
        }

        // 表面着色器函数
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.color.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
