Shader "unityCookie/Matcap multiple textures" {
	Properties {
		_Color ("Color Tint", Color) = (0.5,0.5,0.5,1.0)
		_Blend ("Blend", Range (0, 1)) = 0.5
		_Intensity ("Intensity", Range(0.0,2.0)) = 0.8
		_LightingIntensity ("Lighting Intensity", Range(0.0,2.0)) = 0.8
	    _MainTex ("Main Texture", 2D) = "white" {}
	    _BlendTex ("Blend", 2D) = "black" {}
	    _MatcapTex ("Matcap Texture", 2D) = "white" {}
        _BumpMap ("Bump Map", 2D) = "bump" {}
	}
	SubShader {
		Pass {
			SetTexture [_MainTex]{
				combine texture
			}
			
			SetTexture[_BlendTex]{
				constantColor (0,0,0, [_BlendTex])
				combine texture lerp (constant) previous
			}

		}
		Pass {

			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined variables
			uniform sampler2D _MainTex;
            uniform fixed4 _MainTex_ST;
            uniform sampler2D _BlendTex;
            uniform fixed4 _BlendTex_ST;
			uniform sampler2D _MatcapTex;
            uniform sampler2D _BumpMap;
            uniform fixed4 _BumpMap_ST;
			uniform fixed4 _Color;
			uniform fixed _Intensity;
			uniform fixed _LightingIntensity;
			uniform fixed _Blend;
			//Unity defined Variables
         	uniform fixed4 _LightColor0; 
			
			//Base Input Structs
			struct vertexInput{
				half4 vertex : POSITION;
            	half3 normal : NORMAL;
				fixed4 texcoord : TEXCOORD0;
				half4 tangent : TANGENT;
			};
			struct vertexOutput {
	            half4 pos : SV_POSITION;
				fixed4 tex : TEXCOORD0;
	            half4 posWorld : TEXCOORD1;
	            fixed3 tangentWorld : TEXCOORD2;  
	            fixed3 normalWorld : TEXCOORD3;
	            fixed3 binormalWorld : TEXCOORD4;
	            fixed3 viewDir : TEXCOORD5;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v){
				
				vertexOutput o;
								
				o.tangentWorld = normalize( mul( _Object2World, half4( v.tangent.xyz, 0.0) ).xyz );
	            o.normalWorld = normalize( mul( half4(v.normal.xyz, 0.0), _World2Object).xyz );
	            o.binormalWorld = normalize( cross(o.normalWorld, o.tangentWorld).xyz * v.tangent.w);
	            o.posWorld = mul(_Object2World, v.vertex);
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - float3(o.posWorld.xyz));
	            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	            o.tex = v.texcoord;
	            
	            return o;
			}
			
			//fragment function
			fixed4 frag(vertexOutput i) : COLOR
			{
	 			half3 lightDir = half3(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z);
	 			//Texture Maps
                fixed4 texN =  tex2D (_BumpMap, _BumpMap_ST.xy * i.tex.xy + _BumpMap_ST.zw);
	 			fixed4 tex = tex2D(_MainTex, _MainTex_ST.xy * i.tex.xy + _MainTex_ST.zw);
	 			fixed4 texB = tex2D(_BlendTex, _BlendTex_ST.xy * i.tex.xy + _BlendTex_ST.zw);
                
                //unpackNormal function
	            half3 localCoords = fixed3(2.0 * texN.ag - fixed2(1.0, 1.0), 1.0);
	 
	            float3x3 local2WorldTranspose = float3x3(
	               i.tangentWorld, 
	               i.binormalWorld, 
	               i.normalWorld);
	            fixed3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));
 
	 			//Matcap
	 			fixed nDotL = 1 - max(0.0, dot(normalDirection, _WorldSpaceLightPos0.xyz));
	 			fixed nDotV = max(0.0, dot(normalDirection, i.viewDir.xyz));
	 			
				fixed diff = (nDotL * 0.3) + 0.5;
				fixed2 uvMatcap = float2(nDotV * _Intensity, diff);
				half3 texMatcap = tex2D(_MatcapTex, uvMatcap.xy).rgb;
				
				
	 			fixed3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb;
				
				fixed3 lightFinal = (texMatcap*_LightingIntensity)*_Color.rgb+ambientLight;
				tex.rgb = tex.rgb * (_Blend);
				fixed3 final = lightFinal * (texB.rgb + tex.rgb);
	 			
	 			return fixed4(final.rgb, 1.0);
			}
			ENDCG
		}
		
	}
	Fallback "Diffuse"
}