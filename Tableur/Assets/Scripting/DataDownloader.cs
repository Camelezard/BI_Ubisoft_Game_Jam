using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class DataDownloader : MonoBehaviour {

    public const string linkReplace = "gviz/tq?tqx=out:csv&sheet=";

    // debug
    public string test_SheetName;

    // params
    public string url = "";
    public string path = "";

    public int currentRow;
    public int currentCol;
    public int lineAmount = 0;


    private void Start() {
        DownloadData(test_SheetName);
    }

    #region parse
    public virtual void Load(string sheetName) {

        var textAsset = Resources.Load(sheetName) as TextAsset;
        var text = textAsset.text;

        lineAmount = fgCSVReader.GetLineAmount(text);

        // delegate for handling data
        fgCSVReader.LoadFromString(text, new fgCSVReader.ReadLineDelegate(GetCell));

        OnLoadEnd();
    }
    public virtual void OnLoadEnd() {

    }

    // fonction principale pour load la data
    public virtual void GetCell(int rowIndex, List<string> cells) {
        // setting current row
        currentRow = rowIndex;
    }
    #endregion

    public void DownloadData(string sheetName) {
        // replacing url to switch to data download
        var editIndex = url.IndexOf("edit");
        if (editIndex != -1) {
            var tmpUrl = url.Remove(editIndex) + linkReplace + sheetName;
            Debug.Log("Fetching " + sheetName + "...");
            StartCoroutine(DownloadCSV(tmpUrl, sheetName));
        }
    }

    IEnumerator DownloadCSV(string tmpUrl, string sheetName) {

        // check pour le connection timeout
        _ = Time.realtimeSinceStartup + 10f;

        CreateFile(sheetName);

        // getting url
        var www = UnityWebRequest.Get(tmpUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            // fail safe
            Debug.LogError("Error when requesting CSV file (responseCode:" + www.responseCode + ")");
            Debug.LogError(www.error);
        } else {
            // write file into resours
            var filepath = $"Assets/Resources/Data/{sheetName}.csv";
            System.IO.File.WriteAllText(filepath, www.downloadHandler.text);
        }
    }

    void CreateFile(string sheetName) {
        string folderPath = "Assets/Resources/Data";
        string filePath = Path.Combine(folderPath, $"{sheetName}.csv");

        // create folder
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        if (!File.Exists(filePath)) {
            // create empty .csv file
            File.WriteAllText(filePath, "");
        } else {
            Debug.LogWarning("Le fichier existe déjà : " + filePath);
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        Debug.Log($"Importing {sheetName}");
#endif
    }

}
