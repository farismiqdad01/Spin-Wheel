using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GantiScene : MonoBehaviour
{
    public void ChangeScene(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }
}
