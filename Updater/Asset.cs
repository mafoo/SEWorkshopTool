using System;
using System.Runtime.Serialization;

namespace Gwindalmir.Updater
{
    [DataContract]
    public class Asset
    {
        [DataMember(Name = "created_at")]
        public DateTime Created;

        [DataMember(Name = "id")]
        public uint Id;

        [DataMember(Name = "label")]
        public string Label;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "published_at")]
        public DateTime Published;

        [DataMember(Name = "size")]
        public ulong Size;

        [DataMember(Name = "browser_download_url")]
        public string Url;
    }
}