using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExamManagement
{
    public partial class Calculator : System.Web.UI.Page
    {
        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (User.Identity.IsAuthenticated)
                {
                    StatusText.Text = string.Format("Hello {0}!!", User.Identity.GetUserName());
                    LoginStatus.Visible = true;
                    LogoutButton.Visible = true;
                }
                else
                {
                    LoginForm.Visible = true;
                }
            }
        }

        protected void SignIn(object sender, EventArgs e)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);
            var user = userManager.Find(UserName.Text, Password.Text);

            if (user != null)
            {
                var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, userIdentity);
                Response.Redirect("~/History.aspx");
            }
            else
            {
                StatusText.Text = "Invalid username or password.";
                LoginStatus.Visible = true;
            }
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Calculator.aspx");
        }

        protected void Calculate(object sender, EventArgs e)
        {
            var number1 = Int32.Parse(TextBox1.Text.ToString());

            var number2 = Int32.Parse(TextBox2.Text.ToString());

            if (number1 > 999 || number2 > 999 || number1 < 1 || number2 < 1)
            {
                Label2.Text = "numbers must be between 1 and 999";
                Label2.Visible = true;
                Label1.Visible = false;

                return;
            }

            if (number1 % number2 == 0 || number2 % number1 == 0)
            {
                Label1.Text = "true";
                Label2.Visible = false;
                if (Label1.Visible == false) Label1.Visible = true;


            }
            else
            {
                Label1.Text = "false";
                if (Label1.Visible == false) Label1.Visible = true;
                Label2.Visible = false;
            }

            myConnection.Open();
            string query = "Insert into[dbo].[Calculation] (X, Y, r) Values(@x, @y , @r)";
            SqlCommand insertCommand = new SqlCommand(query, myConnection);
            insertCommand.Parameters.AddWithValue("@x", TextBox1.Text);
            insertCommand.Parameters.AddWithValue("@y", TextBox2.Text);
            insertCommand.Parameters.AddWithValue("@r", Label1.Text);
            insertCommand.ExecuteNonQuery();
            myConnection.Close();
        }

        protected void NumberOneChange(object sender, EventArgs e)
        {
            var number1 = Int32.Parse(TextBox1.Text.ToString());

            if (number1 > 999 || number1 < 1)
            {
                Label2.Text = "numbers must be between 1 and 999";
                Label2.Visible = true;
                Label1.Visible = false;

                return;
            }
        }

        protected void NumberTrueChange(object sender, EventArgs e)
        {
            var number1 = Int32.Parse(TextBox2.Text.ToString());

            if (number1 > 999 || number1 < 1)
            {
                Label2.Text = "numbers must be between 1 and 999";
                Label2.Visible = true;
                Label1.Visible = false;

                return;
            }
        }


    }
}