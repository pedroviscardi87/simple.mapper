using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Mapper
{
    /// <summary>
    /// DataBase Provider - Qual banco de dados vai utilizar? MySQL ou SQL Server
    /// </summary>
	public enum EDbProvider
	{
        /// <summary>
        /// Microsoft® SQL Server
        /// </summary>
		SqlServer = 1,

        /// <summary>
        /// MySQL - Oracle®
        /// </summary>
		MySql = 2
	}
}
