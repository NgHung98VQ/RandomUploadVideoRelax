using Google.Apis.Drive.v3.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace auto_upload_youtube.Services.RequestData
{
    public class RequestDetail
    {
        public async Task<List<Detail>> RunRequest(string userId)
        {
            List<Detail>  detailVideoLists = new List<Detail>();
            try
            {
                var url = "https://183.80.50.40:8443/api/video-release/youtube?pageNumber=1&pageSize=999&userId=" + userId;
                var data = await Get(url);

                var jobjectData = JsonConvert.DeserializeObject<RequestDataVideoModel>(data);
                foreach (var item in jobjectData.data.items)
                {
                    var urlVideo = $"https://183.80.50.40:8443/api/product-video?userId={userId}&id={item.productVideoId}";
                    var dataGoogleDrive = await Get(urlVideo);

                    var jObjectLink = JsonConvert.DeserializeObject<RequestLinkGoogleDriveModel>(dataGoogleDrive);

                    Detail detail = new Detail();

                    detail.title = item.title;
                    detail.description = item.description;
                    detail.tags = item.tags;
                    detail.linkVideo = $"https://drive.google.com/file/d/{jObjectLink.data.items[0].googleDriveFileId}/view";
                    detail.linkImgThumb = $"https://drive.google.com/file/d/{jObjectLink.data.items[0].googleDriveThumbFileId}/view";

                    if(jObjectLink.data.items[0].googleDriveFileId != null && jObjectLink.data.items[0].googleDriveThumbFileId != null)
                    {
                        detailVideoLists.Add(detail);
                    }
                    
                }

                return detailVideoLists;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải file: {ex.Message}");
            }
        }

        public  async Task<string> Get(string url)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient httpClient = new HttpClient(clientHandler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errMess = await response.Content.ReadAsStringAsync();
                    throw new Exception(errMess);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }


    public class Detail
    {
        public string title { get; set; }
        public string description { get; set; } 
        public string tags { get; set; }
        public string linkVideo { get; set; }
        public string linkImgThumb { get; set; }
    }
}
