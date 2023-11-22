using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LetsSparql.Common.Dto
{
    public class SparqlResponse
    {
        public string Response { get; set; }
        public string Error { get; set; }
        public double TimeTaken { get; set; }
        public int TotalRecords { get; set; }
        public DataTable dataTableFormat { get; set; }
    }
}
