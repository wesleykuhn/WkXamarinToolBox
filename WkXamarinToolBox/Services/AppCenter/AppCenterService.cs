using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using WkXamarinToolBox.Enums;
using WkXamarinToolBox.Extensions;
using WkXamarinToolBox.Services.DeviceInfos;

namespace WkXamarinToolBox.Services.AppCenter
{
    public static class AppCenterService
    {
        public static void InitializeAppCenter(string androidSecret, string iOSSecret)
        {
            if (androidSecret.IsNullEmptyOrWhiteSpace() && iOSSecret.IsNullEmptyOrWhiteSpace())
                return;

#if !DEBUG
            Microsoft.AppCenter.AppCenter.Start($"android={androidSecret};ios={iOSSecret};", typeof(Analytics), typeof(Crashes));
            Analytics.SetEnabledAsync(true);
#endif
        }

        public static void TryTrackException(string callerClass, string callerMethod, Exception ex, Dictionary<string, string> usedParameters = null)
        {
#if DEBUG
            throw new Exception($"{ex.Message} Class: {callerClass}. Method: {callerMethod}.");
#else
            Dictionary<string, string> events = new()
            {
                { "DeviceDateTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                { "DeviceInfos", DeviceInfosService.GetAllDeviceBasicInformation() },
                { "Class", callerClass },
                { "Method", callerMethod },
                { "ExceptionMessage", ex.Message }
            };
            if (usedParameters is not null && usedParameters.Any())
            {
                foreach (var usedParam in usedParameters)
                {
                    events.Add($"Param: {usedParam.Key}.", $"ParamValue: {usedParam.Value}.");
                }
            }
            Analytics.TrackEvent("Exception", events);
#endif
        }

        public static void TryTrackRequestResponseIfError(string callerClass, string callerMethod, ResponseEnum response, Dictionary<string, string> usedParameters = null)
        {
            string exceptionMsg = "Request's Exception: ";

            switch (response)
            {
                case ResponseEnum.GotException:
                    exceptionMsg += "There was an exception on the HttpClient while trying to make the request.";
                    break;

                case ResponseEnum.EmptyObject:
                    exceptionMsg += "The request's response was an empty object.";
                    break;

                case ResponseEnum.NoResponse:
                    exceptionMsg += "The request wasn't even made.";
                    break;

                case ResponseEnum.ParallelTasksFail:
                    exceptionMsg += "One or more parallel task(s) fail.";
                    break;

                default:
                    return;
            }

#if DEBUG
            throw new Exception($"{excecaoMsg} Class: {callerClass}. Method: {callerMethod}.");
#else
            Dictionary<string, string> events = new()
            {
                { "DeviceDateTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                { "DeviceInfos", DeviceInfosService.GetAllDeviceBasicInformation() },
                { "Class", callerClass },
                { "Method", callerMethod },
                { "ExceptionMessage", exceptionMsg }
            };
            if (usedParameters is not null && usedParameters.Any())
            {
                foreach (var usedParam in usedParameters)
                {
                    events.Add($"Param: {usedParam.Key}.", $"ParamValue: {usedParam.Value}.");
                }
            }
            Analytics.TrackEvent("Exception", events);
#endif
        }
    }
}
