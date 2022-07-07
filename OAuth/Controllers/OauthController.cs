using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace OAuth.Controllers
{
    public class OauthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(string client_id,string scope,string response_type,string redirect_uri,string state)
        {
            var query = new QueryBuilder
            {
                {"redirectUri", redirect_uri},
                {"state", state}
            };

            return View(model: query.ToString());
        }
        [HttpPost]
        public IActionResult Authorize(string username, string redirectUri, string state)
        {
            var queryBuilder = new QueryBuilder
            {
                { "code", "BBBB" },
                { "state", state }
            };
            return Redirect($"{redirectUri}{queryBuilder.ToString()}");
        }
        public IActionResult Token(
            string grant_type, // flow of access_token request
            string code, // confirmation of the authentication process
            string redirect_uri,
            string client_id,
            string refresh_token)
        {
            // some mechanism for validating the code

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
                new Claim("granny", "cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes("Constants.Secret");
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                "OAuthServer",
                "Api",
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token"
                    ? DateTime.Now.AddMinutes(5)
                    : DateTime.Now.AddMilliseconds(1),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "RefreshTokenSampleValueSomething77"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            return Redirect(redirect_uri);
        }
    }
}
