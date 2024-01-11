using System;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BAAM {
    static class thaclass {
        static void Main() {
            string args;
            Console.WriteLine("Argumento: ");
            args = Console.ReadLine();

            string accented = AccentEngine.Take(args);
            
            Console.WriteLine(accented);

            return;
        }
    }

    static class AccentEngine {
        public static string Take(string message) {
            string[] tokens = message.Split(' ');
            Regex regex = new Regex("\\W+\\Z");
            List<string> moddedTokens = new List<string>();
            Root accent = jsonIO.readFile("../../../carioca.json");
            foreach(var token in tokens) {
                string modifiedToken = "";              //token modificado
                string originalWord = "";               //palavra original
                string punct = "";                             //pontuação?
                int punctIndex = 0;
                Match punctMatch = regex.Match(token);  //indice da pontuação?
                if(punctMatch.Success) {
                    punctIndex = punctMatch.Index;
                    originalWord = token.Remove(punctIndex, 1);
                    punct = token[punctIndex].ToString();
                } else {
                    originalWord = token;
                }
                // if matching token
                for(int i = 0;  i < accent.words.Count; i++ ) {
                    var words = accent.words[i];
                    for(int j = 0; j < words.og.Count; j++) {
                        if(originalWord.ToLower() == words.og[j]) {
                            modifiedToken = ReplaceWord(originalWord, originalWord.ToLower(), Pick(words.mod));
                            break;
                        }
                    }
                    if(modifiedToken != string.Empty) {
                        break;
                    }
                }
                if(modifiedToken == string.Empty)
                    modifiedToken = originalWord;
                // endif
                modifiedToken += punct;

                moddedTokens.Add(modifiedToken);
            }

            string modded = String.Join(" ", moddedTokens);

            //for(int i = 0; i < accent.phrases.Count; i++) {
            //    var phrases = accent.phrases[i];
            //    for(int j = 0; j < phrases.og.Count; j++) {
            //        if(modded.ToLower().Contains(phrases.og[j]))
            //            modded = Regex.Replace();
            //            break;
            //    }
            //}

            return modded;
        } 

        static string Pick(List<string> tokens) {
            var random = new Random();
            int index = random.Next(tokens.Count);

            return tokens[index];
        }
        static string Parse(string message) {

            return "ouafsgh";    
        }

        static string ReplaceWord(string word, string original, string mod) { 
            string[] token = word.Split(' ');
            string[] modifiedTokens = token;
            List<string> toReplace = new List<string>();
            for(int i = 0; i < token.Length; i++) {
                if(token[i].ToLower() == original) {
                    //if first letter is upper
                    if(char.IsUpper(token[i][0])) {
                        //if all are upper
                        if(token[i].All(char.IsUpper)) {
                            modifiedTokens[i] = mod.ToUpper();
                        } else {
                            modifiedTokens[i] = char.ToUpper(mod[0]) + mod.Substring(1);                        
                        }
                    } else {
                        modifiedTokens[i] = mod;
                    }
                } else {
                        modifiedTokens[i] = token[i];
                }
            }

            string message = string.Join(" ", modifiedTokens);

            return message;
        }

        //static string ReplacePhrase(string phrase, string orignal, string mod) {
        //    string section;
        //    if(phrase.Contains(orignal.ToLower())) {
        //        
        //    }
        //}
    }

    struct WordStructure {
        [JsonPropertyName("og")]
        public string[] og;
        [JsonPropertyName("mod")]
        public string[] mod;
    }
      struct AcccentStructure {
        [JsonPropertyName("words")]
        public WordStructure[] words;
        [JsonPropertyName("phrases")]
        public WordStructure[] phrases;
        [JsonPropertyName("ending")]
        public string[] ending;
        [JsonPropertyName("begin")]
        public string[] begin;
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Phrase
    {
        public List<string> og { get; set; }
        public List<string> mod { get; set; }
    }

    public class Root
    {
        public List<Word> words { get; set; }
        public List<Phrase> phrases { get; set; }
        public List<string> ending { get; set; }
        public List<string> begin { get; set; }
    }

    public class Word
    {
        public List<string> og { get; set; }
        public List<string> mod { get; set; }
    }

    static class jsonIO {
        static public Root readFile(string path) {
            string json = File.ReadAllText(path);
            Root deserialized = JsonSerializer.Deserialize<Root>(json);
            return deserialized;
        }
    }
}
