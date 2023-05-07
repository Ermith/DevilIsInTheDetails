using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordManager : MonoBehaviour
{
    public TextAsset WordFile;

    private const int MinWordLength = 4;

    private readonly HashSet<string> _words = new();

    private readonly Dictionary<string, (float, float)> _sentiment = new();

    private readonly  Dictionary<char, int> _letterFrequency = new();

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
            return (0, 6.66f); // devilish >:)
        }
        if (pos == 0 && neg == 0 && word[^1] == 'S')
        {
            // plural
            return GetSentiment(word[..^1]);
        }
        return (pos, neg);
    }

    public char GetLetter(float karma)
    {
        // makes it so karma 0 => power of 0.75 approximately and at 0 and 1 it matches the original
        float pow = MathF.Pow((karma + 1f) / 2, 0.4f) * 2 - 1;

        if (karma < -0.05f && Random.value < 0.02f + 0.15f * (-karma))
        {
            return '#'; // terrible curse
        }

        if (karma > 0.05f && Random.value < 0.02f + 0.15f * karma)
        {
            return '\0'; // no curse at all!
        }

        float total = 0;
        foreach (KeyValuePair<char, int> pair in _letterFrequency)
        {
            total += MathF.Pow(pair.Value, pow);
        }
        // weighted pick based on letter frequencies
        float r = Random.Range(0f, total);
        foreach (KeyValuePair<char, int> pair in _letterFrequency)
        {
            r -= MathF.Pow(pair.Value, pow);
            if (r <= 0)
            {
                return pair.Key;
            }
        }
        throw new System.Exception("Should never happen");
    }
}