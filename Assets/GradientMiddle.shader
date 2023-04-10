Shader "Idrak/GradientMiddle"
{
    Properties
    {
		_Color1 ("Color 1", Color) = (1, 1, 1, 1)
		_Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
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
                float4 vertex : SV_POSITION;
            };

			float4 _Color1;
			float4 _Color2;
            sampler2D _MainTex;
            float4 _MainTex_ST;

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
				fixed4 col = tex2D(_MainTex, i.uv);
				
				if (i.uv.y > 0.5f) {
					col *= lerp(_Color2, _Color1, (i.uv.y-0.5)*2);
				}
				else {
					col *= lerp(_Color1, _Color2, i.uv.y * 2);
				}
				
                return col;
            }
            ENDCG
        }
    }
}
