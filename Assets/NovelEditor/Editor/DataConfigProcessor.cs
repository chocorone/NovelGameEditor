using UnityEditor;
using NovelEditor;

namespace NovelEditor.Editor
{
    public class DataConfigProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            NovelData data = AssetDatabase.LoadAssetAtPath<NovelData>(assetPath);
            if (data != null)
            {
                if (NovelEditorWindow.editingData == data)
                {
                    NovelEditorWindow.Instance.Init(null);
                }
            }
            return AssetDeleteResult.DidNotDelete;
        }

    }
}