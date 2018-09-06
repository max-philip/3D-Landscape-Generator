﻿// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016

//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/PhongShader"
{
	
	// DOESNT WORK ??

	//uniform vector _SunPosition

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		//_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}
	SubShader
	{
			LOD 200 //Level of detail								WHAT THIS DOOOOOOOOOOOOOOOOOOOOOOOOOOO

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" } //For the first light
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _LightColor0; //From UnityCG

			uniform float3 _PointLightColor;
			sampler2D _MainTex;
			//uniform float3 _PointLightPosition;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords.
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Transform vertex in world coordinates to camera coordinates, and pass colour
				o.color = v.color;
				o.uv = v.uv;

				// Pass out the world vertex position and world normal to be interpolated
				// in the fragment shader (and utilised)
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.vertex = UnityObjectToClipPos(v.vertex);


				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : COLOR
			{
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);



				/*

				// TRIED THE COPY


				float3 viewDirection = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 vert2LightSource = _WorldSpaceLightPos0.xyz - v.worldVertex.xyz;
				float oneOverDistance = 1.0 / length(vert2LightSource);
				float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w); //Optimization for spot lights. This isn't needed if you're just getting started.
				float3 lightDirection = _WorldSpaceLightPos0.xyz - v.worldVertex.xyz * _WorldSpaceLightPos0.w;
				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * v.color.rgb; //Ambient component
				float3 diffuseReflection = attenuation * _LightColor0.rgb * v.color.rgb * max(0.0, dot(interpNormal, lightDirection)); //Diffuse component
				float3 specularReflection;
				if (dot(v.worldNormal, lightDirection) < 0.0) //Light on the wrong side - no specular
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					//Specular component
					specularReflection = attenuation * _LightColor0.rgb * v.color.rgb * pow(max(0.0, dot(reflect(-lightDirection, interpNormal), viewDirection)), 1);
				}

				float3 color = (ambientLighting + diffuseReflection) * tex2D(_MainTex, v.uv) + specularReflection; //Texture is not applient on specularReflection
				return float4(color, 1.0);



				//FUUUUUUUUUUUUUUUUCK


				*/




				// Calculate ambient RGB intensities
				float Ka = 1;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1;
				float Kd = 1;
				float3 L = normalize(_WorldSpaceLightPos0);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 0.13;
				float specN = 0.6; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				// Using classic reflection calculation:
				float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
				// Using Blinn-Phong approximation:
				//specN = 25; // We usually need a higher specular power when using Blinn-Phong
				//float3 H = normalize(V + L);
				//float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = 1;

				fixed4 col = tex2D(_MainTex, v.uv);
				return returnColor * col;
			}
			ENDCG
		}
	}
}
