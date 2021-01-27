using System;
using System.IO;
using OpenTracing;
using OpenTracing.Util;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using OpenTracing.Tag;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FeedJaeger
{
    class Program
    {
        static void MT()
        { // #################### MT ####################################
            using (StreamReader sr = new StreamReader(@"F:\sourcedata\new\mt.csv"))
            {
                var loggerFactory = new LoggerFactory();
                var senderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();
                var senderConfiguration = new Configuration.SenderConfiguration(loggerFactory)
                    .WithSenderResolver(senderResolver).WithEndpoint("http://192.168.1.34:14268/api/traces"); // optional, defaults to Configuration.SenderConfiguration.DefaultSenderResolver
                var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                    .WithSender(senderConfiguration) // optional, defaults to UdpSender at localhost:6831 when ThriftSenderFactory is registered
                    .WithLogSpans(true);             // optional, defaults to no LoggingReporter
                var tracer = new Configuration("Campaign", loggerFactory)
                    .WithReporter(reporterConfiguration) // optional, defaults to RemoteReporter with UdpSender at localhost:6831 when ThriftSenderFactory is registered
                                                          .WithSampler(new Configuration.SamplerConfiguration(loggerFactory).WithType("const"))
                    .GetTracer();
                string newLine; //  = sr.ReadLine();
                int counter = 0;

                Dictionary<string /*traceid*/, Stack<KeyValuePair<string /*method name*/, long /*spanid*/>>> dict =
                    new Dictionary<string, Stack<KeyValuePair<string, long>>>();

                while (!string.IsNullOrWhiteSpace(newLine = sr.ReadLine()))
                {
                    counter++;
                    if (counter % 100 == 0)
                    {
                        Console.WriteLine(counter);
                    }
                    var columns = newLine.Split(",");

                    var spanName = $"MTTrace: {columns[1]} {columns[2]} {columns[28]}"; // method name
                    var traceidtxt = columns[8]; // RID
                    var traceidBytes = new Guid(traceidtxt).ToByteArray();
                    var high = BitConverter.ToInt64((new ArraySegment<byte>(traceidBytes, 0, 8)).Array);
                    var low = BitConverter.ToInt64((new ArraySegment<byte>(traceidBytes, 8, 8)).Array);
                    var eventType = columns[27];

                    if (!dict.ContainsKey(traceidtxt))
                    {
                        dict.Add(traceidtxt, new Stack<KeyValuePair<string, long>>());
                    }

                    if (eventType == "MethodExit")
                    {
                        if (dict[traceidtxt].Count > 0 && dict[traceidtxt].Peek().Key == spanName)
                        {
                            //found an exit match on stack top
                            //log it and pop stack
                            //timestamp (3), duration (23), start (timestamp - duration)

                            var current = dict[traceidtxt].Pop();
                            KeyValuePair<string, long>? parent = null;
                            if (dict[traceidtxt].Count > 0)
                            {
                                parent = dict[traceidtxt].Peek();
                            }

                            var finishedTime = DateTime.Parse(columns[3]);
                            var startTime = finishedTime.AddMilliseconds(-Convert.ToInt32(columns[23]));
                            var span = tracer.BuildSpan(spanName).WithStartTimestamp(startTime).Start();

                            var traceId = new TraceId(high, low);

                            //if the span has parent, use parent span id, otherwise use traceid (from client)
                            var spanContext = new SpanContext(traceId, new SpanId(current.Value), parent != null ? new SpanId(parent.Value.Value) : new SpanId(traceId), SpanContextFlags.Debug);
                            span.Context = spanContext;
                            span.SetTag(new StringTag("RID"), traceidtxt);
                            span.Finish(finishedTime);

                            ((Tracer)tracer).Reporter.Report((Span)span);

                            continue;
                        }

                        // if we have an exit but no enter, ignore
                    }

                    if (eventType == "MethodEnter")
                    {
                        // get spanid from random guid
                        if (dict[traceidtxt].Count > 0)
                        {
                            // always decease one from parent span id
                            dict[traceidtxt].Push(new KeyValuePair<string, long>(spanName, dict[traceidtxt].Peek().Value - 1));
                        }
                        else
                        {
                            // otherwise use high as span id
                            dict[traceidtxt].Push(new KeyValuePair<string, long>(spanName, high));
                        }
                    }
                }
            }
        }

        static void Client()
        {
            // ######################### client ############################
            using (StreamReader sr = new StreamReader(@"F:\sourcedata\new\client.csv"))
            {
                var loggerFactory = new LoggerFactory();
                //Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();

                //Configuration config = Configuration.FromEnv(new LoggerFactory());

                //ITracer t = config.GetTracer();

                var senderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();

                var senderConfiguration = new Configuration.SenderConfiguration(loggerFactory)
                    .WithSenderResolver(senderResolver).WithEndpoint("http://192.168.1.34:14268/api/traces"); // optional, defaults to Configuration.SenderConfiguration.DefaultSenderResolver


                var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                    .WithSender(senderConfiguration) // optional, defaults to UdpSender at localhost:6831 when ThriftSenderFactory is registered
                    .WithLogSpans(true);             // optional, defaults to no LoggingReporter

                var tracer = new Configuration("Campaign", loggerFactory)
                    .WithReporter(reporterConfiguration) // optional, defaults to RemoteReporter with UdpSender at localhost:6831 when ThriftSenderFactory is registered
                    .GetTracer();

                // var tracer = new Tracer.Builder("ClientUI").Build();

                // sr.ReadLine();
                string newLine = sr.ReadLine();
                int counter = 0;

                while (!string.IsNullOrWhiteSpace(newLine))
                {
                    counter++;
                    if (counter % 100 == 0)
                    {
                        Console.WriteLine(counter);
                    }
                    var columns = newLine.Split(",");

                    var finishedTime = DateTime.Parse(columns[20]);
                    var startTime = finishedTime.AddMilliseconds(-Convert.ToInt32(columns[17]));
                    var span = tracer.BuildSpan("ClientTrace: " + columns[15].Substring(columns[15].LastIndexOf("/") + 1)).WithStartTimestamp(startTime).Start();
                    var rid = new Guid(columns[14]).ToByteArray();
                    var high = BitConverter.ToInt64((new ArraySegment<byte>(rid, 0, 8)).Array);
                    var low = BitConverter.ToInt64((new ArraySegment<byte>(rid, 8, 8)).Array);
                    var traceid = new TraceId(high, low);
                    var spanContext = new SpanContext(traceid, new SpanId(traceid), new SpanId(traceid), SpanContextFlags.None);
                    span.Context = spanContext;
                    span.Finish(finishedTime);
                    span.SetTag(new StringTag("API"), columns[15]);
                    ((Tracer)tracer).Reporter.Report((Span)span);

                    // Task.Delay(1).Wait();

                    newLine = sr.ReadLine();
                }
            }
        }

        static void DB()
        {
            // #################### MT ####################################
            using (StreamReader sr = new StreamReader(@"F:\sourcedata\new\db.csv"))
            {
                var loggerFactory = new LoggerFactory();
                var senderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();
                var senderConfiguration = new Configuration.SenderConfiguration(loggerFactory)
                    .WithSenderResolver(senderResolver).WithEndpoint("http://192.168.1.34:14268/api/traces"); // optional, defaults to Configuration.SenderConfiguration.DefaultSenderResolver
                var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                    .WithSender(senderConfiguration) // optional, defaults to UdpSender at localhost:6831 when ThriftSenderFactory is registered
                    .WithLogSpans(true);             // optional, defaults to no LoggingReporter
                var tracer = new Configuration("Campaign", loggerFactory)
                    .WithReporter(reporterConfiguration) // optional, defaults to RemoteReporter with UdpSender at localhost:6831 when ThriftSenderFactory is registered
                                                          .WithSampler(new Configuration.SamplerConfiguration(loggerFactory).WithType("const"))
                    .GetTracer();
                string newLine; //  = sr.ReadLine();
                int counter = 0;

                //Dictionary<string /*traceid*/, Stack<KeyValuePair<string /*method name*/, long /*spanid*/>>> dict =
                //    new Dictionary<string, Stack<KeyValuePair<string, long>>>();

                Dictionary<string, int> dict = new Dictionary<string, int>();

                while (!string.IsNullOrWhiteSpace(newLine = sr.ReadLine()))
                {
                    counter++;
                    if (counter % 100 == 0)
                    {
                        Console.WriteLine(counter);
                    }
                    var columns = newLine.Split(",");

                    var spanName = $"DBTrace: {columns[3]}"; // method name
                    var traceidtxt = columns[5]; // RID
                    if (traceidtxt.Length < 10)
                    {
                        continue;
                    }
                    var traceidBytes = new Guid(traceidtxt).ToByteArray();
                    var high = BitConverter.ToInt64((new ArraySegment<byte>(traceidBytes, 0, 8)).Array);
                    var low = BitConverter.ToInt64((new ArraySegment<byte>(traceidBytes, 8, 8)).Array);
                    // var eventType = columns[27];

                    //if (!dict.ContainsKey(traceidtxt))
                    //{
                    //    dict.Add(traceidtxt, new Stack<KeyValuePair<string, long>>());
                    //}

                    //if (eventType == "MethodExit")
                    //{
                    //    if (dict[traceidtxt].Count > 0 && dict[traceidtxt].Peek().Key == spanName)
                    //    {
                    //        //found an exit match on stack top
                    //        //log it and pop stack
                    //        //timestamp (3), duration (23), start (timestamp - duration)

                    //        var current = dict[traceidtxt].Pop();
                    //        KeyValuePair<string, long>? parent = null;
                    //        if (dict[traceidtxt].Count > 0)
                    //        {
                    //            parent = dict[traceidtxt].Peek();
                    //        }

                    var finishedTime = DateTime.Parse(columns[0]);
                    var startTime = finishedTime.AddMilliseconds(-Convert.ToInt32(columns[7]));
                    var span = tracer.BuildSpan(spanName).WithStartTimestamp(startTime).Start();

                    var traceId = new TraceId(high, low);

                    if (!dict.ContainsKey(traceidtxt))
                    {
                        dict.Add(traceidtxt, (int)high - 1);
                    }
                    else
                    {
                        dict[traceidtxt] -= 1;
                    }

                    // always use traceid as parent
                    var spanContext = new SpanContext(traceId, new SpanId(dict[traceidtxt]), new SpanId(traceId), SpanContextFlags.Debug);
                    span.Context = spanContext;
                    span.SetTag(new StringTag("RID"), traceidtxt);
                    span.Finish(finishedTime);

                    ((Tracer)tracer).Reporter.Report((Span)span);

                    //        continue;
                    //    }

                    //    // if we have an exit but no enter, ignore
                    //}

                    //if (eventType == "MethodEnter")
                    //{
                    //    // get spanid from random guid
                    //    if (dict[traceidtxt].Count > 0)
                    //    {
                    //        // always decease one from parent span id
                    //        dict[traceidtxt].Push(new KeyValuePair<string, long>(spanName, dict[traceidtxt].Peek().Value - 1));
                    //    }
                    //    else
                    //    {
                    //        // otherwise use high as span id
                    //        dict[traceidtxt].Push(new KeyValuePair<string, long>(spanName, high));
                    //    }
                    //}
                }
            }
        }

        static void ParentIdTest()
        {
            var loggerFactory = new LoggerFactory();
            var senderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();
            var senderConfiguration = new Configuration.SenderConfiguration(loggerFactory)
                .WithSenderResolver(senderResolver).WithEndpoint("http://192.168.1.34:14268/api/traces"); // optional, defaults to Configuration.SenderConfiguration.DefaultSenderResolver
            var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                .WithSender(senderConfiguration) // optional, defaults to UdpSender at localhost:6831 when ThriftSenderFactory is registered
                .WithLogSpans(true);             // optional, defaults to no LoggingReporter
            var tracer = new Configuration("ParentIdTest", loggerFactory)
                .WithReporter(reporterConfiguration) // optional, defaults to RemoteReporter with UdpSender at localhost:6831 when ThriftSenderFactory is registered
                .WithSampler(new Configuration.SamplerConfiguration(loggerFactory).WithType("const"))
                .GetTracer();

            long high = 1;
            long low = 1;

            var traceid = new TraceId(high, low);
            {
                var span = tracer.BuildSpan("root").Start();
                var spanContext = new SpanContext(traceid, new SpanId(high - 1), new SpanId(traceid), SpanContextFlags.Debug);
                span.Context = spanContext;
                span.Finish();

                ((Tracer)tracer).Reporter.Report((Span)span);
            }

            for (int i = 0; i < 10; ++i)
            {
                var span = tracer.BuildSpan(i.ToString()).Start();
                var spanContext = new SpanContext(traceid, new SpanId(high + 1), new SpanId(high), SpanContextFlags.Debug);
                ++high;
                span.Context = spanContext;
                span.Finish();

                ((Tracer)tracer).Reporter.Report((Span)span);
            }
        }

        static void Main(string[] args)
        {
            Client();
            MT();
            DB();
            //ParentIdTest();
            Console.ReadLine();
        }
    }
}

// clientperf schema
/*
0 X    "EventInfo_Time": 2021-01-22T22:38:01.179Z,
"EventInfo_Name": clientperf,
"EventInfo_BaseType": custom,
"EventInfo_Source": act_default_source,
"PipelineInfo_AccountId": 9cf7b68164b34e7e995562121503c72a,
"EventInfo_SdkVersion": AWT-Web-JS-1.5.0,
"UserInfo_Language": en-us,
"UserInfo_TimeZone": -05:00,
"DeviceInfo_BrowserName": Safari,
"DeviceInfo_BrowserVersion": 14.0.2,
"DeviceInfo_OsName": Mac OS X,
"DeviceInfo_OsVersion": 10,
"Sn": PerfMarker,
"SceID": 5405cbc1-5f85-013c-a059-3368c43ce4de,
14 X    "RID": ,
15 X    "Api": shellpage_webui.recommendations_vnext.recommendationdetailsgridoverall.recommendationdetailsgrid.pgrid-recommendationgrid-load,
"Pass": ,
17 X    "Dur": 0,
"Ime": ,
"Hm": ,
20 X    "Timestamp": 2021-01-22T22:37:59.887Z,
X    "SesID": cebe2e77-cdfd-4bef-bd3a-acb8015cde45,
"UID": 107301954,
"CID": 250665411,
"AID": 150428392,
"LCID": 1033,
"Mes": "{\"Version\":\"perf-marker@0.2.0\",\"Name\":\"pgrid-RecommendationGrid-load\",\"ParentName\":\"RecommendationDetailsGrid\",\"IsParentDone\":false,\"IsPageReadyFulfilled\":true,\"IsFileFetched\":true,\"FileFetchStartTime\":0,\"FileFetchActiveStartTime\":0,\"DataFetchStartTime\":490202,\"DataFetchActiveStartTime\":490202,\"RenderStartTime\":490614.00000000006,\"RenderActiveStartTime\":490614.00000000006,\"InitStartTime\":489394,\"InitActiveStartTime\":489394,\"DoneStartTime\":490799.00000000006,\"DoneActiveStartTime\":490799.00000000006,\"DidUrlChange\":false,\"InitUrl\":\"\",\"DoneUrl\":\"\",\"AdditionalMessage\":\"{\\\"Version\\\":\\\"projection-grid@0.2.1-alpha.6\\\",\\\"GridName\\\":\\\"RecommendationGrid\\\",\\\"IsPageReadyFulfilled\\\":true,\\\"IsVirtualized\\\":false,\\\"TotalRows\\\":20,\\\"TotalServerSideRows\\\":797,\\\"TotalVisibleRows\\\":20,\\\"TotalColumns\\\":11}\"}",
"Lc": ClientPerf,
"Ll": Info,
"Env": ui.ads.microsoft.com,
"As": campaign,
"Asub": webui,
"An": SendLog,
"ActID": 7046a1dc-bf2a-a133-8ba6-990bc29e0df7,
"Et": perfmarker,
"Loc": US,
"Role": SMB,
"MgdBy": Microsoft,
"PFlags": rUrl:ShellPage_WebUI.Recommendations_VNext.RecommendationDetailsGridOverall.RecommendationDetailsGrid.pgrid-RecommendationGrid-load;,
"Ec": 0,
"PipelineInfo_IngestionTime": 2021-01-22T22:38:06.798Z,
"PipelineInfo_ClientIp": 73.55.23.000,
"PipelineInfo_ClientCountry": US,
"PipelineInfo_IngestionPath": ,
"Url": /campaign/vnext/recommendations/All?aid=150428392&campaignId=369403432&ccuisrc=4&cid=250665411&recommendationType=CampaignContextKeywordOpportunity&uid=107301954,
"UserAgent": Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.2 Safari/605.1.15,
"Lo_": ,
"DeviceInfo__________": ,
"PgID": 2a3c82a4-56e0-44d8-ba56-c6150efc76ad,
"PgVer": 2021.0121.1359.45,
"DbgMode": False,
"Session_Id": db55d74b-6404-4c14-aca1-067c9c4a8c13,
"APi": ,
"DeviceInfo_BrowserVerskon": ,
"EventInfo_OriginalTime": 2021-01-22T22:38:00.5830000Z,
"AppInfo_Language": en,
"rID": ,
"deviceInfo_OsName": ,
"DjgMode": ,
"Dev_ceInfo_BrowserName": ,
"UserInfo_Languawe": ,

 */


/*
x0 "ServerName": CH01EAP000002B3,
x1"ApplicationName": MT,
x2"ApplicationSubSystem": AccountReparenting,
3"Timestamp": 2021-01-25T19:05:07.6373254Z,
4"Ticks": 637471983076373254,
5"Category": MTPerformance,
6"Message": ,
7"SessionId": ,
x8"RequestId": 8a17be16-3ee9-4af6-9f6e-d1879da9f788,
9"TrackingId": 250fde99-cdfb-4051-9e64-a5c46b5ebc2b,
10"UserId": ,
11"CustomerId": ,
12"ErrorCode": ,
13"Severity": ,
14"ManagedThreadId": 303,
15"Reserved1": ,
16"Reserved2": ,
17"Reserved3": ,
18"EventId": 0,
19"EventLogType": ,
20"Url": ,
21"TraceLevel": Info,
22"SequenceNumber": 0,
23"Duration": 2,
24"Context1": ,
25"Context2": ManagedThreadId=303,
26"PerformanceNormalization": ,
x27"EventType": MethodExit,
x28"MethodName": TaskEngineProxy=GetNextTaskItemAsync,
29"ErrorSource": ,
30"RowGUID": ,
x31"Env": BingAdsServiceEAPLDC2-Prod-CH01,

 */

/*
x0"Timestamp": 2021-01-25T22:17:36.6278625Z,
1"ServerName": nc-bacmpshardprod7,
2"DatabaseName": Campaign_AdGroupShard_P1105,
x3"ProcName": prc_PublicOrderSummaryGetInShard_V41,
4"SessionId": cb8143e5-33fa-4730-8fa1-727ebf76e759,
x5"RequestId": cb8143e5-33fa-4730-8fa1-727ebf76e759,
6"TrackingId": 656b05dd-f076-46de-af78-f1b918025b4d,
x7"Duration": 875,
8"Plan": 00000000-0000-0000-0000-000000000000,
9"RowCount": 0,
*/