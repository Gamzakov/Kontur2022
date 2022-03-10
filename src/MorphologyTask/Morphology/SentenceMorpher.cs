using System;
using System.Collections.Generic;
using System.Linq;

namespace Morphology
{
    public class SentenceMorpher
    {
        private readonly Dictionary<string, Dictionary<int, string>> _morphTable;
        private readonly Dictionary<string, int> _tagTable;

        private SentenceMorpher(Dictionary<string, Dictionary<int, string>> morphTable,
            Dictionary<string, int> tagTable)
        {
            _morphTable = morphTable;
            _tagTable = tagTable;
        }

        /// <summary>
        ///     Создает <see cref="SentenceMorpher"/> из переданного набора строк словаря.
        /// </summary>
        /// <remarks>
        ///     В этом методе должен быть код инициализации: 
        ///     чтение и преобразование входных данных для дальнейшего их использования
        /// </remarks>
        /// <param name="dictionaryLines">
        ///     Строки исходного словаря OpenCorpora в формате plain-text.
        ///     <code> СЛОВО(знак_табуляции)ЧАСТЬ РЕЧИ( )атрибут1[, ]атрибут2[, ]атрибутN </code>
        /// </param>
        public static SentenceMorpher Create(IEnumerable<string> dictionaryLines)
        {
            var morphTable = new Dictionary<string, Dictionary<int, string>>(StringComparer.CurrentCultureIgnoreCase);
            var tagTable = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

            var isCurrentLineInNormalForm = false;
            var normalForm = string.Empty;

            foreach (var line in dictionaryLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (char.IsDigit(line[0]))
                {
                    isCurrentLineInNormalForm = true;
                    continue;
                }

                var splittedLine = line.Split(new[] {' ', ',', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                var word = splittedLine[0];
                var tags = splittedLine[1..];
                var tagsCode = ComputeTagsCode(tags, tagTable);

                if (isCurrentLineInNormalForm)
                {
                    normalForm = word;

                    if (!morphTable.ContainsKey(normalForm))
                        morphTable.Add(normalForm, new Dictionary<int, string>());

                    isCurrentLineInNormalForm = false;
                    continue;
                }

                if (!morphTable.ContainsKey(normalForm))
                    continue;

                if (!morphTable[normalForm].ContainsKey(tagsCode))
                    morphTable[normalForm].Add(tagsCode, word);
            }

            return new SentenceMorpher(morphTable, tagTable);
        }

        /// <summary>
        /// Вычисляет код для набора тегов
        /// </summary>
        /// <param name="tags">Список тегов, для которых необходимо вычислить код</param>
        /// <param name="tagsTable">Таблица тегов</param>
        /// <returns>Код для набора тегов</returns>
        private static int ComputeTagsCode(IEnumerable<string> tags, IDictionary<string, int> tagsTable)
        {
            return tags.Aggregate(1, (current, tag) => current * GetOrCreateTagCode(tag, tagsTable));
        }

        /// <summary>
        /// Возвращает код для тега, если тег отсутствует в таблице создает новый код
        /// </summary>
        /// <param name="tag">Тег</param>
        /// <param name="tagsTable">Таблица тегов</param>
        /// <returns>Код тега</returns>
        private static int GetOrCreateTagCode(string tag, IDictionary<string, int> tagsTable)
        {
            if (tagsTable.TryGetValue(tag, out var tagCode))
                return tagCode;

            tagCode = PrimeNumberGenerator.GetNextFrom(tagsTable.Count != 0 ? tagsTable.Values.Max() : 0);
            tagsTable.Add(tag, tagCode);

            return tagCode;
        }

        /// <summary>
        ///     Выполняет склонение предложения согласно указанному формату
        /// </summary>
        /// <param name="sentence">
        ///     Входное предложение <para/>
        ///     Формат: набор слов, разделенных пробелами.
        ///     После слова может следовать спецификатор требуемой части речи (формат описан далее),
        ///     если он отсутствует - слово требуется перенести в выходное предложение без изменений.
        ///     Спецификатор имеет следующий формат: <code>{ЧАСТЬ РЕЧИ,аттрибут1,аттрибут2,..,аттрибутN}</code>
        ///     Если для спецификации найдётся несколько совпадений - используется первое из них
        /// </param>
        public virtual string Morph(string sentence)
        {
            var morphedWords = new List<string>();
            var lines = sentence.Split(new[] {' ', '\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var splittedLine = line.Split(new[] {'{', ',', '}'}, StringSplitOptions.RemoveEmptyEntries);
                var word = splittedLine[0];
                var tags = splittedLine[1..];

                if (!tags.Any() || tags.Length < 2)
                {
                    morphedWords.Add(word);
                    continue;
                }

                var allTagsAreValid = tags.Aggregate(true, (current, tag) => current & _tagTable.ContainsKey(tag));

                morphedWords.Add(allTagsAreValid ? GetMorphedForm(word, tags) : word);
            }

            return string.Join(' ', morphedWords);
        }

        /// <summary>
        /// Находит ближайшую морфологическую форму для нормальной формы и набора тегов
        /// </summary>
        /// <param name="normalForm">Нормальная форма слова</param>
        /// <param name="tags">Список тегов для приведенной нормальной формы</param>
        /// <returns>Возвращает ближайшую морфологическую форму</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetMorphedForm(string normalForm, IEnumerable<string> tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            var tagsList = tags.ToList();

            if (!_morphTable.ContainsKey(normalForm))
                return normalForm;

            var closest = 0;
            var morphedWord = normalForm;
            foreach (var (tagsCode, morphedForm) in _morphTable[normalForm])
            {
                var matchScore = tagsList.Select(tag => _tagTable[tag]).Count(tagCode => tagsCode % tagCode == 0);
                if (matchScore <= closest)
                    continue;
                closest = matchScore;
                morphedWord = morphedForm;
            }

            return morphedWord;
        }
    }

    /// <summary>
    /// Класс представляющий генератор простых чисел
    /// </summary>
    public static class PrimeNumberGenerator
    {
        /// <summary>
        /// Возвращает простое число, следующее после начального
        /// </summary>
        /// <param name="start">Начальное число</param>
        /// <returns>Простое число</returns>
        public static int GetNextFrom(int start = 0)
        {
            var current = start + 1;

            if (current is 1 or 2 or 3)
                return current;

            bool isPrime;
            do
            {
                isPrime = true;
                current++;
                for (var i = 2; i <= current / 2; i++)
                {
                    if (current % i != 0)
                        continue;

                    isPrime = false;
                    break;
                }
            } while (!isPrime);

            return current;
        }
    }
}
