using Login_page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Login_signin_page
{
    public partial class Signup_page : System.Web.UI.Page
    {
        private const string CaptchaStringUrl = "https://venus.sod.asu.edu/WSRepository/Services/ImageVerifier/Service.svc/GetVerifierString/5";
        private const string CaptchaImageBaseUrl = "https://venus.sod.asu.edu/WSRepository/Services/ImageVerifier/Service.svc/GetImage/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Load_Captcha();
                Session["captcha_verified"] = false;
            }
            ApplySignupButtonState();
        }

        protected void Refresh_captcha_btn_Click(object sender, EventArgs e)
        {
            Load_Captcha();
            Session["captcha_verified"] = false;
            Captcha_input.Text = "";
            Captcha_status_label.Text = "";
            Captcha_status_label.Style["color"] = "#b00020";
            ApplySignupButtonState();
        }

        private void Load_Captcha()
        {
            string verifier = GetUrlText(CaptchaStringUrl).Trim();

            if (verifier.StartsWith("<"))
            {
                int start = verifier.IndexOf('>') + 1;
                int end = verifier.LastIndexOf('<');

                if (start > 0 && end > start)
                {
                    verifier = verifier.Substring(start, end - start);
                }
            }

            Session["captcha_string"] = verifier;
            Captcha_image.ImageUrl = CaptchaImageBaseUrl + Server.UrlEncode(verifier);
        }
        private string GetUrlText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private void ApplySignupButtonState()
        {
            bool captchaVerified = Session["captcha_verified"] != null && (bool)Session["captcha_verified"];

            Signup_btn.Enabled = captchaVerified;

            if (captchaVerified)
            {
                Signup_btn.CssClass = "signup-btn";
            }
            else
            {
                Signup_btn.CssClass = "signup-btn disabled-btn";
            }
        }

        protected void Verify_captcha_btn_Click(object sender, EventArgs e)
        {
            string expectedCaptcha = Session["captcha_string"] as string;
            string userCaptcha = Captcha_input.Text.Trim();

            if (string.IsNullOrEmpty(expectedCaptcha))
            {
                Session["captcha_verified"] = false;
                Captcha_status_label.Style["color"] = "#b00020";
                Captcha_status_label.Text = "Captcha expired. Please generate a new one.";
                Load_Captcha();
                ApplySignupButtonState();
                return;
            }

            if (string.Equals(expectedCaptcha, userCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                Session["captcha_verified"] = true;
                Captcha_status_label.Style["color"] = "#2e7d32";
                Captcha_status_label.Text = "Captcha verified.";
            }
            else
            {
                Session["captcha_verified"] = false;
                Captcha_status_label.Style["color"] = "#b00020";
                Captcha_status_label.Text = "Incorrect captcha. Try again.";
            }

            ApplySignupButtonState();
        }

        protected void Signup_btn_Click(object sender, EventArgs e)
        {
            bool captchaVerified = Session["captcha_verified"] != null && (bool)Session["captcha_verified"];

            if (!captchaVerified)
            {
                Login_error_label.Text = "Please verify the captcha first.";
                return;
            }

            string userName = Signup_name_input.Text.Trim();
            string password = Signup_password_input.Text.Trim();

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                Login_error_label.Text = "Please enter a non-empty username and password.";
                return;
            }

            Login_error_label.Text = "";

            UserAuthenticationService authService = new UserAuthenticationService(Server);

            string result = authService.Signup(userName, password);

            if (result == "Username already exists")
            {
                Login_error_label.Text = "Username already exists.";
                return;
            }

            // Example result:
            // Signin successful. UserId=1, BankAccountId=1

            string loginResult = authService.Login(userName, password);
            string[] parts = loginResult.Split('|');

            FormsAuthentication.SetAuthCookie(userName, false);

            Session["Username"] = userName;
            Session["Role"] = "Member";
            Session["UserId"] = parts[1];
            Session["BankAccountId"] = parts[2];

            Response.Redirect("~/../Page2/Member_home_page.aspx");
            

        }
    }
}