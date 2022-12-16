using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace MMSDemoTrucks.HelperQueries
{
    public class Queries
    {
        private readonly string conn;
        public Queries(string conn)
        {
            this.conn = conn;
        }

        public string ReadFrom(string entity, string filterField = null, string from = null, string to = null)
        {
            string jsonRes = string.Empty;
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            string par1Name = "from";
            string par2Name = "to";
            bool createParameters = false;
            NpgsqlParameter sqlParam = null;
            StringBuilder sb = new StringBuilder(string.Empty);

            sb = sb.Append($"select * from {entity.ToLower()}");
            if (!string.IsNullOrEmpty(filterField) && (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to)))
            {
                if (from is null)
                    from = string.Empty;
                if (to is null)
                    to = string.Empty;
                sb = sb.Append($" where {filterField.ToLower()} between @{par1Name} and @{par2Name}");
                createParameters = true;
            }

            try 
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = new NpgsqlConnection(this.conn);
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sb.ToString();
                    if (createParameters)
                    {
                        sqlParam = new NpgsqlParameter();
                        sqlParam.ParameterName = par1Name;
                        sqlParam.Direction = ParameterDirection.Input;
                        sqlParam.Value = from;
                        cmd.Parameters.Add(sqlParam);

                        sqlParam = new NpgsqlParameter();
                        sqlParam.ParameterName = par2Name;
                        sqlParam.Direction = ParameterDirection.Input;
                        sqlParam.Value = to;
                        cmd.Parameters.Add(sqlParam);
                    }
                    //using (cmd)
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //List<Tuple<string, object>> row = new List<Tuple<string, object>>();
                            Dictionary<string, string> row = new Dictionary<string, string>();
                            int nCols = reader.FieldCount;
                            for (int i = 0; i < nCols; i++)
                                row.Add(reader.GetName(i), reader[i].ToString());
                            table.Add(row);
                        }
                        reader.Close();
                        cmd.Connection.Close();
                    }
                }           
            }
            catch (NpgsqlException exc)
            {
                //do nothing;
            }
            finally
            {
                jsonRes = JsonConvert.SerializeObject(table);
            }

            return jsonRes;
        }
    
        public List<string> ReadFromLogical(string entity, string owner, bool isMaster, bool isOneToMany = false)
        {
            List<string> table = new List<string>();
            NpgsqlParameter sqlParam = null;
            string physicalTable = isMaster ? "tb_custommasters" : isOneToMany ? "tb_customslavens" : "tb_customslaves";
            StringBuilder sb = new StringBuilder($"select content from {physicalTable} where tablename=@par1 and owner=@par2");
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = new NpgsqlConnection(this.conn);
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sb.ToString();
                    
                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par1";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = entity;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par2";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = owner;
                    cmd.Parameters.Add(sqlParam);
                    
                    //using (cmd)
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            table.Add(reader.GetString("content"));
                        }
                        reader.Close();
                        cmd.Connection.Close();
                    }
                }
            }
            catch (NpgsqlException exc)
            {
                //do nothing;
            }
            finally
            {
                //do nothing
            }

            return table;
        }
    
        public bool ClearFromLogical(string entity, string owner, bool isMaster, bool isOneToMany = false)
        {
            bool success = true;
            NpgsqlParameter sqlParam = null;
            string physicalTable = isMaster ? "tb_custommasters" : isOneToMany ? "tb_customslavens" : "tb_customslaves";
            StringBuilder sb = new StringBuilder($"delete from {physicalTable} where tablename=@par1 and owner=@par2");
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = new NpgsqlConnection(this.conn);
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sb.ToString();

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par1";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = entity;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par2";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = owner;
                    cmd.Parameters.Add(sqlParam);

                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    
                }
            }
            catch (NpgsqlException exc)
            {
                //do nothing;
                success = false;
            }
            finally
            {
                //do nothing
            }

            return success;
        }

        public bool SaveEntityIntoLogicalTable(string entity, string owner, 
                    string jsonContent, string keys, string parentkeys, bool isMaster, bool isOneToMany = false)
        {
            bool success = true;
            NpgsqlParameter sqlParam = null;
            string physicalTable = isMaster ? "tb_custommasters" : isOneToMany ? "tb_customslavens" : "tb_customslaves";
            StringBuilder sb = new StringBuilder($"insert into {physicalTable} (tablename, owner, keys, parentkeys, content) values (@par1, @par2, @par3, @par4, @par5)");

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = new NpgsqlConnection(this.conn);
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sb.ToString();

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par1";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = entity;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par2";
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = owner;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par3"; //keys
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = keys;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par4"; //ParentKeys
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = parentkeys;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new NpgsqlParameter();
                    sqlParam.ParameterName = "par5"; //Content
                    sqlParam.Direction = ParameterDirection.Input;
                    sqlParam.Value = jsonContent;
                    cmd.Parameters.Add(sqlParam);

                    success = cmd.ExecuteNonQuery() > 0;
                    cmd.Connection.Close();
                }
            }
            catch (NpgsqlException exc)
            {
                //do nothing;
                success = false;
            }
            finally
            {
                //do nothing
            }

            return success;
        }
    }
}
