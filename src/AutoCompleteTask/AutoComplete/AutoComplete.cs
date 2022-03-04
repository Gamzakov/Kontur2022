using System;
using System.Collections.Generic;

namespace AutoComplete
{

    public struct FullName
    {
        public string Name;
        public string Surname;
        public string Patronymic;
    }

    public class AutoCompleter
    {

        public void AddToSearch(List<FullName> fullNames)
        {
            //Добавьте новый элемент чтобы он начал участвовать в поиске по фио
            throw new NotImplementedException();
        }

        public List<string> Search(string prefix)
        {
            //Реализуйте алгоритм поиска по префиксу фио, как составить фио смотри условия задачи.
            throw new NotImplementedException();
        }
    }
}
