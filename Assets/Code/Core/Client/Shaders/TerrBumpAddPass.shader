Shader "Hidden/Nature/Terrain/Bumped Specular AddPass" {
// Bumped terrain AddPass Shader replacement by Tim Leonard (aka Syanyde / Method)
Properties {
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	// set by terrain engine
	_Control ("Control (RGBA)", 2D) = "red" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
	_Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	_Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	_Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	_Normal0 ("Normal 0 (R)", 2D) = "bump" {}
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-99"
		"IgnoreProjector"="True"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert decal:add
#pragma target 3.0

void vert (inout appdata_full v)
{
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;
}

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
half _Shininess;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	half3 col;
	half4 splat0 = (tex2D (_Splat0, IN.uv_Splat0));
	half4 splat1 = (tex2D (_Splat1, IN.uv_Splat1));
	half4 splat2 = (tex2D (_Splat2, IN.uv_Splat2));
	half4 splat3 = (tex2D (_Splat3, IN.uv_Splat3));
	col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0);
	col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1);
	col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2);
	col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3);

	col  += splat_control.r * splat0.rgb;
	o.Normal = splat_control.r * UnpackNormal(tex2D(_Normal0, IN.uv_Splat0));
	o.Gloss = splat0.a * splat_control.r;
	o.Specular = 0.5 * splat_control.r;

	col += splat_control.g * splat1.rgb;
	o.Normal += splat_control.g * UnpackNormal(tex2D(_Normal1, IN.uv_Splat1));
	o.Gloss += splat1.a * splat_control.g;
	o.Specular += 0.5 * splat_control.g;
	
	col += splat_control.b * splat2.rgb;
	o.Normal += splat_control.b * UnpackNormal(tex2D(_Normal2, IN.uv_Splat2));
	o.Gloss += splat2.a * splat_control.b;
	o.Specular += 0.5 * splat_control.b;
	
	col += splat_control.a * splat3.rgb;
	o.Normal += splat_control.a * UnpackNormal(tex2D(_Normal3, IN.uv_Splat3));
	o.Gloss += splat3.a * splat_control.a;
	o.Specular += 0.5 * splat_control.a;

	o.Albedo = col;
	o.Normal = normalize(o.Normal);
	o.Alpha = 1;
	
}
ENDCG  
}

Fallback "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass"
}
