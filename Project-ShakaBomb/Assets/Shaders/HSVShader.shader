Shader "Unlit/HSVShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}                // テクスチャ
		[MaterialToggle]									    
		_UseFixHue("UseFixHue", Float) = 0                   // 色相を固定させるかどうか
		_FixHue("FixHue", Color) = (1, 1, 1, 1)              // 固定させる色相
		_HueShift("HueShift", Float) = 0                     // 色相の変化量
		_SaturationMultiply("SaturationMultiply", Float) = 1 // 彩度の倍率
		_SaturationShift("SaturationShift", Float) = 0       // 彩度の変化量
		_BrightnessMultiply("BrightnessMultiply", Float) = 1 // 明度の倍率
		_BrightnessShift("BrightnessShift", Float) = 0       // 明度の変化量
		
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Common.cginc"

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
			float _UseFixHue;
			float4 _FixHue;
			float _HueShift;
			float _SaturationMultiply;
			float _SaturationShift;
			float _BrightnessMultiply;
			float _BrightnessShift;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 diff = tex2D(_MainTex, i.uv);
				// HSVカラーに変換する
				float3 hsv = rgb2hsv(diff.rgb);
				// 色相を調整する
				if (_UseFixHue > 0) {
					hsv.x = frac(rgb2hsv(_FixHue.rgb).x + _HueShift);
				}
				else {
					hsv.x = frac(hsv.x + _HueShift);
				}
				// 彩度を調整する
				hsv.y = saturate(hsv.y * _SaturationMultiply + _SaturationShift);
				// 明度を調整する
				hsv.z = saturate(hsv.z * _BrightnessMultiply + _BrightnessShift);

				// RGBカラーに戻す
				diff.rgb = hsv2rgb(hsv);
				diff *= i.color;

                return diff;
            }
            ENDCG
        }
    }
}
