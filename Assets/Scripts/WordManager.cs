using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public TextAsset WordFile;

    private readonly HashSet<string> _words = new();

    private readonly  Dictionary<char, int> _letterFrequency = new();

    private int _totalLetters;

    private void Awake()
    {
        // read all words from the file
        string[] lines = WordFile.text.Split('\n');
        foreach (string line in lines)
        {
            string word = line.Trim();
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
    }

    public bool IsWord(string word)
    {
        return _words.Contains(word);
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