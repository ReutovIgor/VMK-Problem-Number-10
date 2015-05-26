using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace ServerApplication
{

    #region MesageControl
    class MessageControl : HttpServer
    {
        private UserControl userControl;
        private ConsultationControl consultationControl;
        private MessageChecker messageChecker;
        //private HttpRequestSender requestSender;

        public MessageControl(int port) : base(port)
        {
            userControl = new UserControl();
            consultationControl = new ConsultationControl();
            messageChecker = new MessageChecker();
            //requestSender = new HttpRequestSender("http://127.0.0.1:8080");
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("received GET request");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("received POST request");

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, dynamic> jsonObject = (Dictionary<string, dynamic>) serializer.DeserializeObject(inputData.ReadToEnd());
            string method = jsonObject["method"];
            Dictionary<string, string>[] messageHandler = messageChecker.checkReceivedMessage(method);

            if(messageHandler == null)
            {
                //send error response!!!!
                return;
            }
            
            //add authorization check!
            string str = messageHandler[0]["Authorized"];
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Trim();
            string[] authorized = str.Split(',');

            if(authorized.Length > 0)
            {
                Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
                Data.Add("username", jsonObject["data"]["username"]);
                Data.Add("table", "");
                for(int i = 0; i < authorized.Length; i++)
                {
                    if (authorized[0].Length == 0)
                        continue;
                    authorized[i] = authorized[i].Trim();
                    Data["table"] = authorized[i];
                    var response = DataBaseMessageComposer.SendRequest("check_authorization", Data);
                    
                    if (response.Count > 0)
                        break;
                    else if(i == (authorized.Length - 1))
                    {
                        Defines.Error error = new Defines.Error();
                        Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                        result.Add("Data", "");
                        result.Add("Error", error);
                        error.No_Permission();
                        this.ComposeResponse(result, p);
                        return;
                    }

                }
            }

            switch (messageHandler[1]["Handler"])
            {
                case "ConsultationControl":
                    this.ConsultationHandler(p, method, jsonObject["data"], messageHandler[0]["Authorized"]);
                    break;
                case "UserControl":
                    this.UserHandler(p, method, jsonObject["data"], messageHandler[0]["Authorized"]);
                    break;
            }
        }

        private void ComposeResponse(Dictionary<string,dynamic> data, HttpProcessor p)
        {
            string json = (new JavaScriptSerializer()).Serialize(data);
            p.writeSuccess("text/html", json);
        }

        private void ConsultationHandler(HttpProcessor p, string method, Dictionary<string, dynamic> methodData, string Authorized)
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            Defines.Error error = new Defines.Error();
            dynamic handlerResponse;
            switch (method)
            {
                case "get_subsidiary_list":
                    result.Add("Data", this.consultationControl.GetSubsidiaryList(ref error));
                    break;
                case "get_specializations":
                    result.Add("Data", this.consultationControl.GetSpecializationList(ref error));
                    break;
                case "get_doctor_list":
                    result.Add("Data", this.consultationControl.GetDoctorList(methodData, ref error));
                    break;
                case "reserve_time":
                    handlerResponse = this.consultationControl.ReserveTime(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "create_consultation":
                    handlerResponse = this.consultationControl.CreateConsultation(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "add_note":
                    handlerResponse = this.consultationControl.AddNote(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "close_consultation":
                    handlerResponse = this.consultationControl.CloseConsultation(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "cancel_consultation":
                    handlerResponse = this.consultationControl.CancelConsultation(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "send_message":
                    handlerResponse = this.consultationControl.SendMessage(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "get_consultations":
                    result.Add("Data", this.consultationControl.GetConsultations(methodData, ref error));
                    break;
                case "get_messages":
                    result.Add("Data", this.consultationControl.GetMessages(methodData, ref error));
                    break;
            }
            result.Add("Error", error);
            this.ComposeResponse(result, p);
        }

        private void UserHandler(HttpProcessor p, string method, Dictionary<string, dynamic> methodData, string Authorized)
        {
            Defines.Error error = new Defines.Error();
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            dynamic handlerResponse;
            switch (method)
            {
                case "get_pendings":
                    result.Add("Data", this.userControl.GetPendings(ref error));
                    break;
                case "login":
                    result.Add("Data", this.userControl.Login(methodData, ref error));
                    break;
                case "register_user":
                    handlerResponse = this.userControl.RegisterUser(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "approve_user":
                    handlerResponse = this.userControl.ApproveUser(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "get_users":
                    result.Add("Data", this.userControl.GetUsers(ref error));
                    break;
                case "add_rights":
                    handlerResponse = this.userControl.AddRights(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "get_schedule":
                    result.Add("Data", this.userControl.GetSchedule(methodData, ref error));
                    break;
                case "change_schedule":
                    handlerResponse = this.userControl.ChangeSchedule(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "vacation_planning":
                    handlerResponse = this.userControl.VacationPlanning(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
                case "approve_reject_plan":
                    handlerResponse = this.userControl.Approve_RejectPlan(methodData, ref error);
                    result.Add("Data","");
                    if(handlerResponse)
                        result["Data"].Add("Status:", "Success");
                    else
                        result["Data"].Add("Status:", "Failed");
                    break;
            }
            
            result.Add("Error", error);
            this.ComposeResponse(result, p);
        }
    }

    class MessageChecker
    {
        Dictionary<string, Dictionary<string, string>[]> messageTypes;
        public MessageChecker()
        {
            DateTime date = DateTime.Now;
            Console.WriteLine(date.ToString() + "Defines:: Initializing MessageChecker");
            messageTypes = readFile();
        }

        private Dictionary<string, Dictionary<string, string>[]> readFile()
        {
            Dictionary<string, Dictionary<string, string>[]> result = new Dictionary<string, Dictionary<string, string>[]>();
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader("MessageTypes.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                    Console.WriteLine(line); // Write to console.
                }
            }

            string[] stringSpliter = { ";" };

            foreach(string el in list)
            {
                string message = el.Substring(0, el.IndexOf(':'));
                string rest = el.Substring(el.IndexOf(':') + 2);

                string[] strDescription = rest.Split(stringSpliter, StringSplitOptions.None);
                Dictionary<string, string>[] dictionaryDescription = new Dictionary<string, string>[2]{
                    new Dictionary<string, string>(),
                    new Dictionary<string, string>()
                };
                dictionaryDescription[0].Add("Authorized", strDescription[0].Substring(strDescription[0].IndexOf(':') + 2));
                dictionaryDescription[1].Add("Handler", strDescription[1].Substring(strDescription[1].IndexOf(':') + 2));
                result.Add(message, dictionaryDescription);
            }
            return result;
        }

        public Dictionary<string, string>[] checkReceivedMessage(string messageType)
        {
            Dictionary<string, string>[] result;
            if (messageTypes.ContainsKey(messageType))
                result = messageTypes[messageType];
            else
                result = null;
            return result;
        }
    }
    #endregion

    #region HttpRequestSender
    class HttpRequestSender
    {
        string url;
        public HttpClient client;

        public HttpRequestSender(string url) 
        {
            Console.WriteLine("Client:: Initializing");
            this.url = url;
            client = new HttpClient();
        }

        public string sendPostRequest(Dictionary<string,string> data)
        {
            Console.WriteLine("Client:: Sending data to url: " + this.url);
            var content = new FormUrlEncodedContent(data);
 
            client.PostAsync(this.url, content);
            
            return "";
        }
    }
    #endregion

    #region HttpListener
    public abstract class HttpServer
    {
        private int port;
        private TcpListener tcpListener;
        bool isActive;

        public HttpServer(int port)
        {
            this.port = port;
            isActive = true;
        }

        public void Listen()
        {
            Console.WriteLine("Server:: Initialized");
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Console.WriteLine("Server:: Begin Listening");
            while (isActive)
            {
                TcpClient s = tcpListener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        ~HttpServer()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
                isActive = false;
            }
        }        

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        private const int BUF_SIZE = 4096;

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }
        
        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }
                
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeSuccess(string content_type = "text/html", string json = " ")
        {
            // this is the successful HTTP response line
            outputStream.WriteLine("HTTP/1.1 200 OK");
            string origin = this.httpHeaders["Origin"].ToString();
            outputStream.WriteLine("Access-Control-Allow-Origin:" + origin);
            // these are the HTTP headers...          
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("response:" + json);
            outputStream.WriteLine("Connection: close");
            // ..add your own headers here if you like

            outputStream.WriteLine(""); // this terminates the HTTP headers.. everything after this is HTTP body..
            outputStream.WriteLine(json);
        }

        public void writeFailure()
        {
            // this is an http 404 failure response
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            // these are the HTTP headers
            outputStream.WriteLine("Connection: close");
            // ..add your own headers here

            outputStream.WriteLine(""); // this terminates the HTTP headers.
        }
    }
    #endregion

    public class ServerMain
    {
        [STAThread]
        static void Main(string[] args)
        {
            HttpServer httpServer;
            httpServer = new MessageControl(8080);

            Thread thread = new Thread(new ThreadStart(httpServer.Listen));
            thread.Start();

            //test application
            Application.EnableVisualStyles();
            Form CommandLog = new ServerCommandLog();
            Application.Run(CommandLog);
        }
    }
}
