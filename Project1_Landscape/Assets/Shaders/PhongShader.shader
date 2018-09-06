// Phong shader for lighting the procedurally generated terrain. Takes in color
// values of vertices which are based on vertex height within the world.

// The solution provided for Lab 5 of COMP30019, Semester 2 2018 was used as
// a resource for the method of calculating Phong reflection. Parameters
// have been altered to suit the terrain, and light handling has been changed.

Shader "Unlit/PhongShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
	}
	SubShader
	{
		Tags{ "LightMode" = "ForwardBase" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float3 _PointLightColor;
			sampler2D _MainTex;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float4 color : COLOR;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords.
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				o.color = v.color;
				o.uv = v.uv;

				// Pass out the world vertex position and world normal.
				// Interpolated in the fragment shader.
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 0.8;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 0.9;
				float Kd = 1;
				float3 L = normalize(_WorldSpaceLightPos0);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 0.13;
				float specN = 0.6;
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				
				// Using classic reflection calculation:
				float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				
				// No transparency for terrain
				returnColor.a = 1;

				fixed4 col = tex2D(_MainTex, v.uv);
				return returnColor * col;
			}
			ENDCG
		}
	}
}
