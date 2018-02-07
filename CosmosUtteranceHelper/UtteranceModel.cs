using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosUtteranceHelper
{    
    public class UtteranceModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Utterance[] Utterances { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Utterance
    {
        public string text { get; set; }
        public string intentName { get; set; }
        public EntityLabel[] entityLabels { get; set; }
    }

    public class EntityLabel
    {
        public string entityName { get; set; }
        public int startCharIndex { get; set; }
        public int endCharIndex { get; set; }
    }

    
}
