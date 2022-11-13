using Newtonsoft.Json;
using System.Text;
using System;
using auto_upload_youtube.Services.RequestData;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace auto_upload_youtube.Services.RandomUpload
{
    public class RandomProcess
    {

        private RandomUploadJobInput _inputRandom;
        private RandomUploadJobOutput _Output;
        private List<Detail> listDetail;
        public RandomProcess(RandomUploadJobInput inputRandom, RandomUploadJobOutput Output)
        {
            _inputRandom = inputRandom;
            _Output = Output;
        }
        public async Task RandomUploadPost(Detail detail, string channelID, string jobId)
        {
            string tag = detail.tags;
            List<string> listTags = new List<string>();
            string[] tags = tag.Split(',');
            foreach(var h in tags)
            {
                listTags.Add(h);
            }
            var uploadJobInput = new UploadJobInput();
            uploadJobInput.ProfilePath = _inputRandom.ProfilePath;
            uploadJobInput.ExecutablePath = _inputRandom.ExecutablePath;
            uploadJobInput.ChannelName = null;
            uploadJobInput.ChannelID = channelID;
            uploadJobInput.VideoFilePath = null;
            uploadJobInput.VideoTitle = detail.title;
            uploadJobInput.VideoDescription = detail.description;
            uploadJobInput.PlaylistName = null;
            uploadJobInput.IsForKid = true;
            uploadJobInput.IsLimitedOld = true;
            
            uploadJobInput.Tags = listTags;
            uploadJobInput.Category = null;
            uploadJobInput.VideoLanguage = null;
            uploadJobInput.DisplayVideoMode = DisplayVideoModeType.Public;
            uploadJobInput.Schedule = null;
            uploadJobInput.IP = null;
            uploadJobInput.Port = null;
            uploadJobInput.ProxyUsername = null;
            uploadJobInput.ImageThumbGoogleDriveURL = detail.linkImgThumb;
            uploadJobInput.VideoGoogleDriveURl = detail.linkVideo;
            uploadJobInput.CookieYoutube = _inputRandom.ProfilePath + "\\" + channelID + ".txt";
            uploadJobInput.ProxyPassword = null;

            
            Input input = new Input();
            input.serviceName = "Upload Service";
            input.input = uploadJobInput;
            input.timeoutMilisecond = 10000000;
            input.jobId = jobId;

            var body = JsonConvert.SerializeObject(input);
            var data = new StringContent(body, Encoding.UTF8, "application/json");

            var url = "http://localhost:6002/api/job/add";

            await PostRequest(url, data);  
        }

        public async Task RunRandomUpload()
        {
            //Request link google drive
            _Output.Message = "Đang lấy link Google drive.....!";

            var userId = _inputRandom.UserId;
            var request = new RequestDetail();
            listDetail = await request.RunRequest(userId);
            if(_inputRandom.Timer != 0)
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                Task timerTask = RunPeriodically(sendRequest, TimeSpan.FromSeconds(_inputRandom.Timer), tokenSource.Token);
            }
            else
            {
                sendRequest();
            }
        }

        static async Task RunPeriodically(Action action, TimeSpan interval, CancellationToken token)
        {
            while (true)
            {
                action();
                await Task.Delay(interval, token);
            }
        }

        public async void sendRequest()
        {
            List<Detail> listItems = listDetail;
            foreach (var channelID in _inputRandom.ListChannelID)
            {
                //Input.ChannelID.RemoveAt(someRandomNumber);
                Random random = new Random();

                const string chars = "qwertyuiopasdfghjklzxcvbnm0123456789";
                string stringRandomId = new string(Enumerable.Repeat(chars, 15).Select(s => s[random.Next(s.Length)]).ToArray());

                // get random number from 0 to 2. 

                Detail item;

                int countTitle = 0;
                int countDescription = 0;
                int countTags = 0;
                string idVideoGGDriver = "";
                string idImgGGDriver = "";
                do
                {
                    int someRandomNumber = random.Next(0, listItems.Count());
                    item = listItems[someRandomNumber];
                    countTitle = item.title.Length;
                    countDescription = item.description.Length;
                    countTags = item.tags.Length;
                    idImgGGDriver = item.linkImgThumb;
                    idVideoGGDriver = item.linkVideo;


                } while (countTitle > 100 || countDescription > 5000 || countTags > 500 || idImgGGDriver == "" || idVideoGGDriver == "");

                var now = DateTime.Now.Ticks;

                string jobId = stringRandomId + now.ToString();
                logId(jobId);

                _Output.Message = "Đang random 1 video lên kênh....." + now.ToString() + "... " + channelID + "...." + item.linkVideo;
                //random upload process
                await Task.Run(() => RandomUploadPost(item, channelID, jobId));
                await RandomUploadPost(item, channelID, jobId);
                //random video 
            }
        }

        public async Task PostRequest(string url, StringContent data)
        {
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

        }

        public async void logId(string log)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("logId.txt", true))
            {
                file.WriteLine(log);
            }
        }
    }

}
