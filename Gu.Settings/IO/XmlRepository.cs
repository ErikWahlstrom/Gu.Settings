namespace Gu.Settings
{
    using System.IO;

    public class XmlRepository : Repository
    {
        public XmlRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public XmlRepository(IRepositorySetting setting) 
            : base(setting)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return XmlHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return XmlHelper.ToStream(item);
        }
    }
}