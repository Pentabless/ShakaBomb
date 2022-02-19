Shader "Custom/ReflectionWater" {
	Properties{
		_Tint("Tint", Color) = (1, 1, 1, 1)                  // カラー
		_BlendColor("BlendColor", Color) = (0, 0, 0, 0)      // 上からブレンドカラー
		_SaturationRate("SaturationRate", Float) = 1         // 彩度の倍率
		_BrightnessRate("BrightnessRate", Float) = 1         // 明度の倍率
		_Displacement("Displacement", 2D) = "defaulttexture" // 歪みテクスチャ
		_Strength("Strength", Float) = 0.015                 // 歪みの強さ
		_LightGrid("LightGrid", Vector) = (35, 70, 0, 0)     // 光用グリッド
		_Lightness("Lightness", Float) = 0.01                // 光の多さ
		_PerlinGrid("PerlinGrid", Vector) = (10, 10, 0, 0)   // 光の有効領域を計算する用のグリッド
		_PerlinThreshold("PerlinThreshold", Float) = 0.7     // 光の有効領域かどうかの閾値
		
	}

	SubShader{
		Tags { "Queue" = "Transparent" }

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Common.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				o.screenuv = ComputeScreenPos(o.vertex);
				return o;
			}

			uniform sampler2D _SSWaterReflectionsTex;
			uniform float _TopEdgePosition;

			fixed4 _Tint;
			fixed4 _BlendColor;
			float _SaturationRate;
			float _BrightnessRate;
			sampler2D _Displacement;
			float4 _Displacement_ST;
			float _Strength;
			float4 _LightGrid;
			float _Lightness;
			float4 _PerlinGrid;
			float _PerlinThreshold;
			float PespectiveCorrection;

			fixed2 random2(fixed2 st) {
				st = fixed2(dot(st, fixed2(127.1, 311.7)),
				dot(st, fixed2(269.5, 183.3)));
				return -1.0 + 2.0*frac(sin(st)*43758.5453123);
			}

			float perlinNoise(fixed2 st)
			{
				fixed2 p = floor(st);
				fixed2 f = frac(st);
				fixed2 u = f * f*(3.0 - 2.0*f);

				float v00 = random2(p + fixed2(0, 0));
				float v10 = random2(p + fixed2(1, 0));
				float v01 = random2(p + fixed2(0, 1));
				float v11 = random2(p + fixed2(1, 1));

				return lerp(lerp(dot(v00, f - fixed2(0, 0)), dot(v10, f - fixed2(1, 0)), u.x),
					  lerp(dot(v01, f - fixed2(0, 1)), dot(v11, f - fixed2(1, 1)), u.x),
					  u.y) + 0.5f;
			}

			float4 frag(v2f i) : SV_TARGET {
				// 下側を近く、上側を遠く見せる
				float2 perspectiveCorrection = float2(1.0 * (i.uv.x - 0.5) * i.uv.y, 0.0);
				float2 uv = TRANSFORM_TEX(frac(i.uv + perspectiveCorrection), _Displacement);
				// 歪みをテクスチャから取得する
				float4 displacement = normalize(tex2D(_Displacement, uv));
				// 歪み後のUV座標を取得する
				float2 adjusted = i.screenuv.xy + ((displacement.rg - 0.5) * _Strength);

				// 反射した背景を取得する
				float distanceToEdge = abs(_TopEdgePosition - adjusted.y);
				float sampleY = _TopEdgePosition + distanceToEdge;

				float4 output = tex2D(_SSWaterReflectionsTex, float2(adjusted.x, sampleY)) * _Tint;

				// 彩度と明度を調整する
				output.rgb = shift_col(output.rgb, float3(0, _SaturationRate, _BrightnessRate));
				// ブレンドカラーをブレンドする
				output.rgb = output.rgb*(1.0 - _BlendColor.a) + _BlendColor.rgb*_BlendColor.a;

				float2 area = floor(frac(uv)*_LightGrid);
				float2 seed = float2(80, 80);

				// 水面を光らせる
				// グリッドに分割して光るマスをランダムに決める
				// パーリンノイズの値から、光るマスを光らせるかどうか決める
				output.rgb = lerp(output.rgb, 1, step(1,
					step(1-_Lightness, random2(area+seed).x)*
					step(_PerlinThreshold, perlinNoise(i.uv*_PerlinGrid.xy*_Displacement_ST.xy))));

				return output;
			}
			ENDCG
		}
	}
}