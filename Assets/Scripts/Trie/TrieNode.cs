using System.Collections.Generic;

public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; private set; }
    public bool isEndOfTheWord { get; set; }

    public TrieNode()
    {
        Children = new Dictionary<char, TrieNode>();
        isEndOfTheWord = false;
    }
}
