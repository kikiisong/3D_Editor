using UnityEngine;
using System.IO;

#if UNITY_EDITOR 
    using UnityEditor;
#endif
#if (!UNITY_EDITOR && ENABLE_WINMD_SUPPORT && UNITY_WSA)
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
#endif

public class ImageElemController : MonoBehaviour
{
    public GameObject imgPrefab;
    public GameObject logoPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addJHULogo()
    {
        Instantiate(logoPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 1), Quaternion.identity);
    }
    public void createImg()
    {
        GameObject imgPanel = Instantiate(imgPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 1), Quaternion.identity);
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileContent);

            //GameObject imgPanel = Instantiate(imgPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 1), Quaternion.identity);
            MeshRenderer imgRenderer = (MeshRenderer)(imgPanel.transform.GetChild(1).gameObject.GetComponent("MeshRenderer"));
            imgRenderer.material.mainTexture=tex;
        }
#endif

#if (!UNITY_EDITOR && ENABLE_WINMD_SUPPORT && UNITY_WSA)

        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            //filepicker.FileTypeFilter.Add("*");
            //filepicker.FileTypeFilter.Add(".jpg");

            var file = await picker.PickSingleFileAsync();
            Windows.Storage.Streams.IBuffer buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);                      
            Windows.Storage.Streams.DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
            var fileContent = new byte[buffer.Length];     
            dataReader.ReadBytes(fileContent);
            UnityEngine.WSA.Application.InvokeOnAppThread(() => 
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(fileContent);

                MeshRenderer imgRenderer = (MeshRenderer)(imgPanel.transform.GetChild(1).gameObject.GetComponent("MeshRenderer"));
                imgRenderer.material.mainTexture=tex;
        
                Debug.Log("***********************************");
                string name = (file != null) ? file.Name : "No data";
                Debug.Log("Name: " + name);
                Debug.Log("***********************************");
                string path = (file != null) ? file.Path : "No data";
                Debug.Log("Path: " + path);
                Debug.Log("***********************************");

                

                //This section of code reads through the file (and is covered in the link)
                // but if you want to make your own parcing function you can 
                // ReadTextFile(path);
                //StartCoroutine(ReadTextFileCoroutine(path));

            }, false);
        }, false);

#endif

    }
}
