using System;
using System.Globalization;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Threading;

namespace Speech
{
    class Program
    {
        // Indicate whether asynchronous recognition is complete.
        static bool completed;

        static void Main(string[] args)
        {
            using (SpeechRecognitionEngine recognizer =
              new SpeechRecognitionEngine(new CultureInfo("en-US")))
            {
                // Create a simple grammar that recognizes only a few word choices.
                Choices colors = new Choices();

                // speech recog is BAD. Will need keyword to make it listen (yo superman), 
                        //then it should confirm (you want me to off light? if no, go to next choice with light in it)
                colors.Add(new string[] { "light aun", "lite on", "light off", "lite off", "play a song" });

                // Create a GrammarBuilder object and append the Choices object.
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(colors);

                // Create the Grammar instance and load it into the speech recognition engine.
                Grammar g = new Grammar(gb);
                recognizer.LoadGrammar(g);

/*
                // Create and load a grammar.
                Grammar dictation = new DictationGrammar();
                dictation.Name = "Dictation Grammar";

                recognizer.LoadGrammar(dictation);
                */

                // Configure the input to the recognizer.
                recognizer.SetInputToAudioStream(
                  File.OpenRead(@"C:\Users\user1\Documents\liteon.wav"),
                  new SpeechAudioFormatInfo(
                    44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));

               


                // Attach event handlers.
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(
                    SpeechRecognizedHandler);
                recognizer.RecognizeCompleted +=
                  new EventHandler<RecognizeCompletedEventArgs>(
                    RecognizeCompletedHandler);

                // Perform recognition of the whole file.
                Console.WriteLine("Starting asynchronous recognition...");
                completed = false;
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                while (!completed)
                {
                    Thread.Sleep(333);
                }
                Console.WriteLine("Done.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

       

        // Handle the SpeechRecognized event.
        static void SpeechRecognizedHandler(
          object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && e.Result.Text != null)
            {
                Console.WriteLine("  Recognized text =  {0}", e.Result.Text);
            }
            else
            {
                Console.WriteLine("  Recognized text not available.");
            }
        }

        // Handle the RecognizeCompleted event.
        static void RecognizeCompletedHandler(
          object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("  Error encountered, {0}: {1}",
                  e.Error.GetType().Name, e.Error.Message);
            }
            if (e.Cancelled)
            {
                Console.WriteLine("  Operation cancelled.");
            }
            if (e.InputStreamEnded)
            {
                Console.WriteLine("  End of stream encountered.");
            }

            completed = true;
        }
    }
}