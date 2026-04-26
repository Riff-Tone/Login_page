<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login_page.aspx.cs" Inherits="Login_signin_page.Login_page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .auth-page {
            min-height: 70vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .auth-card {
            width: 100%;
            max-width: 420px;
            background-color: #ffffff;
            border: 1px solid #d9d9d9;
            border-radius: 6px;
            padding: 36px 28px 24px 28px;
            box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
        }

        .auth-title {
            text-align: center;
            font-size: 42px;
            font-weight: 300;
            color: #222;
            margin-bottom: 8px;
        }

        .auth-subtitle {
            text-align: center;
            font-size: 14px;
            color: #444;
            margin-bottom: 36px;
        }

        .form-group {
            margin-bottom: 28px;
        }

        .form-input {
            width: 100%;
            box-sizing: border-box;
            border: none;
            border-bottom: 1px solid #bfbfbf;
            padding: 10px 0;
            font-size: 18px;
            font-family: Arial, Helvetica, sans-serif;
            outline: none;
            background-color: transparent;
        }

        .form-input:focus {
            border-bottom: 1px solid #1f73d8;
        }

        .auth-btn {
            width: 100%;
            background-color: #1f73d8;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 14px 16px;
            font-size: 18px;
            font-weight: bold;
            font-family: Arial, Helvetica, sans-serif;
            cursor: pointer;
            margin-top: 8px;
        }

        .auth-btn:hover {
            background-color: #185fb3;
        }

        .remember-row {
            margin-top: 18px;
            margin-bottom: 28px;
            display: flex;
            align-items: center;
            gap: 8px;
            color: #222;
            font-size: 16px;
        }

        .bottom-text {
            text-align: center;
            margin-top: 18px;
            font-size: 15px;
            color: #555;
        }

        .bottom-text a {
            color: #1f73d8;
            text-decoration: none;
            font-weight: bold;
        }

        .bottom-text a:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-page">
        <div class="auth-card">
            <div class="auth-title">Sign in</div>
            <div class="auth-subtitle">Sign in to continue</div>

            <div class="form-group">
                <asp:TextBox ID="Login_UserName_input" runat="server" CssClass="form-input" placeholder="UserName"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:TextBox ID="Login_password_input" runat="server" CssClass="form-input" placeholder="Password"></asp:TextBox>
            </div>

            <asp:Button ID="Login_btn" runat="server" Text="Sign in" CssClass="auth-btn" OnClick="Login_btn_Click" />
            <asp:Label 
                ID="Login_error_label" 
                runat="server" 
                ForeColor="Red" 
                Style="display:block; margin-top:10px; font-size:14px;">
            </asp:Label>

            <div class="bottom-text">
                Don’t have an account?
                <a runat="server" href="~/Signup_page.aspx">Sign up</a>
            </div>

            <asp:Button 
                ID="Member_btn" 
                runat="server" 
                Text="Go to Member Page" 
                PostBackUrl="../Page2/Member_home_page.aspx" />

            <asp:Button 
                ID="Staff_btn" 
                runat="server" 
                Text="Go to Staff Page" 
                PostBackUrl="../Page3/Staff_home_page.aspx" />

        </div>
        
    </div>
</asp:Content>