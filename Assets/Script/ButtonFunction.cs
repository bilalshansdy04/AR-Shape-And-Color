using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunction : MonoBehaviour
{
    [System.Serializable]
    public class ObjectAR{
        public GameObject[] objectLainnyas;
    }

    public ObjectAR[] objectArs;

    GameObject targetAr;
    int indexObjectActive;
    public GameObject[] basicObject;
    public GameObject[] buttonObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonChangeObject(int indexObject){
        objectArs[indexObjectActive].objectLainnyas[indexObject].SetActive(true);
        for (int i = 0; i < objectArs[indexObjectActive].objectLainnyas.Length; i++)
        {
            if (i != indexObject)
            {
                objectArs[indexObjectActive].objectLainnyas[i].SetActive(false);
            }
        }
    }

    void CheckTotalObject(){
        Debug.Log("Target ditemukan: " + targetAr.name);
        for (int i = 0; i < basicObject.Length; i++)
        {
            Debug.Log("Cek dengan: " + basicObject[i].name);
            if (targetAr.name == basicObject[i].name)
            {
                indexObjectActive = i;
                for (int j = 0; j < objectArs[i].objectLainnyas.Length; j++)
                {
                    buttonObject[j].SetActive(true);
                }

                break;
            }
        }
    }

    public void OnTargetFound(GameObject target){
        targetAr = target;

        CheckTotalObject();
    }

    public void OnTargetLost(){{
        for (int i = 0; i < buttonObject.Length; i++)
        {
            buttonObject[i].SetActive(false);
        }
    }}

}
