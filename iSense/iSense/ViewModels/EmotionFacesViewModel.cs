using EmotionAPI;
using EmotionAPI.Contract;
using GalaSoft.MvvmLight.Command;
using iSense.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSense.ViewModels
{
    public class EmotionFacesViewModel : INotifyPropertyChanged
    {


        private ObservableCollection<EmotionFaces> emoFacesCollection;
        public ObservableCollection<EmotionFaces> EmoFacesCollection
        {
            get
            {
                return emoFacesCollection;
            }
            set
            {
                emoFacesCollection = value;
                OnPropertyChanged("EmoFacesCollection");
            }
        }

        private int pplCounter;
        public int PplCounter
        {
            get
            {
                return pplCounter;
            }
            set
            {
                pplCounter = value;
            }
        }

        private bool AngerExceds = false;
        private bool ContemptExceds = false;
        private bool DisgustExceds = false;
        private bool FearExceds = false;
        private bool HappinessExceds = false;
        private bool NeutralExceds = false;
        private bool SadnessExceds = false;
        private bool SurpriseExceds = false;

        //private float[] tempFacesEmos = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
        SortedDictionary<string, float> tempFacesEmos = new SortedDictionary<string, float>();

        public RelayCommand EditFeelsCommand { get; private set; }

        public EmotionFacesViewModel()
        {


            this.EmoFacesCollection = new ObservableCollection<EmotionFaces>()
            {
                new EmotionFaces() {emotion = "Anger", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Contempt", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Disgust", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Fear", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Happiness", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Neutral", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Sadness", FacesWithEmo = 0 },
                new EmotionFaces() {emotion = "Surprise", FacesWithEmo = 0 },
            };
            //this.EditFeelsCommand = new RelayCommand(this.ChangeCollection);

            tempFacesEmos.Add("Anger", 0);
            tempFacesEmos.Add("Contempt", 0);
            tempFacesEmos.Add("Disgust", 0);
            tempFacesEmos.Add("Fear", 0);
            tempFacesEmos.Add("Happiness", 0);
            tempFacesEmos.Add("Neutral", 0);
            tempFacesEmos.Add("Sadness", 0);
            tempFacesEmos.Add("Surprise", 0);
        }

        public async Task<ObservableCollection<EmotionFaces>> GetEmotionsWithFaces(Stream ImageStream)
        {
            var emoServiceClient = new EmotionServiceClient(GeneralConstants.OxfordEmotionAPIKey);
            Emotion[] emotions = await emoServiceClient.RecognizeAsync(ImageStream);
            bool ProcessEmotionScoresSuccess = await ProcessEmotionScores(emotions);
            return this.EmoFacesCollection;
        }

        public async Task<bool> ProcessEmotionScores(Emotion[] emotionsArray)
        {
            pplCounter = emotionsArray.Length;

            try
            {
                //foreach (EmotionFaces emoFace in this.EmoFacesCollection)
                //{
                //    emoFace.NewFacesWithEmo = 0;
                //}

                tempFacesEmos["Anger"] = 0;
                tempFacesEmos["Contempt"] = 0;
                tempFacesEmos["Disgust"] = 0;
                tempFacesEmos["Fear"] = 0;
                tempFacesEmos["Happiness"] = 0;
                tempFacesEmos["Neutral"] = 0;
                tempFacesEmos["Sadness"] = 0;
                tempFacesEmos["Surprise"] = 0;

                foreach (EmotionFaces item in this.EmoFacesCollection)
                {
                    item.FacesWithEmo = 0;

                }


                foreach (Emotion emotion in emotionsArray)
                {
                    AngerExceds = false;
                    ContemptExceds = false;
                    DisgustExceds = false;
                    FearExceds = false;
                    HappinessExceds = false;
                    NeutralExceds = false;
                    SadnessExceds = false;
                    SurpriseExceds = false;

                    if (emotion.Scores.Anger > 0.4)
                    {
                        this.tempFacesEmos["Anger"] = emotion.Scores.Anger;
                        if (this.EmoFacesCollection[0].Anger > 0)
                        {
                            this.EmoFacesCollection[0].Anger = (this.EmoFacesCollection[0].Anger + emotion.Scores.Anger) / 2;
                        }


                        else
                        {
                            this.EmoFacesCollection[0].Anger = emotion.Scores.Anger;
                        }


                        AngerExceds = true;

                    }
                    if (emotion.Scores.Contempt > 0.4)
                    {
                        this.tempFacesEmos["Contempt"] = emotion.Scores.Contempt;
                        if (this.EmoFacesCollection[1].Contempt > 0)
                        {
                            this.EmoFacesCollection[1].Contempt = (this.EmoFacesCollection[1].Contempt + emotion.Scores.Contempt) / 2;

                        }
                        else
                        {
                            this.EmoFacesCollection[1].Contempt = emotion.Scores.Contempt;
                        }

                        ContemptExceds = true;
                    }
                    if (emotion.Scores.Disgust > 0.4)
                    {
                        this.tempFacesEmos["Disgust"] = emotion.Scores.Disgust;
                        if (this.EmoFacesCollection[2].Disgust > 0)
                        {
                            this.EmoFacesCollection[2].Disgust = (this.EmoFacesCollection[2].Disgust + emotion.Scores.Disgust) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[2].Disgust = emotion.Scores.Disgust;
                        }

                        DisgustExceds = true;
                    }
                    if (emotion.Scores.Fear > 0.4)
                    {
                        this.tempFacesEmos["Fear"] = emotion.Scores.Fear;
                        if (this.EmoFacesCollection[3].Fear > 0)
                        {
                            this.EmoFacesCollection[3].Fear = (this.EmoFacesCollection[3].Fear + emotion.Scores.Fear) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[3].Fear = emotion.Scores.Fear;


                        }

                        FearExceds = true;
                    }
                    if (emotion.Scores.Happiness > 0.4)
                    {
                        this.tempFacesEmos["Happiness"] = emotion.Scores.Happiness;
                        if (this.EmoFacesCollection[4].Happiness > 0)
                        {
                            this.EmoFacesCollection[4].Happiness = (this.EmoFacesCollection[4].Happiness + emotion.Scores.Happiness) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[4].Happiness = emotion.Scores.Happiness;
                        }

                        HappinessExceds = true;
                    }
                    if (emotion.Scores.Neutral > 0.4)
                    {
                        this.tempFacesEmos["Neutral"] = emotion.Scores.Neutral;
                        if (this.EmoFacesCollection[5].Neutral > 0)
                        {
                            this.EmoFacesCollection[5].Neutral = (this.EmoFacesCollection[5].Neutral + emotion.Scores.Neutral) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[5].Neutral = emotion.Scores.Neutral;
                        }

                        NeutralExceds = true;
                    }
                    if (emotion.Scores.Sadness > 0.4)
                    {
                        this.tempFacesEmos["Sadness"] = emotion.Scores.Sadness;
                        if (this.EmoFacesCollection[6].Sadness > 0)
                        {
                            this.EmoFacesCollection[6].Sadness = (this.EmoFacesCollection[6].Sadness + emotion.Scores.Sadness) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[6].Sadness = emotion.Scores.Sadness;
                        }

                        SadnessExceds = true;
                    }
                    if (emotion.Scores.Surprise > 0.4)
                    {
                        this.tempFacesEmos["Surprise"] = emotion.Scores.Surprise;
                        if (this.EmoFacesCollection[7].Surprise > 0)
                        {
                            this.EmoFacesCollection[7].Surprise = (this.EmoFacesCollection[7].Surprise + emotion.Scores.Surprise) / 2;
                        }
                        else
                        {
                            this.EmoFacesCollection[7].Surprise = emotion.Scores.Surprise;
                        }

                        SurpriseExceds = true;
                    }
                }
                //var orderedTempFacesEmos = this.tempFacesEmos.OrderBy(x => x.Value).ToDictionary(x => x.Key , x => x.Value);
                var output = this.tempFacesEmos.OrderBy(e => e.Value).Select(e => new { Key = e.Key, Value = e.Value }).ToList();



                switch (output[7].Key.ToString())
                {
                    case "Anger":
                        this.EmoFacesCollection[0].FacesWithEmo++;
                        break;
                    case "Contempt":
                        this.EmoFacesCollection[1].FacesWithEmo++;
                        break;
                    case "Disgust":
                        this.EmoFacesCollection[2].FacesWithEmo++;
                        break;
                    case "Fear":
                        this.EmoFacesCollection[3].FacesWithEmo++;
                        break;
                    case "Happiness":
                        this.EmoFacesCollection[4].FacesWithEmo++;
                        break;
                    case "Neutral":
                        this.EmoFacesCollection[5].FacesWithEmo++;
                        break;
                    case "Sadness":
                        this.EmoFacesCollection[6].FacesWithEmo++;
                        break;
                    case "Surprise":
                        this.EmoFacesCollection[7].FacesWithEmo++;
                        break;
                    default:
                        break;

                }
                //await SyncAsync();

            }
            catch (Exception ex)
            {
                // TODO handle it
                Debug.WriteLine("Exception thrown: {0}", ex.Message);
                return false;
            }
            //if(AngerExceds && ContemptExceds || DisgustExceds || FearExceds)
            return true;
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

