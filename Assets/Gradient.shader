Shader "Idrak/Gradient" {
	Properties {
		_TopColor ("Top Color", Color) = (1,1,1,1)
		_BottomColor("Botton Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _TopColor;
		fixed4 _BottomColor;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_TopColor, _BottomColor, screenUV.y);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "VertexLit"
}
