Shader "Hidden/Terrain/Terrain Colormap Ultra U4 Far Terrain" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CustomColorMap ("Color Map (RGB)", 2D) = "white" {}
		_TerrainNormalMap ("Terrain Normalmap", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma multi_compile USE_COLORMAP DONOT_USE_COLORMAP

		sampler2D _MainTex, _CustomColorMap, _TerrainNormalMap;

		struct Input {
			float2 uv_MainTex;
		};
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			// Supply the shader with tangents for the terrain
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c;
			#ifdef USE_COLORMAP
				c = tex2D (_CustomColorMap, IN.uv_MainTex);
			#endif
			#ifdef DONOT_USE_COLORMAP
				c = tex2D (_MainTex, IN.uv_MainTex);
			#endif
			o.Normal = UnpackNormal(tex2D(_TerrainNormalMap, IN.uv_MainTex)); 
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
