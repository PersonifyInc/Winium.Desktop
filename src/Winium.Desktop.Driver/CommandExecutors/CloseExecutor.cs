namespace Winium.Desktop.Driver.CommandExecutors
{
    #region using

    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion

    internal class CloseExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            if (!this.Automator.ActualCapabilities.DebugConnectToRunningApp)
            {
                // If application had exited, find and terminate all children processes
                if (this.Automator.Application.HasExited())
                {
                    List<Process> children = new List<Process>();
                    children = this.Automator.Application.GetChildPrecesses(this.Automator.Application.GetProcessId());
                    if (children.Count == 0)
                    {
                        children = this.Automator.Application.GetAllPrecessesByName(this.Automator.Application.GetProcessName());
                    }
                    foreach (var child in children)
                    {
                        if (!child.HasExited && !this.Automator.Application.Close(child))
                        {
                            this.Automator.Application.Kill(child);
                        }
                    }
                }

                // If application is still running, terminate it as normal case
                else if (!this.Automator.Application.Close())
                {
                    this.Automator.Application.Kill();
                }

                this.Automator.ElementsRegistry.Clear();
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
