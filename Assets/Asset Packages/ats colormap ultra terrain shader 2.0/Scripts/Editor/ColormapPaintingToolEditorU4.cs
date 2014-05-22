#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.IO;


[CustomEditor (typeof(ColormapPaintingToolU4))]
public class ColormapPaintingToolEditorU4 : Editor {
	
	//private bool painting = false;
	
	public override void OnInspectorGUI () {
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		
		EditorGUILayout.BeginVertical("Box");
		GUI.color = Color.yellow;
		GUILayout.Label ("Color Map Painting Tool","BoldLabel");
		GUI.color = Color.white;
		GUILayout.Space(10);
		
		if(script.newColormap == null) {
			EditorGUILayout.HelpBox("Before you can start painting onto the color map " +
				"you have to assign a readenabled RGB24bit Color Map Texture " +
				"in png-format to the slot at the bottom." +
				"\n\nAlternatively you can let the editor copy the existing " +
				"color map currently assigned to the terrain " +
				"and create a new file to paint on. " +
				"\nSimply hit 'Start Painting Terrain' to do so." +
				"\n\nThen drag the new texture to the slot at the bottom.", MessageType.Warning, true);
		}
		
		// Painting mode
		else {
			script.BrushSize = EditorGUILayout.IntSlider("Brush Size", Mathf.FloorToInt(script.BrushSize), 1, 10);  
			script.Opacity = EditorGUILayout.Slider("Opacity", script.Opacity, 0.1f, 1.0f); 
			script.Hardness = EditorGUILayout.Slider("Hardness", script.Hardness, 0.1f, 5.0f); 
			GUILayout.Space(10);
			
			script.terrainPaintingColor = EditorGUILayout.ColorField("Color", script.terrainPaintingColor);
			GUILayout.Space(5);
			EditorGUILayout.HelpBox("While painting simply hold 'shift' and click in order " +
				"to pick up a color directely from the color map.", MessageType.Info, true);
		}
		
		GUILayout.Space(10);
		if (script.Painting == false) {
			if (GUILayout.Button("Start Painting", GUILayout.Height(36))) {
					if(script.newColormap != null) {
						script.StartPaintingTerrain();
					}
					else {
						CreateNewColorMap();
						script.StartPaintingTerrain();
					}
			}
		}
		else {
			GUI.color = Color.red;
			if (GUILayout.Button("Stop Painting", GUILayout.Height(36))) {
					script.StopPaintingTerrain();
				}
			GUI.color = Color.white;
			EditorGUILayout.HelpBox("If the paint brush does not show up in the scene view tab, " +
				"just hit start and stop again.", MessageType.Warning, true);
		}
		
		GUILayout.Space(15);
		EditorGUILayout.BeginHorizontal();
		script.newColormap = (Texture2D)EditorGUILayout.ObjectField(script.newColormap, typeof(Texture2D), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
		if(script.newColormap != null) {
			EditorGUILayout.BeginVertical();
			if (GUILayout.Button("Save Color Map")) {
					SaveColorMap();
			}
			EditorGUILayout.HelpBox("Do not forget to save your changes to the color map. ", MessageType.Warning, true);
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(5);
		EditorGUILayout.EndVertical();
		
		if (GUI.changed) {
			if(script.newColormap != null) {
				script.SendtoTerrain();	
			}
		}
	}
	// if the editor looses focus
	void OnDisable() {
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		script.StopPaintingTerrain();
	}
	void OnEnable() {
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		script.InitPaintingTerrain();
	}
	
	void CreateNewColorMap() {
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		script.CopyColorMap();
		SaveNewColorMap();
	}
	
	void SaveColorMap() {
		// save Colormap
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		string filePath = EditorUtility.SaveFilePanelInProject
				(
				"Save New Colormap",
				"colormap.png", 
				"png",
				"Choose a file location and name"
				);
			if (filePath!=""){
				byte[] bytes = script.newColormap.EncodeToPNG();
				System.IO.File.WriteAllBytes(filePath, bytes);
			
				AssetDatabase.Refresh();
				TextureImporter ti = AssetImporter.GetAtPath(filePath) as TextureImporter;
				ti.textureFormat = TextureImporterFormat.RGB24; 
				ti.isReadable = true;
				AssetDatabase.ImportAsset(filePath);
				AssetDatabase.Refresh();
			}
	}
	
	void SaveNewColorMap() {
		ColormapPaintingToolU4 script = (ColormapPaintingToolU4)target;
		string filePath = EditorUtility.SaveFilePanelInProject
				(
				"Save New Colormap",
				"colormap.png", 
				"png",
				"Choose a file location and name"
				);
			if (filePath!=""){
				byte[] bytes = script.tempColormap.EncodeToPNG();
				System.IO.File.WriteAllBytes(filePath, bytes);
			
				AssetDatabase.Refresh();
				TextureImporter ti = AssetImporter.GetAtPath(filePath) as TextureImporter;
				ti.textureFormat = TextureImporterFormat.RGB24; 
				ti.isReadable = true;
				AssetDatabase.ImportAsset(filePath);
				AssetDatabase.Refresh();
			
				// assign new color map
				script.newColormap = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D) );
				script.AssignNewColormap();
			}
	}
}
#endif
