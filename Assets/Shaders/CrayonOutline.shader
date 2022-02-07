Shader "lilCRIT/CrayonOutline"
{
	Properties{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(-0.05, 0.05)) = .005
		_OutlineTex("OutTexture", 2D) = "white" { }
		[Enum(UnityEngine.Rendering.CullMode)] _CullOutline("Cull Outline", Float) = 1.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		UNITY_FOG_COORDS(0)
		float2 uv : TEXCOORD0;
	};

	sampler2D _OutlineTex;
	float4 _OutlineTex_ST;
	uniform float _Outline;
	uniform float4 _OutlineColor;

	v2f vert(appdata v) {

		v2f o;

		float3 norm = normalize(v.normal);
		float3 offset = norm * (_Outline + (_SinTime.w + 1) * 0.002);
		float3 position = v.vertex + offset;

		o.pos = UnityObjectToClipPos(position);

		o.uv = TRANSFORM_TEX(v.uv, _OutlineTex);

		UNITY_TRANSFER_FOG(o, o.pos);

		return o;
	}
	ENDCG

	SubShader{
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull [_CullOutline]
			ZWrite On
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = _OutlineColor * tex2D(_OutlineTex, i.uv);
			
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
		ENDCG
		}
	}

	Fallback "lilCRIT/UnlitSimple"
}
