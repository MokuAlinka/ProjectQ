using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;

public class DiaLogmanager : MonoBehaviour
{
    /// <summary>
    /// �Ի�UI����
    /// </summary> 
    public GameObject Canvas;

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

    ///ѡ��
    private int currentOptionIndex = 0;
    private int totalOptions = 0;

    /// <summary>
    /// ������
    /// </summary>
    public Collider2D Collider2D;
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
        Canvas.SetActive(false);
        CanNext = false;
        // UpdateText("��������", "��ʹ������������,Ҳ�������ϰ�����֮��");
        //UpdateImage("��ʬ", false);//�������
        // UpdateImage("��������", true);//�����
    }

    void Update()
    {
        if (Canvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CanNext)
                {
                    OnClickNext();
                }
                else
                {
                    ConfirmOption();
                }
            }
            else if (!CanNext)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SelectOption(-1); // Move left in options
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    SelectOption(1); // Move right in options
                }
            }
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
                if (Canvas.activeSelf)
                {
                    Canvas.SetActive(false);
                }
                dialogIndex = 0;
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
        totalOptions = buttonGroup.childCount -1;
    }
    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ClearOptions();
        ShowDiaLogRow();
    }
    private void ClearOptions()
    {
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
    private void SelectOption(int direction)
    {
        currentOptionIndex += direction;

        // Ensure currentOptionIndex wraps around if it goes out of bounds
        if (currentOptionIndex < 0)
        {
            currentOptionIndex = totalOptions;
        }
        else if (currentOptionIndex > totalOptions)
        {
            currentOptionIndex = 0;
        }

        // Highlight/select the current option visually (if needed)
        HighlightOption(currentOptionIndex);
    }

    private void HighlightOption(int index)
    {
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Button optionButtons = buttonGroup.GetChild(i).GetComponent<Button>();
            ColorBlock color = optionButtons.colors;
            color.colorMultiplier = 1f; // ������ɫ������Ϊѡ��״̬�ı���
            optionButtons.colors = color;
        }
            
        Button optionButton = buttonGroup.GetChild(index).GetComponent<Button>();
        if (optionButton != null)
        {
            ColorBlock colors = optionButton.colors;
            colors.colorMultiplier = 5f; // ������ɫ������Ϊѡ��״̬�ı���
            optionButton.colors = colors;
        }
    }

    private void ConfirmOption()
    {
        if (currentOptionIndex >= 0 && currentOptionIndex <= totalOptions)
        {
            buttonGroup.GetChild(currentOptionIndex).GetComponent<Button>().onClick.Invoke();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!Canvas.activeSelf)
            {
                Canvas.SetActive(true);
                ShowDiaLogRow();
            }
        }
    }
}

