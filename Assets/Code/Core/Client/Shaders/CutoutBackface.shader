Shader "Custom/Vegetation/Grass" {

    Properties {
        _Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WindTex ("Wind", 2D) = "white" {}
        _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
    }


    SubShader {
		
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

		Lighting On
        Cull Off

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alphatest:_Cutoff
		#pragma target 3.0
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		sampler2D _MainTex;
		sampler2D _WindTex;
		fixed4 _Color;

		void vert (inout appdata_full v) {
			//v.vertex.xyz += v.normal;
			float3 worldPos = mul (_Object2World, float4(v.vertex.x, 0, v.vertex.y, 1)).xyz;
			float wind = float4(0);
			float2 windCoord = float2(worldPos.x, worldPos.z) / 250.0 + _Time / 75.0;
			wind += tex2D (_WindTex, windCoord).r;
			float4 worldBonus = mul (_Object2World, float4( wind * max(v.vertex.z, 0.0), 0, 0, 0));
			v.vertex += worldBonus;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			 fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			 o.Albedo = c.rgb;
			 o.Alpha = c.a;
		 }

		ENDCG
    }

	 SubShader {
		 Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		 LOD 200
     
	 CGPROGRAM
	 #pragma surface surf Lambert alphatest:_Cutoff
 
	 sampler2D _MainTex;
	 fixed4 _Color;
 
	 struct Input {
		 float2 uv_MainTex;
	 };
 
	 void surf (Input IN, inout SurfaceOutput o) {
		 fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		 o.Albedo = c.rgb;
		 o.Alpha = c.a;
	 }
	 ENDCG
	 }

	Fallback "Transparent/Cutout/Diffuse"
}