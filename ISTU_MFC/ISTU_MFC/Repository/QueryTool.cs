using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace ISTU_MFC.Repository
{
    public sealed class QueryTool : IDisposable
    {
        private readonly NpgsqlConnection _dataBase;
    
        public QueryTool(NpgsqlConnection dataBase)
        {
            _dataBase = dataBase;
            _dataBase.Open();
        }

        public int QueryWithoutTable(string query) =>
            new NpgsqlCommand(query, _dataBase).ExecuteNonQuery();
    
        public string[][] QueryWithTable(string query) =>
            ProcessForTable(new NpgsqlCommand(query, _dataBase).ExecuteReader());
    
        private string[][] ProcessForTable(NpgsqlDataReader reader)
        {
            var count = reader.FieldCount;
            var result = new List<string[]>
            {
                Enumerable.Range(1, count).Select((name, i)=> reader.GetName(i).ToString()).ToArray()
            };
            if (!reader.HasRows) return result.ToArray();
            while (reader.Read())
                result.Add(Enumerable.Range(1, count).Select((value, j) => reader.GetValue(j).ToString()).ToArray()!);
        
            return result.ToArray();
        }
        
        public Dictionary<string, string> QueryWithDictionary(string query) =>
            ProcessForDictionary(new NpgsqlCommand(query, _dataBase).ExecuteReader());
    
        private Dictionary<string, string> ProcessForDictionary(NpgsqlDataReader reader)
        {
            var count = reader.FieldCount;
            var result = new Dictionary<string, string>();
            if (!reader.HasRows) return result;
            reader.Read();
            for (var i = 0; i < count; i++)
                result.Add(reader.GetName(i), reader.GetValue(i).ToString());
        
            return result;
        }

        private bool _isDisposed;

        ~QueryTool() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            _dataBase.Close();
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool fromDisposeMethod)
        {
            if (_isDisposed) return;
            if (fromDisposeMethod)
            {
                _dataBase.Close();
            }
            
            _isDisposed = true;
        }
    }
}