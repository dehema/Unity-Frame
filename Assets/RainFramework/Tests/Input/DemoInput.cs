using System;
using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoInput : MonoBehaviour
    {
        void Start()
        {
            // 切换输入设备（不会清理回调，方便热切换输入设备）
            RA.Input.SwitchDevice(new StandardInputDevice());

            // 启用或暂停输入
            RA.Input.IsEnableInputDevice = false;
            
            // 设置按钮回调，Started开始按按钮，Performed按下按钮，Canceled结束按钮
            RA.Input.AddButtonStarted(InputButtonType.MouseLeft, MouseLeft);
            RA.Input.AddButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
            RA.Input.AddButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
            
            RA.Input.AddAxisValueChanged(InputAxisType.MouseX, MouseX);
            
            // 移除按钮回调
            RA.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
            RA.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
            RA.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);

            RA.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);
            
            // 移除所有输入回调
            RA.Input.ClearAllAction();
            
            // 移除所有输入状态
            RA.Input.ResetAll();
        }

        // 鼠标左键回调
        void MouseLeft(string name)
        {
            
        }
        
        // 鼠标X轴移动
        void MouseX(float value)
        {
        
        }
        
        void Update()
        {
            if (RA.Input.AnyKeyDown)
            {
                RLog.Log("任意键按下");
            }

            if (RA.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
            {
                RLog.Log("按下组合键Control+Alt+M");
            }

            if (RA.Input.GetButtonDown(InputButtonType.MouseLeft))
            {
                RLog.Log("鼠标左键按下");
            }

            if (RA.Input.GetButton(InputButtonType.MouseRight))
            {
                RLog.Log("鼠标右键按住");
            }

            if (RA.Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick))
            {
                RLog.Log("鼠标左键双击");
            }

            RLog.Log("滚轮：" + RA.Input.GetAxis(InputAxisType.MouseScrollWheel));
            RLog.Log("水平轴线值：" + RA.Input.GetAxis(InputAxisType.Horizontal));
            RLog.Log("垂直轴线值：" + RA.Input.GetAxis(InputAxisType.Vertical));
        }
    }
}
