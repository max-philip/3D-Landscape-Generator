//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/opaqueWave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Pass
	{
		Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

	struct vertIn
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct vertOut
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	// Implementation of the vertex shader
	vertOut vert(vertIn v)
	{
		
		float4 displacement = float4(0.0f, sin(v.vertex.x + _Time.y), 0.0f, 0.0f); // Q5c
		v.vertex += displacement;

		vertOut o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;

		// Challenge answer - replace entire vertex shader with the code below: 
		/*
		// Apply the model and view matrix to the vertex (but not the projection matrix yet)
		v.vertex = mul(UNITY_MATRIX_MV, v.vertex);

		// v.vertex is now in view space. This is the point where we want to apply the displacement.
		v.vertex += float4(0.0f, sin(v.vertex.x + _Time.y), 0.0f, 0.0f);

		// Finally apply the projection matrix to complete the transformation into screen space
		v.vertex = mul(UNITY_MATRIX_P, v.vertex);

		// Build output structure
		vertOut o;
		o.vertex = v.vertex;
		o.uv = v.uv;
		return o;
		*/
	}

	// Implementation of the fragment shader
	fixed4 frag(vertOut v) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, v.uv);
		return col;
	}
		ENDCG
	}
	}
}