using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveListItem : MonoBehaviour
{
    [SerializeField] TMP_Text saveText;
    [SerializeField] Button loadSaveButton;
    [SerializeField] Button confirmDeleteButton;
    
    public void Setup (string saveName)
    {
        saveText.text = saveName;
    }

    public Button GetLoadSaveButton ()
    {
        return loadSaveButton;
    }

    public Button GetConfirmDeleteButton ()
    {
        return confirmDeleteButton;
    }
}
