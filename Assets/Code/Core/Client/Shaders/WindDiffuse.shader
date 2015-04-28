Shader "Custom/WindDiffuse" {
	Properties {
        _Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WindTex ("Wind", 2D) = "white" {}
        _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
		_Strenght ("Strenght", Range (0,5)) = .5
    }


    SubShader {
		
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

		Lighting On
        Cull Off

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alphatest:_Cutoff addshadow
		#pragma target 3.0
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 color;
		};

		sampler2D _MainTex;
		sampler2D _WindTex;
		fixed4 _Color;
		half _Strenght;

		void vert (inout appdata_full v) {
			float3 worldPos = mul (_Object2World, float4(v.vertex.xyz, 1)).xyz;
			//float2 windCoord = float2(worldPos.x, worldPos.z) / 250 + (_Time.xy) / 75;
			float2 windCoord = float2(worldPos.x - _WorldSpaceCameraPos.x +32, worldPos.z - _WorldSpaceCameraPos.z + 32) / 64;

			float wind = tex2Dlod (_WindTex, float4(windCoord.x, windCoord.y,0,0)) *_Strenght;

			float x = wind * max(v.color.r, 0.0);
			float y = wind * max(v.color.g, 0.0);
			float z = wind * max(v.color.b, 0.0);

			v.vertex += float4(x, y, z, 0);

			//float4 worldBonus = mul (_Object2World, float4( side, up, 0, 0));
			//v.vertex += worldBonus;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			 fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			 o.Albedo = c.rgb;
			 o.Alpha = c.a;
		 }

		ENDCG
    }


	Fallback "Transparent/Cutout/Diffuse"
}
