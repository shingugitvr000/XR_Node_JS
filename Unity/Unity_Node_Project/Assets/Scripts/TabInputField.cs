using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabInputField : MonoBehaviour
{
    public InputField[] inputFields;           //TabŰ�� �̵��� InputField �迭  

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            InputField currentInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();

            if(currentInputField != null) 
            {
                int currentIndex = System.Array.IndexOf(inputFields, currentInputField);
                int nextIndex = (currentIndex + 1) % inputFields.Length; //���� InputField �ε��� ��� 

                //���� Input�� ��Ŀ�� �̵�
                inputFields[nextIndex].Select();
                inputFields[nextIndex].ActivateInputField();
            }
        }
    }
}
