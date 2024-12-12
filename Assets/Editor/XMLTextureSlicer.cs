#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SplitSpritesFromXML : EditorWindow
{
    private Texture2D texture;
    private TextAsset xmlFile;

    [MenuItem("Tools/Sprite Splitter")]
    public static void ShowWindow()
    {
        GetWindow<SplitSpritesFromXML>("Sprite Splitter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select PNG and XML File", EditorStyles.boldLabel);

        texture = (Texture2D)EditorGUILayout.ObjectField("Texture (PNG)", texture, typeof(Texture2D), false);
        xmlFile = (TextAsset)EditorGUILayout.ObjectField("XML File", xmlFile, typeof(TextAsset), false);

        if (GUILayout.Button("Split Sprites"))
        {
            if (texture == null || xmlFile == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select both a PNG and an XML file.", "OK");
                return;
            }

            SplitSprites();
        }
    }

    private void SplitSprites()
    {
        string texturePath = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

        if (importer == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to load texture importer.", "OK");
            return;
        }

        importer.spriteImportMode = SpriteImportMode.Multiple;

        // 使用 SpriteDataProviderFactory 獲取 ISpriteEditorDataProvider
        var dataProvider = new SpriteDataProviderFactories().GetSpriteEditorDataProviderFromObject(importer) as ISpriteEditorDataProvider;
        if (dataProvider == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to access ISpriteEditorDataProvider. Ensure the texture is set to Multiple mode.", "OK");
            return;
        }

        dataProvider.InitSpriteEditorDataProvider();

        // Parse XML and set SpriteRects
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlFile.text);

        XmlNodeList subTextures = xmlDocument.SelectNodes("/TextureAtlas/SubTexture");
        List<SpriteRect> spriteRects = new List<SpriteRect>();

        foreach (XmlNode subTexture in subTextures)
        {
            string name = subTexture.Attributes["name"].Value;
            int x = int.Parse(subTexture.Attributes["x"].Value);
            int y = int.Parse(subTexture.Attributes["y"].Value);
            int width = int.Parse(subTexture.Attributes["width"].Value);
            int height = int.Parse(subTexture.Attributes["height"].Value);

            SpriteRect rect = new SpriteRect
            {
                name = Path.GetFileNameWithoutExtension(name),
                rect = new Rect(x, texture.height - y - height, width, height), // Flip Y-axis
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f)
            };
            spriteRects.Add(rect);
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        dataProvider.Apply();

        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        EditorUtility.DisplayDialog("Success", "Sprites have been successfully split!", "OK");
    }
}
#endif