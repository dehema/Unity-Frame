// code generation.

using System.Collections.Generic;

namespace Rain.Core
{
    /// <summary>
    /// ��Դӳ���
    /// </summary>
    public static class ResMap
    {
        /// <summary>
        /// ��Դӳ����Ϣ<assetName,ResMapping>
        /// </summary>
        public static Dictionary<string, ResMapping> ResMappings
        {
            get => _resMappings;
            set => _resMappings = value;
        }

        public static Dictionary<string, ResMapping> _resMappings = new Dictionary<string, ResMapping>();
    }

    /// <summary>
    /// ��Դӳ����Ϣ
    /// </summary>
    public class ResMapping
    {
        public string assetName;    // ��Դ�� AB ���ڵ����ƣ��������ƣ�
        public string logicPath;    // ҵ�����õ�·�����߼�·����
        public string abName;       // ���� AB ������
    }
}