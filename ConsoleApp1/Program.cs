using System;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BAAM {
    static class thaclass {
        static void Main() {
            var accenter = new CariocaAccent();
            string args;
            Console.WriteLine("Argumento: ");
            args = Console.ReadLine();

            string accented = accenter.Take(args);
            
            Console.WriteLine(accented);

            return;
        }
    }

   

    abstract class AccentEngine {
        virtual protected string path {get; set;}
        public string Take(string message) { 
            //freakin modify the word
            string[] tokens = message.Split(' ');
            Regex regex = new Regex("\\W+\\Z");
            List<string> moddedTokens = new List<string>();
            Root accent = jsonIO.readFile(this.path);
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

            for(int i = 0; i < accent.phrases.Count; i++) {
                var phrases = accent.phrases[i];
                for(int j = 0; j < phrases.og.Count; j++) {
                    if(modded.ToLower().Contains(phrases.og[j]))
                        modded = ReplacePhrase(modded, phrases.og[j].ToLower(), Pick(phrases.mod));
                        break;
                }
            }


            var rnd = new Random();
            int rndNumber = rnd.Next(0, 5);
            if(rndNumber == 4) {
                modded = Pick(accent.begin) + " " + modded;
            }
            if(rndNumber == 3) {
                var alphanumeric = new Regex(@"\W$");
                if(alphanumeric.IsMatch(modded)) {
                    modded = modded + " " + Pick(accent.ending);
                } else {
                    modded = modded + ", " + Pick(accent.ending);               
                }
            }
            //fckin parse the message for tha goofy accent
            modded = Parse(modded);



            return modded;
        } 

        string Pick(List<string> tokens) {
            var random = new Random();
            int index = random.Next(tokens.Count);

            return tokens[index];
        }
        abstract protected string Parse(string message);

        string ReplaceWord(string word, string original, string mod) { 
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

        string ReplacePhrase(string phrase, string original, string mod) {
            //find de index of the start of the phrase
            //find the length of the phrase
            int index;
            int end = original.Length;
            if(phrase.ToLower().Contains(original)) {
                index = phrase.ToLower().IndexOf(original);
            } else {
                return phrase;
            }
            
            string section = phrase.Substring(index, end);
            string moddedSection;
            //if first letter is upper
            if(char.IsUpper(section[0])) {
                //if all are upper
                if(section.Replace(" ", "").All(char.IsUpper)) {
                    moddedSection = mod.ToUpper();
                } else {
                    moddedSection = char.ToUpper(mod[0]) + mod.Substring(1);                        
                        }
            } else {
                moddedSection = mod;
            }

            string modded = phrase.Replace(section, moddedSection);
            return modded;

        }
    }

 sealed class CariocaAccent : AccentEngine {
    override protected string path {get;set;} = "../../../carioca.json";
    override protected string Parse(string message) {
        //cranking xenophobia to 101% YEEEEAHHHH
        var s = new Regex(@"([aeiouAEIOU])s($|[bcdfghjklmnpqrtvxzBCDEFGHJKLMNPQRTVXZ\s\W])");
        var S = new Regex(@"([aeiouAEIOU])S($|[bcdfghjklmnpqrtvzxBCDEFGHJKLMNPQRTVXZ\s\W])");
        List<string>moddedToken = new List<string>();
        string[] tokens = message.Split(' ');
        foreach(string token in tokens) {
            if(S.IsMatch(token)) {
                moddedToken.Add(S.Replace(token, "$1X$2"));
                continue;
            }
            if(s.IsMatch(token)) {
                moddedToken.Add(s.Replace(token, "$1x$2"));
                continue;
            }
            moddedToken.Add(token);
        }
        
        string modded = string.Join(" ", moddedToken);
        return modded;    
    }

 } 

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

//"method": "afterconsoant",
//"regex": ["([aeiou])s([qwrtypdfghjklzxcvbnm])"],
//"toReplace" "s"
//"replacement": ["$1x$2"]
