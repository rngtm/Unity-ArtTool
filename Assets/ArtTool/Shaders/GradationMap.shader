Shader "ArtTool/Unlit/GradationMap"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_GradationMap("Gradation Map", 2D) = "white" {}
		[HideInInspector] _UseAlphaClip("Use Alpha Clip", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

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
			sampler2D _GradationMap;
			float4 _MainTex_ST;
			float _UseAlphaClip;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);

				// 元画像の色に対応するピクセルをグラデーションテクスチャから取り出す
				fixed4 gradation = tex2D(_GradationMap, fixed2((color.r + color.g + color.b) / 3.0, 0));
				return gradation * max(_UseAlphaClip, color.a);
			}
			ENDCG
		}
	}
}
