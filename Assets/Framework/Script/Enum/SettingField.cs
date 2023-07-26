using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SettingField
{
    public class UI
    {
        /// <summary>
        /// 血条显示时间
        /// </summary>
        public const string HPSliderDisplay = "HPSliderDisplay";
    }

    public class World
    {
        /// <summary>
        /// 地图尺寸
        /// </summary>
        public const string World_Size = "World_Size";
        /// <summary>
        /// 大地图_单元像素大小（大地图上视为一个单元格的像素大小）
        /// </summary>
        public const string World_UnitPixelSize = "World_UnitPixelSize";
        /// <summary>
        /// 大地图_相机_移动速度
        /// </summary>
        public const string World_Camera_MoveSpeed = "World_Camera_MoveSpeed";
        /// <summary>
        /// 大地图_相机_拖拽速度
        /// </summary>
        public const string World_Camera_DragSpeed = "World_Camera_DragSpeed";
        /// <summary>
        /// 大地图_相机_高度区间
        /// </summary>
        public const string World_Camera_ZoomHeight = "World_Camera_ZoomHeight";
        /// <summary>
        /// 大地图_相机_角度区间
        /// </summary>
        public const string World_Camera_ZoomRot = "World_Camera_ZoomRot";
        /// <summary>
        /// 大地图_相机_缩放速度
        /// </summary>
        public const string World_Camera_ZoomSpeed = "World_Camera_ZoomSpeed";
        /// <summary>
        /// 大地图_玩家_移动速度 
        /// </summary>
        public const string World_Player_MoveSpeed = "World_Player_MoveSpeed";
        /// <summary>
        /// 大地图_单位_X轴旋转量
        /// </summary>
        public const string World_Unit_RotX = "World_Unit_RotX";
        /// <summary>
        /// 大地图_商队_移动速度
        /// </summary>
        public const string World_Trade_MoveSpeed = "World_Trade_MoveSpeed";
        /// <summary>
        /// 大地图_商队_初始资金
        /// </summary>
        public const string World_Trade_InitGold = "World_Trade_InitGold";
    }

    public class Time
    {
        /// <summary>
        /// 大地图_时间_普通的流速
        /// </summary>
        public const string World_Time_NormalSpeed = "World_Time_NormalSpeed";
        /// <summary>
        /// 大地图_时间_加快的流速
        /// </summary>
        public const string World_Time_QuickSpeed = "World_Time_QuickSpeed";
    }
}
