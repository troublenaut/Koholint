Shader "Terrain/Terrain Colormap Ultra U4" {
Properties {

	_Parallax ("Height Tex3, Heigt Tex4, GlossFactor Tex3, GlossFactor Tex4", Vector) = (0.005,0.005,0.0,0.0)
	_DetailNormalStrength ("Detail Normal Strength ", Range (0.005, 1.0)) = 1.0
	_UpscaledNormalStrength ("Upscaled Normal Strength ", Range (0.005, 1.0)) = 1.0

	
	// global terrain vars
	_CustomColorMap ("Color Map (RGB)", 2D) = "white" {}
	_TerrainNormalMap ("Terrain Normalmap", 2D) = "bump" {}
	_Control ("SplatAlpha 0", 2D) = "red" {}
	_Control2nd ("SplatAlpha 1", 2D) = "black" {}

	_terrainCombinedFloats ("MultiUV, Desaturation, Splatting Distance, Specular Power", Vector) = (0.5,600.0,0.5,1.0)
	_terrainDetailsFadeLenght ("terrainDetailsFadeLenght", Float) = 100
	_SpecColor ("Terrain Specular Color", Color) = (0.5, 0.5, 0.5, 1)

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

	// color correction and spec values 
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

	_CombinedNormal12 (" Combined Normal 1 (RG) Normal 2 (BA)", 2D) = "white" {}
	_CombinedNormal34 (" Combined Normal 3 (RG) Normal 4 (BA)", 2D) = "white" {}
	_CombinedNormal56 (" Combined Normal 5 (RG) Normal 6 (BA)", 2D) = "white" {}
	
	_Fresnel ("Fresnel: Intensity/Power/Bias/-)", Vector) = (2.0, 1.5, -0.5,0.0)
	_ReflectionColor ("Terrain Reflection Color", Color) = (1,1,1,1)
	
	_Elev ("Elevation for Tex 1-4)", Vector) = (1.0, 1.0, 1.0, 1.0)
	_Elev1 ("Elevation for Tex 5-6)", Vector) = (1.0, 1.0, 1.0, 1.0)
	
	// used in fallback on old cards
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	
}

SubShader {
	Tags {
		"SplatCount" = "8"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"


#pragma multi_compile USE_BLENDMULTIUV USE_ADDDMULTIUV
#pragma multi_compile USE_FRESNEL NO_FRESNEL
#pragma multi_compile USE_STANDARDNORMALBLENDING USE_ADVANCEDNORMALBLENDING
#pragma multi_compile USE_PARALLAX DONOT_USE_PARALLAX
#pragma multi_compile USE_COLORMAP DONOT_USE_COLORMAP

//#define USE_BLENDMULTIUV
//#define USE_FRESNEL
//#define USE_ADVANCEDNORMALBLENDING
//#define USE_PARALLAX
//#define USE_COLORMAP

	// max number of sampler2d: 13

	sampler2D _Control, _Control2nd;
	sampler2D _Splat0, _Splat1, _Splat2, _Splat3, _Splat4, _Splat5;
	float _Splat0Tiling, _Splat1Tiling, _Splat2Tiling, _Splat3Tiling, _Splat4Tiling, _Splat5Tiling;

	#ifdef USE_COLORMAP
		sampler2D _CustomColorMap;
	#endif
	#ifdef DONOT_USE_COLORMAP
		sampler2D _MainTex; 
	#endif

	sampler2D _TerrainNormalMap;
	sampler2D _CombinedNormal12, _CombinedNormal34, _CombinedNormal56;
	
	float _Spec1, _Spec2, _Spec3, _Spec4, _Spec5, _Spec6;
	//float _Decal1_ColorCorrectionStrenght, _Decal2_ColorCorrectionStrenght, _Decal1_Sharpness, _Decal2_Sharpness;
	float4 _DecalCombinedFloats;

	float3 _ColTex1, _ColTex2, _ColTex3, _ColTex4, _ColTex5, _ColTex6;
	float4 _v4CameraPos, _SpecularColor;
	
	//float _MultiUV, _DesMultiUvFac, _SplattingDistance, _SpecPower;
	float4 _terrainCombinedFloats;
	float _terrainDetailsFadeLenght;
	
	// x: Intensity/ y: Power/ z: Bias
	float4 _Fresnel;
	float4 _ReflectionColor;
	
	float4 _Elev;
	float4 _Elev1;

	#ifdef USE_PARALLAX
		half4 _Parallax;
	#endif

	fixed _DetailNormalStrength;
	fixed _UpscaledNormalStrength;

struct Input {
	float4 viewVectors;
	float2 uv_Control : TEXCOORD0;
	// use all available interpolators to shift work back from frag to vertex programm
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2: TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
	float2 uv_Splat4 : TEXCOORD5;
};

void vert (inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	// Supply the shader with tangents for the terrain
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;

	// create tangent space rotation
	float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
	float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );
	
	// get view Direction
	float3 Direction = ObjSpaceViewDir(v.vertex);

	// and bring it into tangent space
	float3 TanViewDirection = mul( rotation, Direction );
	// reduce artifacts on close viewing distances
	TanViewDirection = TanViewDirection / length(Direction);
	o.viewVectors.xyz = TanViewDirection;

	// store distance
	float3 worldPos = mul(_Object2World, v.vertex).xyz;
	o.viewVectors.w = distance(_WorldSpaceCameraPos, worldPos);
	o.uv_Control = v.texcoord;
}


void surf (Input IN, inout SurfaceOutput o) {
	
	half4 col;
	#ifdef USE_COLORMAP
		half3 colorMap = tex2D(_CustomColorMap, IN.uv_Control);	
	#endif
	#ifdef DONOT_USE_COLORMAP
		half3 colorMap = tex2D(_MainTex, IN.uv_Control);	
	#endif

	float3 farnormal = UnpackNormal(tex2D(_TerrainNormalMap, IN.uv_Control));

	// we have to do all texture fetches outside the if to make it run under win/dx
	half4 splat_control = tex2D(_Control, IN.uv_Control);
	half4 splat_control2nd = tex2D(_Control2nd, IN.uv_Control);

	half h;
	#ifdef USE_PARALLAX
		float2 offset;
		float2 offset2;
		h = tex2D (_Splat2, IN.uv_Splat2).a;
		offset = ParallaxOffset (h, _Parallax.x, IN.viewVectors.xyz);
		h = tex2D (_Splat3, IN.uv_Splat3).a;
		offset2 = ParallaxOffset (h, _Parallax.y, IN.viewVectors.xyz);
	#endif


	// sample detail diffuse maps
	half4 splatcol1 = tex2D (_Splat0, IN.uv_Splat0);
	half4 splatcol2 = tex2D (_Splat1, IN.uv_Splat1);

	#ifdef USE_PARALLAX
		half4 splatcol3 = tex2D (_Splat2, IN.uv_Splat2 + offset);
		half4 splatcol3_2nd = tex2D (_Splat2, IN.uv_Splat2 * _terrainCombinedFloats.x + offset);
		half4 splatcol4 = tex2D (_Splat3, IN.uv_Splat3 + offset2);
	#else
		half4 splatcol3 = tex2D (_Splat2, IN.uv_Splat2);
		half4 splatcol3_2nd = tex2D (_Splat2, IN.uv_Splat2 * _terrainCombinedFloats.x);
		half4 splatcol4 = tex2D (_Splat3, IN.uv_Splat3);
	#endif
	half4 splatcol5 = tex2D (_Splat4, IN.uv_Splat4);
	half4 splatcol6 = tex2D (_Splat5, IN.uv_Control * _Splat5Tiling);
	
	// sample combined detail normal maps	
	fixed2 normalsplat1 = tex2D(_CombinedNormal12, IN.uv_Splat0).rg;
	fixed2 normalsplat2 = tex2D(_CombinedNormal12, IN.uv_Splat1).ba;
	// initialize normalsplat3 as fixed3 as we use there vars to store an uncrompressed normal later on
	#ifdef USE_PARALLAX
		fixed3 normalsplat3 = tex2D(_CombinedNormal34, IN.uv_Splat2  + offset).rgg;	
		fixed3 normalsplat3_2nd = tex2D(_CombinedNormal34, IN.uv_Splat2 * _terrainCombinedFloats.x + offset).rgg;
		fixed2 normalsplat4 = tex2D(_CombinedNormal34, IN.uv_Splat3 + offset2).ba;
	#else
		fixed3 normalsplat3 = tex2D(_CombinedNormal34, IN.uv_Splat2).rgg;	
		fixed3 normalsplat3_2nd = tex2D(_CombinedNormal34, IN.uv_Splat2  * _terrainCombinedFloats.x).rgg;
		fixed2 normalsplat4 = tex2D(_CombinedNormal34, IN.uv_Splat3).ba;
	#endif
	fixed2 normalsplat5 = tex2D(_CombinedNormal56, IN.uv_Splat4).rg;
	fixed2 normalsplat6 = tex2D(_CombinedNormal56, IN.uv_Control * _Splat5Tiling).ba;


	// declare vars
	half2 blendval = 0;
	fixed2 normalsum;
	
	if (IN.viewVectors.w < _terrainCombinedFloats.z)
	{
/// start splatting

		// this changed to float (dx)
		float decalSum = splat_control2nd.b + splat_control2nd.a;
		// reconstruct detail texture coverage below decals
		splat_control = splat_control * (1 / (1 - decalSum));
		splat_control2nd.rg = splat_control2nd.rg * (1 / (1 - decalSum));

		col = splat_control.r * splatcol1;
		col += splat_control.g * splatcol2;

		half2 height;
		#ifdef USE_PARALLAX
			height = half2(splatcol3.a, splatcol4.a);
			// get gloss value from red color channel as alpha stores height
			splatcol3.a = splatcol3.r * ( 1 + _Parallax.z);
			splatcol4.a = splatcol4.r * ( 1 + _Parallax.w);
		#endif

		// third detail texture uses uv mixing
		#ifdef USE_BLENDMULTIUV
			half multifadeFactor = clamp( 1142.857143 / (IN.viewVectors.w * IN.viewVectors.w), 0, 1);
			splatcol3 = splatcol3*multifadeFactor + splatcol3_2nd*(1-multifadeFactor);
		#endif
		#ifdef USE_ADDDMULTIUV
			splatcol3 = clamp(splatcol3 * splatcol3_2nd * 2, 0 , 1);
			// desaturate
			half3 grayXfer = float3(0.3, 0.59, 0.11);
			half gray = dot(grayXfer, splatcol3.rgb);
			splatcol3.rgb = lerp(splatcol3.rgb, half3(gray,gray,gray), 1 - _terrainCombinedFloats.y);
		#endif

		col += splat_control.b * splatcol3;
		col += splat_control.a * splatcol4;
		col += splat_control2nd.r * splatcol5;
		col += splat_control2nd.g * splatcol6;

		//// splat normals
		normalsum = splat_control.r * normalsplat1;
		normalsum += splat_control.g * normalsplat2;

		// third detail texture uses uv mixing
		#ifdef USE_STANDARDNORMALBLENDING
			normalsplat3 = normalize (normalsplat3 + normalsplat3_2nd );
		#endif
		#ifdef USE_ADVANCEDNORMALBLENDING
			normalsplat3 = UnpackNormalDXT5nm(normalsplat3.ggrr);
			normalsplat3_2nd = UnpackNormalDXT5nm(normalsplat3_2nd.ggrr);
			normalsplat3_2nd.xy *= _UpscaledNormalStrength;
			normalsplat3_2nd = normalize (normalsplat3_2nd);
			fixed3x3 nBasis = fixed3x3(
				fixed3 (normalsplat3_2nd.z, normalsplat3_2nd.y,-normalsplat3_2nd.x),
				fixed3 (normalsplat3_2nd.x, normalsplat3_2nd.z,-normalsplat3_2nd.y),
				fixed3 (normalsplat3_2nd.x, normalsplat3_2nd.y, normalsplat3_2nd.z)
			);
			normalsplat3.xy *= _DetailNormalStrength;
			normalsplat3 = normalize (normalsplat3);
			normalsplat3 = normalize ( normalsplat3.x*nBasis[0] + normalsplat3.y*nBasis[1] + normalsplat3.z*nBasis[2] );
			// pack normalsplat3 again
			normalsplat3 = normalsplat3 * 0.5 + 0.5;		
		#endif
		normalsum += splat_control.b * normalsplat3.rg;
		normalsum += splat_control.a * normalsplat4;
		normalsum += splat_control2nd.r * normalsplat5;
		normalsum += splat_control2nd.g * normalsplat6;

/// final composition

/// apply colorcorrection to detail maps // see: http://blog.wolfire.com/2009/12/Detail-texture-color-matching
		#ifdef USE_COLORMAP
			half3 color_correction;
			// color correction for splats 1-6
			#ifdef USE_BLENDMULTIUV
				// we have to use: splat_control.b*_ColTex3*0.99 to get rid of flickering on dx9
				color_correction = splat_control.r*_ColTex1 + splat_control.g*_ColTex2 + splat_control.b*_ColTex3*0.99 + splat_control.a*_ColTex4 + splat_control2nd.r * _ColTex5 + splat_control2nd.g * _ColTex6;
			#endif
			#ifdef USE_ADDDMULTIUV
				color_correction = splat_control.r*_ColTex1 + splat_control.g*_ColTex2 + splat_control.b*_ColTex3*.75 + splat_control.a*_ColTex4 + splat_control2nd.r * _ColTex5 + splat_control2nd.g * _ColTex6;
			#endif
		#endif

		// o.Specular = 0.03: needed by deferred lighting 
		o.Specular = 0.03 + _Spec1 * splat_control.r;
		o.Specular += _Spec2 * splat_control.g;
		o.Specular += _Spec3 * splat_control.b;
		o.Specular += _Spec4 * splat_control.a;
		o.Specular += _Spec5 * splat_control2nd.r;
		o.Specular +=  _Spec6 * splat_control2nd.g;

/// add decals
		if (decalSum > 0) {
			// decals are taken from 3. and 4. base texture
			#ifdef DONOT_USE_PARALLAX
				height = half2(splatcol3.b, splatcol4.b);
			#endif
			blendval.r = max(0, height.r - (1-splat_control2nd.b));
			blendval.r = lerp(0,1,blendval.r*_DecalCombinedFloats.z); // hard edges (was 16)
			blendval.g = max(0, height.g - (1-splat_control2nd.a));
			blendval.g = lerp(0,1,blendval.g*_DecalCombinedFloats.w); // hard edges
			blendval = clamp(half2(1.0,1.0),half2(0.0,0.0),blendval);
			// decal 1 
			col = lerp(col, splatcol3, blendval.r);
			#ifdef USE_COLORMAP
				color_correction = lerp(color_correction, _ColTex3, blendval.r);
			#endif
			normalsum = lerp(normalsum, normalsplat3.rg, blendval.r);
			//o.Gloss is added by col
			o.Specular = lerp(o.Specular, _Spec3, blendval.r);
			// decal 2
			col = lerp(col, splatcol4, blendval.g);
			normalsum = lerp(normalsum, normalsplat4, blendval.g);
			#ifdef USE_COLORMAP
				color_correction = lerp(color_correction, _ColTex4, blendval.g);
			#endif
			//o.Gloss is added by col
			o.Specular = lerp(o.Specular, _Spec4, blendval.g);
		}
		o.Gloss = col.a;
		o.Normal = UnpackNormalDXT5nm(normalsum.ggrr);
		o.Normal.xy *= 1 + (
					splat_control.r * _Elev.x +
					splat_control.g * _Elev.y +
					splat_control.b * _Elev.z +
					splat_control.a * _Elev.w +
					splat_control2nd.r * _Elev1.x +
					splat_control2nd.g * _Elev1.y +
					blendval.r * _Elev.z +
					blendval.g * _Elev.w +
					splat_control.a * _Elev.w
		);
		o.Normal = normalize (o.Normal);
/// color correction
		#ifdef USE_COLORMAP
			col.rgb *= (colorMap/color_correction);
		#endif
/// add decals
		col.rgb = lerp(col.rgb, splatcol3.rgb, blendval.r * (1-_DecalCombinedFloats.x));
		col.rgb = lerp(col.rgb, splatcol4.rgb, blendval.g * (1-_DecalCombinedFloats.y));
/// add spec power
		o.Gloss *= _terrainCombinedFloats.w;
		
		float fadeout = 1.0 - saturate( ( _terrainCombinedFloats.z - IN.viewVectors.w ) / _terrainDetailsFadeLenght );


/// fade out detail maps
		#ifdef USE_COLORMAP
			col.rgb = lerp(col.rgb, colorMap, fadeout);
		#endif
/// fade out detail normal maps
		o.Normal = lerp(normalize(o.Normal + farnormal), farnormal, fadeout);
/// fade out spec and gloss values
		o.Gloss = lerp(o.Gloss, 0, fadeout);
		o.Specular = lerp(o.Specular, 0.03, fadeout);
/// add fresnel
		#ifdef USE_FRESNEL
			fixed facing = clamp(1.0 - max(dot(IN.viewVectors.xyz, o.Normal * _Fresnel.x ), 0.0), 0.0,1.0);
			fixed refl2Refr = max( _Fresnel.z + (1.0-_Fresnel.z) * pow(facing, _Fresnel.y ), 0.0) *(1-col.b);
			/// add reflection color
			half3 reflcol = lerp( col.rgb, _ReflectionColor.rgb, refl2Refr * _ReflectionColor.a);
			col.rgb = lerp( reflcol, col.rgb, fadeout); 
		#endif
	}
	else {
		col.rgb = colorMap;
		o.Normal = farnormal;
		o.Gloss = 0;
		o.Specular = 0.03;
	}

	

	o.Albedo = col.rgb;
	o.Alpha = 0.0;
}
ENDCG  
}
Dependency "BaseMapShader" = "Hidden/Terrain/Terrain Colormap Ultra U4 Far Terrain"
Fallback "Nature/Terrain/Bumped Specular"
}