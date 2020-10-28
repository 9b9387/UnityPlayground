Shader "Custom/GPU Instancing"
{
Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        // 用Shader控制缩放
        _Scale ("Scale", Float) = 1.0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        // 基于物理的标准光照模型，并对所有光照类型启用阴影
        // 语法 vertex:vert 指定顶点函数
        #pragma surface surf Standard fullforwardshadows vertex:vert
        // 引用appdata_full结构体
        #include "UnityCG.cginc"

        // 使用 Shader Model 3.0 目标
        #pragma target 3.0
        sampler2D _MainTex;
        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;

        // 定义实例化缓存属性 注意不要在UNITY_INSTANCING_BUFFER_START/END之外重复定义
        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
           UNITY_DEFINE_INSTANCED_PROP(half, _Scale)
        UNITY_INSTANCING_BUFFER_END(Props)

        // 缩放矩阵
        float4x4 ScaleMat(half scale)
        {
            return float4x4(scale, 0.0, 0.0, 0.0,
                            0.0, scale, 0.0, 0.0,
                            0.0, 0.0, scale, 0.0,
                            0.0, 0.0, 0.0, 1.0);
        }

        void vert(inout appdata_full v)
        {
            // 访问缓冲区中声明的每个实例的缩放属性
            half s = UNITY_ACCESS_INSTANCED_PROP(Props, _Scale);
            // 顶点缩放矩阵变换
            v.vertex = mul(ScaleMat(s), v.vertex);
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
