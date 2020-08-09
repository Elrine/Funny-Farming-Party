using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScroll : MonoBehaviour {
    [SerializeField]
    RectTransform scrollContent = null;
    [SerializeField]
    GameObject buttonPrefab = null;
    // Start is called before the first frame update
    IEnumerator Start () {
        while (SaveManager.Instance == null) {
            yield return new WaitForSeconds (1);
        }
        DirectoryInfo dir = new DirectoryInfo (SaveSystem.SAVE_FOLDER);
        FileInfo[] files = dir.GetFiles ();
        List<string> worldNames = new List<string> ();
        foreach (var file in files) {
            if (Regex.Match (file.Name, "(.*).json$").Success)
                worldNames.Add (Regex.Replace (file.Name, "(.*).json$", "$1"));
        }
        scrollContent.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 20 + worldNames.Count * buttonPrefab.GetComponent<RectTransform> ().rect.height);
        for (int i = 0; i < worldNames.Count; i++) {
            GameObject newButton = Instantiate (buttonPrefab);
            newButton.transform.SetParent (scrollContent);
            newButton.GetComponentInChildren<Text> ().text = worldNames[i];
            string worldName = worldNames[i];
            newButton.GetComponent<Button> ().onClick.AddListener (() => {
                SaveManager.Instance.environementName = worldName;
                SaveManager.Instance.LoadWorld();
            });
        }
    }
}