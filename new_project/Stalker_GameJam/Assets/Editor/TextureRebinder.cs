using UnityEngine;
using UnityEditor;

public class ProPixelizerAssigner : Editor
{
    [MenuItem("Tools/ProPixelizer 텍스처 강제 연결")]
    static void AssignAlbedo()
    {
        // 프로젝트 창에서 선택한 메테리얼들을 가져옴
        Material[] selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.Assets);

        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("메테리얼을 먼저 선택해주세요!");
            return;
        }

        int successCount = 0;

        foreach (Material mat in selectedMaterials)
        {
            // 메테리얼 이름과 똑같은 이름의 텍스처를 찾음
            string filter = $"{mat.name} t:Texture2D";
            string[] guids = AssetDatabase.FindAssets(filter);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (tex != null)
                {
                    Undo.RecordObject(mat, "Assign ProPixelizer Albedo");

                    // 주신 셰이더의 실제 프로퍼티 이름인 "_Albedo"에 할당
                    if (mat.HasProperty("_Albedo"))
                    {
                        mat.SetTexture("_Albedo", tex);
                        successCount++;
                    }
                    else
                    {
                        // 만약 셰이더가 Standard나 URP Lit이라면 기본 이름에도 할당 시도
                        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
                        else if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
                    }
                }
            }
        }

        Debug.Log($"<color=cyan>[작업 완료]</color> 총 {successCount}개의 메테리얼에 텍스처를 연결했습니다.");
    }
}