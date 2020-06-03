Shader "Unlit/TimeGaugeShader"
{
	Properties
	{
		_TimeRate("TimeRate", Range(0, 1)) = 1
		_AlertTime("AlertTime", Float) = 0
		_MainTex("Texture", 2D) = "white" {}
		_GaugeMask("GaugeMask", 2D) = "white" {}
		_MainColor("MainColor", Color) = (1,1,1,1)
		_LightColor("LightColor", Color) = (1,1,1,1)
		_SubColorCoef("SubColorCoef", Vector) = (1.25,1.25,1.25,1)
		_AlertColor("AlertColor", Color) = (0.9,0.1,0.1,0.5)
		_AlertBlinkSpeed("AlertBlinkSpeed", Float) = 2.0
		_UpperLine("UpperLine", Float) = 1.0
		_LowerLine("LowerLine", Float) = -0.05
		_SubHeight("SubHeight", Float) = 0.05
		_SubOffset("SubOffset", Float) = 0.4
		_MainSpeed("MainSpeed", Float) = 0.3
		_SubSpeed("SubSpeed", Float) = 0.3
		_SpeedUpRate("SpeedUpRate", Float) = 2
		_MainFrequency("MainFrequency", Float) = 2.4
		_SubFrequency("SubFrequency", Float) = 2.0
		_MainAmplitude("MainAmplitude", Float) = 0.02
		_SubAmplitude("SubAmplitude", Float) = 0.03

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

				float _TimeRate;
				float _AlertTime;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _GaugeMask;
				fixed4 _MainColor;
				fixed4 _LightColor;
				fixed4 _AlertColor;
				float4 _SubColorCoef;
				float _AlertBlinkSpeed;
				float _UpperLine;
				float _LowerLine;
				float _SubHeight;
				float _SubOffset;
				float _MainSpeed;
				float _SubSpeed;
				float _SpeedUpRate;
				float _MainFrequency;
				float _SubFrequency;
				float _MainAmplitude;
				float _SubAmplitude;

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

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texColor = tex2D(_MainTex, i.uv);
						
					float PI = 3.14159265359;

					fixed4 color = lerp(_LightColor, _MainColor, smoothstep(0.0, 1.0, 1.0 - i.uv.y));

					bool outside = tex2D(_GaugeMask, i.uv).a < 0.1;

					if(outside)
					{
						color.a = -1.0;
					}
					else {

						float line1 = lerp(_LowerLine, _UpperLine, _TimeRate);
						float line2 = line1 + _SubHeight;

						float speedRate = 1.0 + (_SpeedUpRate - 1.0)*(1.0 - _TimeRate);

						float w1 = sin((i.uv.x + _Time.y * _MainSpeed * speedRate)*PI*_MainFrequency)*_MainAmplitude;
						float w2 = sin((i.uv.x + _Time.y * _SubSpeed * speedRate + _SubOffset)*PI*_SubFrequency)*_SubAmplitude;

						color = lerp(color, saturate(color*_SubColorCoef), step(i.uv.y, line1 + w1));
						color *= step(i.uv.y, line2 + w2);
					}

					color.rgb = lerp(color.rgb*(1.0 - texColor.a) + texColor.rgb*texColor.a, texColor.rgb, step(color.a, 0));
					color.a = lerp(color.a, texColor.a, step(color.a, 0));

					if (_AlertTime > 0 && texColor.a > 0.01) {
						float a = abs(sin(_AlertTime*_AlertBlinkSpeed))*_AlertColor.a;
						
						color.rgb = color.rgb*(1 - a) + _AlertColor.rgb * a;
					}

					return color*i.color;
				}
				ENDCG
			}
		}
}
