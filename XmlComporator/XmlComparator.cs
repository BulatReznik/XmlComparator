using System.Text;
using System.Xml.Linq;
using XmlComparator;

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

            if (originalXml != null)
            {
                result.OriginalCount++;
            }
            if (comparedXml != null)
            {
                result.ComparerCount++;
            }

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
        /// <param name="originalXml">Последовательность оригинальныз Xml-файлов</param>
        /// <param name="comparedXml">Последовательность сравниваемых Xml-файлов</param>
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
                differences.Add($"Невероятный оригинал:\n{originalNode}");
                differences.Add($"Жалкая копия:\n{comparedNode}");
                count++;
            }

            if (originalNode.HasElements && comparedNode.HasElements)
            {
                var originalChildren = originalNode.Elements();
                var comparedChildren = comparedNode.Elements();

                var childPairs = originalChildren.Zip(comparedChildren,
                    (originalChild, comparedChild) => new { Original = originalChild, Compared = comparedChild });

                foreach (var pair in childPairs)
                {
                    CountDifferentNodesAndFindDifferences(pair.Original, pair.Compared, ref count, differences);
                }
            }
        }
    }

}
