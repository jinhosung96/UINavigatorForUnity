#if CSV_HELPER_SUPPORT
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoraeGames.Library.Util.Parser
{
    public static class CSVParser
    {
        #region Public Methods

        public static List<T> ReadCsv<T>(string filePath)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#") || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0]),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);
            
            return csv.GetRecords<T>().ToList();
        }

        public static List<T> ReadCsv<T, U>(string filePath) where U : ClassMap
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#") || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0]),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<U>();
            
            return csv.GetRecords<T>().ToList();
        }

        public static List<Dictionary<string, object>> ReadCsv(string filePath)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#") || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0]),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);

            var records = new List<Dictionary<string, object>>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Dictionary<string, object>();
                foreach (var header in csv.HeaderRecord)
                {
                    record[header] = csv.GetField(header);
                }

                records.Add(record);
            }

            return records;
        }

        public static List<T> ReadCsv<T>(string filePath, Func<ShouldSkipRecordArgs, bool> shouldSkipRecord)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#")
                                             || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0])
                                             || shouldSkipRecord(record),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);
            
            return csv.GetRecords<T>().ToList();
        }

        public static List<T> ReadCsv<T, U>(string filePath, Func<ShouldSkipRecordArgs, bool> shouldSkipRecord) where U : ClassMap
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#")
                                             || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0])
                                             || shouldSkipRecord(record),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<U>();
            
            return csv.GetRecords<T>().ToList();
        }

        public static List<Dictionary<string, object>> ReadCsv(string filePath, Func<ShouldSkipRecordArgs, bool> shouldSkipRecord)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(filePath);
            if (csvFile == null)
            {
                Debug.Debug.LogError("CSV file not found in Resources folder");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record[0].StartsWith("#")
                                             || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0])
                                             || shouldSkipRecord(record),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StringReader(csvFile.text);
            using var csv = new CsvReader(reader, config);

            var records = new List<Dictionary<string, object>>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Dictionary<string, object>();
                foreach (var header in csv.HeaderRecord)
                {
                    record[header] = csv.GetField(header);
                }

                records.Add(record);
            }

            return records;
        }

        #endregion
    }
}
#endif