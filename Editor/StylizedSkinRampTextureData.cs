using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    public class StylizedSkinRampTextureData
    {
        public Gradient surface;
        public Gradient subSurface;
        public int size ;
        
        public StylizedSkinRampTextureData(Gradient surface, Gradient subSurface, int size)
        {
            this.surface = surface;
            this.subSurface = subSurface;
            this.size = size;
        }
        
        public StylizedSkinRampTextureData(StylizedSkinRampTextureEditorObject editor)
        {
            this.surface = editor.surface;
            this.subSurface = editor.subSurface;
            this.size = editor.size;
        }

        public StylizedSkinRampTextureData() : this(
            new Gradient() {
                colorKeys = new[] {
                    new GradientColorKey(
                        new Color(
                            0.6449999809265137f, 
                            0.5957291722297669f, 
                            0.5464583039283752f, 
                            1f), 
                        0.259f),
                    new GradientColorKey(
                        new Color(
                            0.7843137383460999f, 
                            0.6894117593765259f, 
                            0.7178823947906494f, 
                            1f), 
                        0.72f),
                    new GradientColorKey(
                        new Color(
                            1f, 
                            0.9875923991203308f, 
                            0.9858490824699402f, 
                            1f), 
                        0.853f),
                }
            },
            new Gradient() {
                colorKeys = new[] {
                    new GradientColorKey(
                        new Color(
                            0.615686297416687f, 
                            0.4759255051612854f, 
                            0.4836900234222412f, 
                            1f),
                        0f),
                    new GradientColorKey(
                        new Color(
                            0.6698113083839417f, 
                            0.5077367424964905f, 
                            0.4834015667438507f, 
                            1f),
                        0.249f),
                    new GradientColorKey(
                        new Color(
                            0.9528301954269409f, 
                            0.5554712414741516f, 
                            0.43596476316452029f, 
                            1f),
                        0.409f),
                    new GradientColorKey(
                        new Color(
                            0.9811320900917053f, 
                            0.5990379452705383f, 
                            0.48593804240226748f, 
                            1f),
                        0.547f),
                    new GradientColorKey(
                        new Color(
                            1.0f, 
                            0.8999326229095459f, 
                            0.8443396091461182f, 
                            1f),
                        0.81f),
                } 
            }, 
            128)
        {
            subSurface.mode = GradientMode.Blend;
            surface.mode = GradientMode.Blend;
        }
        
        public static bool IsInstanceInvalided(StylizedSkinRampTextureData data)
        {
            return data.size == 0 || data.surface == null || data.subSurface == null;
        }
        
        public Texture2D GenerateTexture(TextureFormat format, bool mip, Action<Texture2D> textureSettings)
        {
            Texture2D texture = new Texture2D(size, size,format,mip);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 uv = new Vector2(x, y) / (size -1);
                    uv.x = uv.x * 2 - 1;
                    float scale = Vector2.Dot(uv, new Vector2(0, 1));
                    float position = uv.x / scale * 0.5f + 0.5f;
                    Color col0 = surface.Evaluate(position);
                    Color col1 = subSurface.Evaluate(position);
                    texture.SetPixel(x, y, Color.Lerp(col0, col1, Mathf.Pow(uv.y, 1.2f)));
                }
            }
            textureSettings(texture);
            texture.Apply();
            return texture;
        }
        
        public static void Save(string assetPath,StylizedSkinRampTextureEditorObject editorObject)
        {
            var data = new StylizedSkinRampTextureData(editorObject);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllLines(assetPath, new[] { json });
            AssetDatabase.Refresh();
        }
        
        public static StylizedSkinRampTextureData Load(string assetPath)
        {
            TryFromJson(LoadJson(assetPath),out StylizedSkinRampTextureData data);
            return data;
        }

        static string LoadJson(string path)
        {
            var jsons = File.ReadAllLines(path);
            string json = String.Empty;
            foreach (var j in jsons) json += $"{j}\n";
            return json;
        }
        
        static bool TryFromJson( string json, out StylizedSkinRampTextureData data)
        {
            data = null;
            bool got = false;
            
            try
            {
                data = JsonUtility.FromJson<StylizedSkinRampTextureData>(json);
                got = true;
            }
            catch(Exception e)
            {
                data = new StylizedSkinRampTextureData();
                Debug.LogError($"Json data is invalidated: {e}");
            }
            finally
            {
                if (IsInstanceInvalided(data))
                {
                    var defaultData = new StylizedSkinRampTextureData();
                    data ??= defaultData;
                    data.surface ??= defaultData.surface;
                    data.subSurface ??= defaultData.subSurface;
                    data.size = data.size == 0 ? 128 : data.size;
                    Debug.LogWarning("Serialized data is invalidated");
                }
            }
            return got;
        }
        
        public static void ExportImageAsPNG(string importerPath)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importerPath);
            var exportPath = EditorUtility.SaveFilePanel("Save",Path.GetDirectoryName(importerPath),texture.name,"png");
            if (string.IsNullOrEmpty(exportPath)) return;
            File.WriteAllBytes(exportPath,texture.EncodeToPNG());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    public class StylizedSkinRampTextureEditorObject : ScriptableObject
    {
        public Gradient surface;
        public Gradient subSurface;
        public int size = 128;
        
        public void Init(StylizedSkinRampTextureData data)
        {
            this.surface = data.surface;
            this.subSurface = data.subSurface;
            this.size = data.size;
        }
    }
}