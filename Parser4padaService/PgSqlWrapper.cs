using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Parser4padaService
{
    class PgSqlWrapper
    {
        private string _constring;
        private NpgsqlConnection _conn;
        public PgSqlWrapper(string cred)
        {
            //var credline = cred.Replace("\r", "").Split('\n');
            //_constring = $"Host={credline[0]};Port={credline[1]};Username={credline[3]};Password={credline[4]};Database={credline[2]}";
            //_conn = new NpgsqlConnection(_constring);
            _conn = new NpgsqlConnection(cred);
        }
        public void Update(params string[] par)
        {
            _conn.Open();
            NpgsqlCommand command = new NpgsqlCommand()
            {
                Connection = _conn,
                CommandText = $"SELECT add_post('{Int32.Parse(par[0])}','{par[1]}','{par[2]}','{par[3]}');"
            };
            command.ExecuteNonQuery();
            _conn.Close();
        }
    }
}
