Shader "Custom/ColorMapUltra_MeshTerrainBlend Shader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss(A)", 2D) = "white" {}
		_BumpTex ("BumpTexture (RGB)", 2D) = "bump" {}
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		
		_TerrainSplat0 ("Terrain Tex1 (RGBA)", 2D) = "white" {}
		_TerrainTex1Size ("Terrain Tex1 Size", Float) = 5
		_TerrainTex1Color ("Terrain Tex1 Color", Color) = (1,1,1,1)
		_TerrainSpec1 ("Terrain Tex1 Shininess", Float) = 0.078125

		_TerrainSplat1 ("Terrain Tex2 (RGBA)", 2D) = "white" {}
		_TerrainTex2Size ("Terrain Tex2 Size", Float) = 5
		_TerrainTex2Color ("Terrain Tex2 Color", Color) = (1,1,1,1)
		_TerrainSpec2 ("Terrain Tex2 Shininess", Float) = 0.078125

		_TerrainSplat2 ("Terrain Tex3 (RGBA)", 2D) = "white" {}
		_TerrainTex3Size ("Terrain Tex3 Size", Float) = 5
		_TerrainTex3Color ("Terrain Tex3 Color", Color) = (1,1,1,1)
		_TerrainSpec3 ("Terrain Tex3 Shininess", Float) = 0.078125

		_TerrainSplat3 ("Terrain Tex4 (RGBA)", 2D) = "white" {}
		_TerrainTex4Size ("Terrain Tex4 Size", Float) = 5
		_TerrainTex4Color ("Terrain Tex4 Color", Color) = (1,1,1,1)
		_TerrainSpec4 ("Terrain Tex4 Shininess", Float) = 0.078125

		_TerrainCombinedNormal12 ("Terrain Combined Normal 12 (RGB)", 2D) = "bump" {}	
		_TerrainCombinedNormal34 ("Terrain Combined Normal 34 (RGB)", 2D) = "bump" {}
		
		_TerrainSize ("Terrain Size", Float) = 2000
		_TerrainPos ("Terrain Position (only X and Z are needed)", Vector) = (0.0,0.0,0.0,0.0)
		_ColorMap ("Custom Terrain Color Map 1 (RGB)", 2D) = "white" {}
		_TerrainNormalMap ("Terrain Normal Map (RGB)", 2D) = "bump" {}
		_Control ("Terrain SplatAlpha 0", 2D) = "red" {}
		
		_TMultiUV ("Terrain Multi UV Mixing Factor", Float) = 6
		_TDesMultiUvFac ("Terrain Multi UV Saturation", Float) = .5
		_TSplattingDistance ("Terrain Splatting Distance", Float) = 600
		_TDetailsFadeLenght ("terrainDetailsFadeLenght", Float) = 100
		_TSpecularColor ("Terrain Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_TSpecPower ("Terrain Specular Power", Float) = 1
		
		_TDetailNormalStrength ("Detail Normal Strength", Float) = 1
		_TUpscaledNormalStrength ("Upscaled Normal Strength", Float) = 1
		
		_TerrainFresnel ("Fresnel: Intensity/Power/Bias/-)", Vector) = (1.0, 2.0, 1.15,0.0)
		_TerrainReflectionColor ("Terrain Reflection Color", Color) = (1,1,1,1)
		
		_TerrainElevation ("Elevation of Terrain Textures 1-4", Vector) = (1.0,1.0,1.0,1.0)
		
		_TParallax ("Height Tex3, Heigt Tex4, GlossFactor Tex3, GlossFactor Tex4", Vector) = (0.005,0.005,0.0,0.0)
	
	}
	SubShader {
		Tags { 
			"RenderType"="Opaque"
			"Queue" = "Geometry-101"
		}
		LOD 200
		// Reduce shadow flickering
		Offset -1,-1
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0
		//#pragma glsl
		#include "UnityCG.cginc"
		
		#pragma multi_compile USE_BLENDMULTIUV USE_ADDDMULTIUV
		#pragma multi_compile USE_FRESNEL NO_FRESNEL
		#pragma multi_compile USE_STANDARDNORMALBLENDING USE_ADVANCEDNORMALBLENDING
		#pragma multi_compile USE_PARALLAX DONOT_USE_PARALLAX
		#pragma multi_compile USE_COLORMAP DONOT_USE_COLORMAP

//		#pragma debug
				
//		#define	USE_ADDDMULTIUV
//		#define	USE_PARALLAX
//		#define	USE_ADVANCEDNORMALBLENDING
//		#define USE_FRESNEL

		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _BumpTex;
		
		sampler2D _Control;
		sampler2D _TerrainSplat0,_TerrainSplat1,_TerrainSplat2,_TerrainSplat3;
		sampler2D _TerrainCombinedNormal12, _TerrainCombinedNormal34;
		
		#ifdef USE_COLORMAP
			sampler2D _ColorMap;
		#endif
		sampler2D _TerrainNormalMap;
		
		half _Shininess, _TMultiUV, _TDesMultiUvFac, _TSpecPower;
		
		float _TerrainSize;
		float3 _TerrainPos;
		float _TerrainTex1Size;
		float _TerrainTex2Size;
		float _TerrainTex3Size;
		float _TerrainTex4Size;
		
		fixed4 _TerrainTex1Color, _TerrainTex2Color, _TerrainTex3Color, _TerrainTex4Color;
		half _TerrainSpec1, _TerrainSpec2, _TerrainSpec3, _TerrainSpec4;
		fixed4 _TSpecularColor, _Color;
				
		half _TSplattingDistance;
		half _TDetailsFadeLenght;
		float4 _TerrainFresnel;
		fixed4 _TerrainReflectionColor;
		
		half4 _TerrainElevation;
		fixed _TDetailNormalStrength;
		fixed _TUpscaledNormalStrength;
		
		#ifdef USE_PARALLAX
			half4 _TParallax;
		#endif
		
		struct Input {
			float4 myuv_MainTex; // xy = uv maintex / zw = uv splat 4
			float4 myuv_TerrainTex; // xy = uv splat 3 / zw = uv terrain
			float4 myuv_TerrainTex1; // uv splat 0/1
			float4 myuv_TerrainTex2; // viewdir / w = distance 
			float4 color : COLOR; // vertexcolor
		};
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float3 worldPos = mul(_Object2World, v.vertex).xyz;
			
			// store uv coords for main texture
			o.myuv_MainTex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			
			// normalize mesh position
			float3 normalizedPosition = (worldPos - _TerrainPos);
			// store uv coords for terrain detail textures
			o.myuv_TerrainTex1.xy = (1.0/_TerrainTex1Size) * normalizedPosition.xz;
			o.myuv_TerrainTex1.zw = (1.0/_TerrainTex2Size) * normalizedPosition.xz;
			o.myuv_TerrainTex.xy = (1.0/_TerrainTex3Size) * normalizedPosition.xz;
			o.myuv_MainTex.zw = (1.0/_TerrainTex4Size) * normalizedPosition.xz;
			// store uv coords for terrain color map, terrain normal map and terrain splat map
			o.myuv_TerrainTex.zw = (1.0/_TerrainSize) * normalizedPosition.xz;
			
			// get the vertexcolors
			// g = terrain normal strength
			// r = terrain texture strength
			
			// terrain normal is stored in v.color.ba
			float3 terrainNormal;
			// decode color to normal vector
			terrainNormal.xz = (v.color.ba)*2-1;
			terrainNormal.y = sqrt(1 - dot(terrainNormal.xz, terrainNormal.xz));

			// blend normal based on vertex.color.g
			v.normal = normalize(lerp(v.normal, terrainNormal, v.color.g));
			
			// "estimate" terrain tangent
			float4 terrianTangent;
			terrianTangent.xyz = cross(terrainNormal.xyz, mul(_World2Object, float4(0, 0, 1, 0)).xyz);
			terrianTangent.w = -1;
			// lerp tangent
			v.tangent = lerp(v.tangent, terrianTangent, v.color.g);

			// store ObjectViewDir
			float3 Direction = ObjSpaceViewDir(v.vertex);
			// tangent space rotation
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
			// reduce artifacts on close viewing distances
			o.myuv_TerrainTex2.xyz = mul(rotation, Direction) / length(Direction);
			// store distance
			o.myuv_TerrainTex2.w = distance(_WorldSpaceCameraPos, worldPos);
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			// sample object texture
			fixed4 co = tex2D (_MainTex, IN.myuv_MainTex.xy) * _Color;
			fixed3 c = co.rgb;
			// sample object normal texture
			fixed3 objNormal = UnpackNormal(tex2D(_BumpTex, IN.myuv_MainTex.xy)); 
			
			//sample color map
			#ifdef USE_COLORMAP
				fixed3 colorMap = tex2D (_ColorMap, IN.myuv_TerrainTex.zw);
			#endif
			// sample terrain normal
			fixed3 farnormal = UnpackNormal(tex2D(_TerrainNormalMap, IN.myuv_TerrainTex.zw));
			
			// declare vars
			fixed3 nearnormal;
			fixed4 col;
			fixed3 finalCol;
			half fadeout = 1.0 - saturate( ( _TSplattingDistance - IN.myuv_TerrainTex2.w ) / _TDetailsFadeLenght );

			// we have to do all texture fetches outside the if to make it run under win/dx
			// sample terrain splatMap
			fixed4 splat_control = tex2D (_Control, IN.myuv_TerrainTex.zw);
			
			half h;
			// parallax
			#ifdef USE_PARALLAX
				float2 offset;
				float2 offset2;
				h = tex2D (_TerrainSplat2, IN.myuv_TerrainTex.xy).a;
				offset = ParallaxOffset (h, _TParallax.x, IN.myuv_TerrainTex2.xyz);
				h = tex2D (_TerrainSplat3, IN.myuv_MainTex.zw).a;
				offset2 = ParallaxOffset (h, _TParallax.y, IN.myuv_TerrainTex2.xyz);
			#endif
				
			// sample terrain detail diffuse maps
			fixed4 splatcol1 = tex2D (_TerrainSplat0, IN.myuv_TerrainTex1.xy);
			fixed4 splatcol2 = tex2D (_TerrainSplat1, IN.myuv_TerrainTex1.zw);
			#ifdef USE_PARALLAX
				fixed4 splatcol3 = tex2D (_TerrainSplat2, IN.myuv_TerrainTex.xy + offset);
				fixed4 splatcol3_2nd = tex2D (_TerrainSplat2, IN.myuv_TerrainTex.xy * _TMultiUV + offset);
				fixed4 splatcol4 = tex2D (_TerrainSplat3, IN.myuv_MainTex.zw + offset2);
			#else
				fixed4 splatcol3 = tex2D (_TerrainSplat2, IN.myuv_TerrainTex.xy);
				fixed4 splatcol3_2nd = tex2D (_TerrainSplat2, IN.myuv_TerrainTex.xy * _TMultiUV);
				fixed4 splatcol4 = tex2D (_TerrainSplat3, IN.myuv_MainTex.zw);
			#endif
			
			// sample terrain combined detail normal maps
			fixed2 normalsplat1 = tex2D(_TerrainCombinedNormal12, IN.myuv_TerrainTex1.xy).rg;
			fixed2 normalsplat2 = tex2D(_TerrainCombinedNormal12, IN.myuv_TerrainTex1.zw).ba;
			// initialize normalsplat3 as float3 as we use there vars to store an uncrompressed normal later on
			#ifdef USE_PARALLAX
				fixed3 normalsplat3 = tex2D(_TerrainCombinedNormal34, IN.myuv_TerrainTex.xy + offset).rgg;	
				fixed3 normalsplat3_2nd = tex2D(_TerrainCombinedNormal34, IN.myuv_TerrainTex.xy * _TMultiUV + offset).rgg;	
				fixed2 normalsplat4 = tex2D(_TerrainCombinedNormal34, IN.myuv_MainTex.zw + offset2).ba;
			#else
				fixed3 normalsplat3 = tex2D(_TerrainCombinedNormal34, IN.myuv_TerrainTex.xy).rgg;	
				fixed3 normalsplat3_2nd = tex2D(_TerrainCombinedNormal34, IN.myuv_TerrainTex.xy * _TMultiUV).rgg;	
				fixed2 normalsplat4 = tex2D(_TerrainCombinedNormal34, IN.myuv_MainTex.zw).ba;
			#endif
			
			if (IN.myuv_TerrainTex2.w < _TSplattingDistance) {
			
				if (IN.color.r > 0) {
					// combine terrain detail textures (1-4)
					col = splat_control.r * splatcol1;
					col += splat_control.g * splatcol2;
					// third detail texture uses uv mixing
					#ifdef USE_BLENDMULTIUV
						// diffuse and normal map according to distance
						//half multifadeFactor = clamp((0.35 / pow(IN.myuv_TerrainTex2.w * 0.0175,2)), 0, 1);
						fixed multifadeFactor = clamp( 1142.857143 / (IN.myuv_TerrainTex2.w * IN.myuv_TerrainTex2.w), 0, 1);
						splatcol3 = splatcol3*multifadeFactor + splatcol3_2nd*(1-multifadeFactor);
					#endif
					#ifdef USE_ADDDMULTIUV
						splatcol3 = clamp(splatcol3 * splatcol3_2nd * 2, 0 , 1);
						// desaturate
						fixed3 grayXfer = half3(0.3, 0.59, 0.11);
						fixed gray = dot(grayXfer, splatcol3.rgb);
						splatcol3.rgb = lerp(splatcol3.rgb, float3(gray,gray,gray), 1 - _TDesMultiUvFac);
					#endif
		
					#ifdef USE_PARALLAX
						// get gloss value from red color channel
						splatcol3.a = splatcol3.r * ( 1 + _TParallax.z);
						splatcol4.a = splatcol4.r * ( 1 + _TParallax.w);
					#endif
		
					col += splat_control.b * splatcol3;
					col += splat_control.a * splatcol4;

		//			splat normals
					nearnormal.xy = splat_control.r * normalsplat1;
					nearnormal.xy += splat_control.g * normalsplat2;
					#ifdef USE_STANDARDNORMALBLENDING
						normalsplat3 = normalize (normalsplat3 + normalsplat3_2nd );
					#endif
					#ifdef USE_ADVANCEDNORMALBLENDING
						normalsplat3 = UnpackNormalDXT5nm(normalsplat3.ggrr);
						normalsplat3_2nd = UnpackNormalDXT5nm(normalsplat3_2nd.ggrr);
						normalsplat3_2nd.xy *= _TUpscaledNormalStrength;
						normalsplat3_2nd = normalize (normalsplat3_2nd);
						fixed3x3 nBasis = fixed3x3(
							fixed3 (normalsplat3_2nd.z, normalsplat3_2nd.y,-normalsplat3_2nd.x),
							fixed3 (normalsplat3_2nd.x, normalsplat3_2nd.z,-normalsplat3_2nd.y),
							fixed3 (normalsplat3_2nd.x, normalsplat3_2nd.y, normalsplat3_2nd.z)
						);
						normalsplat3.xy *= _TDetailNormalStrength;
						normalsplat3 = normalize (normalsplat3);
						normalsplat3 = normalize ( normalsplat3.x*nBasis[0] + normalsplat3.y*nBasis[1] + normalsplat3.z*nBasis[2] );
						// pack normalsplat3 again
						normalsplat3 = normalsplat3 * 0.5 + 0.5;
					#endif
					nearnormal.xy += splat_control.b * normalsplat3.rg;
					nearnormal.xy += splat_control.a * normalsplat4;

				// final composition of terrain textures
					nearnormal = UnpackNormalDXT5nm(nearnormal.ggrr);
					nearnormal.xy *= 1 + (
						splat_control.r * _TerrainElevation.x +
						splat_control.g * _TerrainElevation.y +
						splat_control.b * _TerrainElevation.z +
						splat_control.a * _TerrainElevation.w
					);
					nearnormal = normalize(nearnormal);
					// fade out detail normal maps
					nearnormal = lerp(normalize(nearnormal + farnormal), farnormal, fadeout);
				
					#ifdef USE_COLORMAP
						half3 color_correction;
					// add color correction 
						#ifdef USE_BLENDMULTIUV
							color_correction = splat_control.r*_TerrainTex1Color + splat_control.g*_TerrainTex2Color + splat_control.b*_TerrainTex3Color + splat_control.a*_TerrainTex4Color;
						#endif
						#ifdef USE_ADDDMULTIUV
							color_correction = splat_control.r*_TerrainTex1Color + splat_control.g*_TerrainTex2Color + splat_control.b*_TerrainTex3Color*.75 + splat_control.a*_TerrainTex4Color;
						#endif
						finalCol.rgb = col * (colorMap/color_correction );
					#else
						finalCol.rgb = col;
					#endif	
					
				// o.Specular = 0.03: needed by deferred lighting 
					o.Specular = 0.03 + _TerrainSpec1 * splat_control.r;
					o.Specular += _TerrainSpec2 * splat_control.g;
					o.Specular += _TerrainSpec3 * splat_control.b;
					o.Specular += _TerrainSpec4 * splat_control.a;
				// add spec power
				//
					o.Gloss = col.a * _TSpecPower;
				// fade out detail maps
					#ifdef USE_COLORMAP 
						finalCol = lerp(finalCol, colorMap, fadeout);
					#endif
				// fade out detail normal maps
				// although this should be correct it looks better without it!?
					//nearnormal = lerp(normalize(nearnormal + farnormal), farnormal, fadeout);	
				// fade out spec and gloss values
					o.Gloss = lerp(o.Gloss, 0, fadeout);
					o.Specular = lerp(o.Specular, 0.03, fadeout);
				}
				
			}
			else {
				#ifdef USE_COLORMAP
					finalCol = colorMap;
				#else
					finalCol = c;
				#endif
				nearnormal = farnormal;
				o.Gloss = 0;
				o.Specular = 0.03;
			}
			// set spec color
			_SpecColor = _TSpecularColor;
			
			// combine based on vertex.color.redIN.color.r > 0
			o.Normal = lerp(objNormal, nearnormal, IN.color.r );
    		o.Gloss = lerp (co.a * _TSpecPower, o.Gloss, IN.color.r);
			o.Specular = lerp (_Shininess, o.Specular, IN.color.r);
			c.rgb = lerp (c.rgb, finalCol, IN.color.r);
			
			#ifdef USE_FRESNEL
				fixed facing =  clamp(1.0 - max(dot(IN.myuv_TerrainTex2.xyz, o.Normal * _TerrainFresnel.x ), 0.0), 0.0,1.0);
				fixed refl2Refr = max( _TerrainFresnel.z + (1.0-_TerrainFresnel.z) * pow(facing, _TerrainFresnel.y), 0.0) * (1 - c.b);
				half3 reflcol = lerp( c.rgb, _TerrainReflectionColor.rgb, refl2Refr * _TerrainReflectionColor.a);
				c.rgb = lerp( reflcol, c.rgb, fadeout); 
			#endif
			
			
			o.Albedo = c.rgb;
			o.Alpha = 0.0;
		}
		ENDCG
	} 
	FallBack "Specular"
}
