using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSense.Models
{
    public class EmotionFaces
    {
        public int id { get; set; }

        [JsonProperty(PropertyName ="Anger")]
        public float Anger;
        [JsonProperty(PropertyName = "Contempt")]
        public float Contempt;
        [JsonProperty(PropertyName = "Disgust")]
        public float Disgust;
        [JsonProperty(PropertyName = "Fear")]
        public float Fear;
        [JsonProperty(PropertyName = "Happinenss")]
        public float Happiness;
        [JsonProperty(PropertyName = "Neutral")]
        public float Neutral;
        [JsonProperty(PropertyName = "Sadness")]
        public float Sadness;
        [JsonProperty(PropertyName = "Surprise")]
        public float Surprise;


        [JsonProperty(PropertyName ="EmoDate")]
        public DateTime EmoDate;
        [JsonProperty(PropertyName = "emotion")]
        public string emotion { get; set; }
        [JsonProperty(PropertyName = "FacesWithEmo")]
        public int FacesWithEmo { get; set; }
        //public int NewFacesWithEmo { get; set; }

    }
}
