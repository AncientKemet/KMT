Shader "Custom/TricolorShader" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		Red ("Red", Color) = (1.0,1.0,1.0,1.0)
		Green ("Green", Color) = (1.0,1.0,1.0,1.0)
		Blue ("Blue", Color) = (1.0,1.0,1.0,1.0)
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
	}

	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

		Lighting Off
        Cull Off
		
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff
		#pragma target 3.0

		sampler2D _MainTex;
		float4 Red;
		float4 Green;
		float4 Blue;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.r * Red + c.g * Green + c.b * Blue;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Transparent/Cutout/Diffuse"
}
