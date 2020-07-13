using System;

namespace Core
{
    public class MenuAction : CoreBase
    {
        public Action SelectionAction { get; private set; }

        public MenuAction(string name,  Action selectionAction) : base(name, null, InfectionState.Null, ScanResult.Null)
        {
            SelectionAction = selectionAction;
        }
    }
}