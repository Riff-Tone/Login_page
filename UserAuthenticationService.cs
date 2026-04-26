using HashLibrary;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Login_page
{
    public class UserAuthenticationService
    {
        private readonly HttpServerUtility Server;

        public UserAuthenticationService(HttpServerUtility server)
        {
            Server = server;
        }

        private string MemberXmlPath
        {
            get { return Server.MapPath("~/../Page10/Data/Member.xml"); }
        }

        private string StaffXmlPath
        {
            get { return Server.MapPath("~/../Page10/Data/Staff.xml"); }
        }

        public string Login(string username, string password)
        {
            EnsureMemberXmlExists();
            EnsureStaffXmlExists();

            string passwordHash = HashPassword(password);

            XDocument memberDoc = XDocument.Load(MemberXmlPath);

            var member = memberDoc.Root.Elements("Member")
                .FirstOrDefault(m =>
                    string.Equals((string)m.Element("Username"), username, StringComparison.OrdinalIgnoreCase)
                    && (string)m.Element("PasswordHash") == passwordHash
                );

            if (member != null)
            {
                string userId = (string)member.Element("UserId");
                string bankId = (string)member.Element("BankAccountId");

                return "Member|" + userId + "|" + bankId;
            }

            XDocument staffDoc = XDocument.Load(StaffXmlPath);

            var staff = staffDoc.Root.Elements("Staff")
                .FirstOrDefault(s =>
                    string.Equals((string)s.Element("Username"), username, StringComparison.OrdinalIgnoreCase)
                    && (string)s.Element("PasswordHash") == passwordHash
                );

            if (staff != null)
            {
                string userId = (string)staff.Element("UserId");
                return "Staff|" + userId;
            }

            return "Invalid";
        }

        public string Signup(string username, string password)
        {
            EnsureMemberXmlExists();
            EnsureStaffXmlExists();

            XDocument memberDoc = XDocument.Load(MemberXmlPath);

            bool exists = memberDoc.Root.Elements("Member")
                .Any(m => string.Equals((string)m.Element("Username"), username, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                return "Username already exists.";
            }

            string userId = GenerateUserId();
            string bankId = GenerateBankAccountId(memberDoc);
            string hash = HashPassword(password);

            memberDoc.Root.Add(
                new XElement("Member",
                    new XElement("UserId", userId),
                    new XElement("Username", username),
                    new XElement("PasswordHash", hash),
                    new XElement("BankAccountId", bankId)
                )
            );

            memberDoc.Save(MemberXmlPath);

            return "UserId=" + userId + ", BankAccountId=" + bankId;
        }

        public string CreateStaff(string username, string password)
        {
            EnsureMemberXmlExists();
            EnsureStaffXmlExists();

            XDocument staffDoc = XDocument.Load(StaffXmlPath);

            bool exists = staffDoc.Root.Elements("Staff")
                .Any(s => string.Equals((string)s.Element("Username"), username, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                return "Staff username already exists.";
            }

            string userId = GenerateUserId();
            string hash = HashPassword(password);

            staffDoc.Root.Add(
                new XElement("Staff",
                    new XElement("UserId", userId),
                    new XElement("Username", username),
                    new XElement("PasswordHash", hash)
                )
            );

            staffDoc.Save(StaffXmlPath);

            return "Staff created. UserId=" + userId;
        }

        public string GetUserByUsername(string username)
        {
            EnsureMemberXmlExists();
            EnsureStaffXmlExists();

            XDocument memberDoc = XDocument.Load(MemberXmlPath);

            var member = memberDoc.Root.Elements("Member")
                .FirstOrDefault(m =>
                    string.Equals((string)m.Element("Username"), username, StringComparison.OrdinalIgnoreCase)
                );

            if (member != null)
            {
                string userId = (string)member.Element("UserId");
                string bankId = (string)member.Element("BankAccountId");

                return "Member|" + userId + "|" + bankId;
            }

            XDocument staffDoc = XDocument.Load(StaffXmlPath);

            var staff = staffDoc.Root.Elements("Staff")
                .FirstOrDefault(s =>
                    string.Equals((string)s.Element("Username"), username, StringComparison.OrdinalIgnoreCase)
                );

            if (staff != null)
            {
                string userId = (string)staff.Element("UserId");
                return "Staff|" + userId;
            }

            return "Invalid";
        }

        private void EnsureMemberXmlExists()
        {
            string folder = Path.GetDirectoryName(MemberXmlPath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!File.Exists(MemberXmlPath))
            {
                XDocument doc = new XDocument(new XElement("Members"));
                doc.Save(MemberXmlPath);
            }
        }

        private void EnsureStaffXmlExists()
        {
            string folder = Path.GetDirectoryName(StaffXmlPath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!File.Exists(StaffXmlPath))
            {
                XDocument doc = new XDocument(new XElement("StaffUsers"));
                doc.Save(StaffXmlPath);
            }
        }

        private string GenerateUserId()
        {
            int maxId = 0;

            XDocument memberDoc = XDocument.Load(MemberXmlPath);

            foreach (var member in memberDoc.Root.Elements("Member"))
            {
                int id;
                if (int.TryParse((string)member.Element("UserId"), out id) && id > maxId)
                {
                    maxId = id;
                }
            }

            XDocument staffDoc = XDocument.Load(StaffXmlPath);

            foreach (var staff in staffDoc.Root.Elements("Staff"))
            {
                int id;
                if (int.TryParse((string)staff.Element("UserId"), out id) && id > maxId)
                {
                    maxId = id;
                }
            }

            return (maxId + 1).ToString();
        }

        private string GenerateBankAccountId(XDocument doc)
        {
            int maxId = 0;

            foreach (var member in doc.Root.Elements("Member"))
            {
                int id;
                if (int.TryParse((string)member.Element("BankAccountId"), out id) && id > maxId)
                {
                    maxId = id;
                }
            }

            return (maxId + 1).ToString();
        }

        private string HashPassword(string password)
        {
            return HashUtility.ComputeSha256(password);
        }
    }
}