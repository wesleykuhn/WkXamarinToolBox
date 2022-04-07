using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WkXamarinToolBox.Converters;
using WkXamarinToolBox.Enums;
using WkXamarinToolBox.Extensions;
using WkXamarinToolBox.Models;
using WkXamarinToolBox.Services.AppCenter;
using Xamarin.Forms;

[assembly: Dependency(typeof(WkXamarinToolBox.Services.Http.HttpService))]
namespace WkXamarinToolBox.Services.Http
{
    public class HttpService : IHttpService
    {
        private static AuthenticationHeaderValue bearerToken;
        private static DateTime tokenExpiration;

        private HttpClient _httpClient;
        private string _rawResponseContent;
        private long _singleIdParameterOnEndpoint = -1;
        private Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

        public virtual HttpService Init(string baseAddress, double timeoutSeconds = 30)
        {
            if (_httpClient is not null)
                return this;

            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new(clientHandler, false);
            _httpClient.BaseAddress = new(baseAddress);
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            return this;
        }

        public void AddQueryParameter(string key, string value) =>
            _queryParameters.Add(key, value);

        public void AddQueryParameter(string key, int value) =>
            _queryParameters.Add(key, value.ToString());

        public void AddQueryParameter(string key, long value) =>
            _queryParameters.Add(key, value.ToString());

        public void SetSingleIdParameterOnEndpoint(long id) =>
            _singleIdParameterOnEndpoint = id;

        public virtual async Task<ResponseEnum> RequestGetAsync(string endpoint)
        {
            if (_queryParameters.Any())
                endpoint += CollectionConverter.DictionaryToQueryParameters(_queryParameters);

            var request = _singleIdParameterOnEndpoint > -1 ?
                new HttpRequestMessage(HttpMethod.Get, endpoint + $"/{_singleIdParameterOnEndpoint}") :
                new HttpRequestMessage(HttpMethod.Get, endpoint);

            return await RequestAsync(request, endpoint);
        }

        public virtual async Task<ResponseEnum> RequestPostAsync(string endpoint,
            SendObjectPostMethodEnum sendObjectPostMethod,
            object objectToSend = null,
            IEnumerable<DeviceFileModel> attachFiles = null)
        {
            var request = MontarRequisicaoPost(endpoint, sendObjectPostMethod, objectToSend, attachFiles);
            return await RequestAsync(request, endpoint);
        }

        protected virtual HttpRequestMessage MontarRequisicaoPost(string endpoint,
            SendObjectPostMethodEnum sendObjectPostMethod,
            object objectToSend = null,
            IEnumerable<DeviceFileModel> attachFiles = null)
        {
            if (objectToSend is null && attachFiles is null)
                throw new Exception("There wasn't send any object or file! Why are you trying to make a POST request?");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (objectToSend is not null && sendObjectPostMethod == SendObjectPostMethodEnum.Body)
            {
                var serialized = JsonConvert.SerializeObject(objectToSend);
                request.Content = new StringContent(serialized, Encoding.UTF8, "application/json");

                return request;
            }

            MultipartFormDataContent multiFormData = new();
            multiFormData.Headers.ContentType.MediaType = "multipart/form-data";

            if (objectToSend is not null)
            {
                var serializedObj = JsonConvert.SerializeObject(objectToSend);
                var serializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedObj);

                foreach (string key in serializedDictionary.Keys)
                {
                    multiFormData.Add(new StringContent(serializedDictionary[key] ?? ""), key);
                }
            }

            if (attachFiles is not null)
            {
                foreach (var file in attachFiles)
                {
                    var fileNameOnly = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                    multiFormData.Add(new ByteArrayContent(file.ByteArray), fileNameOnly, file.FileName);
                }
            }

            request.Content = multiFormData;
            return request;
        }

        protected virtual async Task<ResponseEnum> RequestAsync(HttpRequestMessage requestMessage, string endpoint)
        {
            if (_httpClient is null)
                throw new Exception("The HttpClient wasn't initialized! Please, call the Init() method before making any request.");

            if (bearerToken is not null)
                _httpClient.DefaultRequestHeaders.Authorization = bearerToken;

            _queryParameters.Clear();
            _singleIdParameterOnEndpoint = -1;

            try
            {
                var rawResponse = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                _rawResponseContent = await rawResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                return ResponseEnum.Success;
            }
            catch (Exception ex)
            {
                AppCenterService.TryTrackException(nameof(HttpService), nameof(RequestAsync), ex);
#if DEBUG
                throw ex;
#else
                return ResponseEnum.GotException;
#endif
            }
        }

        public string GetRawResponseContent() => _rawResponseContent;

        public T RawResponseContentToObject<T>()
        {
            try
            {
                var desserialized = JsonConvert.DeserializeObject<T>(_rawResponseContent);
                return (T)desserialized;
            }
            catch (Exception ex)
            {
                AppCenterService.TryTrackException(nameof(HttpService), nameof(RawResponseContentToObject), ex);
                return default(T);
            }
        }

        public virtual DetailedResponseModel HandleRequestResponseForSingleObject<TObjetoTipo>(
            ResponseEnum requestResponse,
            string callerClass,
            string callerMethod) where TObjetoTipo : class
        {
            if (requestResponse != ResponseEnum.Success)
            {
                AppCenterService.TryTrackRequestResponseIfError(callerClass, callerMethod, requestResponse);
                return new DetailedResponseModel(requestResponse);
            }

            var desserializedObj = RawResponseContentToObject<TObjetoTipo>();

            if (desserializedObj is null || desserializedObj == default(TObjetoTipo))
            {
                AppCenterService.TryTrackRequestResponseIfError(callerClass, callerMethod, ResponseEnum.EmptyObject);
                return new DetailedResponseModel(ResponseEnum.EmptyObject);
            }

            return new DetailedResponseModel(ResponseEnum.Success, desserializedObj);
        }

        public virtual DetailedResponseModel HandleRequestResponseForCollection<TObjetoTipo>(
            ResponseEnum requestResponse,
            string callerClass,
            string callerMethod)
        {
            if (requestResponse != ResponseEnum.Success)
            {
                AppCenterService.TryTrackRequestResponseIfError(callerClass, callerMethod, requestResponse);
                return new DetailedResponseModel(requestResponse);
            }

            var desserializedObj = RawResponseContentToObject<IEnumerable<TObjetoTipo>>();

            if (desserializedObj is null || !desserializedObj.Any())
            {
                AppCenterService.TryTrackRequestResponseIfError(callerClass, callerMethod, ResponseEnum.EmptyObject);
                return new DetailedResponseModel(ResponseEnum.EmptyObject);
            }

            return new DetailedResponseModel(ResponseEnum.Success, desserializedObj);
        }

        public virtual void SetBearerToken(string newToken, DateTime expiration)
        {
            if (newToken.IsNullEmptyOrWhiteSpace())
                throw new Exception("You are trying to set a null, white space or empty token as authentication bearer!");
            else
            {
                bearerToken = new AuthenticationHeaderValue("Bearer", newToken);
                tokenExpiration = expiration;
            }
        }

        public virtual void ClearBearerToken()
        {
            bearerToken = null;
            tokenExpiration = DateTime.MinValue;
        }

        public virtual void RestartService()
        {
            bearerToken = null;
            tokenExpiration = DateTime.MinValue;

            _httpClient = null;
            _rawResponseContent = null;
            _singleIdParameterOnEndpoint = -1;
            _queryParameters.Clear();
        }
    }
}
