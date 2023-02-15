using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Newtonsoft.Json;

using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.ExportImport.Attributes;
using Skyline.DataMiner.Utils.ExportImport.Factories;
using Skyline.DataMiner.Utils.ExportImport.Readers;
using Skyline.DataMiner.Utils.ExportImport.Writers;

using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class: Export/Import.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Export(SLProtocol protocol)
	{
		try
		{
			List<DataRow> rows = GetRows(protocol);

			string filePath = GetFilePath(protocol);

			Writer<DataRow> writer = WriterFactory.GetWriter<DataRow>(filePath);
			writer.Write(rows);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Export|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Import(SLProtocol protocol)
	{
		try
		{
			string filePath = GetFilePath(protocol);

			Reader<DataRow> reader = ReaderFactory.GetReader<DataRow>(filePath);
			List<DataRow> rows = reader.Read();

			protocol.FillArray(Parameter.Data.tablePid, rows.Select(row => row.ToRow()).ToList(), NotifyProtocol.SaveOption.Full);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Import|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static string GetFilePath(SLProtocol protocol)
	{
		object[] parameters = (object[])protocol.GetParameters(
			new uint[]
			{
				(uint)protocol.GetTriggerParameter(),
				Parameter.filelocation,
			});

		int value = Convert.ToInt32(parameters[0]);
		string extension;
		switch (value)
		{
			case 0:
				extension = "csv";
				break;

			case 1:
				extension = "xml";
				break;

			case 2:
				extension = "json";
				break;

			default:
				extension = "unknown";
				break;
		}

		string folder = Convert.ToString(parameters[1]);
		string filePath = Path.Combine(folder, $"Data.{extension}");
		return filePath;
	}

	private static List<DataRow> GetRows(SLProtocol protocol)
	{
		uint[] columnIdx = new uint[] { Parameter.Data.Idx.data_index, Parameter.Data.Idx.data_name, Parameter.Data.Idx.data_number };
		object[] columns = (object[])protocol.NotifyProtocol((int)NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Data.tablePid, columnIdx);

		object[] indexes = (object[])columns[0];
		object[] names = (object[])columns[1];
		object[] numbers = (object[])columns[2];

		List<DataRow> rows = new List<DataRow>();
		for (int i = 0; i < indexes.Length; i++)
		{
			DataRow row = new DataRow
			{
				Index = Convert.ToString(indexes[i]),
				Name = Convert.ToString(names[i]),
				Number = Convert.ToInt16(numbers[i]),
				UnrelatedProperty = "Random thing " + i,
			};

			rows.Add(row);
		}

		return rows;
	}

	/// <summary>
	/// Has to be public for XML.
	/// </summary>
	public class DataRow
	{
		/// <summary>
		/// Gets or sets an index.
		/// </summary>
		[CsvHeader("PK")]
		[JsonProperty("PK")]
		[XmlElement("MyIndex")]
		public string Index { get; set; }

		/// <summary>
		/// Gets or sets a name.
		/// </summary>
		[CsvHeader("MyName")]
		[JsonProperty("MyName")]
		[XmlElement("MyName")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a number.
		/// </summary>
		[CsvHeader("MyNumber")]
		public int Number { get; set; }

		/// <summary>
		/// Gets or sets an unrelated property.
		/// Unrelated property that we don't want in our exported file.
		/// Will be empty when importing.
		/// </summary>
		[JsonIgnore]
		[XmlIgnore]
		[CsvIgnore]
		public string UnrelatedProperty { get; set; }

		/// <summary>
		/// Convert the DataRow to an object array.
		/// </summary>
		/// <returns>Object array containing the values from the DataRow.</returns>
		public object[] ToRow()
		{
			return new DataQActionRow
			{
				Data_index = Index,
				Data_name = Name,
				Data_number = Number,
			};
		}
	}

	/// <summary>
	/// Example of having a CSV that doesn't need/has header names. Then it can be done based on position.
	/// </summary>
	public class CsvDataRowBasedOnPositions
	{
		/// <summary>
		/// Gets or sets an index.
		/// </summary>
		[CsvHeader(0)]
		public string Index { get; set; }

		/// <summary>
		/// Gets or sets a name.
		/// </summary>
		[CsvHeader(1)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a number.
		/// </summary>
		[CsvHeader(2)]
		public int Number { get; set; }

		/// <summary>
		/// Gets or sets an unrelated property.
		/// Unrelated property that we don't want in our exported file.
		/// Will be empty when importing.
		/// </summary>
		[CsvIgnore]
		public string UnrelatedProperty { get; set; }

		/// <summary>
		/// Convert the DataRow to an object array.
		/// </summary>
		/// <returns>Object array containing the values from the DataRow.</returns>
		public object[] ToRow()
		{
			return new DataQActionRow
			{
				Data_index = Index,
				Data_name = Name,
				Data_number = Number,
			};
		}
	}
}