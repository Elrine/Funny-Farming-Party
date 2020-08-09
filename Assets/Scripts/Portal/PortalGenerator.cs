using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGenerator : MonoBehaviour {
    public GameObject portal;
    public GameObject character;
    // Start is called before the first frame update
    IEnumerator Start () {
        GameObject.Instantiate (character, Vector3.zero, Quaternion.identity);
        if (SaveManager.Instance.env.portalPosition.y > -1) {
            GameObject.Instantiate (portal, SaveManager.Instance.env.portalPosition, Quaternion.identity);
        } else {
            while (MapGenerator.Instance == null || MapGenerator.Instance.textureSettings.waterHeight == -1) {
                yield return new WaitForSeconds (1);
            }
            Vector3 point = new Vector3 (0, 100, 0);
            while (SaveManager.Instance.env.portalPosition.y < -1) {
                RaycastHit[] hits = Physics.RaycastAll (point, Vector3.down, 100);
                Debug.DrawRay (point, Vector3.down * 100, Color.green, 30);
                while (hits.Length == 0) {
                    yield return new WaitForSeconds (1);
                    hits = Physics.RaycastAll (point, Vector3.down, 100);
                }
                foreach (var hit in hits) {
                    if (hit.collider.CompareTag ("Chunk")) {
                        if (hit.point.y > MapGenerator.Instance.textureSettings.waterHeight) {
                            SaveManager.Instance.env.portalPosition = hit.point;
                            GameObject.Instantiate (portal, hit.point, Quaternion.identity);
                        }
                        break;
                    }
                }
                point = new Vector3 (Random.Range (-50, 50), 100, Random.Range (-50, 50));
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }
}