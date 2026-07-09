using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace CRS.ADMIN.APPLICATION.Helper
{
    public static class VercelHelper
    {
        private static readonly string _token = WebConfigurationManager.AppSettings["Vercel:Token"];
        private static readonly string _projectId = WebConfigurationManager.AppSettings["Vercel:ProjectId"];
        private static readonly string _domain = WebConfigurationManager.AppSettings["Vercel:Domain"];
        private static readonly string _teamId = WebConfigurationManager.AppSettings["Vercel:TeamId"];

        /// <summary>
        /// Adds a subdomain to your Vercel project.
        /// </summary>
        public static async Task<string> AddSubdomainAsync(string subdomain)
        {
            if (string.IsNullOrEmpty(subdomain))
                throw new Exception($"Invalid subdomain");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                //var url = $"https://api.vercel.com/v9/projects/{"prj_E2UW7O3vBMkPRvvQx7udKyFYRdSF"}/domains?teamId=team_8A9hGWbKkLHcyaYofXoUnwhS";
                var url = $"https://api.vercel.com/v9/projects/{_projectId}/domains?teamId={_teamId}";
                var payload = new { name = $"{subdomain}{_domain}" };
                var jsonPayload = JsonConvert.SerializeObject(payload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error adding subdomain: {result}");

                return result;
            }
        }

        /// <summary>
        /// Removes a subdomain from your Vercel project.
        /// </summary>
        //public static async Task<string> RemoveSubdomainAsync(string subdomain)
        //{
        //    if (string.IsNullOrEmpty(subdomain))
        //        throw new Exception($"Invalid subdomain");

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", _token);

        //        var domainName = $"{subdomain}{_domain}";
        //        var url = $"https://api.vercel.com/v9/projects/{_projectId}/domains/{domainName}?teamId={_teamId}";

        //        var response = await client.DeleteAsync(url);
        //        var result = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //            throw new Exception($"Error removing subdomain: {result}");

        //        return result;
        //    }
        //}

        public static async Task<string> RemoveSubdomainAsync(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                throw new Exception("Invalid subdomain");

            subdomain = subdomain.Trim().ToLowerInvariant();

            if (!Regex.IsMatch(subdomain, "^[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?$"))
                throw new Exception("Invalid subdomain format");

            var baseDomain = _domain.StartsWith(".") ? _domain : "." + _domain;
            var domainName = $"{subdomain}{baseDomain}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                var encodedProjectId = Uri.EscapeDataString(_projectId);
                var encodedDomainName = Uri.EscapeDataString(domainName);
                var encodedTeamId = Uri.EscapeDataString(_teamId);

                var url =
                    $"https://api.vercel.com/v9/projects/{encodedProjectId}/domains/{encodedDomainName}?teamId={encodedTeamId}";

                using (var response = await client.DeleteAsync(url))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception($"Subdomain does not exist in Vercel project: {domainName}");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"Error removing subdomain. Domain: {domainName}, Status: {(int)response.StatusCode}, Response: {result}"
                        );
                    }

                    return result;
                }
            }
        }
        public static async Task<bool> SubdomainExistsAsync(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return false;

            subdomain = subdomain.Trim().ToLowerInvariant();

            var baseDomain = _domain.StartsWith(".") ? _domain : "." + _domain;
            var domainName = $"{subdomain}{baseDomain}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

                var encodedProjectId = Uri.EscapeDataString(_projectId);
                var encodedDomain = Uri.EscapeDataString(domainName);
                var encodedTeamId = Uri.EscapeDataString(_teamId);

                var url =
                    $"https://api.vercel.com/v9/projects/{encodedProjectId}/domains/{encodedDomain}?teamId={encodedTeamId}";

                using (var response = await client.GetAsync(url))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return false;

                    if (response.IsSuccessStatusCode)
                        return true;

                    var errorBody = await response.Content.ReadAsStringAsync();

                    throw new Exception(
                        $"Failed to check Vercel domain. Status: {(int)response.StatusCode}, Body: {errorBody}"
                    );
                }
            }
        }
        /// <summary>
        /// Checks whether a subdomain exists in the Vercel project.
        /// </summary>
        //public static async Task<bool> SubdomainExistsAsync(string subdomain)
        //{
        //    if (string.IsNullOrEmpty(subdomain))
        //        return false;

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", _token);

        //        var domainName = $"{subdomain}{_domain}";
        //        var url = $"https://api.vercel.com/v9/projects/{_projectId}/domains/{domainName}?teamId={_teamId}";

        //        var response = await client.GetAsync(url);

        //        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //            return false;

        //        return response.IsSuccessStatusCode;
        //    }
        //}
    }
}