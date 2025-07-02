using System.Collections.Generic;

namespace Rain.Core
{
    public class FunctionLogView : LogViewBase
    {
        public CommandList commandList = null;

        private int showCommandCount = 0;

        public void InputCheatKey(string cheatKey)
        {
            Function.Ins.InvokeCheatKey(cheatKey);
        }

        private void Update()
        {
            List<Function.CommandData> commands = Function.Ins.GetCommandDatas();

            if (showCommandCount < commands.Count)
            {
                for (int index = showCommandCount; index < commands.Count; ++index)
                {
                    commandList.Insert(commands[index]);
                    ++showCommandCount;
                }
            }
        }
    }
}