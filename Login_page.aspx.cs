using Login_page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Login_signin_page
{
    public partial class Login_page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && User.Identity.IsAuthenticated)
            {
                // Rebuild session from the auth cookie
                string username = User.Identity.Name;

                UserAuthenticationService authService = new UserAuthenticationService(Server);
                string result = authService.GetUserByUsername(username);

                if (result == "Invalid")
                {
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                    return;
                }

                string[] parts = result.Split('|');
                string role = parts[0];

                Session["Username"] = username;
                Session["Role"] = role;

                if (role == "Member")
                {
                    Session["UserId"] = parts[1];
                    Session["BankAccountId"] = parts[2];
                    Response.Redirect("~/../Page2/Member_home_page.aspx", false);
                }
                else if (role == "Staff")
                {
                    Session["UserId"] = parts[1];
                    Response.Redirect("~/../Page3/Staff_home_page.aspx", false);
                }
            }
        }

        private void RedirectByRole(string role)
        {
            if (role == "Member")
            {
                Response.Redirect("~/../Page2/Member_home_page.aspx");
            }
            else if (role == "Staff")
            {
                Response.Redirect("~/../Page3/Staff_home_page.aspx");
            }
        }


        protected void Refresh_captcha_btn_Click(object sender, EventArgs e)
        {

        }

        protected void Login_btn_Click(object sender, EventArgs e)
        {
            string userName = Login_UserName_input.Text;
            string password = Login_password_input.Text;
            if (String.IsNullOrEmpty(userName.Trim()) || String.IsNullOrEmpty(password.Trim())) {
                Login_error_label.Text = "Please enter a non-empty username and password.";
                return;
            }
            Login_error_label.Text = "";

            // authenticate

            // making ther service
            UserAuthenticationService authService = new UserAuthenticationService(Server);

            string result = authService.Login(userName, password);

            if (result == "Invalid")
            {
                Login_error_label.Text = "Invalid username or password.";
                return;
            }

            string[] parts = result.Split('|');

            string role = parts[0];

            // Keep user logged in
            FormsAuthentication.SetAuthCookie(userName, true);

            // Saving useful into for the pages
            Session["Username"] = userName;
            Session["Role"] = role;

            if (role == "Member")
            {
                Session["UserId"] = parts[1];
                Session["BankAccountId"] = parts[2];

                Response.Redirect("~/../Page2/Member_home_page.aspx");
            }
            else if (role == "Staff")
            {
                Session["UserId"] = parts[1];

                Response.Redirect("~/../Page3/Staff_home_page.aspx");
            }
            else
            {
                Login_error_label.Text = "Unknown login type.";
            }
        }


    }
}