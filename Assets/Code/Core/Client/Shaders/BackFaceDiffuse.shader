Shader "Custom/Backface Diffuse" {

    Properties {
        _Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
    }

    SubShader {
		
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}

		Lighting On
        Cull Off
		AlphaTest Greater 0.1

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		float _Cutoff;

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * (_Color * 2.0);
	
			if (c.a > _Cutoff)
			  o.Alpha = c.a;
			else
			  o.Alpha = 0;
		}

		ENDCG
    }

	Fallback "Transparent/Cutout/Diffuse"
}