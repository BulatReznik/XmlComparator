using System.Xml.Linq;

namespace XmlComparator
{
    public class Program
    {
        public static void Main(string[] args)
        {

            XmlComparator xmlComparator = new();

            XDocument xmlOriginal = XmlComparator.LoadXmlDocument(@"XML\xmlOriginal1.xml");
            XDocument xmlCopyFalse = XmlComparator.LoadXmlDocument(@"XML\xmlCopyFalse1.xml");

            
            XDocument[] xmlDocumentsOriginal = new XDocument[]
            {
                xmlOriginal,xmlOriginal,xmlOriginal,xmlOriginal
            };

            List<XDocument> xmlDocumentsCopy = new()
            {
                xmlCopyFalse, xmlOriginal,xmlCopyFalse,xmlOriginal
            };

            //xmlComparator.OutputComparison(xmlDocumentsOriginal, xmlDocumentsCopy);
            xmlComparator.OutputComparison(xmlOriginal, xmlCopyFalse);

        }
    }
}
