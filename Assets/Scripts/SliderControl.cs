﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderControl : MonoBehaviour
{

    public Scrollbar m_Scrollbar;

    private float mTargetValue;

    private bool mNeedMove = false;

    private const float MOVE_SPEED = 1F;

    private const float SMOOTH_TIME = 0.2F;

    private float mMoveSpeed = 0f;

    public void OnPointerDown()
    {
        mNeedMove = false;
    }

    public void OnPointerUp()
    {
        // 判断当前位于哪个区间，设置自动滑动至的位置
        if (m_Scrollbar.value <= 0.1f)
        {
            mTargetValue = 0;
        }
        else if (m_Scrollbar.value <= 0.3f)
        {
            mTargetValue = 0.2f;
        }
        else if (m_Scrollbar.value <= 0.5f)
        {
            mTargetValue = 0.4f;
        }
        else if (m_Scrollbar.value <= 0.7f)
        {
            mTargetValue = 0.6f;
        }
        else if (m_Scrollbar.value <= 0.9f)
        {
            mTargetValue = 0.8f;
        }
        else
        {
            mTargetValue = 1f;
        }

        mNeedMove = true;
        mMoveSpeed = 0;
    }

    public void OnButtonClick(int value)
    {
        switch (value)
        {
            case 1:
                mTargetValue = 0;
                break;
            case 2:
                mTargetValue = 0.25f;
                break;
            case 3:
                mTargetValue = 0.5f;
                break;
            case 4:
                mTargetValue = 0.75f;
                break;
            case 5:
                mTargetValue = 1f;
                break;
            default:
                Debug.LogError("!!!!!");
                break;
        }
        mNeedMove = true;
    }

    void Update()
    {
        if (mNeedMove)
        {
            if (Mathf.Abs(m_Scrollbar.value - mTargetValue) < 0.01f)
            {
                m_Scrollbar.value = mTargetValue;
                mNeedMove = false;
                return;
            }
            m_Scrollbar.value = Mathf.SmoothDamp(m_Scrollbar.value, mTargetValue, ref mMoveSpeed, SMOOTH_TIME);
        }
    }

}