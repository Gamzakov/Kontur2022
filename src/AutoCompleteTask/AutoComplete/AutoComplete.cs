using System;
using System.Collections.Generic;

namespace AutoComplete
{
    public class AutoCompleter
    {
        private readonly Trie _fullNamesCollection = new();

        public void AddToSearch(List<FullName> fullNames)
        {
            if (fullNames == null)
                throw new ArgumentNullException(nameof(fullNames));

            foreach (var fullName in fullNames)
            {
                _fullNamesCollection.Add(fullName.ToString());
            }
        }

        public List<string> Search(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            if (prefix.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(prefix));

            prefix = string.Join(' ', prefix.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToLower();

            return _fullNamesCollection.GetByPrefix(prefix);
        }
    }

    /// <summary>
    /// Наивная реализация Trie
    /// </summary>
    public class Trie
    {
        private readonly TrieNode _root;

        public Trie()
        {
            _root = new TrieNode();
        }

        /// <summary>
        /// Добавляет слово в коллекцию
        /// </summary>
        /// <param name="value">Слово</param>
        public void Add(string value)
        {
            var currentNode = _root;

            foreach (var letter in value)
            {
                var key = char.ToLower(letter);

                if (!currentNode.ChildrenNodes.TryGetValue(key, out var nextNode))
                {
                    nextNode = new TrieNode();
                    currentNode.ChildrenNodes.Add(key, nextNode);
                }

                currentNode = nextNode;
            }

            currentNode.IsTerminated = true;
            currentNode.Word = value;
        }

        /// <summary>
        /// Выполняет поиск по коллекции
        /// </summary>
        /// <param name="prefix">Префикс, по которому выполняется поиск</param>
        /// <returns>Список найденных слов в коллекции</returns>
        public List<string> GetByPrefix(string prefix)
        {
            var currentNode = _root;

            foreach (var letter in prefix)
            {
                var key = char.ToLower(letter);

                if (!currentNode.ChildrenNodes.TryGetValue(key, out var nextNode))
                    break;

                currentNode = nextNode;
            }

            return new List<string>(CollectWords(currentNode));
        }

        private static IEnumerable<string> CollectWords(TrieNode node)
        {
            if (node.IsTerminated)
                yield return node.Word!;

            foreach (var childNode in node.ChildrenNodes.Values)
            {
                foreach (var word in CollectWords(childNode))
                {
                    yield return word;
                }
            }
        }

        private class TrieNode
        {
            public string? Word { get; set; }
            public Dictionary<char, TrieNode> ChildrenNodes { get; }
            public bool IsTerminated { get; set; }

            public TrieNode()
            {
                ChildrenNodes = new Dictionary<char, TrieNode>();
            }
        }
    }

    public struct FullName
    {
        public string Name;
        public string Surname;
        public string Patronymic;

        public override string ToString()
        {
            var fullName = string.Join(' ', Surname, Name, Patronymic);

            return string.Join(' ', fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
