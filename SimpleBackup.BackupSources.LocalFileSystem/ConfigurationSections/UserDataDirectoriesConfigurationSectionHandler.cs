namespace SimpleBackup.BackupSources.LocalFileSystem.ConfigurationSections
{
    using System.Configuration;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using SimpleBackup.BackupSources.LocalFileSystem.Entities;

    public class UserDataDirectoriesConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var document = XDocument.Parse(section.SelectSingleNode("//userDataDirectories").OuterXml);
            var children = document.Descendants("DirectoryConfiguration");
            var directories = children.Select(c => new UserDataDirectory(c.Element("FriendlyName").Value, c.Element("Path").Value)).ToList();
            return new UserDataDirectoriesConfiguration { Directories = directories.ToArray() };
        }
    }
}