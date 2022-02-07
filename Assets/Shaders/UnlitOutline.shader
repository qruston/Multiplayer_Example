Shader "lilCRIT/UnlitOutline"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		[HDR] _Emission("Emission", Color) = (0,0,0,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(-0.05, 0.05)) = .005
		_OutlineTex("OutTexture", 2D) = "white" { }
		[Enum(UnityEngine.Rendering.CullMode)] _CullOutline("Cull Outline", Float) = 1.0
	    [Enum(UnityEngine.Rendering.CullMode)] _CullMesh("Cull Mesh", Float) = 2.0
	}

	SubShader{
		Tags { "RenderType" = "Opaque" }
		UsePass "lilCRIT/UnlitSimple/UNLIT"
		UsePass "lilCRIT/CrayonOutline/OUTLINE"
	}

	Fallback "lilCRIT/UnlitSimple"
}