using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    [CustomEditor(typeof(StylizedSkinRampTextureImporter))]
    public class StylizedSkinRampTextureImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject => false;
    
        private StylizedSkinRampTextureEditorObject _editorObject;
        private SerializedObject _serializedDataObject;
        private SerializedProperty _surfaceColor;
        private SerializedProperty _subsurfaceColor;
        private SerializedProperty _textureSize;
        
        private StylizedSkinRampTextureImporter _importer;
        private SerializedObject _serializedImporter;
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
            _serializedImporter.ApplyModifiedProperties();
            StylizedSkinRampTextureData.Save(_importer.assetPath, _editorObject);
        }
    
        bool TryInitPropertiesWhenNull()
        {
            if (_editorObject && _serializedDataObject != null && _importer && _serializedImporter != null)
                return true;
            
            if (target is not StylizedSkinRampTextureImporter importer)
                return false;
            
            if (!_importer)
            {
                _importer = importer;
            }
            
            if (_importer && !_editorObject)
            {
                _editorObject = ScriptableObject.CreateInstance<StylizedSkinRampTextureEditorObject>();
                _editorObject.Init(StylizedSkinRampTextureData.Load(_importer.assetPath));
            }
            
            if (_editorObject && _serializedDataObject == null)
            {
                _serializedDataObject = new SerializedObject(_editorObject);
                _surfaceColor = _serializedDataObject.FindProperty("surface");
                _subsurfaceColor = _serializedDataObject.FindProperty("subSurface");
                _textureSize = _serializedDataObject.FindProperty("size");
                
            }
            
            if (_importer && _serializedImporter == null)
            {
                _serializedImporter = new SerializedObject(_importer);
                _mip = _serializedImporter.FindProperty("mip");
                _format = _serializedImporter.FindProperty("format");
                _filterMode = _serializedImporter.FindProperty("filterMode");
                _wrapMode = _serializedImporter.FindProperty("wrapMode");
                _isDataDirty = _serializedImporter.FindProperty("isDataDirty");
            }
            
            return _editorObject && _serializedDataObject != null && _importer && _serializedImporter != null;
        }

        public override void OnEnable()
        {
            TryInitPropertiesWhenNull();
            base.OnEnable();
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
                _serializedImporter.Update();
                EditorGUI.indentLevel--;
                GUILayout.Label("Texture Settings",EditorStyles.boldLabel);
                GUILayout.Space(5);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mip, new GUIContent("Generate Mip Map"));
                EditorGUILayout.PropertyField(_format, new GUIContent("Texture Format"));
                EditorGUILayout.PropertyField(_filterMode, new GUIContent("Texture Filter Mode"));
                EditorGUILayout.PropertyField(_wrapMode, new GUIContent("Texture Wrap Mode"));
                _serializedImporter.ApplyModifiedProperties();
            }
            
            GUILayout.Space(10);
            
            // Apply button
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
    
        public override void OnDisable()
        {
            base.OnDisable();
            DestroyImmediate(_editorObject);
        }
    }
}