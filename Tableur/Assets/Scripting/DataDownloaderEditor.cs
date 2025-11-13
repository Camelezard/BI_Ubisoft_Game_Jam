#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataDownloader), true)]
public class DataDownloaderEditor : Editor {
    public override void OnInspectorGUI() {
        var dataDownloader = (DataDownloader)target;

        if (GUILayout.Button("Download Data")) {
            dataDownloader.DownloadData(dataDownloader.test_SheetName);
        }

        if (GUILayout.Button("Open Link")) {
            Application.OpenURL(dataDownloader.url);
        }

        _ = DrawDefaultInspector();
    }
}
#endif