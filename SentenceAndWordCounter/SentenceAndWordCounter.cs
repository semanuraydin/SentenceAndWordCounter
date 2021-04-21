using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SentenceAndWordCounter
{
    /// <summary>
    /// * , noktalama işareti fakat sayısal verilerde yanlış kelime üretir. 17,5'i 17 ve 5 hesaplar.
    /// " ve ' işaretleri arasında geçen cümleleri ayırmalı.
    /// </summary>
    class SentenceAndWordCounter
    {
        int threadCount = 5;
        int runningThreadCount = 0;
        char previousChar = ' ';
        char[] character = new char[1];
        bool isFileReadFinished = false;
        int fileCharacterIndex = 0;
        int sentenceCount = 0;
        int totalWordCount = 0;


        Hashtable htWords;
        List<Queue<string>> queueList;
        List<char> punctuationMarks;

        public SentenceAndWordCounter()
        {
            htWords = new Hashtable();
            queueList = new List<Queue<string>>();

            punctuationMarks = new List<char>();
            punctuationMarks.Add(' ');
            punctuationMarks.Add('.');
            punctuationMarks.Add(',');
            punctuationMarks.Add('?');
            punctuationMarks.Add('!');
        }

        public void MainThread()
        {
            try
            {
                //Console.WriteLine("Dosyanın yolunu giriniz...");
                //string filePath = Console.ReadLine();
                string filePath = "C:\\file.txt";

                //Console.WriteLine("Yardımcı Thread sayısını giriniz...");
                //int.TryParse(Console.ReadLine(), out threadCount);
                for (int i = 0; i < threadCount; i++)
                {
                    queueList.Add(new Queue<string>());
                }

                for (int i = 0; i < threadCount; i++)
                {
                    Thread thread = new Thread(SubThread);
                    thread.Start(i);
                }

                FileStream fs = new FileStream(filePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                StringBuilder sbSentence = new StringBuilder();

                while (!sr.EndOfStream)
                {
                    sr.Read(character, fileCharacterIndex, 1);
                    if (character[0] == '.' || character[0] == '?' || character[0] == '!')
                    {
                        string[] kelimeler = sbSentence.ToString().Replace(',', ' ').Split(' ');
                        totalWordCount += kelimeler.Length;
                        queueList[sentenceCount % threadCount].Enqueue(sbSentence.ToString().Replace(',', ' ')); // cümlenin kaçıncı olduğuna bakıyoruz. threadCount'a göre mod alıp ilgili kuyruğa ekliyoruz.

                        sentenceCount++;
                        sbSentence.Clear();
                    }
                    else
                    {
                        if (punctuationMarks.Contains(previousChar) && punctuationMarks.Contains(character[0]))
                        {
                            // üst üste birden fazla noktalama işareti gelmiş ise, kelime olarak algılaması engelleniyor.
                        }
                        else
                        {
                            sbSentence.Append(character);
                        }
                    }
                    previousChar = character[0];
                }
                sr.Dispose();
                fs.Dispose();
                isFileReadFinished = true;
                decimal averageWordCount = (decimal)totalWordCount / sentenceCount;
                Console.WriteLine("Cümle Sayısı           : " + sentenceCount);
                Console.WriteLine("Ortalama Kelime Sayısı : " + averageWordCount);

                while (runningThreadCount > 0)
                {
                    Thread.Sleep(1000);
                }

                foreach (string key in htWords.Keys)
                {
                    Console.WriteLine(key.PadRight(30, ' ') + " : " + htWords[key]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private void SubThread(object threadNumberParam)
        {
            bool working = true;
            runningThreadCount++;
            int threadIndex = 0;
            try
            {
                int.TryParse(threadNumberParam.ToString(), out threadIndex);
                //#if(DEBUG)
                Console.WriteLine(DateTime.Now.ToString() + " : " + threadIndex + ". thread başladı");
                //#endif
                Queue<string> queue = queueList[threadIndex];
                do
                {
                    if (isFileReadFinished && queue.Count == 0)
                    {
                        working = false;
                        break;
                    }
                    else
                    {
                        if (queue.Count > 0)
                        {
                            string sentence = queue.Dequeue();
                            string[] words = sentence.Split(' ');
                            foreach (string word in words)
                            {
                                if (htWords.ContainsKey(word))
                                {
                                    htWords[word] = int.Parse(htWords[word].ToString()) + 1;
                                }
                                else
                                {
                                    htWords.Add(word, 1);
                                }
                            }
                        }
                    }
                } while (working);
                //#if(DEBUG)
                //                Console.WriteLine(DateTime.Now.ToString() + " : " + threadIndex + ". thread bitti");
                //#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " : " + threadIndex + ". Thread : " + ex.Message);
            }
            runningThreadCount--;
        }
    }
}
