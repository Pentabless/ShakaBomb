Shader "Custom/WipeShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Center("Center", Vector) = (0, 0, 0, 0)
		_Radius("Radius", FLoat) = 0
		_Thick("Thick", Float) = 0
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
			float4 _Center;
			float _Radius;
			float _Thick;

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

				float2 uv = i.uv - 0.5;
				uv.x *= aspectRatio;

				float2 pos = _Center.xy - 0.5f;
				pos.x *= aspectRatio;

				float dist = distance(uv, pos);
				if (dist > _Radius - _Thick) {
					col.rgb *= 1 - smoothstep(_Radius - _Thick, _Radius, dist);
				}

				return col;
			}

		ENDCG

		}
	}
}