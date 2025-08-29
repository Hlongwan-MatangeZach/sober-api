using Microsoft.AspNetCore.Http.HttpResults;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace SoberPath_API.NewFolder
{
    public class EmailSender
    {
        public async Task Client_Registration_Email(string subject, string toEmail, string username)
        {
            var apiKey = "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";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("spha2729@gmail.com", "Sober Path");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to Your Journey to Sobriety</title>
    <style>
        body {{
            font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #eeeeee;
        }}
        .header h1 {{
            color: #FF8E29; /* Updated to orange */
            margin: 0;
            font-size: 28px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content p {{
            margin-bottom: 15px;
        }}
        .button-container {{
            text-align: center;
            padding: 20px 0;
        }}
        .button {{
            display: inline-block;
            background-color: #FF8E29; /* Updated to orange */
            color: #ffffff;
            padding: 12px 25px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }}
        .footer {{
            text-align: center;
            padding-top: 20px;
            border-top: 1px solid #eeeeee;
            font-size: 12px;
            color: #777777;
        }}
        .footer a {{
            color: #777777;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome to a New Beginning!</h1>
        </div>
        <div class=""content"">
            <p>Dear {username},</p>
            <p>Welcome to our community! We are incredibly proud of you for taking this courageous and significant step towards a life of sobriety and freedom from drugs. This is a powerful decision, and we are here to support you every step of the way.</p>
            <p>Your journey to healing and recovery starts now, and you are not alone. We understand that this path can have its challenges, but with dedication, support, and the right tools, a fulfilling life in sobriety is absolutely within your reach.</p>
            
            <p>We believe in your strength and your ability to achieve lasting change. Remember, every small step forward is a victory. Be kind to yourself, stay persistent, and lean on the support system we've built for you.</p>
            <p>Ready to begin?</p>
        </div>
        <div class=""button-container"">
            <a href=""http://localhost:2025/elegent/authentication/login"" class=""button"">Start Your Journey Now</a>
        </div>
        <div class=""content"">
            <p>If you have any questions or need immediate assistance, please don't hesitate to reach out to our support team as seen on our website.</p>
            <p>With warmth and encouragement,</p>
            <p>The Sober Path Team</p>
        </div>
        <div class=""footer"">
            <p>&copy; 2025 SoberPath.org.za All rights reserved.</p>
            <p><a href=""#"">Privacy Policy</a> | <a href=""#"">Terms of Service</a></p>
        </div>
    </div>
</body>
</html>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

        }




        public async Task Forgot_Password_Email(string subject, string toEmail, string username)
        {
            var apiKey = "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";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("spha2729@gmail.com", "Sober Path");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Request</title>
    <style>
        body {{
            font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #eeeeee;
        }}
        .header h1 {{
            color: #FF8E29;
            margin: 0;
            font-size: 28px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content p {{
            margin-bottom: 15px;
        }}
        .button-container {{
            text-align: center;
            padding: 20px 0;
        }}
        .button {{
            display: inline-block;
            background-color: #FF8E29;
            color: #ffffff;
            padding: 12px 25px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }}
        .code {{
            display: inline-block;
            background-color: #f8f8f8;
            border: 1px solid #eeeeee;
            padding: 10px 15px;
            font-family: monospace;
            font-size: 18px;
            letter-spacing: 2px;
            margin: 15px 0;
        }}
        .footer {{
            text-align: center;
            padding-top: 20px;
            border-top: 1px solid #eeeeee;
            font-size: 12px;
            color: #777777;
        }}
        .footer a {{
            color: #777777;
            text-decoration: none;
        }}
        .warning {{
            color: #ff4444;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Password Reset Request</h1>
        </div>
        <div class=""content"">
            <p>Dear {username},</p>
            <p>We received a request to reset your password for your Sober Path account. If you didn't make this request, please ignore this email.</p>
            
            <p>To reset your password, please click the button below:</p>
        </div>
        <div class=""button-container"">
            <a href=""http://localhost:2025/elegent/authentication/newpassword/"" class=""button"">Reset Password</a>
        </div>
        <div class=""content"">
            <p>Alternatively, you can copy and paste this link into your browser:</p>
            <p><a href=""http://localhost:2025/elegent/authentication/newpassword/"">Click here</a></p>
            
            <p class=""warning"">This link will expire in 24 hours for security reasons.</p>
            
            <p>If you have any questions or need assistance, please contact our support team.</p>
            <p>With care,</p>
            <p>The Sober Path Team</p>
        </div>
        <div class=""footer"">
            <p>&copy; 2025 SoberPath.org.za All rights reserved.</p>
            <p><a href=""#"">Privacy Policy</a> | <a href=""#"">Terms of Service</a></p>
        </div>
    </div>
</body>
</html>
";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

        }
    }
}
