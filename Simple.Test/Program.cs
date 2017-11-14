using Simple.Mapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Simple.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Marcas marca = new Marcas()
            {
                nome = "Marca 1",
                ativo = true
            };
            marca.id = marca.Insert();
        }
    }

    public class Marcas
    {
        public int id { get; set; }
        public string nome { get; set; }
        public bool ativo { get; set; }

        public Marcas()
        {

        }

        #region Insert

        /// <summary>
        /// Insert utilizando a escrita da SQL
        /// </summary>
        /// <returns></returns>
        public int Insert()
        {
            try
            {
                Sql sql = new Sql("insert into [dbo].[marcas]([nome],[ativo]) values (@0,@1);", this.nome, this.ativo);
                return new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true).ExecuteScalar(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserindo objeto - Monta a SQL automaticamente pelo mapeador de acordo com o Objeto
        /// </summary>
        /// <returns></returns>
        public int InsertObject()
        {
            try
            {
                Sql sql = new Sql("dbo", this, EDbOperation.Insert, true);
                return new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true).ExecuteScalar(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Update utilizando a escrita da SQL
        /// </summary>
        /// <returns></returns>
        public Boolean Update()
        {
            try
            {
                Sql sql = new Sql("update [dbo].[marcas] set [nome]=@0, [ativo]=@1 where [id]=@2;", this.nome, this.ativo, this.id);
                return new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true).Execute(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Atualizando objeto - Monta a SQL automaticamente pelo mapeador de acordo com o Objeto
        /// </summary>
        /// <returns></returns>
        public Boolean UpdateObject()
        {
            try
            {
                Sql sql = new Sql("dbo", this, EDbOperation.Update, true);
                return new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true).Execute(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete utilizando a escrita da SQL
        /// </summary>
        /// <returns></returns>
        public Boolean Delete()
        {
            try
            {
                Sql sql = new Sql("delete from [dbo].[marcas] where [id]=@0;", this.id);
                return new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true).Execute(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Select

        public Marcas GetById(int id)
        {
            try
            {
                using (var dt = new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true)
                    .Select(new Sql("select * from [dbo].[marcas] where [id]=@0", id)))
                    if (dt.Rows.Count > 0)
                        return (from DataRow row in dt.Rows
                                select new Marcas
                                {
                                    id = Convert.ToInt32(row["id"]),
                                    nome = Convert.ToString(row["nome"]),
                                    ativo = row["ativo"] != DBNull.Value ? Convert.ToBoolean(row["ativo"]) : false
                                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<Marcas> GetAll(string where = "")
        {
            try
            {
                using (var dt = new Db(ConfigurationManager.ConnectionStrings["conn"].ConnectionString, EDbProvider.SqlServer, true)
                    .Select(new Sql("select * from [dbo].[marcas] " + where)))
                    if (dt.Rows.Count > 0)
                        return (from DataRow row in dt.Rows
                                select new Marcas
                                {
                                    id = Convert.ToInt32(row["id"]),
                                    nome = Convert.ToString(row["nome"]),
                                    ativo = row["ativo"] != DBNull.Value ? Convert.ToBoolean(row["ativo"]) : false
                                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        #endregion

    }
}
