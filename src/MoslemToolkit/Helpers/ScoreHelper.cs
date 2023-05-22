using MoslemToolkit.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Helpers
{
    public class ScoreHelper
    {
        static DataTable scoreData;

        public static string getScore(int Nilai)
        {
            if (scoreData == null)
            {
                scoreData = CSVReader.ConvertCSVtoDataTable(AppConstants.SCORE_DATA);

            }
            foreach(DataRow dr in scoreData.Rows)
            {
                var formula = dr["Formula"].ToString().Replace("[A]", Nilai.ToString());
                if (LogicEvaluator.EvaluateLogicalExpression(formula))
                {
                    return dr["Nilai"].ToString();
                }
               
            }
            return "";
        }
    }

    
}
