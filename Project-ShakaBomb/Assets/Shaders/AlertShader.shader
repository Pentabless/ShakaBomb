Shader "Custom/AlertShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}             // テクスチャ
		_MainColor("MainColor", Color) = (1, 1, 1, 1)    // カラー1
		_SubColor("SubColor", Color) = (1, 1, 1, 1)      // カラー2
		_Thick("Thick", Range(0, 1)) = 0                 // 枠線の太さ
		_Transparency("Transparency", Range(0, 1)) = 0.5 // 不透明度
		_Border("Border", Float) = 10                    // 斜線の量
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

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

				sampler2D _MainTex;
				float4 _MainColor;
				float4 _SubColor;
				float _Thick;
				float _Transparency;
				float _Border;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);

					float aspectRatio = _ScreenParams.x / _ScreenParams.y;

					float2 uv = i.uv;
					
					// 画面端からの距離が枠線の太さ以内か判定する
					if (min(uv.x, 1 - uv.x) < _Thick / aspectRatio ||
						min(uv.y, 1 - uv.y) < _Thick) {
						// 斜線の色を選択する
						float4 blendColor = lerp(_MainColor, _SubColor, step(frac((uv.x - uv.y)*_Border), 0.5));
						// 不透明度を調整する
						blendColor.a *= _Transparency;
						// テクスチャとブレンドする
						col.rgb = col.rgb*(1-blendColor.a)+blendColor.rgb*blendColor.a;
					}

					return col;
				}

			ENDCG

			}
		}
}