using FX.Core.OCR.Barcodes.Transformer.Abstract;
using FX.Core.OCR.Barcodes.Transformer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FX.Core.OCR.Barcodes.Transformer.Implementations
{
    public class StandardOutputTransformer : IBarcodeOutputTransformer<OutputStatistics>
    {
        const string REGEX = "(.+)_(\\d+)_(\\d+)";
        static Regex COMPILED_REGEX = new Regex(REGEX, RegexOptions.Compiled);

        public OutputStatistics Transform(TransformationInput input)
        {
            var data = input.Results;
            var output = new OutputStatistics()
            {
                TotalProcessTime = input.Duration,
                ThreadsUsed = input.ThreadsUsed
            };

            foreach (var file in data)
            {
                // information about the file
                var fileInfo = new StandardFileInformation()
                {
                    FileName = file.FileName,
                    Duration = file.ExecuteDuration + file.PreprocessDuration,
                    NumberOfDocuments = 0,
                    NumberOfPages = file.Results.Count,
                    NumberOfPagesDetected = file.Results.Count(x => x.Success),
                    Note = file.Note
                };

                // information about each page
                var tempPages = new List<StandardPageInformation>();
                foreach (var item in file.Results)
                {
                    var namePages = RetrieveNameAndPages(item.Text);
                    var si = new StandardPageInformation()
                    {
                        FileName = item.FileName,
                        DocumentName = namePages.Item1,
                        PageNumber = namePages.Item2,
                        TotalPages = namePages.Item3,
                        BarcodeFormat = item.BarcodeFormat,
                        BarcodeText = item.Text,
                        Success = item.Success,
                        Note = item.Note,
                        Duration = item.Duration,
                        PageInTiff = item.PageInTiff
                    };
                    tempPages.Add(si);
                    output.AllPages.Add(si);
                }

                // added info after knowing pages
                fileInfo.NumberOfDocuments = tempPages.Where(x => !string.IsNullOrWhiteSpace(x.DocumentName)).Select(x => x.DocumentName).Distinct().Count();
                output.Files.Add(fileInfo);
                foreach (var item in tempPages)
                {
                    var document = item.DocumentName;

                    // IGNORES EMPTY DOCUMENTS
                    if (string.IsNullOrWhiteSpace(document))
                        continue;

                    if (!output.Documents.ContainsKey(document))
                    {
                        output.Documents.Add(document, new List<StandardPageInformation>());
                        output.DocumentsMetadata.Add(document);
                    }
                    output.Documents[document].Add(item);
                }
            }

            // sort each document by page number
            foreach (var document in output.Documents)
            {
                document.Value.Sort((x, y) => x.PageNumber - y.PageNumber);
            }

            // insert missing pages
            foreach (var document in output.Documents)
            {
                var nrPages = document.Value.FirstOrDefault()?.TotalPages ?? 0;
                if (nrPages == document.Value.Count)
                    continue;

                if (string.IsNullOrWhiteSpace(document.Key)) // the 'empty' document
                    continue;

                for (var i = 0; i < document.Value.Count; i++)
                {
                    var element = document.Value[i];

                    // previously found
                    var duplicateIndex = document.Value.Take(i).ToList()
                        .FindIndex(x => x.PageNumber == element.PageNumber);
                    if (duplicateIndex >= 0)
                    {
                        output.Duplicates.Add(element);
                        document.Value.RemoveAt(i);
                        i--;
                        continue;
                    }

                    // page number is correct
                    if ((i + 1) == element.PageNumber)
                        continue;

                    // case: page was not identified
                    if (element.PageNumber == 0)
                    {
                        element.PageNumber = i + 1;
                        element.Note = element.Note + " (missing page)";
                        continue;
                    }

                    // special case: need to insert missing page
                    document.Value.Insert(i, new StandardPageInformation()
                    {
                        PageNumber = i + 1,
                        Success = false,
                        Note = "Page missing",
                        DocumentName = document.Key,
                        TotalPages = element.TotalPages,
                        PageInTiff = -1
                    });
                    i--;
                }
            }

            // test at the end if one doc is in multiple files
            //var exists = output.AllPages.Where(x => output.AllPages.Any(y => y.DocumentName != "" && y.FileName != x.FileName && x.DocumentName == y.DocumentName)).ToList();

            return output;
        }

        public static Tuple<string, int, int> RetrieveNameAndPages(string text)
        {
            var regex = COMPILED_REGEX;

            Match match = null;
            if (!string.IsNullOrWhiteSpace(text))
                match = regex.Match(text);

            string docName = ""; int docPage = 0; int docTotalPages = 0;

            if (match != null)
            {
                var groups = match.Groups.OfType<Group>().ToArray();
                if (groups.Length > 1 && !string.IsNullOrWhiteSpace(groups[1].Value))
                {
                    docName = groups[1].Value;
                }
                if (groups.Length > 2 && int.TryParse(groups[2].Value, out docPage)) ;
                if (groups.Length > 3 && int.TryParse(groups[3].Value, out docTotalPages)) ;
            }

            return new Tuple<string, int, int>(docName, docPage, docTotalPages);
        }
    }
}
