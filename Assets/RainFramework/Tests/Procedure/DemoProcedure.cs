using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoProcedure : MonoBehaviour
    {
        void Start()
        {
            // 添加流程节点
            // 可选（会在初始化模块时自动搜索ProcedureNode的子类添加）
            RA.Procedure.AddProcedureNodes(new DemoInitState());

            // 运行指定类型的流程节点
            RA.Procedure.RunProcedureNode<DemoInitState>();

            // 移除指定类型的流程节点
            RA.Procedure.RemoveProcedureNode<DemoInitState>();

            // 检查是否存在指定类型的流程节点
            RA.Procedure.HasProcedureNode<DemoInitState>();

            // 获取指定类型的流程节点
            RA.Procedure.PeekProcedureNode(out DemoInitState initState);

            // 获取当前流程节点
            ProcedureNode procedureNode = RA.Procedure.CurrentProcedureNode;

            // 获取流程节点的数量
            int procedureNodeCount = RA.Procedure.ProcedureNodeCount;
        }
    }

    public class DemoInitState : ProcedureNode
    {
        public override void OnInit(ProcedureProcessor processor)
        {

        }

        public override void OnEnter(ProcedureProcessor processor)
        {

        }

        public override void OnExit(ProcedureProcessor processor)
        {

        }

        public override void OnUpdate(ProcedureProcessor processor)
        {

        }

        public override void OnDestroy(ProcedureProcessor processor)
        {

        }
    }
}
