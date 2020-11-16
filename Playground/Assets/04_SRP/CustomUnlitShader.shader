Shader "Custom/UnlitShader"
{
    Properties
    {
		_Color("Color Tint", Color) = (0.5,0.5,0.5)
        _MainTex ("Texture", 2D) = "white" {}
    }

	SubShader
	{		
		Tags{ "Queue" = "Geometry" }
		LOD 100
		Pass
		{
			//注意这里,默认是没写光照类型的,自定义管线要求必须写,渲染脚本中会调用,否则无法渲染
			//这也是为啥新建一个默认unlitshader,无法被渲染的原因
			Tags{ "LightMode" = "Always" }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
			ENDHLSL
		}
	}
}
