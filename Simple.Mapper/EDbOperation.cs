using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Mapper
{
    /// <summary>    
    /// DQL - Data Query Language, ou LInguagem de Consulta de Dados.
    /// DML - Data Manipulation Language, ou Linguagem de Manipulação de Dados. 
    /// </summary>
	public enum EDbOperation
	{
        /// <summary>
        /// Inserção de dados
        /// </summary>
		Insert = 1,

        /// <summary>
        /// Deleção dos dados
        /// </summary>
		Delete = 2,

        /// <summary>
        /// Atualização dos dados
        /// </summary>
		Update = 3,

        /// <summary>
        /// Consulta
        /// </summary>
		Select = 4
	}
}
