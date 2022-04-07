using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WkXamarinToolBox.Enums;
using WkXamarinToolBox.Models;

namespace WkXamarinToolBox.Services.Http
{
    public interface IHttpService
    {
        void AddQueryParameter(string key, int value);
        void AddQueryParameter(string key, long value);
        void AddQueryParameter(string key, string value);
        void ClearBearerToken();
        string GetRawResponseContent();
        DetailedResponseModel HandleRequestResponseForCollection<TObjetoTipo>(ResponseEnum requestResponse, string callerClass, string callerMethod);
        DetailedResponseModel HandleRequestResponseForSingleObject<TObjetoTipo>(ResponseEnum requestResponse, string callerClass, string callerMethod) where TObjetoTipo : class;
        HttpService Init(string baseAddress, double timeoutSeconds = 30);
        T RawResponseContentToObject<T>();
        Task<ResponseEnum> RequestGetAsync(string endpoint);
        Task<ResponseEnum> RequestPostAsync(string endpoint, SendObjectPostMethodEnum sendObjectPostMethod, object objectToSend = null, IEnumerable<DeviceFileModel> attachFiles = null);
        void RestartService();
        void SetBearerToken(string newToken, DateTime expiration);
        void SetSingleIdParameterOnEndpoint(long id);
    }
}