using UnityEngine;
using System.Collections.Generic;

public class Trie
{
    //The starting point of the Trie.
    private TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    // Insertion
    public void Insert(string word)
    {
        TrieNode node = root;
        //Starts from the root node and iterates over each character in the word.
        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
            {
                //If a character does not have a corresponding child node, it creates one.
                node.Children[c] = new TrieNode();
            }
            node = node.Children[c];
        }
        //Marks the last node as the end of the word 
        node.isEndOfTheWord = true;
    }

    // Deletion
    public bool Delete(string word)
    {
        return DeleteHelper(root, word, 0);
    }

    private bool DeleteHelper(TrieNode node, string word, int index)
    {
        if (index == word.Length)
        {
            //If the end of the word is reached, it unmarks the end-of-word flag.
            if (!node.isEndOfTheWord) return false; // Word not found
            node.isEndOfTheWord = false;
            //If the node has no children, it can be deleted.
            return node.Children.Count == 0;
        }

        char c = word[index];
        if (!node.Children.ContainsKey(c))
        {
            return false; // Word not found
        }

        bool shouldDeleteChild = DeleteHelper(node.Children[c], word, index + 1);

        if (shouldDeleteChild)
        {
            //If the child node can be deleted, it removes it from the current node's children.
            node.Children.Remove(c);
            //Returns true if the current node can also be deleted.
            return node.Children.Count == 0 && !node.isEndOfTheWord;
        }

        return false;
    }

    // Searching
    //Finds the node corresponding to a given prefix.
    private TrieNode SearchNode(string prefix)
    {
        TrieNode node = root;
        //Starts from the root node and iterates over each character in the prefix.
        foreach (char c in prefix)
        {
            if (!node.Children.ContainsKey(c))
            {
                //f a character does not have a corresponding child node, it returns null.
                return null;
            }
            // it moves to the child node and continues.
            node = node.Children[c];
        }
        return node;
    }

    // Collect all words
    // Recursively collects all words starting from a given node.
    private void CollectWords(TrieNode node, string prefix, List<string> results)
    {
        //If the node is null, it returns.
        if (node == null) return;
        if (node.isEndOfTheWord)
        {
            //If the node marks the end of a word, it adds the word (prefix) to the results.
            results.Add(prefix);
            Debug.Log("Found word: " + prefix);
        }

        foreach (var child in node.Children)
        {
            //Recursively collects words from all child nodes, appending each character to the prefix.
            CollectWords(child.Value, prefix + child.Key, results);
        }
    }

    //Returns all words in the Trie that start with the given prefix.
    public List<string> Autocomplete(string prefix)
    {
        //Uses SearchNode to find the node corresponding to the prefix.
        TrieNode node = SearchNode(prefix);
        if (node == null)
        {
            //If the node is null, it logs a message and returns an empty list.
            Debug.Log("No node found for prefix: " + prefix);
            return new List<string>();
        }
        //Otherwise, it collects all words starting from that node and returns them.
        List<string> results = new List<string>();
        CollectWords(node, prefix, results);
        return results;
    }

    // Printing
    public void PrintAllWords()
    {
        Debug.Log("PRINTING WORDS");
        List<string> results = new List<string>();
        CollectWords(root, "", results);
        foreach (string word in results)
        {
            Debug.Log(word);
        }
    }
}