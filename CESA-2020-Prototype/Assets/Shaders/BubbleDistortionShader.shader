﻿Shader "Unlit/BubbleDistortionShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Seed("Seed", Float) = 0
		_Timer("Timer", Float) = 0
		_Distortion("Distortion", Float) = 0.1
		_DistortionSpeed("DistortionSpeed", Float) = 1.0
		_ScaleRate("ScaleRate", Vector) = (1.3, 0.95, 0, 0)
		_ScaleVec("ScaleVec", Vector) = (0, 0, 0, 0)
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Seed;
				float _Timer;
				float _Distortion;
				float _DistortionSpeed;
				float4 _ScaleRate;
				float4 _ScaleVec;
				

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				float smootherstep(float edge0, float edge1, float x) {
					x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
					return x * x * x * (x * (x * 6 - 15) + 10);
				}

				float hash(float n) {
					return frac(sin(n)*43758.5453);
				}

				float noise(in float3 x) {
					float3 p = floor(x);
					float3 f = frac(x);

					f = f * f*(3.0 - 2.0*f);
					float n = p.x + p.y*57.0 + 113.0*p.z;
					return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
						lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
						lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
							lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
				}

				float2 noise2(float2 p) {
					float3 x = float3(p.x, p.y, 0);
					return float2(noise(x + float3(123.456, .567, .37)),
						noise(x + float3(.11, 47.43, 19.17)));
				}

				float2 getPos(float2 p)
				{
					float rand = hash(_Seed) * 10000;
					float2 n = float2(sin(_Timer*0.7*_DistortionSpeed), cos(_Timer*0.3*_DistortionSpeed));
					float2 q = _Distortion * (noise2(p + n + rand) - 0.5);

					return p + q;
				}


				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = (i.uv - 0.5)*_ScaleRate.x;
					float2 p = getPos(uv);
	
					float2 posX = p * (1.0+(_ScaleRate.xy-1.0)*_ScaleVec.x);
					float2 posY = p * (1.0+(_ScaleRate.yx-1.0)*_ScaleVec.y);
	
					float2 pos = lerp(posX, posY, step(_ScaleVec.x, _ScaleVec.y));

					float4 color = tex2D(_MainTex, pos+0.5);

					int v = pos.x*2;
					float range01 = 1 - (float)v / (v - 0.00001);
					v = pos.y*2;
					range01 *= 1 - (float)v / (v - 0.00001);

					//color.a *= step(dot(pos, pos), 0.25);
					color.a *= range01;

					return color;
				}
				ENDCG
			}
		}
}
