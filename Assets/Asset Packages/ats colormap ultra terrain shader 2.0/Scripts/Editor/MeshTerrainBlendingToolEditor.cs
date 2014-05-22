#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor (typeof(MeshTerrainBlendingTool))]
public class MeshTerrainBlendingToolEditor : Editor {
	
	//private bool painting = false;
	
	public override void OnInspectorGUI () {
		MeshTerrainBlendingTool script = (MeshTerrainBlendingTool)target;
		
		EditorGUILayout.BeginVertical("Box");
		GUILayout.Label ("Bake Terrain Normals into the Mesh and save it","BoldLabel");
		GUILayout.Space(10);
		if (GUILayout.Button("Bake Terrain Normals and save Mesh")) {
					script.BakeTerrainNormals2Mesh();
		}
		script.bakeForStaticBatching = EditorGUILayout.Toggle("Bake for static batching", script.bakeForStaticBatching);
		
		
		if (script.vertexPainterMessage1 != "") {
			EditorGUILayout.HelpBox(script.vertexPainterMessage1, MessageType.Warning, true);
		}
		GUILayout.Space(10);
		GUILayout.Label ("Paint Vertex Colors","BoldLabel");
		GUILayout.Space(5);
		script.TextureBlend = EditorGUILayout.Toggle("Paint Texture Blend", script.TextureBlend);
		script.NormalBlend = EditorGUILayout.Toggle("Paint Normal Blend", script.NormalBlend);
		GUILayout.Space(10);
		script.BrushSize = EditorGUILayout.Slider("Brush Size", script.BrushSize, 0.1f, 10.0f);  
		script.Opacity = EditorGUILayout.Slider("Opacity", script.Opacity, 0.1f, 1.0f); 
		script.Strength = EditorGUILayout.Slider("Target Strength", script.Strength, 0.0f, 1.0f); 
		GUILayout.Space(10);
		script.hideWireframe = EditorGUILayout.Toggle("Hide Wireframe", script.hideWireframe);
		GUILayout.Space(10);
		if (script.Painting == false) {
			if (GUILayout.Button("Start Painting")) {
					script.StartPainting();
				}
		}
		else {
			GUI.color = Color.red;
			if (GUILayout.Button("Stop Painting")) {
					script.StopPainting();
				}	
		}
		GUI.color = Color.white;
		if (script.vertexPainterMessage !="") {
			EditorGUILayout.HelpBox(script.vertexPainterMessage, MessageType.Error, true);
		}
		EditorGUILayout.EndVertical();
		
	}
	
	// if the editor looses focus
	void OnDisable() {
		MeshTerrainBlendingTool script = (MeshTerrainBlendingTool)target;
		script.StopPainting();
	}
		
}
#endif