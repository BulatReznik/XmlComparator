using System.Xml.Linq;

namespace XmlComparator
{
    public class Program
    {
        public static void Main(string[] args)
        {

            XmlComparator xmlComparator = new();

            XDocument xmlOriginal = XmlComparator.LoadXmlDocument(@"XML\xmlOriginal.xml");
            XDocument xmlCopyFalse = XmlComparator.LoadXmlDocument(@"XML\xmlCopyFalse.xml"); 
            XDocument xmlCopyTrue = XmlComparator.LoadXmlDocument(@"XML\xmlCopyTrue.xml");

            List<XDocument> xmlDocumentsOriginal = new()
            {
                xmlOriginal, xmlOriginal, xmlOriginal, xmlOriginal
            };

            List<XDocument> xmlDocumentsCopy = new()
            {
                xmlCopyFalse, xmlCopyTrue, xmlCopyTrue
            };

            //Сравнение для последовательности xml файлов
            List<XmlComparisonResult> resultsSequence = xmlComparator.CompareXml(xmlDocumentsOriginal, xmlDocumentsCopy);

            Console.WriteLine("Сравнение для последовательности xml файлов:");
            Console.WriteLine();
            Console.WriteLine("Количество документов в первой последовательности: " + xmlDocumentsOriginal.Count);
            Console.WriteLine("Количество документов во второй последовательности: " + xmlDocumentsCopy.Count);
            Console.WriteLine();

            for (int i = 0; i < resultsSequence.Count; i++)
            {
                XmlComparisonResult result = resultsSequence[i];

                Console.WriteLine("Сравнение документов: #" + (i + 1)); 
                if (result.Differences.Count > 0)
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
                    Console.WriteLine("Отличий нет");
                }

                Console.WriteLine();
            }
            
            //Сравнение для двух xml файлов
            XmlComparisonResult resultTakeOne = xmlComparator.CompareXml(xmlOriginal, xmlCopyFalse);

            Console.WriteLine();
            Console.WriteLine("Сравнение для двух xml файлов:");
            Console.WriteLine();
            
            if (resultTakeOne.Differences.Count > 0)
            {
                Console.WriteLine("Коэффициент разности xml: " + resultTakeOne.DifferenceCoefficient);
                Console.WriteLine("Количество различающихся узлов: " + resultTakeOne.DifferentNodesCount);
                Console.WriteLine("Отличия:");
                foreach (string difference in resultTakeOne.Differences)
                {
                    Console.WriteLine(difference);
                }
            }
            else
            {
                Console.WriteLine("Отличий нет");
            }
        }
    }
}
