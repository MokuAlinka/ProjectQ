using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;

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
        // UpdateText("安吉丽娜", "即使引导早已破碎,也请您当上艾尔登之王");
        //UpdateImage("僵尸", false);//不在左侧
        // UpdateImage("安吉丽娜", true);//在左侧
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
                Debug.Log("剧情结束"); //这里结束
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
            color.colorMultiplier = 1f; // 设置颜色倍增器为选中状态的倍数
            optionButtons.colors = color;
        }
            
        Button optionButton = buttonGroup.GetChild(index).GetComponent<Button>();
        if (optionButton != null)
        {
            ColorBlock colors = optionButton.colors;
            colors.colorMultiplier = 5f; // 设置颜色倍增器为选中状态的倍数
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

