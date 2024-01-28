using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    [CustomEditor(typeof(StylizedSkinRampTextureImporter))]
    public class StylizedSkinRampTextureImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject => false;
    
        private StylizedSkinRampTextureData _data;
        private SerializedObject _serializedDataObject;
        private SerializedProperty _surfaceColor;
        private SerializedProperty _subsurfaceColor;
        private SerializedProperty _textureSize;
        
        private StylizedSkinRampTextureImporter _importer;
        private SerializedObject _serializedImporterObject;
        private SerializedProperty _format;
        private SerializedProperty _mip;
        private SerializedProperty _filterMode;
        private SerializedProperty _wrapMode;
        private SerializedProperty _isDataDirty;
    
        protected override void Apply()
        {
            base.Apply();
            ApplyData();
        }
    
        public void ApplyData()
        {
            _isDataDirty.boolValue = false; 
            _serializedImporterObject.ApplyModifiedProperties();
            StylizedSkinRampTextureData.Save(_importer.assetPath, _data);
        }
    
        bool TryInitPropertiesWhenNull()
        {
            if (_data && _serializedDataObject != null && _importer && _serializedImporterObject != null)
                return true;
            
            if (target is not StylizedSkinRampTextureImporter importer)
                return false;
            
            if (!_importer)
            {
                _importer = importer;
            }
            
            if (_importer && !_data)
            {
                _data = StylizedSkinRampTextureData.Load(_importer.assetPath);
            }
            
            if (_data && _serializedDataObject == null)
            {
                _serializedDataObject = new SerializedObject(_data);
                _surfaceColor = _serializedDataObject.FindProperty("surface");
                _subsurfaceColor = _serializedDataObject.FindProperty("subSurface");
                _textureSize = _serializedDataObject.FindProperty("size");
            }
            
            if (_importer && _serializedImporterObject == null)
            {
                _serializedImporterObject = new SerializedObject(_importer);
                _mip = _serializedImporterObject.FindProperty("mip");
                _format = _serializedImporterObject.FindProperty("format");
                _filterMode = _serializedImporterObject.FindProperty("filterMode");
                _wrapMode = _serializedImporterObject.FindProperty("wrapMode");
                _isDataDirty = _serializedImporterObject.FindProperty("isDataDirty");
            }
            
            return _data && _serializedDataObject != null && _importer && _serializedImporterObject != null;
        }

        public override void OnEnable()
        {
            TryInitPropertiesWhenNull();
            base.OnEnable();
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            _data.Unload();
        }
        
        public override void OnInspectorGUI()
        {
            if(!TryInitPropertiesWhenNull()) return;
            
            GUILayout.Space(10);
            
            // Gradient Data
            {
                _serializedDataObject.Update();
                GUILayout.Label("Ramp Settings",EditorStyles.boldLabel);
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_surfaceColor, new GUIContent("Surface Color"));
                EditorGUILayout.PropertyField(_subsurfaceColor, new GUIContent("Subsurface Color"));
                EditorGUILayout.PropertyField(_textureSize, new GUIContent("Texture Size"));
                if (_serializedDataObject.ApplyModifiedProperties())
                    _importer.isDataDirty = true;
            }
            
            GUILayout.Space(10);
            
            // Texture settings
            {
                _serializedImporterObject.Update();
                EditorGUI.indentLevel--;
                GUILayout.Label("Texture Settings",EditorStyles.boldLabel);
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mip, new GUIContent("Generate Mip Map"));
                EditorGUILayout.PropertyField(_format, new GUIContent("Texture Format"));
                EditorGUILayout.PropertyField(_filterMode, new GUIContent("Texture Filter Mode"));
                EditorGUILayout.PropertyField(_wrapMode, new GUIContent("Texture Wrap Mode"));
                _serializedImporterObject.ApplyModifiedProperties();
            }
            
            GUILayout.Space(10);
            
            // Apply Revert button
            {
                ApplyRevertGUI();
            }
            
            GUILayout.Space(10);
            
            // Other tools
            {
                if (GUILayout.Button("Export Image"))
                    StylizedSkinRampTextureData.ExportImageAsPNG(_importer.assetPath);
            }
        }
        
        
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (!(AssetImporter.GetAtPath(path) as StylizedSkinRampTextureImporter)) return false;
            EditorUtility.OpenPropertyEditor(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            return true;
        }
    }
}