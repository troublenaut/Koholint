#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class MeshTerrainBlendingTool : MonoBehaviour {

	public bool Painting = false;
	public bool hideWireframe = false;
	public bool bakeForStaticBatching = false;
	static Transform currentSelection;
	static Mesh currentMesh;
	static Collider currentCollider;
	static Mesh currentColliderMesh;
	
	public string oldFileName;

	static Color currentColor;
	public float BrushSize = 4.0f;
	public float Opacity = 0.5f;
	public float Strength = 0.5f;
	static SceneView.OnSceneFunc onSceneGUIFunc;
	
	public bool	TextureBlend = true;
	public bool	NormalBlend = true;
	public string vertexPainterMessage = "";
	public string vertexPainterMessage1 = "";


	void OnSceneGUI(SceneView sceneview)
	{
		Event current = Event.current;
		// enable rotation
		if (current.alt) return;
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
					EditorUtility.SetDirty(currentMesh);
					vertexPainterMessage1 = "Do not forget to bake the terrain normals and save the Mesh.";
					Paint();
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
				if(Painting == true) {
					EditorUtility.SetDirty(currentMesh);
					vertexPainterMessage1 = "Do not forget to bake the terrain normals and save the Mesh.";
					Paint();
					current.Use();
				}
				break;
		}
	}
	
	public void StartPainting () {
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		currentSelection = GetComponent<Transform>();
		currentCollider = GetComponent<Collider>();
		if (currentCollider) {
			currentColliderMesh = GetComponent<MeshCollider>().sharedMesh;
		}
		if (currentColliderMesh != null ) {
			vertexPainterMessage = "";
			onSceneGUIFunc = new SceneView.OnSceneFunc(OnSceneGUI);
			SceneView.onSceneGUIDelegate += onSceneGUIFunc;
			Painting = true;
			if (hideWireframe) {
				EditorUtility.SetSelectedWireframeHidden(currentSelection.renderer, true);
			}
		}
		else {
			StopPainting ();
			vertexPainterMessage = "You have to add a Mesh Collider.";
		}
	}
	
	public void StopPainting () {
		if (Painting == true && hideWireframe == true) {
			EditorUtility.SetSelectedWireframeHidden(currentSelection.renderer, false);
		}
		SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
		Painting = false;
	}
	
	void Paint() {
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (currentCollider.Raycast(ray, out hit, float.MaxValue)) {
			Vector3[] vertices = currentMesh.vertices;
			Color[] colors  = currentMesh.colors;
			Vector3 brushPosition = currentSelection.InverseTransformPoint(hit.point);
			for (int i=0; i < vertices.Length; i++)
			{
				float distanceToBrushCenter = (vertices[i] - brushPosition).magnitude;
				if (distanceToBrushCenter < BrushSize) {
					if (TextureBlend == true) {
						colors[i].r = Mathf.Lerp(colors[i].r, Strength, Opacity);
					}
					if (NormalBlend == true) {
						colors[i].g = Mathf.Lerp(colors[i].g, Strength, Opacity);
					}
				}
			}
			currentMesh.colors = colors;
		} 
	}

	void DrawBrush() {
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (currentCollider.Raycast(ray, out hit, float.MaxValue)) {
			Handles.DrawWireDisc(hit.point, hit.normal, BrushSize);
		}
	}
	
	public void BakeTerrainNormals2Mesh () {
		StopPainting ();
		currentMesh = GetComponent<MeshFilter>().sharedMesh;
		currentSelection = GetComponent<Transform>();
		Vector3[] vertices = currentMesh.vertices;
		Color[] colors = currentMesh.colors;
		
		// create vertex color in case there are no
		if (colors.Length == 0) {
			colors = new Color[vertices.Length];
		}
		
		
		Terrain myterrain = Terrain.activeTerrain;
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 worldPosition = currentSelection.TransformPoint(vertices[i]);
			// sample terrain normal
			Vector3 terrainPos = (worldPosition - myterrain.transform.position) / myterrain.terrainData.size.x;
			Vector4 terrainNormal = myterrain.terrainData.GetInterpolatedNormal(terrainPos.x, terrainPos.z);
			
			if(!bakeForStaticBatching) {
				terrainNormal = currentSelection.InverseTransformDirection(terrainNormal);
			}
			// encode normal vector to color
			colors[i].b = terrainNormal.x * 0.5f + 0.5f;
			colors[i].a = terrainNormal.z * 0.5f + 0.5f;
		}
		//// update mesh
		currentMesh.colors = colors;
		///// create a new mesh    
		Mesh newMesh = new Mesh();
		newMesh.vertices = currentMesh.vertices;
		newMesh.colors = currentMesh.colors;
		newMesh.uv = currentMesh.uv;
		newMesh.uv2 = currentMesh.uv2;
		newMesh.normals = currentMesh.normals;
		newMesh.tangents = currentMesh.tangents;
		newMesh.triangles = currentMesh.triangles;
		
		///// save newMesh
		string filePath;
		if (oldFileName == "") {
			oldFileName = "blendedMesh.asset";
		}
		//else {
			filePath = EditorUtility.SaveFilePanelInProject
			(
				"Save blended Mesh",
				oldFileName, 
				"asset",
				"Choose a file location and name"
			);
		//}
		if (filePath!=""){
			UnityEditor.AssetDatabase.DeleteAsset(filePath);
			UnityEditor.AssetDatabase.CreateAsset(newMesh,filePath);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			oldFileName = Path.GetFileName(filePath);
		}
		
		///// assign newMesh
		currentSelection.GetComponent<MeshFilter>().sharedMesh = newMesh;
		if (currentSelection.GetComponent<MeshCollider>()) {
			currentSelection.GetComponent<MeshCollider>().sharedMesh = newMesh;
		}
		vertexPainterMessage1 = "";
	}
}
#endif
