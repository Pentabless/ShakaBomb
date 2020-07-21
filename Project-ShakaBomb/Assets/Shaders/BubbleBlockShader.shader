Shader "Unlit/BubbleBlockShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_CutStart("CutStart", Range(0.0,1.0)) = 0.0
		_CutEnd("CutEnd", Range(0.0,1.0)) = 0.2
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			//Blend One OneMinusSrcAlpha
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2g
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct g2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 vRange : TEXCOORD1;
				};

				fixed4 _Color;

				v2g vert(appdata_t IN)
				{
					v2g OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				[maxvertexcount(3)]
				void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
				{
					g2f o;

					float2 vRange = float2(1.0, 0.0);
					int i;
					for (i = 0; i < 3; i++)
					{
						vRange.x = min(vRange.x, IN[i].texcoord.y);
						vRange.y = max(vRange.y, IN[i].texcoord.y);
					}

					for (i = 0; i < 3; i++)
					{
						o.vertex = IN[i].vertex;
						o.color = IN[i].color;
						o.texcoord = IN[i].texcoord;
						o.vRange = vRange;
						triStream.Append(o);
					}
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _CutStart;
				float _CutEnd;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);

	#if ETC1_EXTERNAL_ALPHA
					// get the color from an external texture (usecase: Alpha support for ETC1 on android)
					color.a = tex2D(_AlphaTex, uv).r;
	#endif //ETC1_EXTERNAL_ALPHA

					return color;
				}

			 fixed4 frag(g2f IN) : SV_Target
				{
					float height = _CutEnd - _CutStart;
					float2 uv = IN.texcoord;
					float v = (uv.y - IN.vRange.x) / (IN.vRange.y - IN.vRange.x);
					v += step(_CutStart, v)*height;
					uv.y = lerp(IN.vRange.x, IN.vRange.y, v);

					fixed4 c = SampleSpriteTexture(uv) * IN.color;
					c.a = step(v, 1.0);
					
					return c;
				}
			ENDCG
			}
		}

}
