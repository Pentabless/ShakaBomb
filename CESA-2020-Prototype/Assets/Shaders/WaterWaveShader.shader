Shader "Unlit/WaterWaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainColor ("MainColor", Color) = (1,1,1,1)
		_LightColor ("LightColor", Color) = (1,1,1,1)
		_WaterHeight ("WaterHeight", Float) = 0.2
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _MainColor;
			fixed4 _LightColor;
			float _WaterHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float smootherstep(float edge0, float edge1, float x) {
				x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
				return x * x * x * (x * (x * 6 - 15) + 10);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				float t = smootherstep(1.0/(1.0+_WaterHeight), 1.0, i.uv.y);
				col *= lerp(_MainColor, _LightColor, t);

				//float t3 = smoothstep(1.0/(1.0+_WaterHeight), 1.0, i.uv.y);
				//float lim = 1.0 / (1.0 + _WaterHeight);
				//float t2 = (max(lim, i.uv.y)-lim)/(1.0 - lim);
				//t2 *= t2;
				//if (i.uv.x < 0.5) {
				//	col *= lerp(_MainColor, _LightColor, t);
				//}
				//else {
				//	col *= lerp(_MainColor, _LightColor, t3);
				//}

                return col;
            }
            ENDCG
        }
    }
}
