namespace SimpleBackup.Domain.UserDefinedDirectories
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class UserDefinedDirectoriesConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var document = XDocument.Parse(section.SelectSingleNode("//userDefinedDirectories").OuterXml);
            var children = document.Descendants("DirectoryConfiguration");
            var directories = children.Select(c => new UserDefinedDirectory(c.Element("FriendlyName").Value, c.Element("Path").Value)).ToList();
            return new UserDefinedDirectoryConfiguration { DirectoryConfiguration = directories.ToArray() };
        }
    }
}