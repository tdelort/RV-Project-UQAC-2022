Shader "Unlit/GlassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_IOR ("IOR", Float) = 1.5
    }
    SubShader
    {
		Tags {"Queue" = "Transparent"}
        LOD 100

        GrabPass{
		}
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 grabPos : TEXCOORD0;
				float4 pos : SV_POSITION;
				float3 viewDir : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float4 scrPos : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _IOR;

			// builtin variable to get Grabbed Texture if GrabPass has no name
			sampler2D _GrabTexture;

			v2f vert(appdata v) {
				//Basic stuff
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// builtin function to get screen coordinates for tex2Dproj()
				o.grabPos = ComputeGrabScreenPos(o.pos);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float2 screenPosition = i.scrPos.xy / i.scrPos.w; //ScreenPosition Default Mode
				float3 recfracted = refract(i.viewDir, i.worldNormal, _IOR);
				fixed4 grab = tex2Dproj(_GrabTexture, i.grabPos);
				return grab;
			}
			ENDCG
        }
    }
}
