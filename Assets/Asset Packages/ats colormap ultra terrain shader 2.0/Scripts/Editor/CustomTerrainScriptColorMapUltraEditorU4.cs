#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.IO;


[CustomEditor (typeof(CustomTerrainScriptColorMapUltraU4))]
public class CustomTerrainScriptColorMapUltraEditorU4 : Editor {
	
	// help boxes
//	private bool hSplatMap1 = false;
//	private bool hSplatMap2 = false;
	private bool hMultiUv = false;
	private bool hSplattingDistance = false;
	private bool hColCor = false;
	private bool hCreateComNormals = false;
	private bool hDecColCor = false;
	private bool hCreateMbM  = false;
	private bool hAddMbM  = false;
	private bool hFresnel  = false;
	
	// default values and registers
	protected Material defaultMat = null;
	protected Color avrgCol = Color.white;
	protected bool wasReadable = false;

	// messages
	private string error0 = "";
	private string error1 = "";
	private string error2 = "";

	//


	public override void OnInspectorGUI () {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		Terrain targetTerrain = (Terrain)script.GetComponent(typeof(Terrain));



		//SplattingDistance = script.SplattingDistance;

		Color myBlue = new Color(0.5f,0.7f,1.0f,1.0f);
		
		GUILayout.MinWidth(100);
		EditorGUI.indentLevel = 0;

//EditorGUI.BeginChangeCheck();

// //////////////////// change inspector
		EditorGUILayout.BeginVertical("Box");
		GUILayout.Label ("Select Interface","BoldLabel");

		EditorGUILayout.BeginHorizontal();
		script.showNewInspector = EditorGUILayout.Toggle("", script.showNewInspector, GUILayout.Width(14));
		GUILayout.Label ("Show custom Editor");
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

// //////////////////// all settings will be visible only if any texture is assigned to the terrain
		if (targetTerrain.terrainData.splatPrototypes.Length > 0) {
		// start new editor
			if (script.showNewInspector) {
	// start new editor
	
	// //////////////////// Introduction
			GUI.color = myBlue;
			EditorGUILayout.BeginVertical("Box");
			GUILayout.Label ("ATS Color Map Ultra Shader Setup","BoldLabel");
			script.detailsIntro = EditorGUILayout.Foldout(script.detailsIntro," Introduction\n");
			GUI.color = Color.white;
			if(script.detailsIntro){
				GUILayout.Space(5);
				EditorGUILayout.HelpBox("Welcome to the ATS Color Map Ultra Shader Setup! " +
					"After having prepared all needed textures according to the documentation this editor will help you to setup your terrain and the shader correctely." +
					"\n\nStart by assigning the first 4 detail maps (diffuse only) to the painting tab of the terrain - just like you are used to do." +
					"\nThen proceed with the next step: 'Base Terrain Settings and Textures' and go through all other foldouts." +
					"\nPlease note that unless you have not created a new terrain material, you won't see the shader at work, " +
					"but unity's built in one. So go through all opened foldouts, then assign the shader by pressing 'Create new Terrain Material' at the bottom of this editor" +
					"\nFinally open the other foldouts and start to fine tune all values.", MessageType.None, true);
			}
			EditorGUILayout.EndVertical();
				
	// //////////////////// Base Terrain Settings and Textures
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Base Terrain Textures and Settings","BoldLabel");
			GUI.color = Color.white;
			script.detailsBase = EditorGUILayout.Foldout(script.detailsBase," Details\n");
			if(script.detailsBase){
				GUILayout.Space(5);


				EditorGUILayout.BeginHorizontal();
				bool UseColormap = EditorGUILayout.Toggle("", script.UseColormap, GUILayout.Width(14));
				GUILayout.Label ("Enable Color Map");
				if ( UseColormap != script.UseColormap ){
					Undo.RecordObject(script, "enable Color Map");
					script.UseColormap = UseColormap;
				} 
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(5);



				EditorGUILayout.BeginHorizontal();
					GUILayout.Label ("Custom \nColor Map", GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					GUILayout.Label ("Terrain \nNormal Map", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
				EditorGUILayout.EndHorizontal();
					GUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
					Texture2D CustomColorMap = (Texture2D)EditorGUILayout.ObjectField(script.CustomColorMap, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					if ( CustomColorMap != script.CustomColorMap ){
						Undo.RecordObject(script, "CustomColorMap");
						script.CustomColorMap = CustomColorMap;
					}
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					Texture2D TerrainNormalMap = (Texture2D)EditorGUILayout.ObjectField(script.TerrainNormalMap, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					if ( TerrainNormalMap != script.TerrainNormalMap ){
						Undo.RecordObject(script, "TerrainNormalMap");
						script.TerrainNormalMap = TerrainNormalMap;
					}
				EditorGUILayout.EndHorizontal();
					
				
				/* GUILayout.Space(15);
				script.SplatMap1 = (Texture2D)EditorGUILayout.ObjectField("Terrain SplatAlpha 0", script.SplatMap1, typeof(Texture2D), false);
	
				GUI.color = myBlue;
				hSplatMap1 = EditorGUILayout.Foldout(hSplatMap1," Help");
				GUI.color = Color.white;
				if(hSplatMap1){
				EditorGUILayout.HelpBox("Terrain SplatAlpha 0 controls the distribution of detail texture 1 - 4." +
					"\nIn order to assign it switch to the project tab, find your terrain and open it's foldout. " +
					"According to the number of assigned textures the terrain asset will contain one or two splatmaps. Find 'SplatAlpha 0' and drag it to the slot above." +
					"\nPlease note: There won't be any splatmap unless you have assigned at least one texture to the terrain." +
					"\nIn case you do not see the any splatmap although you have already added 4 detail textures, simply reimport or refresh your terrain asset." +
					"\n\nYou only have to do this once." +
					"\n\nIf you want to import a custom splat map please switch to the foldout: 'Import custom splatmap'.", MessageType.None, true);
				}
	
				GUILayout.Space(10);
				script.SplatMap2 = (Texture2D)EditorGUILayout.ObjectField("Terrain SplatAlpha 1", script.SplatMap2, typeof(Texture2D), false);
				
				GUI.color = myBlue;
				hSplatMap2 = EditorGUILayout.Foldout(hSplatMap2," Help");
				GUI.color = Color.white;
				if(hSplatMap2){
				EditorGUILayout.HelpBox("Terrain SplatAlpha 1 controls the distribution of detail texture 5, 6 and the two decals." +
					"\nIn order to assign it switch to the project tab, find your terrain and open it's foldout. " +
					"According to the number of assigned textures the terrain asset will contain one or two splatmaps. Find 'SplatAlpha 1' and drag it to the slot above." +
					"\nPlease note: There won't be a second splatmap unless you have assigned at least 5 textures to the terrain." +
					"\nIn case you do not see the second splatmap although you have already added a 5th texture, simply reimport or refresh your terrain asset." +
					"\n\nYou only have to do this once.", MessageType.None, true);
				} */
					
				// automatically assign terrain’s alphaMaps	
				if (script.SplatMap1 == null && targetTerrain.terrainData.alphamapLayers > 0) {
					grabTerrainSplatMaps();
				}
				if (script.SplatMap2 == null && targetTerrain.terrainData.alphamapLayers > 3) {
					grabTerrainSplatMaps();
				}
				
				GUILayout.Space(15);
				Color SpecularColor = EditorGUILayout.ColorField("Terrain Specular Color", script.SpecularColor);
				if ( SpecularColor != script.SpecularColor ){
					Undo.RecordObject(script, "SpecularColor");
					script.SpecularColor = SpecularColor;
				}
				GUILayout.Space(5);
				float SpecPower = EditorGUILayout.Slider("Terrain Specular Power", script.SpecPower, 0.01f, 50.0f);
				if ( SpecPower != script.SpecPower ){
					Undo.RecordObject(script, "SpecPower");
					script.SpecPower = SpecPower;
				}  
				
				GUILayout.Space(15);
				EditorGUILayout.BeginHorizontal();
				bool TerrainFresnel = EditorGUILayout.Toggle("", script.TerrainFresnel, GUILayout.Width(14));
				GUILayout.Label ("Enable color reflection");
				if ( TerrainFresnel != script.TerrainFresnel ){
					Undo.RecordObject(script, "Enable color reflection");
					script.TerrainFresnel = TerrainFresnel;
				}
				EditorGUILayout.EndHorizontal();
 
				GUI.color = myBlue;
				hFresnel = EditorGUILayout.Foldout(hFresnel," Help");
				GUI.color = Color.white;
				if(hFresnel){
					EditorGUILayout.HelpBox("Enabling color reflection allows you to add some kind of a wet look to your terrain." +
						"\nPlease note: In case you use this feature mesh blending only supports 3 detail terrain textures.", MessageType.None, true);
				}
				if (script.TerrainFresnel) {
					GUILayout.Space(5);
					float FresnelIntensity = EditorGUILayout.Slider("Fresnel Intensity", script.FresnelIntensity, 0.01f, 10.0f);
					if ( FresnelIntensity != script.FresnelIntensity ){
						Undo.RecordObject(script, "FresnelIntensity");
						script.FresnelIntensity = FresnelIntensity;
					}
					GUILayout.Space(5);
					float FresnelPower = EditorGUILayout.Slider("Fresnel Power", script.FresnelPower, 0.01f, 10.0f);
					if ( FresnelPower != script.FresnelPower ){
						Undo.RecordObject(script, "FresnelPower");
						script.FresnelPower = FresnelPower;
					}
					GUILayout.Space(5);
					float FresnelBias = EditorGUILayout.Slider("Fresnel Bias", script.FresnelBias, -1.0f, 1.0f);
					if ( FresnelBias != script.FresnelBias ){
						Undo.RecordObject(script, "FresnelBias");
						script.FresnelBias = FresnelBias;
					}
					GUILayout.Space(5);
					Color ReflectionColor = EditorGUILayout.ColorField("Terrain Reflection Color", script.ReflectionColor);
					if ( ReflectionColor != script.ReflectionColor ){
						Undo.RecordObject(script, "ReflectionColor");
						script.ReflectionColor = ReflectionColor;
					}
				}
	
				GUILayout.Space(15);
				float SplattingDistance = EditorGUILayout.FloatField("Splatting Distance", script.SplattingDistance);
				if ( SplattingDistance != script.SplattingDistance ){
					Undo.RecordObject(script, "SplattingDistance");
					script.SplattingDistance = SplattingDistance;
				}
				float TerrainDetailsFadeLenght = EditorGUILayout.FloatField("Fade Lenght", script.TerrainDetailsFadeLenght);
				if ( TerrainDetailsFadeLenght != script.TerrainDetailsFadeLenght ){
					Undo.RecordObject(script, "TerrainDetailsFadeLenght");
					script.TerrainDetailsFadeLenght = TerrainDetailsFadeLenght;
				}
				GUI.color = myBlue;
				hSplattingDistance = EditorGUILayout.Foldout(hSplattingDistance," Help");
				GUI.color = Color.white;
				if(hSplattingDistance){
					EditorGUILayout.HelpBox("The splatting distance determines the distance from the active camera in which the shader will drop the detail textures and only render the custom color and the terrain normal map." +
						"\nThe lower the distance the faster the rendering for dropping the detail textures will reduce the needed calculations per pixel.", MessageType.None, true);
				}
				
			}
			EditorGUILayout.EndVertical();
	
	// //////////////////// Import custom splatmaps
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Import Custom Splat Maps","BoldLabel");
			GUI.color = Color.white;
			script.detailsImportSplat = EditorGUILayout.Foldout(script.detailsImportSplat," Details\n");
			if(script.detailsImportSplat){
				GUILayout.Space(5);
				if (targetTerrain.terrainData.alphamapLayers > 3 ) {
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label ("Custom \nSplat Map1", GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (targetTerrain.terrainData.alphamapLayers < 8 ) {
						GUI.enabled = false;
					}
					else {
						GUI.enabled = true;
					}
					GUILayout.Label ("Custom \nSplat Map2", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(4);
					EditorGUILayout.BeginHorizontal();
					script.customSplatMap1 = (Texture2D)EditorGUILayout.ObjectField(script.customSplatMap1, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (targetTerrain.terrainData.alphamapLayers < 8 ) {
						GUI.enabled = false;
					}
					else {
						GUI.enabled = true;
					}
					script.customSplatMap2 = (Texture2D)EditorGUILayout.ObjectField(script.customSplatMap2, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					
					GUI.enabled = true;
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (GUILayout.Button("Apply Splat Map(s)")) {
						//applyCustomSplatMap(ref script.customSplatMap1, targetTerrain);
							applyCustomSplatMap(script.customSplatMap1, targetTerrain);
					}	
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(5);
					
					if (targetTerrain.terrainData.alphamapLayers < 8 ) {
							script.customSplatMap2 = null;
							EditorGUILayout.HelpBox("You are not able to import the second splatmap unless you have assigned all 8 detail textures.", MessageType.Info, true);
					}
					if (script.customSplatMap2 == null) {
						EditorGUILayout.HelpBox("Not having assigned a second splatmap " +
							"will completely set the terrain's second splat map to black if you hit 'Apply Splat Map(s)'.", MessageType.Warning, true);
					}
				}
				else {
					EditorGUILayout.HelpBox("You are not able to import a custom splatmap unless you have assigned at least 4 detail textures.", MessageType.Warning, true);
				}
			}
			EditorGUILayout.EndVertical();

	// //////////////////// Parallax mapping
			EditorGUILayout.BeginVertical("Box");
			GUILayout.Label ("Parallax Mapping","BoldLabel");
			EditorGUILayout.BeginHorizontal();
			bool Terrain_PM = EditorGUILayout.Toggle("", script.Terrain_PM, GUILayout.Width(14));
			GUILayout.Label ("Use Parallax Mapping");
			if ( Terrain_PM != script.Terrain_PM ){
				Undo.RecordObject(script, "enable Parallax Mapping");
				script.Terrain_PM = Terrain_PM;
			} 
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
			if ( Terrain_PM) {
				float P_Elevation_Tex3 = EditorGUILayout.Slider("Parallax Elevation Tex3", script.P_Elevation_Tex3, 0.005f, 0.08f);
				if ( P_Elevation_Tex3 != script.P_Elevation_Tex3 ){
					Undo.RecordObject(script, "Parallax Elevation Tex3");
					script.P_Elevation_Tex3 = P_Elevation_Tex3;
				}
				float P_GlossFactor_Tex3 = EditorGUILayout.Slider("Parallax GlossFactor Tex3", script.P_GlossFactor_Tex3, -0.9f, 0.9f);
				if ( P_GlossFactor_Tex3 != script.P_GlossFactor_Tex3 ){
					Undo.RecordObject(script, "Parallax GlossFactor Tex3");
					script.P_GlossFactor_Tex3 = P_GlossFactor_Tex3;
				}
				GUILayout.Space(5);
				float P_Elevation_Tex4 = EditorGUILayout.Slider("Parallax Elevation Tex4", script.P_Elevation_Tex4, 0.005f, 0.08f);
				if ( P_Elevation_Tex4 != script.P_Elevation_Tex4 ){
					Undo.RecordObject(script, "Parallax Elevation Tex4");
					script.P_Elevation_Tex4 = P_Elevation_Tex4;
				}
				float P_GlossFactor_Tex4 = EditorGUILayout.Slider("Parallax GlossFactor Tex4", script.P_GlossFactor_Tex4, -0.9f, 0.9f);
				if ( P_GlossFactor_Tex4 != script.P_GlossFactor_Tex4 ){
					Undo.RecordObject(script, "Parallax GlossFactor Tex4");
					script.P_GlossFactor_Tex4 = P_GlossFactor_Tex4;
				}

			}
			EditorGUILayout.EndVertical();

	// //////////////////// Multi uv mixing
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("MultiUV Mixing","BoldLabel");
			GUI.color = Color.white;
			script.detailsMultiuv = EditorGUILayout.Foldout(script.detailsMultiuv," Details\n");
			if(script.detailsMultiuv){
				GUI.color = myBlue;
				hMultiUv = EditorGUILayout.Foldout(hMultiUv," Help");
				GUI.color = Color.white;
				if(hMultiUv){
					EditorGUILayout.HelpBox("The shader supports multi uv mixing which will reduce tiling artifacts by mixing 2 different scaled version of the same texture. " +
						"\n\nPlease note: Multi UV mixing is only supported on the third detail texture (stored in the blue channel of the first splat map).", MessageType.None, true);
				}
				GUILayout.Space(5);
				//script.MultiUv = EditorGUILayout.Slider("MultiUV Mixing Factor", Mathf.FloorToInt(script.MultiUv), 0, 20);  
				float MultiUv = EditorGUILayout.Slider("MultiUV Mixing Factor", script.MultiUv, 0.01f, 1.0f);
				if ( MultiUv != script.MultiUv ){
					Undo.RecordObject(script, "MultiUv");
					script.MultiUv = MultiUv;
				}
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				//bool BlendMultiUv = EditorGUILayout.Toggle("Use MultiUV Blending", script.BlendMultiUv);
				bool BlendMultiUv = EditorGUILayout.Toggle("", script.BlendMultiUv, GUILayout.Width(14));
				GUILayout.Label ("Use MultiUV Blending");
				if ( BlendMultiUv != script.BlendMultiUv ){
					Undo.RecordObject(script, "BlendMultiUv");
					script.BlendMultiUv = BlendMultiUv;
				} 
				EditorGUILayout.EndHorizontal();
				if(script.BlendMultiUv == false) {
					float DesMultiUvFac = EditorGUILayout.Slider("Adjust Saturation", script.DesMultiUvFac, 0.0f, 1.0f);
					if ( DesMultiUvFac != script.DesMultiUvFac ){
						Undo.RecordObject(script, "DesMultiUvFac");
						script.DesMultiUvFac = DesMultiUvFac;
					} 
					EditorGUILayout.HelpBox("If 'Use MultiUV Blending' is unchecked, the shader will simply add both instances of the 3rd texture, which leads to a result very rich in contrast. So you might want to adjust its final saturation.", MessageType.None, true);
				}
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				bool AdvancedNormalBlending = EditorGUILayout.Toggle("", script.AdvancedNormalBlending, GUILayout.Width(14));
				GUILayout.Label ("Use Advanced Normal Blending");
				if ( AdvancedNormalBlending != script.AdvancedNormalBlending ){
					Undo.RecordObject(script, "AdvancedNormalBlending");
					script.AdvancedNormalBlending = AdvancedNormalBlending;
				}
				EditorGUILayout.EndHorizontal();
				if(script.AdvancedNormalBlending) {
					float DetailNormalStrength = EditorGUILayout.Slider("Detail Normal Strength", script.DetailNormalStrength, 0.0f, 1.0f);
					if ( DetailNormalStrength != script.DetailNormalStrength ){
						Undo.RecordObject(script, "DetailNormalStrength");
						script.DetailNormalStrength = DetailNormalStrength;
					}
					float UpscaledNormalStrength = EditorGUILayout.Slider("Upscaled Normal Strength", script.UpscaledNormalStrength, 0.0f, 1.0f);
					if ( UpscaledNormalStrength != script.UpscaledNormalStrength ){
						Undo.RecordObject(script, "UpscaledNormalStrength");
						script.UpscaledNormalStrength = UpscaledNormalStrength;
					}
				}

			}
			EditorGUILayout.EndVertical();
	
	// //////////////////// Create Combine Normal Textures
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Create Combined Detail Normal Maps","BoldLabel");
			GUI.color = Color.white;
			script.detailsCreateComNormals = EditorGUILayout.Foldout(script.detailsCreateComNormals," Details\n");
			if(script.detailsCreateComNormals){
					
				GUI.color = myBlue;
				hCreateComNormals = EditorGUILayout.Foldout(hCreateComNormals," Help");
				GUI.color = Color.white;
				if(hCreateComNormals){
				EditorGUILayout.HelpBox("Create combined normal maps as needed by the shader right within unity." +
					" Simply assign 2 source textures and press 'Combine'.", MessageType.None, true);
				}
					
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
					string name = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[0].texture) );
					GUILayout.Label ("Normal 1 for:\n" + name, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
					//GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					string name1 = "unassigned";
					if (targetTerrain.terrainData.splatPrototypes.Length > 1){
						name1 = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[1].texture) );
					}
					GUILayout.Label ("Normal 2 for:\n" + name1, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
					script.SourceNormal1 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal1, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(13), GUILayout.MaxWidth(23));
					script.SourceNormal2 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal2, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (script.SourceNormal1 != null && script.SourceNormal2 != null) {
						if (GUILayout.Button("Combine")) {
							error0 = combineNormalMaps( script.SourceNormal1, script.SourceNormal2, 1);
						}
					}
					else {
						GUI.enabled = false;
						GUILayout.Button("Combine");
						GUI.enabled = true;
					}
				EditorGUILayout.EndHorizontal();
				if (error0 != "") {
					EditorGUILayout.HelpBox(error0, MessageType.Error, true);
				}
				
				GUILayout.Space(10);
				EditorGUILayout.BeginHorizontal();
					string name2 = "unassigned";
					if (targetTerrain.terrainData.splatPrototypes.Length > 2){
						name2 = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[2].texture) );
					}
					GUILayout.Label ("Normal 3 for:\n" + name2, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
					//GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					string name3 = "unassigned";
					if (targetTerrain.terrainData.splatPrototypes.Length > 3){
						name3 = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[3].texture) );
					}
					GUILayout.Label ("Normal 4 for:\n" + name3, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
					
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
					script.SourceNormal3 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal3, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(13), GUILayout.MaxWidth(23));
					script.SourceNormal4 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal4, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (script.SourceNormal2 != null && script.SourceNormal3 != null) {
						if (GUILayout.Button("Combine")) {
							error1 = combineNormalMaps( script.SourceNormal3, script.SourceNormal4, 2);
						}
					}
					else {
						GUI.enabled = false;
						GUILayout.Button("Combine");
						GUI.enabled = true;	
					}
				EditorGUILayout.EndHorizontal();
				if (error1 != "") {
					EditorGUILayout.HelpBox(error1, MessageType.Error, true);
				}
					
				GUILayout.Space(10);
				EditorGUILayout.BeginHorizontal();
					string name4 = "unassigned";
					if (targetTerrain.terrainData.splatPrototypes.Length > 4){
						name4 = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[4].texture) );
					}
					GUILayout.Label ("Normal 5 for:\n" + name4, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
					//GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					string name5 = "unassigned";
					if (targetTerrain.terrainData.splatPrototypes.Length > 5){
						name5 = Path.GetFileName( UnityEditor.AssetDatabase.GetAssetPath(targetTerrain.terrainData.splatPrototypes[5].texture) );
					}
					GUILayout.Label ("Normal 6 for:\n" + name5, GUILayout.MinWidth(90), GUILayout.MaxWidth(90));
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
					script.SourceNormal5 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal5, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(23));
					script.SourceNormal6 = (Texture2D)EditorGUILayout.ObjectField(script.SourceNormal6, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					if (script.SourceNormal4 != null && script.SourceNormal5 != null) {
						if (GUILayout.Button("Combine")) {
							error2 = combineNormalMaps( script.SourceNormal5, script.SourceNormal6, 3);
						}
					}
					else {
						GUI.enabled = false;
						GUILayout.Button("Combine");
						GUI.enabled = true;	
					}
				EditorGUILayout.EndHorizontal();
				if (error2 != "") {
					EditorGUILayout.HelpBox(error2, MessageType.Error, true);
				}
				
				GUILayout.Space(4);
			}		
			EditorGUILayout.EndVertical();	
	
// //////////////////// Assign Combined Normal Textures
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Assign Combined Detail Normal Maps","BoldLabel");
			GUI.color = Color.white;
			script.detailsComNormals = EditorGUILayout.Foldout(script.detailsComNormals," Details\n");
			if(script.detailsComNormals){
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();	
					GUILayout.Label ("Normal 12", GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					GUILayout.Label ("Normal 34", GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					GUILayout.Label ("Normal 56", GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
				EditorGUILayout.BeginHorizontal();
					Texture2D CombinedNormal12 = (Texture2D)EditorGUILayout.ObjectField(script.CombinedNormal12, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					if ( CombinedNormal12 != script.CombinedNormal12 ){
						Undo.RecordObject(script, "CombinedNormal12");
						script.CombinedNormal12 = CombinedNormal12;
					}
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					Texture2D CombinedNormal34 = (Texture2D)EditorGUILayout.ObjectField(script.CombinedNormal34, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					if ( CombinedNormal34 != script.CombinedNormal34 ){
						Undo.RecordObject(script, "CombinedNormal34");
						script.CombinedNormal34 = CombinedNormal34;
					}
					GUILayout.Label ("", GUILayout.MinWidth(3), GUILayout.MaxWidth(3));
					Texture2D CombinedNormal56 = (Texture2D)EditorGUILayout.ObjectField(script.CombinedNormal56, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
					if ( CombinedNormal56 != script.CombinedNormal56 ){
						Undo.RecordObject(script, "CombinedNormal56");
						script.CombinedNormal56 = CombinedNormal56;
					}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
			}
			EditorGUILayout.EndVertical();
	
	// //////////////////// Color Correction
			EditorGUILayout.BeginVertical("Box");
			///////	
			GUI.color = Color.yellow;
			GUILayout.Label ("Texture Blending & Color Correction","BoldLabel");
			GUI.color = Color.white;
			script.detailsColCor = EditorGUILayout.Foldout(script.detailsColCor," Details\n");
			if(script.detailsColCor){
				GUILayout.Space(5);
				GUILayout.Label("Assign the average Color Value for each\nDetail Texture:");
				GUI.color = myBlue;
				hColCor = EditorGUILayout.Foldout(hColCor," Help");
				GUI.color = Color.white;
				if(hColCor){
					EditorGUILayout.HelpBox("In order to blend nicely between color map and detail textures you have to setup the average color value for each texture. You can simply grab a color which more or less fits the average color value of the whole texture from the small thumbnails within 'project tab', choose it manually - which can give you very interesting results on the terrain, just give it a try - or use the button below every field.\nPlease note: If you choose 'Get average Color...' this might take some time as unity might have to reimport the textures to make them readable.\nCome back here and adjust your values whenever you have changed any detail texture.",MessageType.None, true);
				}
				//
				GUILayout.Space(5);
				Color ColTex1 = EditorGUILayout.ColorField("Avrg. Color Tex 1", script.ColTex1);
				if ( ColTex1 != script.ColTex1 ){
						Undo.RecordObject(script, "Set average Color Tex1");
						script.ColTex1 = ColTex1;
				}
				if (GUILayout.Button("Get average Color for Tex 1")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[0].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex1");
						script.ColTex1 = avrgCol;
				}
				//
				GUILayout.Space(10);
				if (targetTerrain.terrainData.splatPrototypes.Length > 1) {
					Color ColTex2 = EditorGUILayout.ColorField("Avrg. Color Tex 2", script.ColTex2);
					if ( ColTex2 != script.ColTex2 ){
						Undo.RecordObject(script, "Set average Color Tex2");
						script.ColTex2 = ColTex2;
					}
					if (GUILayout.Button("Get average Color for Tex 2")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[1].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex1");
						script.ColTex2 = avrgCol;
					}
				}
				else {
					GUI.enabled = false;
					script.ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 2", script.ColTex2);
					GUILayout.Button("Get average Color for Tex 2");
					GUI.enabled = true;		
				}
				//
				GUILayout.Space(10);
				if (targetTerrain.terrainData.splatPrototypes.Length > 2) {
					Color ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 3", script.ColTex3);
					if ( ColTex3 != script.ColTex3 ){
						Undo.RecordObject(script, "Set average Color Tex3");
						script.ColTex3 = ColTex3;
					}
					if (GUILayout.Button("Get average Color for Tex 3")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[2].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex3");
						script.ColTex3 = avrgCol;
					}
				}
				else {
					GUI.enabled = false;
					script.ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 3", script.ColTex3);
					GUILayout.Button("Get average Color for Tex 3");
					GUI.enabled = true;		
				}
				//
				GUILayout.Space(10);
				if (targetTerrain.terrainData.splatPrototypes.Length > 3) {
					Color ColTex4 = EditorGUILayout.ColorField("Avrg. Color Tex 4", script.ColTex4);
					if ( ColTex4 != script.ColTex4 ){
						Undo.RecordObject(script, "Set average Color Tex4");
						script.ColTex4 = ColTex4;
					}
					if (GUILayout.Button("Get average Color for Tex 4")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[3].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex4");
						script.ColTex4 = avrgCol;
					}
				}
				else {
					GUI.enabled = false;
					script.ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 4", script.ColTex3);
					GUILayout.Button("Get average Color for Tex 4");
					GUI.enabled = true;		
				}
				//
				GUILayout.Space(10);
				if (targetTerrain.terrainData.splatPrototypes.Length > 4) {
					Color ColTex5 = EditorGUILayout.ColorField("Avrg. Color Tex 5", script.ColTex5);
					if ( ColTex5 != script.ColTex5 ){
						Undo.RecordObject(script, "Set average Color Tex5");
						script.ColTex5 = ColTex5;
					}
					if (GUILayout.Button("Get average Color for Tex 5")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[4].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex5");
						script.ColTex5 = avrgCol;
					}
				}
				else {
					GUI.enabled = false;
					script.ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 5", script.ColTex3);
					GUILayout.Button("Get average Color for Tex 5");
					GUI.enabled = true;		
				}
				//
				GUILayout.Space(10);
				if (targetTerrain.terrainData.splatPrototypes.Length > 5) {
					Color ColTex6 = EditorGUILayout.ColorField("Avrg. Color Tex 6", script.ColTex6);
					if ( ColTex6 != script.ColTex6 ){
						Undo.RecordObject(script, "Set average Color Tex6");
						script.ColTex6 = ColTex6;
					}
					if (GUILayout.Button("Get average Color for Tex 6")) {
						Texture2D terrainTex = targetTerrain.terrainData.splatPrototypes[5].texture;
						readFromTerrainTexture(ref terrainTex);
						Undo.RecordObject(script, "Set average Color Tex6");
						script.ColTex6 = avrgCol;
					}
				}
				else {
					GUI.enabled = false;
					script.ColTex3 = EditorGUILayout.ColorField("Avrg. Color Tex 6", script.ColTex3);
					GUILayout.Button("Get average Color for Tex 6");
					GUI.enabled = true;		
				}
				// ColorCorrection
				GUILayout.Space(15);
				GUILayout.Label("Set up the strength of color correction\napplied to the decals:");
				GUI.color = myBlue;
				hDecColCor = EditorGUILayout.Foldout(hDecColCor," Help");
				GUI.color = Color.white;
				if(hDecColCor){
					EditorGUILayout.HelpBox("You can create quite outstanding decals or nicely blended ones depending on the choosen color correction strength: 1.0 = full color correction so the decal is treated like a regular terrain texture and fades well with the color map / 0.0 no color correction at all: the decal fully keeps it color.",MessageType.None, true);
				}
				float Decal1_ColorCorrectionStrenght = EditorGUILayout.Slider("Decal 1", script.Decal1_ColorCorrectionStrenght, 0.0f, 1.0f);
				if ( Decal1_ColorCorrectionStrenght != script.Decal1_ColorCorrectionStrenght ){
					Undo.RecordObject(script, "Decal1 ColorCorrectionStrenght");
					script.Decal1_ColorCorrectionStrenght = Decal1_ColorCorrectionStrenght;
				}
				float Decal1_Sharpness = EditorGUILayout.Slider("Decal 1 Sharpness", script.Decal1_Sharpness, 0.1f, 32.0f);
				if ( Decal1_Sharpness != script.Decal1_Sharpness ){
					Undo.RecordObject(script, "Decal1 Sharpness");
					script.Decal1_Sharpness = Decal1_Sharpness;
				}
				GUILayout.Space(5);
				float Decal2_ColorCorrectionStrenght = EditorGUILayout.Slider("Decal 2", script.Decal2_ColorCorrectionStrenght, 0.0f, 1.0f);    		
				if ( Decal2_ColorCorrectionStrenght != script.Decal2_ColorCorrectionStrenght ){
					Undo.RecordObject(script, "Decal2 ColorCorrectionStrenght");
					script.Decal2_ColorCorrectionStrenght = Decal2_ColorCorrectionStrenght;
				}
				float Decal2_Sharpness = EditorGUILayout.Slider("Decal 2 Sharpness", script.Decal2_Sharpness, 0.1f, 32.0f);
				if ( Decal2_Sharpness != script.Decal2_Sharpness ){
					Undo.RecordObject(script, "Decal2 Sharpness");
					script.Decal2_Sharpness = Decal2_Sharpness;
				}
			}
			EditorGUILayout.EndVertical();
	
	// //////////////////// Elevation and Spec Values
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Elevation & Specular Values","BoldLabel");
			GUI.color = Color.white;
			script.detailsSpecVal = EditorGUILayout.Foldout(script.detailsSpecVal," Details\n");
			if(script.detailsSpecVal){
				GUILayout.Space(5);
				GUILayout.Label("Assign Elevation and Shininess for each\nDetail Texture:");
				GUILayout.Space(5);
				float Elev1 = EditorGUILayout.Slider("Elevation Tex 1", script.Elev1, 0.1f, 20.0f);
				if ( Elev1 != script.Elev1 ){
					Undo.RecordObject(script, "Elevation Tex1");
					script.Elev1 = Elev1;
				}
				float Spec1 = EditorGUILayout.Slider("Shininess Tex 1", script.Spec1, 0.003f, 1.0f);
				if ( Spec1 != script.Spec1 ){
					Undo.RecordObject(script, "Shininess Tex1");
					script.Spec1 = Spec1;
				}
				GUILayout.Space(5);
				float Elev2 = EditorGUILayout.Slider("Elevation Tex 2", script.Elev2, 0.1f, 20.0f);
				if ( Elev2 != script.Elev2 ){
					Undo.RecordObject(script, "Elevation Tex2");
					script.Elev2 = Elev2;
				}
				float Spec2 = EditorGUILayout.Slider("Shininess Tex 2", script.Spec2, 0.003f, 1.0f);
				if ( Spec2 != script.Spec2 ){
					Undo.RecordObject(script, "Shininess Tex2");
					script.Spec2 = Spec2;
				}
				GUILayout.Space(5);
				float Elev3 = EditorGUILayout.Slider("Elevation Tex 3", script.Elev3, 0.1f, 20.0f);
				if ( Elev3 != script.Elev3 ){
					Undo.RecordObject(script, "Elevation Tex3");
					script.Elev3 = Elev3;
				}
				float Spec3 = EditorGUILayout.Slider("Shininess Tex 3", script.Spec3, 0.003f, 1.0f);
				if ( Spec3 != script.Spec3 ){
					Undo.RecordObject(script, "Shininess Tex3");
					script.Spec3 = Spec3;
				}
				GUILayout.Space(5);
				float Elev4 = EditorGUILayout.Slider("Elevation Tex 4", script.Elev4, 0.1f, 20.0f);
				if ( Elev4 != script.Elev4 ){
					Undo.RecordObject(script, "Elevation Tex4");
					script.Elev4 = Elev4;
				}
				float Spec4 = EditorGUILayout.Slider("Shininess Tex 4", script.Spec4, 0.003f, 1.0f);
				if ( Spec4 != script.Spec4 ){
					Undo.RecordObject(script, "Shininess Tex4");
					script.Spec4 = Spec4;
				}
				GUILayout.Space(5);
				float Elev5 = EditorGUILayout.Slider("Elevation Tex 5", script.Elev5, 0.1f, 20.0f);
				if ( Elev5 != script.Elev5 ){
					Undo.RecordObject(script, "Elevation Tex5");
					script.Elev5 = Elev5;
				}
				float Spec5 = EditorGUILayout.Slider("Shininess Tex 5", script.Spec5, 0.003f, 1.0f);
				if ( Spec5 != script.Spec5 ){
					Undo.RecordObject(script, "Shininess Tex5");
					script.Spec5 = Spec5;
				}
				GUILayout.Space(5);
				float Elev6 = EditorGUILayout.Slider("Elevation Tex 6", script.Elev6, 0.1f, 20.0f);
				if ( Elev6 != script.Elev6 ){
					Undo.RecordObject(script, "Elevation Tex6");
					script.Elev6 = Elev6;
				}
				float Spec6 = EditorGUILayout.Slider("Shininess Tex 6", script.Spec6, 0.003f, 1.0f);
				if ( Spec6 != script.Spec6 ){
					Undo.RecordObject(script, "Shininess Tex6");
					script.Spec6 = Spec6;
				}
			}
			EditorGUILayout.EndVertical();
				
	// //////////////////// Create Terrain Mat
			GUI.color = myBlue;
			EditorGUILayout.BeginVertical("Box");
			GUILayout.Label ("Create Terrain Material","BoldLabel");
			GUI.color = Color.white;
			if (GUILayout.Button("Create new Terrain Material", GUILayout.Height(36) )) {
	       			bool save = true;
					updateOrcreateTerrainMaterial(ref save);
			}
			GUILayout.Space(5);
			script.TerrainMaterial = (Material)EditorGUILayout.ObjectField("Terrain Material", script.TerrainMaterial, typeof(Material), false);
			GUILayout.Space(4);
			//EditorGUILayout.HelpBox("Make sure you edit the created material and add the terrain's splat map(s) to it:" +
			//	"\nIn order to assign it switch to the project tab, find your terrain and open it's foldout. " +
			//	"According to the number of assigned textures the terrain asset will contain one or two splatmaps. " +
			//	"Find 'SplatAlpha 0' and 'SplatAlpha 1' and drag it to the appropriate slots of the material." +
			//	"\n\nPlease note: There won't be a second splatmap unless you have assigned at least 5 textures to the terrain." +
			//	"\n\nIn case you do not see any splatmap although you have already added the detail textures, simply reimport or refresh your terrain asset.", MessageType.None, true);
			EditorGUILayout.EndVertical();
	
	
	// update terrain material
			script.updateColormapMaterial();
	


	// end new editor
		
			}
			else {
				DrawDefaultInspector ();
			}
	
	// //////////////////// mesh blending material
			EditorGUILayout.BeginVertical("Box");
			GUI.color = Color.yellow;
			GUILayout.Label ("Manage Mesh Blending Materials","BoldLabel");
			GUI.color = Color.white;
			script.detailsMeshMat = EditorGUILayout.Foldout(script.detailsMeshMat," Details\n");
	
			if(script.detailsMeshMat){
				GUILayout.Space(5);
				if (GUILayout.Button("Create new Mesh blending Material")) {
						bool save = true;
						updateOrcreateMeshBlendMaterial(ref save);
				}
				GUI.color = myBlue;
				hCreateMbM = EditorGUILayout.Foldout(hCreateMbM ," Help");
				GUI.color = Color.white;
				if(hCreateMbM){
				EditorGUILayout.HelpBox("This option lets you simply create a new mesh blending material suitable for the given terrain.", MessageType.None, true);
				}
				GUILayout.Space(5);
				for (int i = 0; i < script.blendedMaterials.Count; i++) {

					EditorGUILayout.BeginHorizontal();
					// EditorGUILayout.LabelField("Material", GUILayout.Width(50));					
					Material blendedMaterials = (Material)EditorGUILayout.ObjectField("", script.blendedMaterials[i], typeof(Material), false);
					if ( blendedMaterials != script.blendedMaterials[i] ){
						Undo.RecordObject(script, "Blended Material");
						script.blendedMaterials[i] = blendedMaterials;
					}
					if (GUILayout.Button("Remove", GUILayout.Width(60))) {
						Undo.RecordObject(script, "Remove Blended Material");
						script.blendedMaterials.Remove(script.blendedMaterials[i]);
					}
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(5);
				}
				GUILayout.Space(10);
				if (GUILayout.Button("Manually add Mesh blending Material")) {
					script.blendedMaterials.Add(defaultMat);
				}
				GUI.color = myBlue;
				hAddMbM = EditorGUILayout.Foldout(hAddMbM ," Help");
				GUI.color = Color.white;
				if(hAddMbM){
				EditorGUILayout.HelpBox("Add all mesh blending materials that you use on the given terrain in order to take advantage of the 'Update all assigned Materials' function.", MessageType.None, true);
				}
			}
			GUILayout.Space(10);
			if ( GUILayout.Button("Update all assigned Materials", GUILayout.Height(36)) || script.autoUpdateMeshMat ) {
				bool save = false;
				updateOrcreateMeshBlendMaterial(ref save);
			}
			EditorGUILayout.BeginHorizontal();
			script.autoUpdateMeshMat = EditorGUILayout.Toggle("", script.autoUpdateMeshMat, GUILayout.Width(14));
			GUILayout.Label ("Automatically update materials");
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(4);
			EditorGUILayout.EndVertical();

			EditorUtility.SetDirty(script);

		} // end if terrain textures
	
	else {
		EditorGUILayout.HelpBox("Please add at least one detail tetxure to the terrain.", MessageType.Warning, true);
	}
}

// ///////////////
	// if the editor gets focus
	void OnEnable() {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		script.updateColormapMaterial();
	}
	


// ///////////////
	private string combineNormalMaps( Texture2D sourceTex0, Texture2D sourceTex1, int targetTex ) {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		string error = null;
		if (sourceTex0 && sourceTex1 ) {
		
			// check textures
			bool wasReadable0 = false;
			TextureImporterFormat format0;
			TextureImporterType type0;
			bool wasReadable1 = false;
			TextureImporterFormat format1;
			TextureImporterType type1;
			// check texture0
			string path0 = AssetDatabase.GetAssetPath(sourceTex0);
			TextureImporter ti0 = (TextureImporter) TextureImporter.GetAtPath(path0);
			format0 = ti0.textureFormat;
			type0 = ti0.textureType;
			if (ti0.isReadable == true) {
				wasReadable0 = true;
			}
			else {
				ti0.isReadable = true;
			}
			if (ti0.textureFormat != TextureImporterFormat.AutomaticTruecolor || ti0.textureType != TextureImporterType.Image ) {
				ti0.textureType = TextureImporterType.Image; 
				ti0.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				// refresh texture
				AssetDatabase.ImportAsset( path0, ImportAssetOptions.ForceUpdate );
			}
			
			// check texture1
			string path1 = AssetDatabase.GetAssetPath(sourceTex1);
			TextureImporter ti1 = (TextureImporter) TextureImporter.GetAtPath(path1);
			format1 = ti1.textureFormat;
			type1 = ti1.textureType;
			if (ti1.isReadable == true) {
				wasReadable1 = true;
			}
			else {
				ti1.isReadable = true;
			}
			if (ti1.textureFormat != TextureImporterFormat.AutomaticTruecolor || ti1.textureType != TextureImporterType.Image ) {
				ti1.textureType = TextureImporterType.Image; 
				ti1.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				// refresh texture
				AssetDatabase.ImportAsset( path1, ImportAssetOptions.ForceUpdate ); 
			}
					
			// check dimensions
			if (sourceTex0.width == sourceTex1.width)
				{
				error = "";
				// start combining
				Texture2D combinedTex = new Texture2D( sourceTex0.width, sourceTex0.height, TextureFormat.ARGB32, true);
				Color combinedColor;
				for (int y = 0; y < sourceTex0.height; y++) {
					for (int x = 0; x < sourceTex0.width; x++) {
						combinedColor.r = sourceTex0.GetPixel(x,y).r;
						combinedColor.g = sourceTex0.GetPixel(x,y).g;
						combinedColor.b = sourceTex1.GetPixel(x,y).r;
						combinedColor.a = sourceTex1.GetPixel(x,y).g;
						combinedTex.SetPixel(x,y,combinedColor);
					}
				}
				// save texture
				String filePath = EditorUtility.SaveFilePanelInProject
				(
				"Save Combined Normals",
				"combinedNormals.png", 
				"png",
				"Choose a file location and name"
				);
				if (filePath!=""){
					var bytes = combinedTex.EncodeToPNG(); 
					File.WriteAllBytes(filePath, bytes); 
				
					AssetDatabase.Refresh();
					TextureImporter ti2 = AssetImporter.GetAtPath(filePath) as TextureImporter;
					ti2.anisoLevel = 7;
					ti2.textureType = TextureImporterType.Advanced;
					ti2.textureFormat = TextureImporterFormat.ARGB32;
					AssetDatabase.ImportAsset(filePath);
					AssetDatabase.Refresh();
					
					if (targetTex == 1) {
						script.CombinedNormal12 = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D) );
					}
					else if (targetTex == 2) {
						script.CombinedNormal34 = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D) );
					}
					else {
						script.CombinedNormal56 = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D) );
					}
				}
				DestroyImmediate(combinedTex, true);
			}
			else {
				//Debug.Log ("Both Textures have to fit in size.");
				error = "Both Textures have to fit in size.";
			}
			// reset texture settings
			ti0.textureFormat = format0;
			ti0.textureType = type0;
			ti1.textureFormat = format1;
			ti1.textureType = type1;
			if (wasReadable0 == false) {
				ti0.isReadable = false;
			}
			if (wasReadable1 == false) {
				ti1.isReadable = false;
			}
			AssetDatabase.ImportAsset( path0, ImportAssetOptions.ForceUpdate );
			AssetDatabase.ImportAsset( path1, ImportAssetOptions.ForceUpdate );
			Resources.UnloadUnusedAssets();
		}
		else {
			// no textures assigned to combined
			error = "You have to assign 2 textures.";
		}
		return error;
	}
	
	private void updateOrcreateTerrainMaterial(ref bool save) {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		Material tempMaterial = new Material( Shader.Find("Terrain/Terrain Colormap Ultra U4") );
		if (script.TerrainMaterial) {
			script.updateColormapMaterial();
			tempMaterial.CopyPropertiesFromMaterial(script.TerrainMaterial);
		}
		// save material
		if(save) {
			String filePath = EditorUtility.SaveFilePanelInProject
			(
				"Save Terrain Material",
				"terrainMaterial.mat", 
				"mat",
				"Choose a file location and name"
			);
			if (filePath!=""){
				AssetDatabase.DeleteAsset(filePath);
				AssetDatabase.CreateAsset(tempMaterial,filePath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			script.TerrainMaterial = tempMaterial;
		}
		Resources.UnloadUnusedAssets();
		tempMaterial = null;
	}

	private void updateOrcreateMeshBlendMaterial(ref bool save) {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		Terrain targetTerrain = (Terrain)script.GetComponent(typeof(Terrain));
		//
		Vector2 tileSize;
		// either create a new material
		if(save) {
			Material tempMaterial = new Material( Shader.Find("Custom/ColorMapUltra_MeshTerrainBlend Shader") );
			// set up terrain texture
			tempMaterial.SetTexture( "_TerrainSplat0", targetTerrain.terrainData.splatPrototypes[0].texture );
			tileSize = targetTerrain.terrainData.splatPrototypes[0].tileSize;
			tempMaterial.SetFloat( "_TerrainTex1Size", tileSize.x );
			tempMaterial.SetColor( "_TerrainTex1Color", script.ColTex1 );
			tempMaterial.SetFloat( "_TerrainSpec1", script.Spec1 );

			tempMaterial.SetTexture( "_TerrainSplat1", targetTerrain.terrainData.splatPrototypes[1].texture );
			tileSize = targetTerrain.terrainData.splatPrototypes[1].tileSize;
			tempMaterial.SetFloat( "_TerrainTex2Size", tileSize.x );
			tempMaterial.SetColor( "_TerrainTex2Color", script.ColTex2 );
			tempMaterial.SetFloat( "_TerrainSpec2", script.Spec2 );

			tempMaterial.SetTexture( "_TerrainSplat2", targetTerrain.terrainData.splatPrototypes[2].texture );
			tileSize = targetTerrain.terrainData.splatPrototypes[2].tileSize;
			tempMaterial.SetFloat( "_TerrainTex3Size", tileSize.x );
			tempMaterial.SetColor( "_TerrainTex3Color", script.ColTex3 );
			tempMaterial.SetFloat( "_TerrainSpec3", script.Spec3 );

			tempMaterial.SetTexture( "_TerrainSplat3", targetTerrain.terrainData.splatPrototypes[3].texture );
			tileSize = targetTerrain.terrainData.splatPrototypes[3].tileSize;
			tempMaterial.SetFloat( "_TerrainTex4Size", tileSize.x );
			tempMaterial.SetColor( "_TerrainTex4Color", script.ColTex4 );
			tempMaterial.SetFloat( "_TerrainSpec4", script.Spec4 );
			
			// set up combined normal maps
			tempMaterial.SetTexture( "_TerrainCombinedNormal12", script.CombinedNormal12 );
			tempMaterial.SetTexture( "_TerrainCombinedNormal34", script.CombinedNormal34 );

			// set up terrain values
			tempMaterial.SetFloat( "_TerrainSize", targetTerrain.terrainData.size.x);
			tempMaterial.SetVector( "_TerrainPos", targetTerrain.transform.position);
			tempMaterial.SetTexture( "_ColorMap", script.CustomColorMap );
			tempMaterial.SetTexture( "_TerrainNormalMap", script.TerrainNormalMap );
			
			tempMaterial.SetTexture( "_Control", script.SplatMap1 );
			
			tempMaterial.SetFloat( "_TMultiUV", script.MultiUv);
			tempMaterial.SetFloat( "_TDesMultiUvFac", script.DesMultiUvFac);
			tempMaterial.SetFloat( "_TSplattingDistance", script.SplattingDistance);
			tempMaterial.SetFloat( "_TDetailsFadeLenght", script.TerrainDetailsFadeLenght);
			tempMaterial.SetColor( "_TSpecularColor", script.SpecularColor );
			tempMaterial.SetFloat( "_TSpecPower", script.SpecPower );
			tempMaterial.SetFloat( "_TDetailNormalStrength", script.DetailNormalStrength );
			tempMaterial.SetFloat( "_TUpscaledNormalStrength", script.UpscaledNormalStrength );

			tempMaterial.SetVector( "_TerrainFresnel", new Vector4(script.FresnelIntensity, script.FresnelPower, script.FresnelBias, 0f ) );
			tempMaterial.SetColor( "_TerrainReflectionColor", script.ReflectionColor );
			
			tempMaterial.SetVector( "_TerrainElevation", new Vector4(script.Elev1, script.Elev2, script.Elev3, script.Elev4) );

			tempMaterial.SetVector( "_TParallax", new Vector4(script.P_Elevation_Tex3, script.P_Elevation_Tex4, script.P_GlossFactor_Tex3, script.P_GlossFactor_Tex4) );

			
			// save material
			String filePath = EditorUtility.SaveFilePanelInProject
			(
				"Save Material",
				"blendMaterial.mat", 
				"mat",
				"Choose a file location and name"
			);
			if (filePath!=""){
				AssetDatabase.DeleteAsset(filePath);
				AssetDatabase.CreateAsset(tempMaterial,filePath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				// add new material
				tempMaterial = (Material)AssetDatabase.LoadAssetAtPath(filePath, typeof(Material) );
				script.blendedMaterials.Add(tempMaterial);
			}
			tempMaterial=null; 
			//Resources.UnloadUnusedAssets();
		}
		// or update all assigned ones
		else {
			for (int i = 0; i < script.blendedMaterials.Count; i++) {
				if (script.blendedMaterials[i]) {
					script.blendedMaterials[i].SetTexture( "_TerrainSplat0", targetTerrain.terrainData.splatPrototypes[0].texture );
					tileSize = targetTerrain.terrainData.splatPrototypes[0].tileSize;
					script.blendedMaterials[i].SetFloat( "_TerrainTex1Size", tileSize.x );
					script.blendedMaterials[i].SetColor( "_TerrainTex1Color", script.ColTex1 );
					script.blendedMaterials[i].SetFloat( "_TerrainSpec1", script.Spec1 );

					script.blendedMaterials[i].SetTexture( "_TerrainSplat1", targetTerrain.terrainData.splatPrototypes[1].texture );
					tileSize = targetTerrain.terrainData.splatPrototypes[1].tileSize;
					script.blendedMaterials[i].SetFloat( "_TerrainTex2Size", tileSize.x );
					script.blendedMaterials[i].SetColor( "_TerrainTex2Color", script.ColTex2 );
					script.blendedMaterials[i].SetFloat( "_TerrainSpec2", script.Spec2 );

					script.blendedMaterials[i].SetTexture( "_TerrainSplat2", targetTerrain.terrainData.splatPrototypes[2].texture );
					tileSize = targetTerrain.terrainData.splatPrototypes[2].tileSize;
					script.blendedMaterials[i].SetFloat( "_TerrainTex3Size", tileSize.x );
					script.blendedMaterials[i].SetColor( "_TerrainTex3Color", script.ColTex3 );
					script.blendedMaterials[i].SetFloat( "_TerrainSpec3", script.Spec3 );

					script.blendedMaterials[i].SetTexture( "_TerrainSplat3", targetTerrain.terrainData.splatPrototypes[3].texture );
					tileSize = targetTerrain.terrainData.splatPrototypes[3].tileSize;
					script.blendedMaterials[i].SetFloat( "_TerrainTex4Size", tileSize.x );
					script.blendedMaterials[i].SetColor( "_TerrainTex4Color", script.ColTex4 );
					script.blendedMaterials[i].SetFloat( "_TerrainSpec4", script.Spec4 );
					
					// set up combined normal maps
					script.blendedMaterials[i].SetTexture( "_TerrainCombinedNormal12", script.CombinedNormal12 );
					script.blendedMaterials[i].SetTexture( "_TerrainCombinedNormal34", script.CombinedNormal34 );

					// set up terrain values
					script.blendedMaterials[i].SetFloat( "_TerrainSize", targetTerrain.terrainData.size.x);
					script.blendedMaterials[i].SetVector( "_TerrainPos", targetTerrain.transform.position);
					script.blendedMaterials[i].SetTexture( "_ColorMap", script.CustomColorMap);
					script.blendedMaterials[i].SetTexture( "_TerrainNormalMap", script.TerrainNormalMap);

					script.blendedMaterials[i].SetTexture( "_Control", script.SplatMap1 );
					
					script.blendedMaterials[i].SetFloat( "_TMultiUV", script.MultiUv);
					script.blendedMaterials[i].SetFloat( "_TDesMultiUvFac", script.DesMultiUvFac);
					script.blendedMaterials[i].SetFloat( "_TSplattingDistance", script.SplattingDistance);
					script.blendedMaterials[i].SetFloat( "_TDetailsFadeLenght", script.TerrainDetailsFadeLenght);
					script.blendedMaterials[i].SetColor( "_TSpecularColor", script.SpecularColor );
					script.blendedMaterials[i].SetFloat( "_TSpecPower", script.SpecPower );
					script.blendedMaterials[i].SetFloat( "_TDetailNormalStrength", script.DetailNormalStrength );
					script.blendedMaterials[i].SetFloat( "_TUpscaledNormalStrength", script.UpscaledNormalStrength );
				
					script.blendedMaterials[i].SetVector( "_TerrainFresnel", new Vector4(script.FresnelIntensity, script.FresnelPower, script.FresnelBias, 0f ) );
					script.blendedMaterials[i].SetColor( "_TerrainReflectionColor", script.ReflectionColor );
					
					script.blendedMaterials[i].SetVector( "_TerrainElevation", new Vector4(script.Elev1, script.Elev2, script.Elev3, script.Elev4) );

					script.blendedMaterials[i].SetVector( "_TParallax", new Vector4(script.P_Elevation_Tex3, script.P_Elevation_Tex4, script.P_GlossFactor_Tex3, script.P_GlossFactor_Tex4) );

				}
			}

		}

		Resources.UnloadUnusedAssets();
	}
	
	private void getAverageColor(Texture2D sourceTex) {
		if (sourceTex) {
			// read from lowest mipmaplevel
			int mip = sourceTex.mipmapCount - 1;
			// is array
			Color[] aColor = sourceTex.GetPixels(0, 0, 1, 1, mip);
			avrgCol = aColor[0];
			avrgCol.a = 0.0f;
		}
		else {	
			avrgCol = Color.black;
			avrgCol.a = 0.0f;
			Debug.Log("No Texture assigned yet.");
		}
	}

	//private void applyCustomSplatMap(ref Texture2D customSplatmap, Terrain targetTerrain) {
	private void applyCustomSplatMap(Texture2D customSplatmap, Terrain targetTerrain) {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;

		int everythingChecked = 0;
		string path = AssetDatabase.GetAssetPath(script.customSplatMap1);
		int w = script.customSplatMap1.width;
		
		// check both input textures
		for (int i = 0; i < 2; i++) {
			TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(path);
			// right size?
			if (customSplatmap.height != w) {
				EditorUtility.DisplayDialog("Wrong size", "Width and height of all splat maps must be the same", "Cancel"); 
				return;
			}
			// right format?
			if (customSplatmap.format != TextureFormat.ARGB32) {
				EditorUtility.DisplayDialog(
					"Wrong format",
					"Splat map " + (i+1) + " must be in ARGB 32 bit format. Please fix this in the import settings.",
					"Cancel"); 
				return;
			}
			// readable?
			if (ti.isReadable == false) {
				ti.isReadable = true;
				// refresh texture
				AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate ); 	
			}
			everythingChecked++;
			// do only a second check if needed
			if (script.customSplatMap2 != null && targetTerrain.terrainData.alphamapLayers > 7) {
				path = AssetDatabase.GetAssetPath(script.customSplatMap2);
				customSplatmap = script.customSplatMap2;
			}
			else {
				everythingChecked++;
				break;
			}
		}
		
		///
		if (everythingChecked < 2) {
			return;
		}
		else {
			// apply splatmap
			if ( EditorUtility.DisplayDialog(
					"Apply Splat Map(s)",
					"Do you really want to apply the custom splat map(s)? This will overwrite your current one(s).",
					"Apply",
					"Cancel")
				) {
				targetTerrain.terrainData.alphamapResolution = w;
				//int w = targetTerrain.terrainData.alphamapResolution;
				float[,,] splatmapData = targetTerrain.terrainData.GetAlphamaps(0, 0, w, w);
				Color[] mapColors = script.customSplatMap1.GetPixels();
				
				if ( script.customSplatMap1 != null ) {
					if (splatmapData.Length < mapColors.Length*4) {
						EditorUtility.DisplayDialog("Add textures", "The terrain must have at least 4 textures and" +
							"/or the size of your splat map has to fit the size you have choosen in the terrain set up.", "Cancel"); 
						return;	
					}
					else {
						for (int z = 0; z < 4; z++) {
							for (int y = 0; y < w; y++) {
								for (int x = 0; x < w; x++) {
									splatmapData[x,y,z] = mapColors[((w-1)-x)*w + y][z];
								}
							}
						}
						if (targetTerrain.terrainData.alphamapLayers > 7 && script.customSplatMap2 != null) {
							
							Color[] mapColors1 = script.customSplatMap2.GetPixels();
							
							if (targetTerrain.terrainData.alphamapLayers > 4) {
								// apply also second splat map
								for (int z = 4; z < 8; z++) {
									for (int y = 0; y < w; y++) {
										for (int x = 0; x < w; x++) {
											splatmapData[x,y,z] = mapColors1[((w-1)-x)*w + y][z-4];
										}
									}
								}
							}
						}
						targetTerrain.terrainData.SetAlphamaps(0, 0, splatmapData);
					}
				}
			}
			Resources.UnloadUnusedAssets();
		}
	}

	private void readFromTerrainTexture(ref Texture2D terrainTex) {
		wasReadable = false;
		string path = AssetDatabase.GetAssetPath(terrainTex);
		TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(path);
		if (ti.isReadable == true) {
			wasReadable = true;
		}
		else {
			ti.isReadable = true;
			// refresh texture
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate ); 	
		}
		getAverageColor(terrainTex);

		// reset texture settings
		if (wasReadable == false) {
			ti.isReadable = false;
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate ); 
		}
	}
	public void grabTerrainSplatMaps() {
		CustomTerrainScriptColorMapUltraU4 script = (CustomTerrainScriptColorMapUltraU4)target;
		Terrain targetTerrain = (Terrain)script.GetComponent(typeof(Terrain));
		Type terrainDataType = targetTerrain.terrainData.GetType();
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		PropertyInfo PropInfo = terrainDataType.GetProperty("alphamapTextures", flags);
		if (PropInfo != null) {
			Texture2D[] alphamapTextures = (Texture2D[])PropInfo.GetValue(targetTerrain.terrainData, null);
			if (alphamapTextures.Length > 0) script.SplatMap1 = alphamapTextures[0];
			if (alphamapTextures.Length > 1) script.SplatMap2 = alphamapTextures[1];
		}
	}


}
#endif