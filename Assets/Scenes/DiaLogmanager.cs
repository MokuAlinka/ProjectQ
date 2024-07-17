using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;
using RPGM.UI;
using RPGM.Core;
using RPGM.Gameplay;

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

    GameModel model = Schedule.GetModel<GameModel>();
    private void Awake()
    {
        foreach (var sprite in sprites)
        {
            imageDic[sprite.name] = sprite;
        }
    }

    void Start()
    {
        UpdateImage("NoneImage", "Right");
        UpdateImage("NoneImage", "Left");
        ReadText(dialogDataFile);
        Canvas.SetActive(false);
        CanNext = false;
    }

    void Update()
    {
        
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
                model.input.ChangeState(InputController.State.CharacterControl);
                Debug.Log("�������"); //�������
            }
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
        totalOptions = 0;
    }
    public void SelectOption(int direction)
    {
        if (totalOptions > 0)
        {
            HighlightOption(currentOptionIndex);
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
            HighlightOption(currentOptionIndex,5f);
        }
    }
    public void ConfirmOption()
    {
        if (totalOptions > 0)
        {
            if (currentOptionIndex >= 0 && currentOptionIndex <= totalOptions)
            {
                buttonGroup.GetChild(currentOptionIndex).GetComponent<Button>().onClick.Invoke();
            }
        }
        else
        {
            Debug.Log("1111");
            ShowDiaLogRow();
        }
    }
    private void HighlightOption(int index,float color = 1f)
    {
        Button optionButton = buttonGroup.GetChild(index).GetComponent<Button>();
        if (optionButton != null)
        {
            ColorBlock colors = optionButton.colors;
            colors.colorMultiplier = color; // ������ɫ������Ϊѡ��״̬�ı���
            optionButton.colors = colors;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!Canvas.activeSelf)
            {
                Canvas.SetActive(true);
                model.input.ChangeState(InputController.State.DialogControl);
                ShowDiaLogRow();
            }
        }
    }

}

