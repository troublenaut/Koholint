using System;
using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode] // Make live-update even when not in play mode (e.g. undo)

[Serializable]
public class CustomTerrainScriptColorMapUltraU4 : MonoBehaviour {
	
	public Material TerrainMaterial;

	public float SplattingDistance = 600.0f;
	public float TerrainDetailsFadeLenght = 100.0f;
	public Texture2D CustomColorMap;
	public Texture2D TerrainNormalMap;

	public bool UseColormap = true;
	
	public Texture2D SplatMap1;
	public Texture2D SplatMap2;
	public Color SpecularColor = Color.gray;

	public Texture2D customSplatMap1;
	public Texture2D customSplatMap2;

	// detail maps start with 1
	public Color ColTex1 = Color.gray;
	public Color ColTex2 = Color.gray;
	public Color ColTex3 = Color.gray;
	public Color ColTex4 = Color.gray;
	public Color ColTex5 = Color.gray;
	public Color ColTex6 = Color.gray;
	
	public float Elev1 = 1f;
	public float Elev2 = 1f;
	public float Elev3 = 1f;
	public float Elev4 = 1f;
	public float Elev5 = 1f;
	public float Elev6 = 1f;

	public float MultiUv = .5f;
	public bool BlendMultiUv = true;
	public float DetailNormalStrength = 1f;
	public float UpscaledNormalStrength = 1f;
	public float DesMultiUvFac = .5f;
	
	public float SpecPower = 1.0f;
	
	public float Spec1 = 0.078125f;
	public float Spec2 = 0.078125f;
	public float Spec3 = 0.078125f;
	public float Spec4 = 0.078125f;
	public float Spec5 = 0.078125f;
	public float Spec6 = 0.078125f;

	// Parallax
	public bool Terrain_PM = false;
	public float P_Elevation_Tex3 = 0.005f;
	public float P_GlossFactor_Tex3 = 0.0f;
	public float P_Elevation_Tex4 = 0.005f;
	public float P_GlossFactor_Tex4 = 0.0f;
	
	// new
	public bool TerrainFresnel = false;
	public float FresnelIntensity = 1.0f;
	public float FresnelPower = 1.0f;
	public float FresnelBias = -.5f;
	
	public Color ReflectionColor = Color.white;

	public bool AdvancedNormalBlending = false;
	
	public Texture2D SourceNormal1;
	public Texture2D SourceNormal2;
	public Texture2D SourceNormal3;
	public Texture2D SourceNormal4;
	public Texture2D SourceNormal5;
	public Texture2D SourceNormal6;
	
	public Texture2D CombinedNormal12;
	public Texture2D CombinedNormal34;
	public Texture2D CombinedNormal56;
	
	public float Decal1_ColorCorrectionStrenght = .5f;
	public float Decal2_ColorCorrectionStrenght = .5f;
	public float Decal1_Sharpness = 16f;
	public float Decal2_Sharpness = 16f;


	// used by the editor
	[HideInInspector]
	public bool showNewInspector = true;
	public List<Material> blendedMaterials = new List<Material>();
	
	[HideInInspector]
	public bool saveMaterial = false;

	// show-hide boxes in new inspector
	[HideInInspector]
	public bool detailsIntro = true;
	[HideInInspector]
	public bool detailsBase = true;
	[HideInInspector]
	public bool detailsMultiuv = false;
	[HideInInspector]
	public bool detailsImportSplat = true;
	[HideInInspector]
	public bool detailsComNormals = true;
	[HideInInspector]
	public bool detailsCreateComNormals = true;
	[HideInInspector]
	public bool detailsColCor = true;
	[HideInInspector]
	public bool detailsSpecVal = false;
	[HideInInspector]
	public bool detailsMeshMat = true;
	[HideInInspector]
	public bool autoUpdateMeshMat = false;
	
	
	void Awake () {
		Terrain targetTerrain = (Terrain)GetComponent(typeof(Terrain));
		targetTerrain.basemapDistance = SplattingDistance;
		updateColormapMaterial();
	}
	
	void Start () {
		Terrain targetTerrain = (Terrain)GetComponent(typeof(Terrain));
		targetTerrain.basemapDistance = SplattingDistance;
		updateColormapMaterial();
	}


	public void updateColormapMaterial() {	
		//Debug.Log("Update Colormap MAterial");

		Terrain targetTerrain = (Terrain)GetComponent(typeof(Terrain));
		targetTerrain.basemapDistance = SplattingDistance;
		targetTerrain.materialTemplate = TerrainMaterial;
		float terrainWidth = targetTerrain.terrainData.size.x;
		
		if (TerrainMaterial) {
			if(UseColormap){
				Shader.EnableKeyword("USE_COLORMAP");
				Shader.DisableKeyword("DONOT_USE_COLORMAP");
			}
			else {
				Shader.DisableKeyword("USE_COLORMAP");
				Shader.EnableKeyword("DONOT_USE_COLORMAP");
			}
			if (CustomColorMap) {
				TerrainMaterial.SetTexture( "_CustomColorMap", CustomColorMap );
			}
			else
				TerrainMaterial.SetTexture( "_CustomColorMap", null );
			if (TerrainNormalMap)
				TerrainMaterial.SetTexture( "_TerrainNormalMap", TerrainNormalMap );
			else
				TerrainMaterial.SetTexture( "_TerrainNormalMap", null );
			if(SplatMap1)
				TerrainMaterial.SetTexture("_Control", SplatMap1);
			if(SplatMap2)
				TerrainMaterial.SetTexture("_Control2nd", SplatMap2);
			
			TerrainMaterial.SetColor( "_SpecColor", SpecularColor );


			if(CombinedNormal12)
				TerrainMaterial.SetTexture( "_CombinedNormal12", CombinedNormal12 );
			else
				TerrainMaterial.SetTexture( "_CombinedNormal12", null );
			if(CombinedNormal34)
				TerrainMaterial.SetTexture( "_CombinedNormal34", CombinedNormal34 );
			else
				TerrainMaterial.SetTexture( "_CombinedNormal34", null );
			if(CombinedNormal56)
				TerrainMaterial.SetTexture( "_CombinedNormal56", CombinedNormal56 );
			else
				TerrainMaterial.SetTexture( "_CombinedNormal56", null );
			
			TerrainMaterial.SetVector( "_terrainCombinedFloats", new Vector4(MultiUv, DesMultiUvFac, SplattingDistance, SpecPower) );
			TerrainMaterial.SetFloat( "_terrainDetailsFadeLenght", TerrainDetailsFadeLenght );

			TerrainMaterial.SetFloat( "_DetailNormalStrength", DetailNormalStrength );
			TerrainMaterial.SetFloat( "_UpscaledNormalStrength", UpscaledNormalStrength );
			TerrainMaterial.SetVector( "_Parallax", new Vector4(P_Elevation_Tex3, P_Elevation_Tex4, P_GlossFactor_Tex3, P_GlossFactor_Tex4) );

			if (BlendMultiUv) {
				Shader.EnableKeyword("USE_BLENDMULTIUV");
				Shader.DisableKeyword("USE_ADDDMULTIUV");
			}
			else {
				Shader.DisableKeyword("USE_BLENDMULTIUV");
				Shader.EnableKeyword("USE_ADDDMULTIUV");
			}
			if (TerrainFresnel) {
				Shader.EnableKeyword("USE_FRESNEL");
				Shader.DisableKeyword("NO_FRESNEL");
			}
			else {
				Shader.EnableKeyword("NO_FRESNEL");
				Shader.DisableKeyword("USE_FRESNEL");	
			}
			if (AdvancedNormalBlending) {
				Shader.DisableKeyword("USE_STANDARDNORMALBLENDING");
				Shader.EnableKeyword("USE_ADVANCEDNORMALBLENDING");
			}
			else {
				Shader.EnableKeyword("USE_STANDARDNORMALBLENDING");
				Shader.DisableKeyword("USE_ADVANCEDNORMALBLENDING");	
			}
			if (Terrain_PM) {
				Shader.DisableKeyword("DONOT_USE_PARALLAX");
				Shader.EnableKeyword("USE_PARALLAX");
			}
			else {
				Shader.EnableKeyword("DONOT_USE_PARALLAX");
				Shader.DisableKeyword("USE_PARALLAX");	
			}
			
			TerrainMaterial.SetColor( "_ColTex1", ColTex1 );
			TerrainMaterial.SetFloat( "_Spec1", Spec1 );
			TerrainMaterial.SetColor( "_ColTex2", ColTex2 );
			TerrainMaterial.SetFloat( "_Spec2", Spec2 );
			TerrainMaterial.SetColor( "_ColTex3", ColTex3 );
			TerrainMaterial.SetFloat( "_Spec3", Spec3 );
			TerrainMaterial.SetColor( "_ColTex4", ColTex4 );
			TerrainMaterial.SetFloat( "_Spec4", Spec4 );
			TerrainMaterial.SetColor( "_ColTex5", ColTex5 );
			TerrainMaterial.SetFloat( "_Spec5", Spec5 );
			TerrainMaterial.SetColor( "_ColTex6", ColTex6 );
			TerrainMaterial.SetFloat( "_Spec6", Spec6 );
			
			TerrainMaterial.SetVector("_Elev", new Vector4(Elev1, Elev2, Elev3, Elev4) );
			TerrainMaterial.SetVector("_Elev1", new Vector4(Elev5, Elev6, 1f, 1f) );

			TerrainMaterial.SetVector("_Fresnel", new Vector4(FresnelIntensity, FresnelPower, FresnelBias, 0f ) );
			TerrainMaterial.SetColor("_ReflectionColor", ReflectionColor);
			
			TerrainMaterial.SetVector("_DecalCombinedFloats", new Vector4(Decal1_ColorCorrectionStrenght, Decal2_ColorCorrectionStrenght, Decal1_Sharpness, Decal2_Sharpness));
		
			for (int i = 0; i < targetTerrain.terrainData.splatPrototypes.Length; i++) {
				TerrainMaterial.SetTexture( "_Splat" + i.ToString(), targetTerrain.terrainData.splatPrototypes[i].texture );

				TerrainMaterial.SetTextureScale( "_Splat" + i.ToString(), new Vector2( terrainWidth / targetTerrain.terrainData.splatPrototypes[i].tileSize.x, terrainWidth / targetTerrain.terrainData.splatPrototypes[i].tileSize.x ) );

				TerrainMaterial.SetFloat( "_Splat" + i.ToString() + "Tiling", terrainWidth / targetTerrain.terrainData.splatPrototypes[i].tileSize.x );
			}			
		}
	}
}