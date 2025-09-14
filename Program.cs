using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите текст построчно. Для завершения ввода введите 'END':");

        List<string> inputLines = new List<string>();
        while (true)
        {
            string line = Console.ReadLine();
            if (line == null || line.Trim().ToUpper() == "END")
                break;
            inputLines.Add(line);
        }

        // Собираем все числа с дробной частью
        List<double> numbers = new List<double>();
        var culture = CultureInfo.InvariantCulture; // Используем культуру с точкой как разделителем
        foreach (var line in inputLines)
        {
            foreach (Match match in Regex.Matches(line, @"\d+\.\d+"))
            {
                if (double.TryParse(match.Value, NumberStyles.Float, culture, out double num))
                {
                    numbers.Add(num);
                }
            }
        }

        Console.WriteLine($"Найдено чисел с дробной частью: {numbers.Count}");

        if (numbers.Count < 10)
        {
            Console.WriteLine("Недостаточно чисел для формирования комбинаций из 10 элементов.");
            return;
        }

        // Ваш список шаблонов
        List<double> patterns = new List<double>
        {
            0.12345676543210, //Менять на желаемые шаблоны необходимо непосредственно в этом коде
            0.11111111111111, //
            0.10000000000000
        };

        // Перебираем все комбинации из 10 чисел
        var combinations = GetCombinations(numbers, 10);

        double epsilon = 1e-9; // погрешность для сравнения
        bool foundExactMatch = false;

        double closestDifference = double.MaxValue;
        List<double> closestCombination = null;
        double closestAverage = 0;

        foreach (var combo in combinations)
        {
            double sum = 0;
            foreach (var num in combo)
                sum += num;
            double average = sum / combo.Count;

            bool isExactMatch = false;

            // Проверяем равенство с шаблонами
            foreach (var pattern in patterns)
            {
                if (Math.Abs(average - pattern) < epsilon)
                {
                    Console.WriteLine("Найдена комбинация:");
                    Console.WriteLine(string.Join(", ", combo));
                    Console.WriteLine($"Среднее: {average} равно шаблону {pattern}\n");
                    foundExactMatch = true;
                    isExactMatch = true;
                }
            }

            // Если не нашли точное совпадение, ищем ближайшее
            if (!isExactMatch)
            {
                foreach (var pattern in patterns)
                {
                    double diff = Math.Abs(average - pattern);
                    if (diff < closestDifference)
                    {
                        closestDifference = diff;
                        closestCombination = new List<double>(combo);
                        closestAverage = average;
                    }
                }
            }
        }

        if (!foundExactMatch && closestCombination != null)
        {
            Console.WriteLine("Не найдено точных совпадений. Ближайшая комбинация:");
            Console.WriteLine(string.Join(", ", closestCombination));
            Console.WriteLine($"Среднее: {closestAverage} (ближе всего к шаблонам)");
        }
    }

    // Метод для получения всех комбинаций длины 'length' из списка 'list'
    static IEnumerable<List<T>> GetCombinations<T>(List<T> list, int length)
    {
        if (length == 0)
            yield return new List<T>();
        else
        {
            for (int i = 0; i <= list.Count - length; i++)
            {
                var head = list[i];
                var tailCombinations = GetCombinations(list.GetRange(i + 1, list.Count - i - 1), length - 1);
                foreach (var tail in tailCombinations)
                {
                    var combo = new List<T> { head };
                    combo.AddRange(tail);
                    yield return combo;
                }
            }
        }
    }
}