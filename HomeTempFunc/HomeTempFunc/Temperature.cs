using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace HomeTempFunc
{
    class Data
    {
        public DateTime Timestamp { get; set; }
        public int Temp { get; set; }
        public int humidity { get; set; }
    }

    public static class Temperature
    {
        static string connectionStr = Environment.GetEnvironmentVariable("sqldb_connection");

        [FunctionName("Temperature")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("hello world");

            //string name = req.Query["name"];

            // log.LogInformation(req.Method);

            try
            {
                if (req.Method == "POST")
                {
                    var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    log.LogInformation(requestBody);
                    dynamic data = JsonConvert.DeserializeObject(requestBody);

                    // log.LogInformation(data);
                    if (data != null)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionStr))
                        {
                            conn.Open();

                            // var text = "CREATE TABLE tempTable (Timestamp datetime, Temp int, humidity int, PRIMARY KEY (Timestamp));";
                            // var text = "DROP TABLE tempTable"; //  (Timestamp datetime, Temp int, humidity int)";

                            string text = String.Format("INSERT INTO tempTable (Timestamp, Temp, humidity) VALUES ('{0}', {1}, {2}); ", data?.Timestamp, data?.Temp, data?.humidity);

                            log.LogInformation(text);

                            using (SqlCommand cmd = new SqlCommand(text, conn))
                            {
                                // Execute the command and log the # rows affected.
                                var rows = await cmd.ExecuteNonQueryAsync();
                                log.LogInformation($"{rows} rows were updated");
                            }
                        }
                    }
                } else if (req.Method == "GET")
                {
                    string start = req.Query["start"];
                    string end = req.Query["end"];

                    if(!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                    {
                        using (SqlConnection conn = new SqlConnection(connectionStr))
                        {
                            conn.Open();

                            string text = String.Format("SELECT * FROM tempTable WHERE Timestamp >= '{0}' and TimeStamp <= '{1}'", start, end);

                            log.LogInformation(text);

                            List<Data> ret = new List<Data>();

                            using (SqlCommand cmd = new SqlCommand(text, conn))
                            {
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                adapter.SelectCommand = cmd;
                                DataSet ds = new DataSet();
                                adapter.Fill(ds);
                                var dataList = ds.Tables[0].AsEnumerable()
                                    .Select(dataRow => new Data
                                    {
                                        Timestamp = dataRow.Field<DateTime>("Timestamp"),
                                        Temp = dataRow.Field<int>("Temp"),
                                        humidity = dataRow.Field<int>("humidity"),
                                    }); // .ToList();

                                return new OkObjectResult(dataList);
                                //var reader = cmd.ExecuteReader();
                                //while (reader.Read())
                                //{
                                //    var data = new Data()
                                //    {
                                //        Timestamp = (DateTime)((IDataRecord)reader)[0],
                                //        Temp = (int)((IDataRecord)reader)[1],
                                //        humidity = (int)((IDataRecord)reader)[2]
                                //    };

                                //    ret.Add(data);
                                //}                                
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return new OkObjectResult("hello world");
        }
    }
}
