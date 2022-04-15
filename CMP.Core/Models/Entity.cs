using Newtonsoft.Json;

namespace CMP.Core.Models
{
    public abstract class Entity
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        /// <example>5fe3fc2a-cbac-4df0-8031-fdca0f682989</example>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
