using LetsSparql.Common.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LetsSparql.Service
{
    public interface ISparqlExecuter
    {
        Task<SparqlResponse> GetResponse(string endPointUrl, string queryString, bool explain, bool update);       
        
        
    }
}
