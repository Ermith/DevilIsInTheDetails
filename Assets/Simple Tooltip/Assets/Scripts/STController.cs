﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class STController : MonoBehaviour
{
    public enum TextAlign { Left, Right };

    private Image panel;
    private TextMeshProUGUI toolTipTextLeft;
    private TextMeshProUGUI toolTipTextRight;
    private RectTransform rect;
    private int showInFrames = -1;
    private bool showNow = false;
    [CanBeNull] private SimpleTooltip _lastActiveTooltip = null;
    private float _timeActive = 0;
    private float _tooltipDelay = 0.35f;

    private void Awake()
    {
        // Load up both text layers
        var tmps = GetComponentsInChildren<TextMeshProUGUI>();
        for(int i = 0; i < tmps.Length; i++)
        {
            if (tmps[i].name == "_left")
                toolTipTextLeft = tmps[i];

            if (tmps[i].name == "_right")
                toolTipTextRight = tmps[i];
        }

        // Keep a reference for the panel image and transform
        panel = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        // Hide at the start
        HideTooltip();
    }

    void Update()
    {
        ResizeToMatchText();
        UpdateShow();

        if (SimpleTooltip.ActiveTooltip != null && SimpleTooltip.ActiveTooltip == _lastActiveTooltip &&
            !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            _timeActive += Time.deltaTime;
        }
        else
        {
            if (_timeActive > _tooltipDelay)
            {
                if (SimpleTooltip.ActiveTooltip != null)
                    SimpleTooltip.ActiveTooltip.HideTooltip();
                if (_lastActiveTooltip != null)
                    _lastActiveTooltip.HideTooltip();
            }
            _timeActive = 0;
        }

        if (SimpleTooltip.ActiveTooltip != null && _timeActive > _tooltipDelay && _timeActive - Time.deltaTime < _tooltipDelay)
        {
            SimpleTooltip.ActiveTooltip.ShowTooltipForReal();
        }

        _lastActiveTooltip = SimpleTooltip.ActiveTooltip;
    }

    private void ResizeToMatchText()
    {
        // Find the biggest height between both text layers
        var bounds = toolTipTextLeft.textBounds;
        float biggestY = toolTipTextLeft.textBounds.size.y;
        float rightY = toolTipTextRight.textBounds.size.y;
        if (rightY > biggestY)
            biggestY = rightY;

        // Dont forget to add the margins
        var margins = toolTipTextLeft.margin.y * 2;

        // Update the height of the tooltip panel
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, biggestY + margins);
    }

    private void UpdateShow()
    {
        if (showInFrames == -1)
            return;

        if (showInFrames == 0)
            showNow = true;

        if (showNow)
        {
            var position = Input.mousePosition;
            // clip position not to poke out of camera
            position.x = Mathf.Clamp(position.x, 0, Screen.width - rect.sizeDelta.x);
            position.y = Mathf.Clamp(position.y, 0, Screen.height - rect.sizeDelta.y);
            rect.anchoredPosition = position;
        }

        showInFrames -= 1;
    }

    public void SetRawText(string text, TextAlign align = TextAlign.Left)
    {
        // Doesn't change style, just the text
        if(align == TextAlign.Left)
            toolTipTextLeft.text = text;
        if (align == TextAlign.Right)
            toolTipTextRight.text = text;
        ResizeToMatchText();
    }

    public void SetCustomStyledText(string text, SimpleTooltipStyle style, TextAlign align = TextAlign.Left)
    {
        // Update the panel sprite and color
        panel.sprite = style.slicedSprite;
        panel.color = style.color;

        // Update the font asset, size and default color
        toolTipTextLeft.font = style.fontAsset;
        toolTipTextLeft.color = style.defaultColor;
        toolTipTextRight.font = style.fontAsset;
        toolTipTextRight.color = style.defaultColor;

        // Convert all tags to TMPro markup
        var styles = style.fontStyles;
        for(int i = 0; i < styles.Length; i++)
        {
            string addTags = "</b></i></u></s>";
            addTags += "<color=#" + ColorToHex(styles[i].color) + ">";
            if (styles[i].bold) addTags += "<b>";
            if (styles[i].italic) addTags += "<i>";
            if (styles[i].underline) addTags += "<u>";
            if (styles[i].strikethrough) addTags += "<s>";
            text = text.Replace(styles[i].tag, addTags);
        }
        if (align == TextAlign.Left)
            toolTipTextLeft.text = text;
        if (align == TextAlign.Right)
            toolTipTextRight.text = text;
        ResizeToMatchText();
    }

    public string ColorToHex(Color color)
    {
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);
        return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
    }

    public void ShowTooltip()
    {
        // After 2 frames, showNow will be set to TRUE
        // after that the frame count wont matter
        if (showInFrames == -1)
            showInFrames = 2;
    }

    public void HideTooltip()
    {
        showInFrames = -1;
        showNow = false;
        rect.anchoredPosition = new Vector2(Screen.currentResolution.width * 420, Screen.currentResolution.height * 420);

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            SimpleTooltip tooltip = result.gameObject.GetComponent<SimpleTooltip>();
            if (tooltip != null)
            {
                tooltip.OnPointerEnter(pointerData);
            }
        }
    }
}
