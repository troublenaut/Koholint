#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using UnityEditor;
using System.IO;

public class ColormapPaintingToolU4 : MonoBehaviour {

	public bool Painting = false;
	public bool Grabbing = false;
	
	static Terrain currentTerrain;
	
	static int colormapWidth;
	static int colormapHeight;
	
	public Color terrainPaintingColor;
	
	public Texture2D tempColormap;
	public Texture2D newColormap = null;
	
	static Collider currentCollider;
	static Mesh currentColliderMesh;
	
	public string oldFileName;

	static Color currentColor;
	public float BrushSize = 4.0f;
	public float Hardness = .5f;
	static float BrushSizeFactor = 1.0f;
	public float Opacity = 0.5f;

	static Vector3 paintTarget;
	
	static SceneView.OnSceneFunc onSceneGUIFunc;
	
	public string TerrainPainterMessage = "";
	public string TerrainPainterMessage1 = "";


	void OnSceneGUI(SceneView sceneview)
	{
		Event current = Event.current;
		// enable rotation
		if (current.alt) return;
		
		if (current.shift) {
			Grabbing = true;
		}
		else {
			Grabbing = false;
		}
		
		switch(current.type){
			case EventType.layout:
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				break;
			// draw custom Brush
			case EventType.repaint:
				if (Painting == true) {
					DrawBrush();
				}
				break;
			//redraw Brush on mouseMove
			case EventType.mouseMove:
				HandleUtility.Repaint();
				break;
			// start painting
			case EventType.mouseDrag:
				if(Painting == true) {
					PaintTerrain();
				}
				else {
					break;
				}
				break;
			case EventType.mouseUp:
				if(Painting == true) {
					break;
				}
				break;
			case EventType.mouseDown:
				if(Painting == true && Grabbing == false) {
					//EditorUtility.SetDirty(currentMesh);
					PaintTerrain();
					current.Use();
				}
				else if (Grabbing == true) {
					GetColor();
					current.Use();
				}
				break;
		}
	}
	
	public void StartPaintingTerrain () {
		currentTerrain = (Terrain) GetComponent(typeof(Terrain));
		// BUG: We have to force an update to make it work after play etc.
		//set up
		BrushSizeFactor = currentTerrain.terrainData.size.x / colormapWidth;
		onSceneGUIFunc = new SceneView.OnSceneFunc(OnSceneGUI);
		SceneView.onSceneGUIDelegate += onSceneGUIFunc;
		// get params
		colormapWidth = newColormap.width;
		colormapHeight = newColormap.height;
		//start
		Painting = true;
	}
	
	public void StopPaintingTerrain () {
		SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
		Painting = false;
	}
	
	public void InitPaintingTerrain() {
		//Debug.Log ("init");	
	}
	
	public void SendtoTerrain() {
		CustomTerrainScriptColorMapUltraU4 terrainscript = GetComponent<CustomTerrainScriptColorMapUltraU4>();
		if (newColormap != null) {
			//assign new color map to the terrain
			terrainscript.CustomColorMap = newColormap;
			terrainscript.updateColormapMaterial();
		}
	}
	
	void PaintTerrain() {
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue)) {
			// Vector3 brushPosition = currentSelection.InverseTransformPoint(hit.point);
			// colormap resolution
			float posX = colormapWidth / currentTerrain.terrainData.size.x * paintTarget.x;
			float posY = colormapHeight / currentTerrain.terrainData.size.z * paintTarget.z;

			// must be rgb24
			//Color oldColor = newColormap.GetPixel(Mathf.FloorToInt(posX),Mathf.FloorToInt(posY));
			//Color finalColor = Color.Lerp(oldColor, terrainPaintingColor, Opacity);
			
			Color[] oldColor = new Color[Mathf.FloorToInt(BrushSize*BrushSize)];
			Color[] finalColor = new Color[Mathf.FloorToInt(BrushSize*BrushSize)];
			int index = 0;
			int x = 0;
			int y = 0;
			for (x = 0; x < BrushSize; x++) {
				for (y = 0; y < BrushSize; y++) {
					float Distance = 1.0f;
					if (BrushSize ==1) {
						oldColor[index] = newColormap.GetPixel(Mathf.CeilToInt(posX + (x - BrushSize*0.5f ) ), Mathf.CeilToInt(posY + (y - BrushSize*0.5f )));
					}
					else {
						oldColor[index] = newColormap.GetPixel(Mathf.FloorToInt(posX + (x - BrushSize*0.5f ) ), Mathf.FloorToInt(posY + (y - BrushSize*0.5f )));
						Distance = ( new Vector2( (x - BrushSize*0.5f ), (y - BrushSize*0.5f )) ).magnitude;
						Distance = GaussianFalloff(Distance, BrushSize) * Hardness;
					}
					
					finalColor[index] = Color.Lerp(oldColor[index], terrainPaintingColor, Opacity * Distance );
					index++;
				}
			}
			index = 0;
			for (x = 0; x < BrushSize; x++) {
				for (y = 0; y < BrushSize; y++) {
					if (BrushSize ==1) {
						newColormap.SetPixel(Mathf.CeilToInt(posX + (x - BrushSize*0.5f ) ), Mathf.CeilToInt(posY + (y - BrushSize*0.5f ) ), finalColor[index]);
					}
					else {
						newColormap.SetPixel(Mathf.FloorToInt(posX + (x - BrushSize*0.5f ) ), Mathf.FloorToInt(posY + (y - BrushSize*0.5f ) ), finalColor[index]);
					}
					index++;
				}
			} 
			//
			//approx. center of brush
			//newColormap.SetPixel(Mathf.FloorToInt(posX),Mathf.FloorToInt(posY), Color.white);
			newColormap.Apply();
		} 
	}
	
	void GetColor() {
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue)) {
			float posX = colormapWidth / currentTerrain.terrainData.size.x * paintTarget.x;
			float posY = colormapHeight / currentTerrain.terrainData.size.z * paintTarget.z;
			terrainPaintingColor = newColormap.GetPixel(Mathf.FloorToInt(posX),Mathf.FloorToInt(posY));
		}
	}

	void DrawBrush() {
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue)) {
			// normalize
			paintTarget = hit.point - transform.position;
			paintTarget.x = Mathf.Round(paintTarget.x / BrushSizeFactor) * BrushSizeFactor + (BrushSize * BrushSizeFactor * 0.25f);
			paintTarget.z = Mathf.Round(paintTarget.z / BrushSizeFactor) * BrushSizeFactor + (BrushSize * BrushSizeFactor * 0.25f);
			if (Grabbing==true) {
				Handles.color = Color.red;
				Handles.DrawWireDisc(paintTarget, hit.normal, BrushSizeFactor * 0.5f);
			}
			else {
				Handles.color = Color.white;
				Handles.DrawWireDisc(paintTarget, hit.normal, BrushSize * BrushSizeFactor * 0.5f);
			}
		}
	}
		
	static float GaussianFalloff (float Distance, float Radius) {
		return Mathf.Clamp01(Mathf.Pow(360.0f, -Mathf.Pow (Distance / Radius, 2.5f) - 0.01f));
	}
	
	public void CopyColorMap() {
		CustomTerrainScriptColorMapUltraU4 terrainscript = GetComponent<CustomTerrainScriptColorMapUltraU4>();
		currentTerrain = (Terrain) GetComponent(typeof(Terrain));
		
		TerrainPainterMessage = "Save a new instance of your Color map.";	

		// check custom colormap
		string path = AssetDatabase.GetAssetPath(terrainscript.CustomColorMap);
		TextureImporter ti0 = (TextureImporter) TextureImporter.GetAtPath(path);
			
		bool wasReadable = ti0.isReadable;
		TextureImporterFormat wasFormat = ti0.textureFormat;
		//print (wasFormat);
		
		if (ti0.isReadable == false || wasFormat != TextureImporterFormat.RGB24 ) {
			wasReadable = ti0.isReadable;
			ti0.isReadable = true;
			ti0.textureFormat = TextureImporterFormat.RGB24;	
			// refresh texture
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate ); 
		}
		
		// get params
		colormapWidth = terrainscript.CustomColorMap.width;
		colormapHeight = terrainscript.CustomColorMap.height;
		
		// copy color map
		tempColormap = new Texture2D(terrainscript.CustomColorMap.width, terrainscript.CustomColorMap.height);
		Color[] pix = terrainscript.CustomColorMap.GetPixels(0, 0, tempColormap.width, tempColormap.height);
		tempColormap.SetPixels(pix);
		tempColormap.Apply();
		// clean up
		ti0.isReadable = wasReadable;
		ti0.textureFormat = wasFormat;
		// refresh texture
		AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
	}
	
	public void AssignNewColormap() {
		CustomTerrainScriptColorMapUltraU4 terrainscript = GetComponent<CustomTerrainScriptColorMapUltraU4>();
		//assign new color map to the terrain
		terrainscript.CustomColorMap = newColormap;
		Debug.Log ("New Color Map created and assigned.");	
		DestroyImmediate(tempColormap, true);
	}
}
#endif
