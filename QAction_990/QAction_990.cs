using System;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Table.ContextMenu;

/// <summary>
/// DataMiner QAction Class: Data - ContextMenu.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    /// <param name="contextMenuData"><see cref="object"/> containing the table ContextMenu data.</param>
    public static void Run(SLProtocol protocol, object contextMenuData)
    {
        try
        {
            ContextMenuTableManagerBasic contextMenu = new ContextMenuTableManagerBasic(
                protocol,
                contextMenuData,
                Parameter.Data.tablePid);
            contextMenu.ProcessContextMenuAction();
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}