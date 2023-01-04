﻿using AutomationCore.Managers.Models;
using AutomationCore.Utils;
using RestSharp;
using System.Collections.Concurrent;
using System.Net;

namespace AutomationCore.Managers
{
    public class ApiM
    {
        private RestClient _client;
        private TestsLogger _logger;
        private RunSettings _runSettings;

        public ApiM(TestsLogger logger)
        {
            _logger = logger;
            _runSettings = RunSettings.GetRunSettings;
            _client = new RestClient(_runSettings.ApiInstanceUrl);
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(
            string endPoint, Method method,
            ConcurrentDictionary<string, string>? headers = null,
            ConcurrentDictionary<string, string>? parameters = null,
            IRestObject? restObject = null) where T : new()
        {
            _logger.LogTestAction(LogMessages.MethodExecution(methodName: nameof(ExecuteAsync), $"End point: {endPoint} Method: {method}"));
            var request = CreateRequest(endPoint, method, headers, parameters, restObject);

            var result = await _client.ExecuteAsync<T>(request);

            _logger.LogTestAction(LogMessages.MethodExecution(methodName: nameof(ExecuteAsync), $"End point: {endPoint} Method: {method} Response code: {result.StatusCode}"));

            return result;
        }

        private RestRequest CreateRequest(
            string endPoint,
            Method method,
            ConcurrentDictionary<string, string>? headers = null,
            ConcurrentDictionary<string, string>? parameters = null,
            IRestObject? restObject = null)
        {
            var request = new RestRequest(endPoint, method);
            request.AddParameter("key", _runSettings.ApiKey);
            request.AddParameter("token", _runSettings.ApiToken);

            if (headers != null)
            {
                Parallel.ForEach(headers, header => { request.AddParameter(header.Key, header.Value, ParameterType.UrlSegment); });
            }

            if (parameters != null)
            {
                Parallel.ForEach(parameters, parameter => { request.AddParameter(parameter.Key, parameter.Value); });
            }

            if (restObject != null)
            {
                Parallel.ForEach(UString.GetAllClassPropertiesWithValuesAsStrings(restObject), property => { request.AddParameter(property.Key, property.Value); });
            }

            return request;
        }

        public T GetZephyrFolders<T>()
        {
            var zephyrUrl = "https://api.zephyrscale.smartbear.com";
            var requestUrl = "/v2/folders";

            _logger.LogTestAction(LogMessages.MethodExecution(methodName: nameof(ExecuteAsync), $"End point: {zephyrUrl}{requestUrl} Method: {Method.Get}"));
            var localCliend = new RestClient(zephyrUrl);
            var newRequest = new RestRequest(requestUrl, Method.Get);
            newRequest.AddHeader("Authorization", $"{_runSettings.ZephyrToken}");

            var response = localCliend.Execute<T>(newRequest);
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                var msg = $"Unable to get zephyr test cycle folders. https://api.zephyrscale.smartbear.com/v2/folders returns {response.StatusCode} for GET request";
                _logger.LogError(LogMessages.MethodExecution($"Method throws exception: {msg}"));
                throw new Exception(msg);
            }

            _logger.LogTestAction(LogMessages.MethodExecution(methodName: nameof(ExecuteAsync), $"End point: {zephyrUrl}{requestUrl} Method: {Method.Get} Response Code: {response.StatusCode}"));


            return response.Data;
        }
    }
}
