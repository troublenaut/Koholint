Shader "Custom/ColorMapUltra_SimpleBumpedSpecReflective" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss(A)", 2D) = "white" {}
		_BumpTex ("BumpTexture (RGB)", 2D) = "bump" {}
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		
		_TSpecularColor ("Terrain Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_TSpecPower ("Terrain Specular Power", Float) = 1 
		_TerrainFresnel ("Fresnel: Intensity/Power/Bias/-)", Vector) = (1.0, 2.0, 1.15,0.0)
		_TerrainReflectionColor ("Terrain Reflection Color", Color) = (1,1,1,1)
		
	}
	SubShader {
		Tags { 
			"RenderType"="Opaque"
			"Queue" = "Geometry"
		}
		LOD 200

		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0
		#include "UnityCG.cginc"
		#pragma multi_compile USE_FRESNEL NO_FRESNEL

		sampler2D _MainTex;
		sampler2D _BumpTex;
		float _Shininess, _TSpecPower;
		float4 _TSpecularColor, _Color;
		float4 _TerrainFresnel;
		float4 _TerrainReflectionColor;
		
		
		struct Input {
			float2 uv_MainTex;
			//float3 _normal;
			float3 _viewDir; // viewdir / w = distance 
		};
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.uv_MainTex = v.texcoord;
			#ifdef USE_FRESNEL
			//o._normal = v.normal;
			// store ObjectViewDir
			float3 Direction = ObjSpaceViewDir(v.vertex);
			// tangent space rotation
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
			o._viewDir = normalize(mul( rotation, Direction ));
			#endif
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 co = tex2D (_MainTex, IN.uv_MainTex.xy) * _Color;
			half3 c = co.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_MainTex.xy));
			o.Specular = _Shininess;
			o.Gloss = co.a * _TSpecPower;
			// set spec color
			_SpecColor = _TSpecularColor;
			//	fresnel
			#ifdef USE_FRESNEL
			// get Normal
			//float3 myNormal;
			//myNormal = IN._normal;
			//myNormal = normalize( myNormal + o.Normal.xxy * float3(1,0,1) );
			//myNormal = normalize( myNormal );
			//myNormal *= _TerrainFresnel.x;
			float3 myViewDir = IN._viewDir;
			//half facing =  clamp(1.0 - max(dot(myViewDir, myNormal ), 0.0), 0.0,1.0);
			
			half facing =  clamp(1.0 - max(dot(IN._viewDir, o.Normal ), 0.0), 0.0,1.0);
			half refl2Refr = max( _TerrainFresnel.z + (1.0-_TerrainFresnel.z) * pow(facing, _TerrainFresnel.y), 0.0) * (1 - c.b);
			c = lerp( c, _TerrainReflectionColor.rgb, refl2Refr * _TerrainReflectionColor.a);
			#endif
			o.Albedo = c.rgb;
			o.Alpha = 0.0;

		}
		ENDCG
	} 
	FallBack "Diffuse"
}
