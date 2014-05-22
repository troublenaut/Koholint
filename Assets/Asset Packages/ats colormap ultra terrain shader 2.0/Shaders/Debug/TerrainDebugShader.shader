Shader "Terrain/Terrain Colormap Ultra U4 Debug" {
	Properties {
	
		_CustomColorMap ("Color Map (RGB)", 2D) = "white" {}
		_TerrainNormalMap ("Terrain Normalmap", 2D) = "bump" {}
		_Control ("SplatAlpha 0", 2D) = "red" {}
		_Control2nd ("SplatAlpha 1", 2D) = "black" {}
		
		_Splat0 ("Layer 0 (R)", 2D) = "white" {}
		_Splat0Tiling ("Tiling Detail Texture 1", Float) = 100
		_Splat1 ("Layer 1 (G)", 2D) = "white" {}
		_Splat1Tiling ("Tiling Detail Texture 2", Float) = 100
		_Splat2 ("Layer 2 (B)", 2D) = "white" {}
		_Splat2Tiling ("Tiling Detail Texture 3", Float) = 100
		_Splat3 ("Layer 3 (A)", 2D) = "white" {}
		_Splat3Tiling ("Tiling Detail Texture 4", Float) = 100
		_Splat4 ("Layer 4 (R)", 2D) = "white" {}
		_Splat4Tiling ("Tiling Detail Texture 5", Float) = 100
		_Splat5 ("Layer 5 (G)", 2D) = "white" {}
		_Splat5Tiling ("Tiling Detail Texture 6", Float) = 100
		
		_ColTex1 ("Avrg. Color Tex 1", Color) = (.5,.5,.5,1)
		_Spec1 ("Shininess Tex 1", Range (0.03, 1)) = 0.078125
		_ColTex2 ("Avrg. Color Tex 2", Color) = (.5,.5,.5,1)
		_Spec2 ("Shininess Tex 2", Range (0.03, 1)) = 0.078125
		_ColTex3 ("Avrg. Color Tex 3", Color) = (.5,.5,.5,1)
		_Spec3 ("Shininess Tex 3", Range (0.03, 1)) = 0.078125
		_ColTex4 ("Avrg. Color Tex 4", Color) = (.5,.5,.5,1)
		_Spec4 ("Shininess Tex 4", Range (0.03, 1)) = 0.078125
		_ColTex5 ("Avrg. Color Tex 5", Color) = (.5,.5,.5,1)
		_Spec5 ("Shininess Tex 5", Range (0.03, 1)) = 0.078125
		_ColTex6 ("Avrg. Color Tex 6", Color) = (.5,.5,.5,1)
		_Spec6 ("Shininess Tex 6", Range (0.03, 1)) = 0.078125
		
		_DecalCombinedFloats ("Correction1, Sharpness1, Correction2, Sharpness2", Vector) = (0.5, 16.0, 0.5, 16.0)
	
	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	}
	SubShader {
		Tags { 
			"SplatCount" = "8"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		#pragma glsl
		#include "UnityCG.cginc"
		
		sampler2D _Control, _Control2nd;
		sampler2D _Splat0, _Splat1, _Splat2, _Splat3, _Splat4, _Splat5;
		float _Splat0Tiling, _Splat1Tiling, _Splat2Tiling, _Splat3Tiling, _Splat4Tiling, _Splat5Tiling;
		sampler2D _CustomColorMap;
		
		float3 _ColTex1, _ColTex2, _ColTex3, _ColTex4, _ColTex5, _ColTex6;
		float4 _DecalCombinedFloats;
		float4 _terrainCombinedFloats;

		sampler2D _MainTex;
		

		struct Input {
			float2 uv_MainTex;
			float2 uv_Control;
			float2 uv_Splat0;
			float2 uv_Splat1;
			float2 uv_Splat2;
			float2 uv_Splat3;
			float2 uv_Splat4;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 splat_control = tex2D(_Control, IN.uv_MainTex);
			half4 splat_control2nd = tex2D(_Control2nd, IN.uv_MainTex);
			
			half3 colorMap = tex2D(_CustomColorMap, IN.uv_Control);
			
			
			half4 splatcol1 = tex2D (_Splat0, IN.uv_Splat0);
			half4 splatcol2 = tex2D (_Splat1, IN.uv_Splat1);
			half4 splatcol3 = tex2D (_Splat2, IN.uv_Splat2);
			half4 splatcol3_2nd = tex2D (_Splat2, IN.uv_Splat2 * _terrainCombinedFloats.x);
			half4 splatcol4 = tex2D (_Splat3, IN.uv_Splat3);
			half4 splatcol5 = tex2D (_Splat4, IN.uv_Splat4);
			half4 splatcol6 = tex2D (_Splat5, IN.uv_Control * _Splat5Tiling);
			
			half4 col;
			
			float decalSum = splat_control2nd.b + splat_control2nd.a;
			// reconstruct detail texture coverage below decals
			splat_control = splat_control * (1 / (1 - decalSum));
			splat_control2nd.rg = splat_control2nd.rg * (1 / (1 - decalSum));
			
			col = splat_control.r * splatcol1;
			col += splat_control.g * splatcol2;
			col += splat_control.b * splatcol3;
			col += splat_control.a * splatcol4;
			col += splat_control2nd.r * splatcol5;
			col += splat_control2nd.g * splatcol6;
			
			half3 color_correction;
			half2 blendval = 0;
			
			color_correction = splat_control.r*_ColTex1 + splat_control.g*_ColTex2 + splat_control.b*_ColTex3*0.99 + splat_control.a*_ColTex4 + splat_control2nd.r * _ColTex5 + splat_control2nd.g * _ColTex6;

			
			if (decalSum > 0) {
			// decals are taken from 3. and 4. base texture
				half2 height = half2(splatcol3.b, splatcol4.b);
				blendval.r = max(0, height.r - (1-splat_control2nd.b));
				blendval.r = lerp(0,1,blendval.r*_DecalCombinedFloats.z); // hard edges (was 16)
				blendval.g = max(0, height.g - (1-splat_control2nd.a));
				blendval.g = lerp(0,1,blendval.g*_DecalCombinedFloats.w); // hard edges
				blendval = clamp(half2(1.0,1.0),half2(0.0,0.0),blendval);
				
				
				if (splat_control2nd.b + splat_control2nd.a > 0.99) {
					col.rgb = half3(0,0,1);
				}
				
				// decal 1
				if (splat_control2nd.b > 0.99) {
					splatcol3.rgb = half3(1,0,0);
				}
				col = lerp(col, splatcol3, blendval.r);
				color_correction = lerp(color_correction, _ColTex3, blendval.r);
				// decal 2
				if (splat_control2nd.a > 0.99) {
					splatcol4.rgb = half3(0,1,0);
				}
				col = lerp(col, splatcol4, blendval.g);
				
				color_correction = lerp(color_correction, _ColTex4, blendval.g);
				
			}
			
			col.rgb *= (colorMap/color_correction);
			
			/// add decals
			col.rgb = lerp(col.rgb, splatcol3.rgb, blendval.r * (1-_DecalCombinedFloats.x));
			col.rgb = lerp(col.rgb, splatcol4.rgb, blendval.g * (1-_DecalCombinedFloats.y));
			
			o.Albedo = col.rgb;
			o.Alpha = 0.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
