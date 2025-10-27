using System;
using System.Collections.Generic;
using System.Linq;
using java.io;
using java.lang;
using javax.xml.transform;
using javax.xml.transform.sax;
using javax.xml.transform.stream;
using org.apache.tika.io;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using Exception = System.Exception;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

namespace RaaiVan.Modules.GlobalUtilities
{
    class TextExtractionResult
    {
        public string Text { get; set; }
        public string ContentType { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Text:\n" + Text + "MetaData:\n");
            foreach (var keypair in Metadata)
                builder.AppendFormat("{0} - {1}\n", keypair.Key, keypair.Value);
            return builder.ToString();
        }
    }

    class TextExtractor
    {
        private StringWriter _outputWriter;
        public TextExtractionResult Extract(DocFileInfo file, ref bool succeed, ref string errorText)
        {
            try
            {
                AutoDetectParser parser = new AutoDetectParser();
                Metadata metadata = new Metadata();
                ParseContext parseContext = new ParseContext();
                Class parserClass = parser.GetType();
                parseContext.set(parserClass, parser);

                byte[] fileContent = file.toByteArray();

                using (InputStream inputStream = TikaInputStream.get(fileContent, metadata))
                {
                    parser.parse(inputStream, getTransformerHandler(), metadata, parseContext);
                    inputStream.close();
                }

                return assembleExtractionResult(_outputWriter.toString(), metadata);
            }
            catch (Exception ex)
            {
                errorText = ex.StackTrace.ToString();
                succeed = false;
                return null;
            }
        }

        private static TextExtractionResult assembleExtractionResult(string text, Metadata metadata)
        {
            Dictionary<string, string> metaDataResult = metadata.names().ToDictionary(name => name,
            name => String.Join(", ", metadata.getValues(name)));
            string contentType = metaDataResult["Content-Type"];
            return new TextExtractionResult
            {
                Text = text,
                ContentType = contentType,
                Metadata = metaDataResult
            };
        }
        
        private TransformerHandler getTransformerHandler()
        {
            SAXTransformerFactory factory = (SAXTransformerFactory)TransformerFactory.newInstance();
            TransformerHandler transformerHandler = factory.newTransformerHandler();

            transformerHandler.getTransformer().setOutputProperty(OutputKeys.METHOD, "text");
            transformerHandler.getTransformer().setOutputProperty(OutputKeys.INDENT, "yes");

            _outputWriter = new StringWriter();
            transformerHandler.setResult(new StreamResult(_outputWriter));
            return transformerHandler;
        }
    }

    public class FileContentExtractor
    {
        public static string ExtractFileContent(DocFileInfo file, ref string errorText)
        {
            bool succeed = true;
            TextExtractor textExtractor = new TextExtractor();
            TextExtractionResult result = textExtractor.Extract(file, ref succeed, ref errorText);

            if (!succeed) return string.Empty;

            if (result.ContentType == "application/pdf")
            {
                string text = result.Text.ToString();
                text = text.Replace(')', ' ');
                text = text.Replace('(', ' ');
                text = text.Replace('.', ' ');
                text = text.Replace(',', ' ');
                text = text.Replace('?', ' ');
                text = text.Replace('\n', ' ');
                text = text.Replace('\r', ' ');
                string[] words = text.ToString().Split(' ');
                text = string.Empty;
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 1)
                    {
                        if ((words[i][0] >= 'A' && words[i][0] <= 'Z') || (words[i][0] >= 'a' && words[i][0] <= 'z') ||
                            (words[i][0] >= '0' && words[i][0] <= '9'))
                        {
                            char[] cArray = words[i].ToCharArray();
                            Array.Reverse(cArray);
                            text += (new string(cArray) + " ");
                        }
                        else
                            text += (words[i] + " ");
                    }
                    else 
                        text += (words[i] + " ");
                }

                return text.Trim();
            }
            else
                return result.Text.Trim();
        }
    }
}
