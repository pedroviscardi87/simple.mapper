using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Mapper
{
    /// <summary>
    /// Classe SQL => Structured Query Language, ou Linguagem de Consulta Estruturada
    /// </summary>
    public class Sql
    {
        /// <summary>
        /// IDENTITY(1,1) or AUTO_INCREMENT
        /// </summary>
        public bool Identity { get; set; }

        /// <summary>
        /// Schema do Banco de Dados: EX: [dbo]
        /// </summary>
        public String Schema { get; set; }

        /// <summary>
        /// Objeto
        /// </summary>
        public Object Object { get; set; }

        /// <summary>
        /// Qual comando SQL deseja utilizar? Insert, Delete, Update ou Select
        /// </summary>
        public EDbOperation Operation { get; set; }

        /// <summary>
        /// Código SQL
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// Parâmetros SQL, exemplo @0,@1,@2,@3
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Construtor Padrão
        /// </summary>
        public Sql()
        {
        }

        /// <summary>
        /// Construtor Padrão para qualquer consulta ou operação no Banco de Dados
        /// </summary>
        /// <param name="sql">Sua SQL (Ex: select * from [tabela])</param>
        public Sql(String sql)
        {
            this.Operation = GetOperation(sql);
            this.Code = sql;
            this.Identity = false;
        }

        /// <summary>
		/// O Construtor SQL retorna sua sql efetuando replace nos campos (@{i}) para values!!
		/// </summary>
		/// <param name="sql">Sua SQL (Ex: insert into table (campo1, campo2) values (@{0},@{1}))</param>
		/// <param name="values">Valores que vão substituir os (?) na SQL</param>
		public Sql(String sql, params object[] values)
        {
            try
            {
                if (values != null && values.Length > 0)
                {
                    this.Parameters = new Dictionary<string, object>();
                    this.Operation = GetOperation(sql);
                    this.Code = sql;

                    for (int i = 0; i < values.Count(); i++)
                    {
                        var variable = "@" + i;
                        this.Parameters.Add(variable, values[i]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
		/// O Construtor SQL retorna sua sql efetuando replace nos campos (@{i}) para values!!
		/// </summary>
        /// <param name="operation">Valores que vão substituir os (?) na SQL</param>
		/// <param name="sql">Sua SQL (Ex: insert into table (campo1, campo2) values (@{0},@{1}))</param>
		/// <param name="values">Valores que vão substituir os (?) na SQL</param>
		public Sql(EDbOperation operation, String sql, params object[] values)
        {
            try
            {
                if (values != null && values.Length > 0)
                {
                    this.Parameters = new Dictionary<string, object>();
                    this.Operation = operation;
                    this.Code = sql;

                    for (int i = 0; i < values.Count(); i++)
                    {
                        var variable = "@" + i;
                        this.Parameters.Add(variable, values[i]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Envie qualquer tipo de Objeto selecionando a Operação Desejado, seu códido SQL será formatado automaticamente.
        /// </summary>
        /// <param name="schema">Exemplo [dbo]</param>
        /// <param name="obj">Qualquer tipo de objeto com os tipos de dados em atributos (String, DateTime, int, Boolean)</param>
        /// <param name="operation">Insert, Delete, Update</param>
        /// <param name="identity">Se o campo PK é Identity incremental</param>
        public Sql(String schema, Object obj, EDbOperation operation, bool identity)
        {
            try
            {
                this.Schema = schema;
                this.Object = obj;
                this.Operation = operation;

                var type = this.Object.GetType();
                var properties = type.GetProperties();

                if (properties.Count() > 0)
                {
                    var first = false;
                    var id = type.GetProperty("id").GetValue(this.Object).ToString();
                    //var identifier = type.GetProperty("identifier").GetValue(this.Object).ToString();

                    switch (this.Operation)
                    {
                        case EDbOperation.Insert:
                            {
                                var sqlPart1 = new StringBuilder();
                                var sqlPart2 = new StringBuilder();

                                this.Parameters = new Dictionary<string, object>();
                                sqlPart1.Append("insert into [" + schema + "].[" + type.Name.ToLower() + "] (");

                                foreach (var prop in properties)
                                {
                                    //verify identity(1,1)
                                    if (identity)
                                    {
                                        if (prop != null && prop.Name != "id")
                                        {
                                            if (!first && prop.GetValue(this.Object) != null)
                                            {
                                                first = true; //Primeiro item da SQL
                                                sqlPart1.Append("[" + GetName(prop, this.Object) + "]");
                                                sqlPart2.Append("@" + GetName(prop, this.Object));
                                                this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                            }
                                            else if (first && prop.GetValue(this.Object) != null)
                                            {
                                                sqlPart1.Append(",[" + GetName(prop, this.Object) + "]");
                                                sqlPart2.Append(",@" + GetName(prop, this.Object));
                                                this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!first && prop.GetValue(this.Object) != null)
                                        {
                                            first = true; //Primeiro item da SQL
                                            sqlPart1.Append("[" + GetName(prop, this.Object) + "]");
                                            sqlPart2.Append("@" + GetName(prop, this.Object));
                                            this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                        }
                                        else if (first && prop.GetValue(this.Object) != null)
                                        {
                                            sqlPart1.Append(",[" + GetName(prop, this.Object) + "]");
                                            sqlPart2.Append(",@" + GetName(prop, this.Object));
                                            this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                        }
                                    }
                                }

                                this.Code = sqlPart1.ToString() + ") values (" + sqlPart2.ToString() + ");";

                                break;
                            }
                        case EDbOperation.Delete:
                            {
                                this.Code = "delete from [" + schema + "].[" + type.Name.ToLower() + "] where [id]=@id;";
                                this.Parameters = new Dictionary<string, object>();
                                this.Parameters.Add("@id", id);

                                break;
                            }
                        case EDbOperation.Update:
                            {
                                var sql = new StringBuilder();

                                this.Parameters = new Dictionary<string, object>();
                                sql.Append("update [" + schema + "].[" + type.Name.ToLower() + "] set ");

                                foreach (var prop in properties)
                                {
                                    if (prop != null && prop.Name != "id")
                                        if (!first && prop.GetValue(this.Object) != null)
                                        {
                                            first = true;
                                            sql.Append("[" + GetName(prop, this.Object) + "]=@" + GetName(prop, this.Object));
                                            this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                        }
                                        else if (first && prop.GetValue(this.Object) != null)
                                        {
                                            sql.Append(",[" + GetName(prop, this.Object) + "]=@" + GetName(prop, this.Object));
                                            this.Parameters.Add("@" + GetName(prop, this.Object), GetValue(prop, this.Object));
                                        }
                                }
                                sql.Append(" where [id]=@id;");

                                this.Parameters.Add("@id", id);
                                this.Code = sql.ToString();

                                break;
                            }
                        case EDbOperation.Select:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Properties

        /// <summary>
        /// Retorna o nome da Propriedade com base nos dados do BANCO (Class.cs) => [dbo].[class]
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected String GetName(PropertyInfo prop, Object obj)
        {
            var propertyType = prop.PropertyType;
            var name = propertyType != typeof(Decimal) &&
                       propertyType != typeof(float) &&
                       propertyType != typeof(Double) &&
                       propertyType != typeof(Boolean) &&
                       propertyType != typeof(String) &&
                       propertyType != typeof(Int32) &&
                       propertyType != typeof(DateTime) &&
                       propertyType != typeof(DateTime?) &&
                       propertyType != typeof(DateTimeOffset) ? "id_" + prop.Name : prop.Name;
            return name;
        }

        /// <summary>
        /// Retorna o valor da Propriedade com base nos dados do BANCO (Class.cs) => [dbo].[class]
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected object GetValue(PropertyInfo prop, Object obj)
        {
            var propertyType = prop.PropertyType;

            if (propertyType == typeof(Decimal))
                return prop.GetValue(obj);
            if (propertyType == typeof(float))
                return prop.GetValue(obj);
            if (propertyType == typeof(Double))
                return prop.GetValue(obj);
            else if (propertyType == typeof(Boolean))
                return prop.GetValue(obj);
            else if (propertyType == typeof(String))
                return prop.GetValue(obj);
            else if (propertyType == typeof(DateTime))
                return prop.GetValue(obj);
            else if (propertyType == typeof(DateTime?))
                return prop.GetValue(obj);
            else
                return prop.GetValue(obj).GetType().GetProperties().Count() > 0 ? prop.GetValue(obj).GetType().GetProperties()[0].GetValue(prop.GetValue(obj)) : prop.GetValue(obj);
        }

        /// <summary>
        /// GetOperation - Retorna a operação de acordo com a SQL
        /// </summary>
        /// <param name="sql">Insert, Delete, Update ou Select</param>
        /// <returns></returns>
        protected EDbOperation GetOperation(String sql)
        {
            if (sql.StartsWith("insert "))
                return EDbOperation.Insert;
            else if (sql.StartsWith("delete "))
                return EDbOperation.Delete;
            else if (sql.StartsWith("update "))
                return EDbOperation.Update;
            else
                return EDbOperation.Select;
        }

        #endregion
    }
}
