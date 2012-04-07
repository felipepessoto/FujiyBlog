using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using FujiyBlog.Web.Models;

namespace FujiyBlog.Web.Controllers
{
    public partial class SocialController : Controller
    {
        private static void SaveUserFromOpenId(IAuthenticationResponse response)
        {
            var openIdUserData = new SocialUserData();

            var claimsResponse = response.GetExtension<ClaimsResponse>();
            if (claimsResponse != null)
            {
                if (!string.IsNullOrWhiteSpace(claimsResponse.Nickname))
                    openIdUserData.Name = claimsResponse.Nickname;
                else if (!string.IsNullOrWhiteSpace(claimsResponse.FullName))
                    openIdUserData.Name = claimsResponse.FullName;
                
                if ( !string.IsNullOrWhiteSpace(claimsResponse.Email))
                    openIdUserData.Email = claimsResponse.Email;
            }

            var fetchResponse = response.GetExtension<FetchResponse>();
            if (fetchResponse != null)
            {
                if (string.IsNullOrWhiteSpace(openIdUserData.Email))
                    openIdUserData.Email = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);

                if (string.IsNullOrWhiteSpace(openIdUserData.Name))
                {
                    openIdUserData.Name = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.FullName) ??
                                     fetchResponse.GetAttributeValue(WellKnownAttributes.Name.First) + " " +
                                     fetchResponse.GetAttributeValue(WellKnownAttributes.Name.Last);
                }


                openIdUserData.WebSite = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Web.Blog) ??
                                         fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Web.Homepage);

            }

            System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("openid", new JavaScriptSerializer().Serialize(openIdUserData)));
        }

        [NonAction]
        public static SocialUserData GetLoggedUser()
        {
            HttpCookie openIdCookie = System.Web.HttpContext.Current.Request.Cookies["openid"];
            if (openIdCookie != null && !string.IsNullOrEmpty(openIdCookie.Value))
            {
                return new JavaScriptSerializer().Deserialize<SocialUserData>(openIdCookie.Value);
            }
            return null;
        }

        public virtual ActionResult LoginOpenId(string openIdIdentifier)
        {
            if(GetLoggedUser() != null)
            {
                return Content("<script>window.opener.fujiyBlog.socialId.callbackLogin(true);</script>");
            }

            using (var openIdRelyingParty = new OpenIdRelyingParty())
            {
                IAuthenticationResponse response = openIdRelyingParty.GetResponse();
                if (response != null)
                {
                    switch (response.Status)
                    {
                        case AuthenticationStatus.Authenticated:
                            SaveUserFromOpenId(response);
                            return Content("<script>window.opener.fujiyBlog.socialId.callbackLogin(true); window.close();</script>");
                        case AuthenticationStatus.Canceled:
                            return Content("<script>window.opener.fujiyBlog.socialId.callbackLogin(false, 'Canceled at provider');</script>");
                        case AuthenticationStatus.Failed:
                            return Content("<script>window.opener.fujiyBlog.socialId.callbackLogin(false, '" + response.Exception.Message + "');</script>");
                    }
                    return new EmptyResult();
                }

                return SendToLoginPage(openIdIdentifier);
            }
        }

        public virtual ActionResult Logout()
        {
            System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("openid", ""){Expires = DateTime.Now.AddDays(-1)});
            
            return new EmptyResult();
        }

        private ActionResult SendToLoginPage(string openIdIdentifier)
        {
            using (OpenIdRelyingParty openIdRelyingParty = new OpenIdRelyingParty())
            {
                Identifier id;
                if (!Identifier.TryParse(openIdIdentifier, out id))
                {
                    return Content("<script>window.opener.fujiyBlog.socialId.callbackLogin(false, 'The specified login identifier is invalid');</script>");
                }

                try
                {
                    var request = openIdRelyingParty.CreateRequest(openIdIdentifier, Realm.AutoDetect, new Uri(Url.Action(MVC.Social.LoginOpenId(), Request.Url.Scheme)));
                    request.AddExtension(new FetchRequest
                                             {
                                                 Attributes =
                                                     {
                                                         new AttributeRequest(WellKnownAttributes.Name.First, true),
                                                         new AttributeRequest(WellKnownAttributes.Name.Last, true),
                                                         new AttributeRequest(WellKnownAttributes.Contact.Email,
                                                                              true),
                                                         new AttributeRequest(WellKnownAttributes.Contact.Web.Blog,
                                                                              true),
                                                         new AttributeRequest(
                                                             WellKnownAttributes.Contact.Web.Homepage, true),
                                                         new AttributeRequest(WellKnownAttributes.Name.FullName,
                                                                              false),
                                                     }
                                             });
                    request.AddExtension(new ClaimsRequest
                                             {
                                                 Email = DemandLevel.Require,
                                                 FullName = DemandLevel.Require,
                                                 Nickname = DemandLevel.Require,
                                             });

                    return request.RedirectingResponse.AsActionResult();
                }
                catch (ProtocolException ex)
                {
                    return Content("<script>alert('" + ex.Message + "');</script>");
                }
            }
        }
    }
}
