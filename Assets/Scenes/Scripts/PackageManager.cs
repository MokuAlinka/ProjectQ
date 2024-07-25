using RPGM.Core;
using RPGM.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static System.TimeZoneInfo;

public class PackageManager : MonoBehaviour
{
    private int currentOption = 0;
    private int totalOption = 0;
    public GameObject PackageCanvas;
    public Transform Group_1_buttonGroup;
    public GameObject Group_2_package;
    public Transform package_buttonGroup;
    public GameObject package_ItemButton;

    public RectTransform Kard; // UIԪ��
    public GameObject startObject;  // ��ʼλ�õ�GameObject
    public GameObject endObject;    // Ŀ��λ�õ�GameObject
    public float transitionTime = 2.0f; // ����ʱ��
    public bool reverse = false;    // �Ƿ������
    public bool isTransition = false;

    public Image ItemImage;
    public TMP_Text ItemNameText;
    public TMP_Text ItemDescriptionText;
    GameModel model = Schedule.GetModel<GameModel>();
    IEnumerator TransitionUI(GameObject from, GameObject to)
    {
        float elapsedTime = 0;

        Vector3 startPosition = from.transform.localPosition;
        Vector3 endPosition = to.transform.localPosition;

        Vector3 startScale = from.transform.localScale;
        Vector3 endScale = to.transform.localScale;

        Quaternion startRotation = from.transform.localRotation;
        Quaternion endRotation = to.transform.localRotation;

        while (elapsedTime < transitionTime)
        {
            // ������ɽ���
            float t = elapsedTime / transitionTime;

            // λ�ò�ֵ
            Kard.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            // ���Ų�ֵ
            Kard.localScale = Vector3.Lerp(startScale, endScale, t);

            // ��ת��ֵ
            Kard.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            // ���Ӿ�����ʱ��
            elapsedTime += Time.deltaTime;

            // �ȴ���һ֡
            yield return null;
        }

        // ȷ�����λ�á����ź���ת��Ŀ��ֵ
        Kard.localPosition = endPosition;
        Kard.localScale = endScale;
        Kard.localRotation = endRotation;
    }
    public void HorizontalSelection(int direction)
    {

    }
    public void VerticalSelection(int direction)
    {

    }
    public void ConfirmOption()
    {
        
    }
    public void DisableOption()
    {

    }
    public void LoadItemsList()
    {
        if (!PackageCanvas.activeSelf)
        {
            ClearItemsList();
            foreach (var item_name in model.GetItemsList())
            {
                Dictionary<string, object> c = model.GetItemDic(item_name);
                GameObject button = Instantiate(package_ItemButton, package_buttonGroup);
                button.GetComponentInChildren<TMP_Text>().text = item_name + "    *" + (int)c["Count"];
                button.GetComponent<Button>().onClick.AddListener(delegate { LoadItem(item_name, c); });
            }
            if (package_buttonGroup.childCount > 0)
            {
                package_buttonGroup.GetChild(0).GetComponent<Button>().onClick.Invoke();
            }
            PackageCanvas.SetActive(true);
            StartCoroutine(TransitionUI(startObject, endObject));
        }
        else
        {
            PackageCanvas.SetActive(false);
        }
        
    }
    public void LoadItem(string itemName,Dictionary<string, object> itemDate)
    {
        ItemImage.sprite = (Sprite)itemDate["Sprite_real"];
        ItemNameText.text = itemName;
        ItemDescriptionText.text = (string)itemDate["Introduction"];
    }
    private void ClearItemsList()
    {
        if (package_buttonGroup.childCount > 0)
        {
            for (int i = 0; i < package_buttonGroup.childCount; i++)
            {
                Destroy(package_buttonGroup.GetChild(i).gameObject);
            }
        }
    }
}
