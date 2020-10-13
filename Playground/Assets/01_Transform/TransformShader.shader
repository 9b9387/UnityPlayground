Shader "Custom/TransformShader"
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

        // 位移矩阵
        float4x4 Translate(float4 translate)
        {
            return float4x4(1.0, 0.0, 0.0, translate.x,
                            0.0, 1.0, 0.0, translate.y,
                            0.0, 0.0, 1.0, translate.z,
                            0.0, 0.0, 0.0, 1.0);
        }

        // 缩放矩阵
        float4x4 Scale(float4 scale)
        {
            return float4x4(scale.x, 0.0, 0.0, 0.0,
                            0.0, scale.y, 0.0, 0.0,
                            0.0, 0.0, scale.z, 0.0,
                            0.0, 0.0, 0.0, 1.0);
        }

        // 旋转矩阵
        float4x4 Rotation(float4 rotation)
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

            return float4x4(cosY * cosZ, -cosY * sinZ, sinY, 0.0,
                            cosX * sinZ + sinX * sinY * cosZ, cosX * cosZ - sinX * sinY * sinZ, -sinX * cosY, 0.0,
                            sinX * sinZ - cosX * sinY * cosZ, sinX * cosZ + cosX * sinY * sinZ, cosX * cosY, 0.0,
                            0.0, 0.0, 0.0, 1.0);
        }

        // 顶点函数
        void vert(inout appdata_full v)
        {
            v.color = _Color;

            // 先缩放、后旋转、再平移
            // 应用缩放
            v.vertex = mul(Scale(_Scale), v.vertex);

            // 应用旋转
            v.vertex = mul(Rotation(_Rotation), v.vertex);

            // 应用位移
            v.vertex = mul(Translate(_Translate), v.vertex);
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
