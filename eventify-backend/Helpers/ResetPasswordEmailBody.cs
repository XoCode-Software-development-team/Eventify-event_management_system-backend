namespace eventify_backend.Helpers
{
    public class ResetPasswordEmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"<!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Reset Password</title>
                        <style>
                            body {{
                                margin: 0;
                                padding: 0;
                                font-family: 'Lato', sans-serif;
                                background-color: #f2f2f2;
                            }}

                            .container {{
                                width: 100%;
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #ffffff;
                                border-radius: 10px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}

                            .header {{
                                text-align: center;
                                padding: 20px 0;
                                color: #333333;
                            }}

                            .header h1 {{
                                margin: 0;
                                font-size: 24px;
                            }}

                            .content {{
                                padding: 20px;
                                text-align: center;
                                color: #333333;
                            }}

                            .content p {{
                                line-height: 1.6;
                                margin-bottom: 15px;
                                text-align:left;    
                            }}

                            .btn {{
                                display: inline-block;
                                padding: 12px 24px;
                                background-color: #2C98AD;
                                color: #ffffff;
                                text-decoration: none;
                                border-radius: 5px;
                                transition: background-color 0.3s ease;
                            }}

                            .btn:hover {{
                                background-color: #257b8c;
                            }}

                            .footer {{
                                text-align: center;
                                padding: 20px 0;
                                color: #ffffff;
                                background-color: #1C5864;
                                border-bottom-left-radius: 10px;
                                border-bottom-right-radius: 10px;
                            }}

                            .footer p {{
                                margin: 0;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h1>Reset Your Password</h1>
                            </div>
                            <div class=""content"">
                                <p>Hello,
                                You are receiving this email because you requested a password reset for your Eventify account.
                                Please click the button below to create a new password:</p>
                                <a href=""http://localhost:4200/reset?email={email}&code={emailToken}"" class=""btn"" target=""_blank"">Reset Password</a>
                            </div>
                            <div class=""footer"">
                                <p>&copy; 2024 Eventify. All Rights Reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }
    }
}
