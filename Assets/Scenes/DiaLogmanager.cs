using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;

public class DiaLogmanager : MonoBehaviour
{
    /// <summary>
    /// �Ի������ı���csv��ʽ
    /// </summary> 
    public TextAsset dialogDataFile;

    /// <summary>
    /// ����ɫͼ��
    /// </summary>
    public Image imageLeft;

    /// <summary>
    /// �Ҳ��ɫͼ��
    /// </summary>
    public Image imageRight;

    /// <summary>
    /// ��ɫ�����ı�
    /// </summary>
    public TMP_Text nameText;

    /// <summary>
    /// �Ի������ı�
    /// </summary>
    public TMP_Text dialogText;

    /// <summary>
    /// ��ɫͼƬ�б�
    /// </summary>
    public List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// ��ɫ���ֶ�ӦͼƬ���ֵ�
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    /// <summary>
    /// ��ǰ�Ի�����ֵ
    /// </summary>
    public int dialogIndex;

    /// <summary>
    /// �Ի��ı����зָ�
    /// </summary>
    public string[] dialogRows;

    /// <summary>
    /// ������ť
    /// </summary>
    public bool CanNext = false;

    /// <summary>
    /// ѡ�ť
    /// </summary>
    public GameObject optionButton;

    /// <summary>
    /// ѡ�ť���ڵ�
    /// </summary>
    public Transform buttonGroup;

    private void Awake()
    {
        imageDic["NoneImage"] = sprites[0];
        imageDic["DingZhen_0"] = sprites[1];
        imageDic["DingZhen_1"] = sprites[2];
    }

    void Start()
    {
        UpdateImage("NoneImage", "Right");
        UpdateImage("NoneImage", "Left");
        ReadText(dialogDataFile);
        ShowDiaLogRow();
        // UpdateText("��������", "��ʹ������������,Ҳ�������ϰ�����֮��");
        //UpdateImage("��ʬ", false);//�������
        // UpdateImage("��������", true);//�����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickNext();
        }
    }

    //�����ı���Ϣ
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    //����ͼƬ��Ϣ
    public void UpdateImage(string _name, string _position) 
    {
        if (_position == "Left")
        {
            imageLeft.sprite = imageDic[_name];
            imageRight.sprite = imageDic["NoneImage"];
        }
        else if (_position == "Right")
        {
            imageRight.sprite = imageDic[_name];
            imageLeft.sprite = imageDic["NoneImage"];
        }
    }   

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n'); //�Ի������ָ�
        Debug.Log("��ȡ�ɹ�");
    }

    public void ShowDiaLogRow()
    {
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[4]);
                UpdateImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                CanNext = true;
                break;
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                CanNext = false;
                GenerateOption(i);
            }
            else if (cells[0] == "end" && int.Parse(cells[1]) == dialogIndex)
            {
                Debug.Log("�������"); //�������
            }
        }
    }

    public void OnClickNext()
    {
        if (CanNext)
        {
            ShowDiaLogRow();
        }
        
    }

    public void GenerateOption(int _index) //���ɰ�ť
    {
        string[] cells = dialogRows[_index].Split(",");
        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            //�����¼�
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate { OnOptionClick(int.Parse(cells[5])); });

            GenerateOption(_index + 1);
        }
    }
    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDiaLogRow();
        for(int i = 0;i <buttonGroup.childCount;i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
}

