using System;
using System.Collections.Generic;

using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocol protocol)
    {
        try
        {
			Random r = new Random();
			List<object[]> rows = new List<object[]>();
			for (int i = 0; i < 5; i++)
			{
				object[] row = new DataQActionRow
				{
					Data_index = i.ToString(),
					Data_name = "Row " + i,
					Data_number = r.Next(0, 101),
				};
				rows.Add(row);
			}

			protocol.FillArray(Parameter.Data.tablePid, rows, NotifyProtocol.SaveOption.Full);
        }
        catch (Exception ex)
        {
            protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
        }
    }
}
