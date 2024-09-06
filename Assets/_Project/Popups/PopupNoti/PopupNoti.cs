using EranCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNoti : BaseUI
{
    [SerializeField] private Text title;
    [SerializeField] private Text content;
    [SerializeField] private Button btnClose;
    public override void OnShow()
    {
        base.OnShow();
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(Close);
    }
    public void Setup(string _title, string _content)
    {
        title.text = _title;
        content.text = _content;
    }
}
