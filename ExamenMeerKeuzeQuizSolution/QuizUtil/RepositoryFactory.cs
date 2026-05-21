using MeerKeuzeDL.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using MeerKeuzeBL.Interface;

namespace QuizUtil
{
    public class RepositoryFactory
    {



       
            // We gaan ervanuit dat je repository de connection string nodig heeft
            public static IVragenRepository CreateVragenRepository(string databaseType, string connectionString)
            {
                switch (databaseType.Trim().ToUpper())
                {
                    case "SQL":
                        return new VragenRepository(connectionString);

                    // Optioneel voor later:
                    // case "MOCK":
                    //     return new TestVragenRepository();

                    default:
                        throw new Exception($"Database type '{databaseType}' wordt niet ondersteund.");
                }
            }
        }
    }


