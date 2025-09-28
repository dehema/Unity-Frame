using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����״̬ö��
/// </summary>
public enum BuildingState
{
    /// <summary>
    /// �ղ�λ
    /// </summary>
    Empty = 0,

    /// <summary>
    /// ������
    /// </summary>
    Building = 1,

    /// <summary>
    /// �������
    /// </summary>
    Completed = 2,

    /// <summary>
    /// ������
    /// </summary>
    Upgrading = 3,

    /// <summary>
    /// ��
    /// </summary>
    Damaged = 4,

    /// <summary>
    /// ���ݻ�
    /// </summary>
    Destroyed = 5,
}
