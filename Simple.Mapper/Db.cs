using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Simple.Mapper
{
    /// <summary>
    /// Clase Db - Database
    /// </summary>
    public class Db
    {

        /// <summary>
        /// Chave para ligar / desligar o Console de SQL's no Debug.Write("")
        /// </summary>
        public Boolean ShowConsoleSql { get; set; }

        /// <summary>
        /// Qual Banco de Dados deseja utilizar? MySql ou SqlServer?
        /// </summary>
        public EDbProvider SetDbProvider { get; set; }

        /// <summary>
        /// String de Conexão
        /// </summary>
        public String ConnectionString { get; set; }

        /// <summary>
        /// SQL Server Conexão
        /// </summary>
        protected SqlConnection SqlServer { get; set; }

        /// <summary>
        /// SQL Server Transação
        /// </summary>
        protected SqlTransaction SqlServerTransaction { get; set; }

        /// <summary>
        /// SQL Server Data
        /// </summary>
        protected SqlDataReader SqlServerDataReader { get; set; }

        /// <summary>
        /// MySQL Conexão
        /// </summary>
        protected MySqlConnection MySqlServer { get; set; }
        
        /// <summary>
        /// MySQL Transação
        /// </summary>
        protected MySqlTransaction MySqlTransaction { get; set; }
        
        /// <summary>
        /// MySQL Data
        /// </summary>
        protected MySqlDataReader MySqlDataReader { get; set; }

        #region Construtores

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Db()
        {

        }

        /// <summary>
        /// Construtor de Criação
        /// </summary>
        /// <param name="ConnectionString">String de Conexão com o Banco de Dados</param>
        /// <param name="SetDbProvider">Qual banco você vai utilizar? MySQL ou SQL Server?</param>
        /// <param name="ShowConsoleSql">Você gostaria de visualizar as SQL's no Console DEBUG?</param>
        public Db(String ConnectionString, EDbProvider SetDbProvider, Boolean ShowConsoleSql = false)
        {
            this.ConnectionString = ConnectionString;
            this.SetDbProvider = SetDbProvider;
            this.ShowConsoleSql = ShowConsoleSql;
        }
        #endregion

        /// <summary>
        /// Conectar ao Banco de Dados (Sql Server ou MySql)
        /// </summary>
        /// <returns>Retorno Boolean (True ou False)</returns>
        public bool Connect()
        {
            try
            {
                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            this.SqlServer = new SqlConnection(ConnectionString);
                            this.SqlServer.Open();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            this.MySqlServer = new MySqlConnection(ConnectionString);
                            this.MySqlServer.Open();
                            break;
                        }
                    default:
                        {
                            this.SqlServer = new SqlConnection(ConnectionString);
                            this.SqlServer.Open();
                            break;
                        }
                }
                return true;
            }
            catch (SqlException ex)
            {
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Desconectar ao Banco de Dados (Sql Server ou MySql)
        /// </summary>
        /// <returns>Retorno Boolean (True ou False)</returns>
        public bool Disconnect()
        {
            try
            {
                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            this.SqlServer.Dispose();
                            this.SqlServer.Close();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            this.MySqlServer.Dispose();
                            this.MySqlServer.Close();
                            break;
                        }
                    default:
                        {
                            this.SqlServer.Dispose();
                            this.SqlServer.Close();
                            break;
                        }
                }
                return true;
            }
            catch (SqlException ex)
            {
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }

        }

        /// <summary>
        /// Execução de Comandos SQL do tipo sem Consulta, ExecuteNonQuery(); Insert, Update, Delete.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool Execute(Sql sql)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                this.Connect();

                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            MySqlCommand cmd = new MySqlCommand(sql.Code, this.MySqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            break;
                        }
                    default:
                        {
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            break;
                        }
                }

                this.Disconnect();
                stopwatch.Stop();

                if (this.ShowConsoleSql)
                    Debug.WriteLine(GetMessage(sql, stopwatch.Elapsed));

                return true;
            }
            catch (SqlException ex)
            {
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Execução de Comandos SQL do tipo sem Consulta, ExecuteNonQuery(); Insert, Update, Delete. com Transações
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(Sql sql)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                this.Connect();

                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            this.SqlServerTransaction = this.SqlServer.BeginTransaction();
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.Transaction = this.SqlServerTransaction;
                            cmd.ExecuteNonQuery();
                            this.SqlServerTransaction.Commit();
                            this.SqlServerTransaction.Dispose();
                            cmd.Dispose();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            this.MySqlTransaction = this.MySqlServer.BeginTransaction();
                            MySqlCommand cmd = new MySqlCommand(sql.Code, this.MySqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.Transaction = this.MySqlTransaction;
                            cmd.ExecuteNonQuery();
                            this.MySqlTransaction.Commit();
                            this.MySqlTransaction.Dispose();
                            cmd.Dispose();
                            break;
                        }
                    default:
                        {
                            this.SqlServerTransaction = SqlServer.BeginTransaction();
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.Transaction = SqlServerTransaction;
                            cmd.ExecuteNonQuery();
                            this.SqlServerTransaction.Commit();
                            this.SqlServerTransaction.Dispose();
                            cmd.Dispose();
                            break;
                        }
                }

                this.Disconnect();
                stopwatch.Stop();

                if (ShowConsoleSql)
                    Debug.WriteLine(GetMessage(sql, stopwatch.Elapsed));

                return true;
            }
            catch (SqlException ex)
            {
                this.SqlServerTransaction.Rollback();
                this.MySqlTransaction.Rollback();
                this.Disconnect();

                if (ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                this.SqlServerTransaction.Rollback();
                this.MySqlTransaction.Rollback();
                this.Disconnect();

                if (ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Execução de Comandos SQL do tipo sem Consulta, ExecuteScalar(); Insert com Retorno de ScopeIdentity
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Int32 ExecuteScalar(Sql sql)
        {
            try
            {
                Int32 executeScalar = new Int32();
                Stopwatch stopwatch = new Stopwatch();
                //count time
                stopwatch.Start();
                this.Connect();
                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            this.SqlServerTransaction = this.SqlServer.BeginTransaction();
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer, this.SqlServerTransaction);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand("select cast(@@identity as int)", this.SqlServer, this.SqlServerTransaction);
                            executeScalar = (Int32)cmd.ExecuteScalar();
                            cmd.Dispose();
                            this.SqlServerTransaction.Commit();
                            this.SqlServerTransaction.Dispose();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            this.MySqlTransaction = this.MySqlServer.BeginTransaction();
                            MySqlCommand cmd = new MySqlCommand(sql.Code, this.MySqlServer, this.MySqlTransaction);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd = new MySqlCommand("select cast(@@identity as int)", this.MySqlServer, this.MySqlTransaction);
                            executeScalar = (Int32)cmd.ExecuteScalar();
                            cmd.Dispose();
                            this.MySqlTransaction.Commit();
                            this.MySqlTransaction.Dispose();
                            break;
                        }
                    default:
                        {
                            this.SqlServerTransaction = this.SqlServer.BeginTransaction();
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer, this.SqlServerTransaction);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand("SELECT CAST(scope_identity() AS int)", this.SqlServer, this.SqlServerTransaction);
                            executeScalar = (Int32)cmd.ExecuteScalar();
                            cmd.Dispose();
                            this.SqlServerTransaction.Commit();
                            this.SqlServerTransaction.Dispose();
                            break;
                        }
                }
                //disconnect
                this.Disconnect();
                //stop count time
                stopwatch.Stop();
                //show console
                if (this.ShowConsoleSql)
                    Debug.WriteLine(GetMessage(sql, stopwatch.Elapsed));
                return executeScalar;
            }
            catch (SqlException ex)
            {
                this.SqlServerTransaction.Rollback();
                this.MySqlTransaction.Rollback();
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                this.SqlServerTransaction.Rollback();
                this.MySqlTransaction.Rollback();
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Dados de um comando SQL para consultas no Banco de Dados.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Select(Sql sql)
        {
            try
            {
                DataTable dt = new DataTable();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                this.Connect();

                switch (this.SetDbProvider)
                {
                    case EDbProvider.SqlServer:
                        {
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                                dt = GetData(dr);
                            cmd.Dispose();
                            break;
                        }
                    case EDbProvider.MySql:
                        {
                            MySqlCommand cmd = new MySqlCommand(sql.Code, this.MySqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            MySqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                                dt = GetData(dr);
                            cmd.Dispose();
                            break;
                        }
                    default:
                        {
                            SqlCommand cmd = new SqlCommand(sql.Code, this.SqlServer);
                            if (sql.Parameters != null && sql.Parameters.Count > 0)
                                foreach (var param in sql.Parameters)
                                    cmd.Parameters.AddWithValue(param.Key, param.Value == null ? DBNull.Value : param.Value);
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                                dt = GetData(dr);
                            cmd.Dispose();
                            break;
                        }
                }

                this.Disconnect();
                stopwatch.Stop();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(GetMessage(sql, stopwatch.Elapsed));

                return dt;
            }
            catch (SqlException ex)
            {
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
            catch (MySqlException ex)
            {
                this.Disconnect();
                if (this.ShowConsoleSql)
                    Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        #region Properties

        /// <summary>
        /// Retorna a mensagem com o tempo de execução da SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        protected String GetMessage(Sql sql, TimeSpan stopwatch)
        {
            if (sql.Operation == EDbOperation.Insert)
                return "Insert em " + stopwatch.ToString("hh:mm:ss.fff tt") + " Código SQL:[" + sql.Code + "]" + GetParametersMessage(sql.Parameters);
            if (sql.Operation == EDbOperation.Update)
                return "Update em " + stopwatch.ToString("hh:mm:ss.fff tt") + " Código SQL:[" + sql.Code + "]" + GetParametersMessage(sql.Parameters);
            if (sql.Operation == EDbOperation.Delete)
                return "Delete em " + stopwatch.ToString("hh:mm:ss.fff tt") + " Código SQL:[" + sql.Code + "]" + GetParametersMessage(sql.Parameters);
            if (sql.Operation == EDbOperation.Select)
                return "Select em " + stopwatch.ToString("hh:mm:ss.fff tt") + " Código SQL:[" + sql.Code + "]" + GetParametersMessage(sql.Parameters);
            return String.Empty;
        }

        /// <summary>
        /// Retorna os parâmetros utilizados na SQL
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected String GetParametersMessage(Dictionary<string, object> parameters)
        {
            if (parameters == null)
                return "";
            if (parameters.Count == 0)
                return "";
            StringBuilder message = new StringBuilder();
            message.Append(" Param: [");
            if (parameters.Count > 0)
                foreach (var param in parameters)
                    message.Append("key:" + param.Key + " value:" + param.Value + "; ");
            message.Append("]");
            return message.ToString();
        }

        /// <summary>
        /// Retorna a consulta em formato DataTable para manutenção e utilização das ROWS
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected DataTable GetData(object reader)
        {
            DataTable tbEsquema = new DataTable();
            DataTable tbRetorno = new DataTable();

            if (this.SetDbProvider == EDbProvider.SqlServer)
            {
                this.SqlServerDataReader = reader as SqlDataReader;
                tbEsquema = this.SqlServerDataReader.GetSchemaTable();
            }
            else
            {
                this.MySqlDataReader = reader as MySqlDataReader;
                tbEsquema = this.MySqlDataReader.GetSchemaTable();
            }

            foreach (DataRow r in tbEsquema.Rows)
                if (!tbRetorno.Columns.Contains(r["ColumnName"].ToString()))
                    tbRetorno.Columns.Add(new DataColumn()
                    {
                        ColumnName = r["ColumnName"].ToString(),
                        Unique = Convert.ToBoolean(r["IsUnique"]),
                        AllowDBNull = Convert.ToBoolean(r["AllowDBNull"]),
                        ReadOnly = Convert.ToBoolean(r["IsReadOnly"])
                    });

            while (this.SetDbProvider == EDbProvider.SqlServer ? this.SqlServerDataReader.Read() : this.MySqlDataReader.Read())
            {
                DataRow novaLinha = tbRetorno.NewRow();
                for (int i = 0; i < tbRetorno.Columns.Count; i++)
                    novaLinha[i] = this.SetDbProvider == EDbProvider.SqlServer ? this.SqlServerDataReader.GetValue(i) : this.MySqlDataReader.GetValue(i);
                tbRetorno.Rows.Add(novaLinha);
            }

            return tbRetorno;
        }

        #endregion
    }
}
