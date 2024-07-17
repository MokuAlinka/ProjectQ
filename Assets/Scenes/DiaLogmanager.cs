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
    /// 对话UI画布
    /// </summary> 
    public GameObject Canvas;

    /// <summary>
    /// 对话内容文本，csv格式
    /// </summary> 
    public TextAsset dialogDataFile;

    /// <summary>
    /// 左侧角色图像
    /// </summary>
    public Image imageLeft;

    /// <summary>
    /// 右侧角色图像
    /// </summary>
    public Image imageRight;

    /// <summary>
    /// 角色名字文本
    /// </summary>
    public TMP_Text nameText;

    /// <summary>
    /// 对话内容文本
    /// </summary>
    public TMP_Text dialogText;

    /// <summary>
    /// 角色图片列表
    /// </summary>
    public List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// 角色名字对应图片的字典
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    /// <summary>
    /// 当前对话索引值
    /// </summary>
    public int dialogIndex;

    /// <summary>
    /// 对话文本按行分割
    /// </summary>
    public string[] dialogRows;

    /// <summary>
    /// 继续按钮
    /// </summary>
    public bool CanNext = false;

    /// <summary>
    /// 选项按钮
    /// </summary>
    public GameObject optionButton;

    /// <summary>
    /// 选项按钮父节点
    /// </summary>
    public Transform buttonGroup;

    ///选择
    private int currentOptionIndex = 0;
    private int totalOptions = 0;

    /// <summary>
    /// 触发器
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

    //更新文本信息
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }
    //更新图片信息
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
        dialogRows = _textAsset.text.Split('\n'); //以换行来分割
        Debug.Log("读取成果");
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
                Debug.Log("剧情结束"); //这里结束
            }
        }
    }


    public void GenerateOption(int _index) //生成按钮
    {
        string[] cells = dialogRows[_index].Split(",");
        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            //按键事件
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
            colors.colorMultiplier = color; // 设置颜色倍增器为选中状态的倍数
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

