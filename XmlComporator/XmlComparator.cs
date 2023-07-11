using System.Text;
using System.Xml.Linq;

namespace XmlComparator
{
    public class XmlComparator
    {
        /// <summary>
        /// Загрузка xml файла с рашифровкой
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>XDocument</returns>
        public static XDocument LoadXmlDocument(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string xmlContent = File.ReadAllText(filePath);
            return XDocument.Parse(xmlContent);
        }

        /// <summary>
        /// Сравнение двух Xml-файлов
        /// </summary>
        /// <param name="originalXml">Оригинальный Xml-файл</param>
        /// <param name="comparedXml">Сравниваемый Xml-файл</param>
        /// <returns>XmlComparisonResult - класс с результатами сравнения</returns>
        public XmlComparisonResult CompareXml(XDocument originalXml, XDocument comparedXml)
        {
            XmlComparisonResult result = new();

            if (originalXml != null && comparedXml != null)
            {
                int differentNodes = 0;
                List<string> differences = new();
                CountDifferentNodesAndFindDifferences(originalXml.Root, comparedXml.Root, ref differentNodes, differences);

                double totalNodes = originalXml.Root.DescendantsAndSelf().Count(); //Подсчет общего количества узлов
                result.DifferenceCoefficient = differentNodes / totalNodes;
                result.DifferentNodesCount = differentNodes;
                result.Differences = differences;
            }
            return result;
        }

        /// <summary>
        /// Сравнение последовательности XML-файлов
        /// </summary>
        /// <param name="originalXmls">Последовательность оригинальныз Xml-файлов</param>
        /// <param name="comparedXmls">Последовательность сравниваемых Xml-файлов</param>
        /// <returns>XmlComparisonResult - класс с результатами сравнения</returns>
        public List<XmlComparisonResult> CompareXml(IEnumerable<XDocument> originalXmls, IEnumerable<XDocument> comparedXmls)
        {
            List<XmlComparisonResult> results = new();

            IEnumerator<XDocument> originalEnumerator = originalXmls.GetEnumerator();
            IEnumerator<XDocument> comparedEnumerator = comparedXmls.GetEnumerator();

            while (originalEnumerator.MoveNext() && comparedEnumerator.MoveNext())
            {
                XDocument originalXml = originalEnumerator.Current;
                XDocument comparedXml = comparedEnumerator.Current;

                XmlComparisonResult result = CompareXml(originalXml, comparedXml);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Подсчет отличающихся узлов и поиск различий в узлах
        /// </summary>
        /// <param name="originalNode">Узел оригинала</param>
        /// <param name="comparedNode">Узел сравниваемого</param>
        /// <param name="count">Количество отличающихся узлов</param>
        /// <param name="differences">Лист со строками, в которых будут отличия</param>
        private void CountDifferentNodesAndFindDifferences(XElement originalNode, XElement comparedNode, ref int count, List<string> differences)
        {
            if (!XNode.DeepEquals(originalNode, comparedNode))
            {
                count++;
                if (originalNode.Value != comparedNode.Value && originalNode.Name == comparedNode.Name)
                {
                    differences.Add($"\nОтличие #{count} Узла {originalNode.Name}:\nРазличаются значения узлов:\nЗначение оригинального узла: {originalNode.Value}\nЗначение сравниваемого узла: {comparedNode.Value}\n");
                }
            }
            if (originalNode.HasElements || comparedNode.HasElements)
            {
                var originalChildren = originalNode.Elements();
                var comparedChildren = comparedNode.Elements();

                var originalEnumerator = originalChildren.GetEnumerator();
                var comparedEnumerator = comparedChildren.GetEnumerator();
                

                while (originalEnumerator.MoveNext())
                {
                    comparedEnumerator.MoveNext();
                    var originalChild = originalEnumerator.Current;
                    var comparedChild = comparedEnumerator.Current;

                    if (originalNode.Elements().Count() > comparedNode.Elements().Count()){

                        while (originalChild.Name != comparedChild.Name)
                        {
                            count++;
                            differences.Add($"\nОтличие #{count}:\nРазличные атрибуты узлов:\nОригинальный узел: {originalChild}\nСравниваемый узел: {comparedChild}\n");
                            differences.Add($"\nАтрибут оригинального узла: {originalChild.Name}\nАтрибут сравниваемого узла: {comparedChild.Name}\n");
                            if (!originalEnumerator.MoveNext())
                            {
                                break;
                            }
                            originalChild = originalEnumerator.Current;
                        }
                    }
                    else if(originalNode.Elements().Count() < comparedNode.Elements().Count()) {
                        
                        while (originalChild.Name != comparedChild.Name)
                        {
                            count++;
                            differences.Add($"\nОтличие #{count}:\nРазличные атрибуты узлов:\nОригинальный узел: {originalChild}\nСравниваемый узел: {comparedChild}\n");
                            differences.Add($"\nАтрибут оригинального узла: {originalChild.Name}\nАтрибут сравниваемого узла: {comparedChild.Name}\n");
                            if (!comparedEnumerator.MoveNext())
                            {
                                break;
                            }
                            comparedChild = comparedEnumerator.Current;
                        }
                    }
                    CountDifferentNodesAndFindDifferences(originalChild, comparedChild, ref count, differences);
                }
            }
        }


        public void OutputComparison(IEnumerable<XDocument> xmlDocumentsOriginal, IEnumerable<XDocument> xmlDocumentsCopy)
        {
            List<XmlComparisonResult> resultsSequence = CompareXml(xmlDocumentsOriginal, xmlDocumentsCopy);

            Console.WriteLine("Сравнение для последовательности xml файлов:");
            Console.WriteLine();
            Console.WriteLine("Количество документов в первой последовательности: " + xmlDocumentsOriginal.Count());
            Console.WriteLine("Количество документов во второй последовательности: " + xmlDocumentsCopy.Count());
            Console.WriteLine();

            for (int i = 0; i < resultsSequence.Count; i++)
            {
                XmlComparisonResult result = resultsSequence[i];

                Console.WriteLine("Сравнение документов: #" + (i + 1));
                if (result.DifferentNodesCount > 0)
                {
                    Console.WriteLine("Коэффициент разности xml: " + result.DifferenceCoefficient);
                    Console.WriteLine("Количество различающихся узлов: " + result.DifferentNodesCount);
                    Console.WriteLine("Отличия:");
                    foreach (string difference in result.Differences)
                    {
                        Console.WriteLine(difference);
                    }
                }
                else
                {
                    Console.WriteLine("Отличий нет\n");
                }
            }
        }
        public void OutputComparison(XDocument xmlOriginal, XDocument xmlCopyFalse)
        {
            XmlComparisonResult resultTakeOne = CompareXml(xmlOriginal, xmlCopyFalse);

            Console.WriteLine("Сравнение для двух xml файлов:");
            Console.WriteLine();

            if (resultTakeOne.DifferenceCoefficient > 0)
            {
                Console.WriteLine("Коэффициент разности xml: " + resultTakeOne.DifferenceCoefficient);
                Console.WriteLine("Количество различающихся узлов: " + resultTakeOne.DifferentNodesCount);
                Console.WriteLine("Отличия:");
                foreach (string difference in resultTakeOne.Differences)
                {
                    Console.WriteLine(difference);
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Отличий нет\n");
            }
        }
    }
}
