Shader "Unlit/BubbleDistortionShader2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}                // テクスチャ
		_Seed("Seed", Float) = 0                            // シード値
		_Timer("Timer", Float) = 0                          // アニメーション用タイマー
		_GrabDistortion("_GrabDistortion", Float) = 0.04    // 背景の歪み量
		_Distortion("Distortion", Float) = 0.1              // 歪み量
		_DistortionSpeed("DistortionSpeed", Float) = 1.0    // 歪むスピード
		_ScaleRate("ScaleRate", Vector) = (1.3, 0.95, 0, 0) // 押し付けた時の変形量
		_ScaleVec("ScaleVec", Vector) = (0, 0, 0, 0)        // 変形の度合い
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			GrabPass {
				"_BackgroundTexture"
			}

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
					fixed4 uvgrab : TEXCOORD1;
					float4 color : COLOR;
					float4 vertex : SV_POSITION;
				};

				sampler2D _BackgroundTexture;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Seed;
				float _Timer;
				float _GrabDistortion;
				float _Distortion;
				float _DistortionSpeed;
				float4 _ScaleRate;
				float4 _ScaleVec;


				v2f vert(appdata v)
				{
					v2f o;
					// 描画範囲を拡大する
					v.vertex.xy *= _ScaleRate.x;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uvgrab = ComputeGrabScreenPos(o.vertex);
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
					// 0.3~0.8の乱数を生成する
					float r = hash(_Seed)*0.5 + 0.3;
					// 歪ませる方向を決める
					float2 n = float2(sin(_Timer*r*_DistortionSpeed), cos(_Timer*(1 - r)*_DistortionSpeed));
					// 歪み量を計算する
					float2 q = _Distortion * (noise2(p + n) - 0.5);
					// 元の座標に歪み量を足して返す
					return p + q;
				}


				fixed4 frag(v2f i) : SV_Target
				{
					// 描画範囲を拡大した分だけ画像を縮小する
					float2 uv = (i.uv - 0.5)*_ScaleRate.x;
					// 歪み後のUV座標を取得する
					float2 p = getPos(uv);

					// 押し付けた時に変形させる
					float2 posX = p * (1.0 + (_ScaleRate.xy - 1.0)*_ScaleVec.x);
					float2 posY = p * (1.0 + (_ScaleRate.yx - 1.0)*_ScaleVec.y);

					float2 pos = lerp(posX, posY, step(_ScaleVec.x, _ScaleVec.y));

					fixed4 diff = tex2D(_MainTex, pos + 0.5);

					// UV座標が0~1の間にない場合は透明にする
					int v = pos.x * 2;
					float range01 = 1 - (float)v / (v - 0.00001);
					v = pos.y * 2;
					range01 *= 1 - (float)v / (v - 0.00001);

					//color.a *= step(dot(pos, pos), 0.25);
					diff.a *= range01;
					clip(diff.a - 0.01f);

					// 中心に近いほど背景の歪みを大きくする
					float2 dist = length(p * 2);
					dist = saturate(1 - dist);
					dist = pow(dist, 1.5);

					i.uvgrab.xy += dist.xy*abs(normalize(dist))*_GrabDistortion;

					fixed4 color = tex2Dproj(_BackgroundTexture, UNITY_PROJ_COORD(i.uvgrab));

					return color;
				}
				ENDCG
			}

			UsePass "Unlit/BubbleDistortionShader/DISTORTION"

		}
}
