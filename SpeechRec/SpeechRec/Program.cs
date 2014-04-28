using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Object;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace SpeechRec
{
    class Program
    {
        static void Main(string[] args)
        {
            //create new SpeechRocognizer instance.
            SpeechRecognizer sr = new SpeechRecognizer();

            Choices colors = new Choices();
            colors.Add( new string[] {"red", "green", "blue"} );

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(colors);

            //Create the Grammar instance.
            Grammar g = new Grammar(gb);

            //after the grammar is created, it must be loaded into the speech recognizer

            sr.LoadGrammar(g);

            //Register for speech recog event notofication
            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);
               //sr_SpeechRecognized is the name of the DEVELOPER WRITTEN EVENT HANDLER


        }

        

        
    }
}
