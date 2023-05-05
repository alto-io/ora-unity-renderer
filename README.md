# ora-unity-renderer

## Usage
  - Copy [Assets/ORARenderer](Assets/ORARenderer/) into your Unity Assets folder
  - Move all .ora files into your 'Assets/StreamingAssets' folder.
    - Make sure to remove StreamingAssets from your .gitignore
  - Add an instance of the [ORA Manager.prefab](Assets/ORARenderer/ORA&#x20;Manager.prefab) to your Main Scene. This game object will carry over between scenes
  - Use the [Arcadian.prefab](Assets/ORARenderer/Arcadian.prefab) and [ArcadianV2.prefab](Assets/ORARenderer/Arcadian&#x20;.prefab) (depending on your need) as the base game object for loading Arcadians
  - Reference the [ArcadianRenderer](Assets/ORARenderer/Scripts/ArcadianRenderer.cs) component and call `RequestParts()` from the component already added to the Arcadian prefabs
    - [ArcadianLoadRequest](Assets/ORARenderer/Scripts/ArcadianPartData.cs) must be created and based off of downloaded metadata. See [SampleScene](Assets/Scenes/SampleScene.unity) and [SampleLoadArcadian.cs](Assets/Scripts/SampleLoadArcadian.cs)

## Modification
  - ORA files can be added/removed/changed. Make sure the file names in the ORAManager are updated. Include subdirectories if you use them.
    - ie. if `arcadian.ora` is in `Assets/StreamingAssets/ORA`, then add `ORA/arcadian.ora` to ORAManager's file names
  - The ArcadianRenderer component has a list of material location names that must be updated if the fbx has new materials. Make sure these location names are 1 to 1 with the materials on the fbx, and in the same order. Use the same names as you would expect from the metadata
