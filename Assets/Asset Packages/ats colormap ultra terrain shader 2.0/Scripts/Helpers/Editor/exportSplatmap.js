// For saving splat map as PNG file. 
import System.IO; 

@MenuItem("Terrain/Export Splatmap") 
static function Apply () { 
var texture : Texture2D = Selection.activeObject as Texture2D; 
if (texture == null) 
{ 
EditorUtility.DisplayDialog("Select Splat Map", "You Must Select a Splat Map first!\nGo to the project tab, find your terrain and open it's foldout. Then select either SplatAlpha0 or SplatAlpha1.", "Ok"); 
return; 
} 
var bytes = texture.EncodeToPNG(); 
File.WriteAllBytes(Application.dataPath + "/exported_texture.png", bytes); 
} 