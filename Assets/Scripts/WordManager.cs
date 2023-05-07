using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public TextAsset WordFile;

    private const int MinWordLength = 4;

    private readonly HashSet<string> _words = new();

    private readonly Dictionary<string, (float, float)> _sentiment = new();

    private readonly  Dictionary<char, int> _letterFrequency = new();

    private int _totalLetters;

    private void Awake()
    {
        // set invariant culture because float parsing and stuff
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        // read all words from the file
        string[] lines = WordFile.text.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Split();
            string word = parts[0];
            float posSentiment = float.Parse(parts[1]);
            float negSentiment = float.Parse(parts[2]);
            _sentiment.Add(word, (posSentiment, negSentiment));
            if (word.Length < MinWordLength)
            {
                continue;
            }
            _words.Add(word);
            foreach (char c in word)
            {
                if (_letterFrequency.ContainsKey(c))
                {
                    _letterFrequency[c]++;
                }
                else
                {
                    _letterFrequency.Add(c, 1);
                }
                _totalLetters++;
            }
        }

        Debug.Log("Loaded " + _words.Count + " words");
    }

    public bool IsWord(string word)
    {
        word = word.ToUpper();
        return _words.Contains(word);
    }

    public (float, float) GetSentiment(string word)
    {
        word = word.ToUpper();
        (float pos, float neg) = _sentiment[word];
        if (word.Contains("DEVIL"))
        {
            (pos, neg) = (0, 6.66f); // devilish >:)
        }
        return (pos, neg);
    }

    public char GetLetter()
    {
        // weighted pick based on letter frequencies
        int r = Random.Range(0, _totalLetters);
        foreach (KeyValuePair<char, int> pair in _letterFrequency)
        {
            r -= pair.Value;
            if (r <= 0)
            {
                return pair.Key;
            }
        }
        throw new System.Exception("Should never happen");
    }
}