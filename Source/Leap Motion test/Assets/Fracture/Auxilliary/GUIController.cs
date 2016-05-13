using System.Collections;
using Destruction.Dynamic;
using Destruction.Tools;
using UnityEngine;

namespace Destruction
{
    public class GUIController : MonoBehaviour
    {
        public int controlWindowWidth = 250;

        public float sceneFadeLength = 0.5f;

        private bool showControls = true;

        private float fpsTimer;
        private int fpsCounter;
        private string fpsLabelText = "";
        private Color fpsColour;

        private Trigger[] fractureObjects;

        private void Start()
        {
            StartCoroutine(FadeIn());

            fractureObjects = FindObjectsOfType(typeof(Trigger)) as Trigger[];
            
            StartCoroutine(FPS());
        }

        private IEnumerator FadeIn()
        {
            GameObject faderObject = new GameObject("Fader");
            GUITexture fader = faderObject.AddComponent<GUITexture>();
            fader.pixelInset = fader.GetScreenRect(Camera.main);

            Texture2D blackTexture = new Texture2D(1, 1);
            fader.texture = blackTexture;
            blackTexture.SetPixel(0, 0, Color.black);

            float timer = sceneFadeLength;

            while (timer > 0)
            {
                fader.color = new Color(0, 0, 0, timer / sceneFadeLength);
                GUI.color = new Color(1, 1, 1, (timer / sceneFadeLength));
                timer -= Time.deltaTime;
                yield return null;
            }

            Destroy(faderObject);
        }

        private void Update()
        {
            // Update FPS counter.
            fpsTimer += Time.timeScale / Time.deltaTime;
            ++fpsCounter;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                showControls = !showControls;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            GUI.color = new Color(fpsColour.r, fpsColour.g, fpsColour.b, GUI.color.a);
            GUI.Window(1, new Rect(10, 10, 75, 50), FrameCounterWindow, "FPS");
            GUI.color = new Color(1, 1, 1, GUI.color.a);
#endif

            GUILayout.Window(2, new Rect(Screen.width - controlWindowWidth - 10, 10, controlWindowWidth, 10), SceneControls, "Controls", GUILayout.Width(controlWindowWidth));
        }

        private void SceneControls(int id)
        {
            Toggle(ref showControls, "Show");

            if (!showControls) return;

            GUILayout.Space(15);

            GUILayout.BeginVertical("box");
            GUILayout.Label("Hold RMB to orbit scene.");
            GUILayout.Label("Press LMB over environment for destruction.");
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginVertical("box");
            GUILayout.Label("Shards : " + ShardPool.ShardsUsed);
            GUILayout.EndVertical();

            //GUILayout.Space(15);

            //GUI.backgroundColor = new Color(1, 1, 1, GUI.color.a);
            //if (GUILayout.Button("Destroy Foundations", GUILayout.Width(controlWindowWidth - 20)))
            //{
            //    StartCoroutine(DestroyFoundations());
            //}

            //GUI.backgroundColor = new Color(1, 0, 0, GUI.color.a);
            //if (GUILayout.Button("Flatten", GUILayout.Width(controlWindowWidth - 20)))
            //{
            //    StartCoroutine(DestroyAll());
            //}
            GUILayout.Space(15);
            GUI.backgroundColor = new Color(0, 1, 0, GUI.color.a);
            if (GUILayout.Button("Reset Scene", GUILayout.Width(controlWindowWidth - 20)))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            GUI.backgroundColor = new Color(1, 1, 1, GUI.color.a);
        }

        private IEnumerator DestroyFoundations()
        {
            int i = 0;
            foreach (var obj in fractureObjects)
            {
                if (obj == null) continue;
                if (obj.transform.localPosition.y < -7f)
                {
                    obj.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000000);
                    obj.TriggerDestruction(obj.transform.position, Random.value);
                }
                
                i++;

                if (i % 2 == 0) yield return null;
            }
        }

        private IEnumerator DestroyAll()
        {
            int i = 0;
            foreach(var obj in fractureObjects)
            {
                if (obj == null) continue;

                obj.TriggerDestruction(obj.transform.position, Random.value);
                i++;

                if (i % 2 == 0) yield return null;
            }
        }

        private IEnumerator FPS()
        {
            while (enabled)
            {
                // Update the FPS
                float fps = fpsTimer / fpsCounter;
                fpsLabelText = fps.ToString("f2");


                //Update the color
                if (fps < 10)
                {
                    fpsColour = Color.red;
                }
                else if (fps < 25)
                {
                    fpsColour = Color.yellow;
                }
                else
                {
                    fpsColour = Color.green;
                }

                fpsTimer = 0.0F;
                fpsCounter = 0;

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void FrameCounterWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(fpsLabelText);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void Toggle(ref bool value, string name)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(name + " : ");
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }
    }
}

