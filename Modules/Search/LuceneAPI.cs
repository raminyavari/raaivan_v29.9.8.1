using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search.Vectorhighlight;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;

namespace RaaiVan.Modules.Search
{
    public class RVLuceneDocument : SearchDoc
    {
        public static float BoostMax = (float)22;
        public static float BoostMin = (float)0.01;
        public static float BoostThreshold = (float)8;
        public static float BoostStep = (float)2;

        public static float BoostAdditinalID = (float)BoostMax;
        public static float BoostTitle = (float)BoostMax;
        public static float BoostTags = (float)(BoostTitle - (5 * BoostStep));
        public static float BoostDescription = (float)(BoostTags - BoostStep);
        public static float BoostContent = (float)(BoostDescription - BoostStep);
        public static float BoostFileContent = (float)(BoostContent - BoostStep);
        public static float BoostDocType = (float)BoostMin;
        public static float BoostTypeID = (float)BoostMin;
        public static float BoostType = (float)BoostMin;
        public static float BoostNoContent = (float)BoostMax;

        public RVLuceneDocument(Guid? applicationId) : base(applicationId) { }

        public Document toDocument()
        {
            try
            {
                Type = PublicMethods.verify_string(Type);
                AdditionalID = PublicMethods.verify_string(AdditionalID);
                Title = PublicMethods.verify_string(Title);
                Description = PublicMethods.verify_string(Description);
                Tags = PublicMethods.verify_string(Tags);
                Content = PublicMethods.verify_string(Content);
                FileContent = PublicMethods.verify_string(FileContent);

                Type = PublicMethods.verify_string(Type);
                Field f;
                Lucene.Net.Documents.Document postDocument = new Lucene.Net.Documents.Document();

                f = new Field("ID", ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
                postDocument.Add(f);

                f = new Field("TypeID", TypeID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
                f.Boost = BoostTypeID;
                postDocument.Add(f);

                if (!string.IsNullOrEmpty(Type))
                {
                    f = new Field("Type", Type, Field.Store.YES,
                       Field.Index.NOT_ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostType;
                    postDocument.Add(f);
                }

                if (!string.IsNullOrEmpty(AdditionalID))
                {
                    f = new Field("AdditionalID", AdditionalID,
                         Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostAdditinalID;
                    postDocument.Add(f);
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    f = new Field("Title", Title,
                       Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostTitle;
                    postDocument.Add(f);
                }

                if (!string.IsNullOrEmpty(Description))
                {
                    f = new Field("Description", Description,
                       Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostDescription;
                    postDocument.Add(f);
                }

                if (!string.IsNullOrEmpty(Tags))
                {
                    f = new Field("Tags", Tags.ToString(),
                       Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostTags;
                    postDocument.Add(f);
                }

                f = new Field("Deleted", Deleted.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
                postDocument.Add(f);

                if (!string.IsNullOrEmpty(Content))
                {
                    f = new Field("Content", Content.ToString(),
                       Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostContent;
                    postDocument.Add(f);
                }

                if (!string.IsNullOrEmpty(FileContent))
                {
                    f = new Field("FileContent", FileContent.ToString(),
                       Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                    f.Boost = BoostFileContent;
                    postDocument.Add(f);
                }

                f = new Field("NoContent", NoContent.ToString().ToLower(),
                    Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                f.Boost = BoostNoContent;
                postDocument.Add(f);

                f = new Field("SearchDocType", SearchDocType.ToString(), Field.Store.YES, Field.Index.ANALYZED,
                    Field.TermVector.WITH_POSITIONS_OFFSETS);
                f.Boost = BoostDocType;
                postDocument.Add(f);

                return postDocument;
            }
            catch (Exception ex)
            {
                LogController.save_error_log(this.ApplicationID, null, "ConvertToLuceneDocument", ex, ModuleIdentifier.SRCH);
                return null;
            }
        }

        public static SearchDoc toSearchDoc(Guid? applicationId, Document doc)
        {
            SearchDoc retSD = new SearchDoc(applicationId);

            switch (doc.GetField("SearchDocType").StringValue)
            {
                case "Node":
                    if (doc.GetField("ID") != null) retSD.ID = Guid.Parse(doc.GetField("ID").StringValue);
                    if (doc.GetField("Deleted") != null) retSD.Deleted = Convert.ToBoolean(doc.GetField("Deleted").StringValue);
                    if (doc.GetField("TypeID") != null) retSD.TypeID = Guid.Parse(doc.GetField("TypeID").StringValue);
                    if (doc.GetField("Type") != null) retSD.Type = doc.GetField("Type").StringValue;
                    if (doc.GetField("AdditionalID") != null) retSD.AdditionalID = doc.GetField("AdditionalID").StringValue;
                    if (doc.GetField("Title") != null) retSD.Title = doc.GetField("Title").StringValue;
                    if (doc.GetField("Description") != null) retSD.Description = doc.GetField("Description").StringValue;
                    if (doc.GetField("Tags") != null) retSD.Tags = doc.GetField("Tags").StringValue;
                    if (doc.GetField("Content") != null) retSD.Content = doc.GetField("Content").StringValue;
                    if (doc.GetField("FileContent") != null) retSD.FileContent = doc.GetField("FileContent").StringValue;
                    retSD.SearchDocType = SearchDocType.Node;
                    break;
                case "NodeType":
                    if (doc.GetField("ID") != null) retSD.ID = Guid.Parse(doc.GetField("ID").StringValue);
                    if (doc.GetField("Deleted") != null) retSD.Deleted = Convert.ToBoolean(doc.GetField("Deleted").StringValue);
                    if (doc.GetField("Title") != null) retSD.Title = doc.GetField("Title").StringValue;
                    if (doc.GetField("Description") != null) retSD.Description = doc.GetField("Description").StringValue;
                    retSD.SearchDocType = SearchDocType.NodeType;
                    break;
                case "Question":
                    if (doc.GetField("ID") != null) retSD.ID = Guid.Parse(doc.GetField("ID").StringValue);
                    if (doc.GetField("Deleted") != null) retSD.Deleted = Convert.ToBoolean(doc.GetField("Deleted").StringValue);
                    if (doc.GetField("Title") != null) retSD.Title = doc.GetField("Title").StringValue;
                    if (doc.GetField("Description") != null) retSD.Description = doc.GetField("Description").StringValue;
                    if (doc.GetField("Content") != null) retSD.Content = doc.GetField("Content").StringValue;
                    retSD.SearchDocType = SearchDocType.Question;
                    break;
                case "File":
                    if (doc.GetField("ID") != null) retSD.ID = Guid.Parse(doc.GetField("ID").StringValue);
                    if (doc.GetField("Type") != null) retSD.Type = doc.GetField("Type").StringValue;
                    if (doc.GetField("Title") != null) retSD.Title = doc.GetField("Title").StringValue;
                    if (doc.GetField("FileContent") != null) retSD.FileContent = doc.GetField("FileContent").StringValue;
                    retSD.SearchDocType = SearchDocType.File;
                    break;
                case "User":
                    if (doc.GetField("ID") != null) retSD.ID = Guid.Parse(doc.GetField("ID").StringValue);
                    if (doc.GetField("Deleted") != null) retSD.Deleted = Convert.ToBoolean(doc.GetField("Deleted").StringValue);
                    if (doc.GetField("AdditionalID") != null) retSD.AdditionalID = doc.GetField("AdditionalID").StringValue;
                    if (doc.GetField("Title") != null) retSD.Title = doc.GetField("Title").StringValue;
                    retSD.SearchDocType = SearchDocType.User;
                    break;
            }

            return retSD;
        }

        public static RVLuceneDocument fromSearchDoc(SearchDoc doc)
        {
            return new RVLuceneDocument(doc.ApplicationID)
            {
                ID = doc.ID,
                TypeID = doc.TypeID,
                Type = doc.Type,
                AdditionalID = doc.AdditionalID,
                Title = doc.Title,
                Description = doc.Description,
                Tags = doc.Tags,
                Deleted = doc.Deleted,
                Content = doc.Content,
                FileContent = doc.FileContent,
                AccessIsDenied = doc.AccessIsDenied,
                SearchDocType = doc.SearchDocType,
                FileInfo = doc.FileInfo
            };
        }
    }

    public static class LuceneAPI
    {
        private enum FieldName
        {
            ID,
            TypeID,
            Type,
            AdditionalID,
            Title,
            Description,
            Tags,
            Deleted,
            Content,
            FileContent,
            SearchDocType,
            NoContent
        };

        private static HashSet<string> _StopWords;
        private static HashSet<string> StopWords
        {
            get
            {
                if (_StopWords != null && _StopWords.Count > 0) return _StopWords;

                _StopWords = new HashSet<string>();

                var stopWords = new[]
                {
                    "به","با","از","تا","و","است","هست","هستم","هستیم","هستید","هستند","نیست","نیستم","نیستیم","نیستند","اما","یا",
                    "این","آن","اینجا","آنجا","بود","باد","برای","که","دارم","داری","دارد","داریم","دارید","دارند","چند","را","ها",
                    "های","می","هم","در","باشم","باشی","باشد","باشیم","باشید","باشند","اگر","مگر","بجز","جز","الا","اینکه","چرا","کی",
                    "چه","چطور","چی","چیست","آیا","چنین","اینچنین","نخست","اول","آخر","انتها","صد","هزار","میلیون","ملیون","میلیارد",
                    "ملیارد","یکهزار","تریلیون","تریلیارد","میان","بین","زیر","بیش","روی","ضمن","همانا","ای","بعد","پس","قبل","پیش",
                    "هیچ","همه","واما","شد","شده","شدم","شدی","شدیم","شدند","یک","یکی","نبود","میکند","میکنم","میکنیم","میکنید",
                    "میکنند","میکنی","طور","اینطور","آنطور","هر","حال","مثل","خواهم","خواهی","خواهد","خواهیم","خواهید","خواهند",
                    "داشته","داشت","داشتی","داشتم","داشتیم","داشتید","داشتند","آنکه","مورد","کنید","کنم","کنی","کنند","کنیم",
                    "نکنم","نکنی","نکند","نکنیم","نکنید","نکنند","نکن","بگو","نگو","مگو","بنابراین","بدین","من","تو","او","ما",
                    "شما","ایشان","ی","ـ","هایی","خیلی","بسیار","1","بر","l","شود","کرد","کرده","نیز","خود","شوند","اند","داد","دهد",
                    "گشت","ز","گفت","آمد","اندر","چون","بد","چو","همی","پر","سوی","دو","گر","بی","گرد","زین","کس","زان","جای","آید"
                };

                foreach (var item in stopWords) _StopWords.Add(PublicMethods.verify_string(item));

                return _StopWords;
            }
        }

        private static StandardAnalyzer _STDAnalyzer;
        private static StandardAnalyzer STDAnalyzer
        {
            get
            {
                if (_STDAnalyzer == null) _STDAnalyzer = new StandardAnalyzer(LuceneVersion, StopWords);
                return _STDAnalyzer;
            }
        }

        private static Lucene.Net.Util.Version LuceneVersion
        {
            get { return Lucene.Net.Util.Version.LUCENE_30; }
        }

        private static SortedList<Guid, RAMDirectory> _RamDirs;
        private static RAMDirectory RamDir(Guid applicationId)
        {
            if (_RamDirs == null) _RamDirs = new SortedList<Guid, RAMDirectory>();

            if (!_RamDirs.ContainsKey(applicationId))
            {
                string path = DocFileInfo.index_folder_address(applicationId);
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                FSDirectory dir = FSDirectory.Open(new DirectoryInfo(path));
                _RamDirs.Add(applicationId, new RAMDirectory(dir));
                dir.Dispose();
            }

            return _RamDirs[applicationId];
        }

        private static SortedList<Guid, FSDirectory> _HardDirs;
        private static FSDirectory HardDir(Guid applicationId)
        {
            if (_HardDirs == null) _HardDirs = new SortedList<Guid, FSDirectory>();

            if (!_HardDirs.ContainsKey(applicationId))
            {
                string path = DocFileInfo.index_folder_address(applicationId);
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                FSDirectory dir = FSDirectory.Open(new DirectoryInfo(path));
                _HardDirs.Add(applicationId, dir);
            }

            return _HardDirs[applicationId];
        }

        private static IndexWriter _create_writer(Guid applicationId, bool ram)
        {
            try
            {
                return new IndexWriter(ram ? (Lucene.Net.Store.Directory)RamDir(applicationId) : HardDir(applicationId),
                    STDAnalyzer, create: false, mfl: IndexWriter.MaxFieldLength.UNLIMITED);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "CreateIndexWriter", ex, ModuleIdentifier.SRCH);

                return new IndexWriter(ram ? (Lucene.Net.Store.Directory)RamDir(applicationId) : HardDir(applicationId),
                        STDAnalyzer, create: true, mfl: IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        private static void _close_writer(Guid applicationId, IndexWriter writer)
        {
            try
            {
                writer.Optimize();
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "CloseIndexWriter", ex, ModuleIdentifier.SRCH);
            }
        }

        private static void _add_docs(Guid applicationId, List<SearchDoc> docs, IndexWriter writer)
        {
            try
            {
                docs.Select(d => RVLuceneDocument.fromSearchDoc(d))
                    .Where(d => d != null && d.Type != SearchDocType.All.ToString())
                    .Select(d => d.toDocument())
                    .Where(d => d != null)
                    .ToList().ForEach(d => writer.AddDocument(d));
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "AddIndexDocuments", ex, ModuleIdentifier.SRCH);
            }
        }

        private static void _remove_docs(Guid applicationId, List<SearchDoc> docs, IndexWriter writer)
        {
            try
            {
                writer.DeleteDocuments(docs.Where(d => d != null).Select(d => new Term("ID", d.ID.ToString())).ToArray());
                writer.Commit();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "RemoveIndexDocuments", ex, ModuleIdentifier.SRCH);
            }
        }

        public static void _remove_docs(Guid applicationId, List<SearchDoc> docs)
        {
            try
            {
                //Delete from Hard
                IndexWriter writer = _create_writer(applicationId, false);
                writer.DeleteDocuments(docs.Where(d => d != null).Select(d => new Term("ID", d.ID.ToString())).ToArray());
                _close_writer(applicationId, writer);

                //Delete from Ram
                if (RaaiVanSettings.IndexUpdate.Ram(applicationId))
                {
                    writer = _create_writer(applicationId, true);
                    writer.DeleteDocuments(docs.Where(d => d != null).Select(d => new Term("ID", d.ID.ToString())).ToArray());
                    _close_writer(applicationId, writer);
                }
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "RemoveIndexDocuments", ex, ModuleIdentifier.SRCH);
            }
        }

        private static void _remove_all_docs(Guid applicationId)
        {
            try
            {
                IndexWriter writer;
                //Delete from Ram
                if (RaaiVanSettings.IndexUpdate.Ram(applicationId))
                {
                    writer = _create_writer(applicationId, true);
                    writer.DeleteAll();
                    _close_writer(applicationId, writer);
                }

                //Delete from Hard
                writer = _create_writer(applicationId, false);
                writer.DeleteAll();
                _close_writer(applicationId, writer);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "RemoveAllIndexDocuments", ex, ModuleIdentifier.SRCH);
            }
        }

        private static void _create_index(Guid applicationId, List<SearchDoc> docs)
        {
            try
            {
                //Write into Hard
                IndexWriter hardWriter = _create_writer(applicationId, false);
                _add_docs(applicationId, docs, hardWriter);
                _close_writer(applicationId, hardWriter);

                //Write into Ram
                if (RaaiVanSettings.IndexUpdate.Ram(applicationId))
                {
                    IndexWriter ramWriter = _create_writer(applicationId, true);
                    _add_docs(applicationId, docs, ramWriter);
                    _close_writer(applicationId, ramWriter);
                }
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "CreateIndexDocuments", ex, ModuleIdentifier.SRCH);
            }
        }

        private static void create_lucene_searcher(Guid applicationId, SearchOptions options, ref Query query, ref IndexSearcher searcher)
        {
            try
            {
                string phrase = options.Phrase;
                List<SearchDocType> docTypes = options.DocTypes;

                if (string.IsNullOrEmpty(phrase) || phrase.Trim().Length == 0) return;

                phrase = PublicMethods.verify_string(phrase).Replace(":", " ");
                docTypes = docTypes.Distinct().ToList();

                StringBuilder __phrase = new StringBuilder(phrase);

                int curQuot = -1;
                int secondQuot = 0;
                char escapeChar = Convert.ToChar((byte)6);
                while (secondQuot >= 0)
                {
                    curQuot = phrase.IndexOf("\"", secondQuot == 0 ? 0 : secondQuot + 1);
                    secondQuot = curQuot < 0 || phrase.Length == curQuot + 1 ? -1 : phrase.IndexOf("\"", curQuot + 1);

                    if (secondQuot >= 0)
                        for (int i = curQuot; i <= secondQuot; ++i) if (phrase[i] == ' ') __phrase[i] = escapeChar;
                }
                phrase = __phrase.ToString();

                List<string> terms = phrase.Trim().Split(' ').Select(u => u.Replace(escapeChar, ' ').Trim()).ToList();

                phrase = string.Empty;
                float maxBoost = RVLuceneDocument.BoostMax;
                foreach (string str in terms)
                {
                    if (string.IsNullOrEmpty(str) || str == "\"" || str == "\"\"") continue;

                    phrase += (string.IsNullOrEmpty(phrase) ? string.Empty : " ") + str + "^" + maxBoost.ToString();
                    maxBoost -= RVLuceneDocument.BoostStep;
                    if (maxBoost <= RVLuceneDocument.BoostThreshold) maxBoost = RVLuceneDocument.BoostThreshold;
                }

                string strItemTypes = string.Join("^" + RVLuceneDocument.BoostDocType.ToString() + " ", docTypes);
                string strTypeIDs = string.Join("^" + RVLuceneDocument.BoostTypeID.ToString() + " ", options.TypeIDs);
                string strTypes = string.Join("^" + RVLuceneDocument.BoostType.ToString() + " ", options.Types);

                List<Occur> FlagesList = new List<Occur>();
                List<string> queries = new List<string>();
                List<string> fields = new List<string>();

                if (!string.IsNullOrEmpty(strItemTypes))
                {
                    fields.Add(FieldName.SearchDocType.ToString());
                    queries.Add(strItemTypes);
                    FlagesList.Add(Occur.MUST);
                }

                if (options.TypeIDs != null && !string.IsNullOrEmpty(strTypeIDs))
                {
                    fields.Add(FieldName.TypeID.ToString());
                    queries.Add(strTypeIDs.ToString());
                    FlagesList.Add(Occur.MUST);
                }

                if (options.Types != null && !string.IsNullOrEmpty(strTypes))
                {
                    fields.Add(FieldName.Type.ToString());
                    queries.Add(strTypes.ToString());
                    FlagesList.Add(Occur.MUST);
                }

                if (options.AdditionalID)
                {
                    fields.Add(FieldName.AdditionalID.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.Title)
                {
                    fields.Add(FieldName.Title.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.Description)
                {
                    fields.Add(FieldName.Description.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.Content)
                {
                    fields.Add(FieldName.Content.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.Tags)
                {
                    fields.Add(FieldName.Tags.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.FileContent)
                {
                    fields.Add(FieldName.FileContent.ToString());
                    queries.Add(phrase);
                    FlagesList.Add(Occur.SHOULD);
                }

                if (options.ForceHasContent)
                {
                    fields.Add(FieldName.NoContent.ToString());
                    queries.Add(true.ToString().ToLower());
                    FlagesList.Add(Occur.MUST_NOT);
                }

                if (queries.Count == 0 && fields.Count == 0 && FlagesList.Count == 0) return;

                query = MultiFieldQueryParser.Parse(LuceneVersion,
                    queries.ToArray(), fields.ToArray(), FlagesList.ToArray(), STDAnalyzer);

                bool inRam = RaaiVanSettings.IndexUpdate.Ram(applicationId);
                searcher = inRam ? new IndexSearcher(RamDir(applicationId)) : new IndexSearcher(HardDir(applicationId));
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "CreateLuceneSearcher", ex, ModuleIdentifier.SRCH, LogLevel.Fatal);
            }
        }

        private static List<SearchDoc> lucene_search(Guid applicationId, 
            SearchOptions options, ref Query query, ref IndexSearcher searcher)
        {
            try
            {
                List<SearchDoc> listDocs = new List<SearchDoc>();

                TopDocs hits = searcher.Search(query, options.LowerBoundary + options.Count + (options.Count / 2));
                FastVectorHighlighter fvHighlighter = new FastVectorHighlighter(true, true);

                for (int i = options.LowerBoundary, lnt = hits.ScoreDocs.Length; i < lnt; ++i)
                {
                    ScoreDoc sd = hits.ScoreDocs[i];

                    string addIdFr = !options.AdditionalID ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                        searcher.IndexReader, docId: sd.Doc, fieldName: "AdditionalID", fragCharSize: 200);
                    string titleFr = !options.Title ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                        searcher.IndexReader, docId: sd.Doc, fieldName: "Title", fragCharSize: 200);
                    string descFr = !options.Description ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                            searcher.IndexReader, docId: sd.Doc, fieldName: "Description", fragCharSize: 200);
                    string contentFr = !options.Content ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                            searcher.IndexReader, docId: sd.Doc, fieldName: "Content", fragCharSize: 200);
                    string tagsFr = !options.Tags ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                            searcher.IndexReader, docId: sd.Doc, fieldName: "Tags", fragCharSize: 200);
                    string fileFr = !options.FileContent ? string.Empty :
                        fvHighlighter.GetBestFragment(fvHighlighter.GetFieldQuery(query),
                            searcher.IndexReader, docId: sd.Doc, fieldName: "FileContent", fragCharSize: 200);

                    if (!string.IsNullOrEmpty(titleFr)) titleFr = titleFr.Trim();
                    if (!string.IsNullOrEmpty(addIdFr)) addIdFr = addIdFr.Trim();

                    string highlightedText = ((string.IsNullOrEmpty(descFr) ? string.Empty : descFr + " ") +
                        (string.IsNullOrEmpty(contentFr) ? string.Empty : contentFr + " ") +
                        (string.IsNullOrEmpty(tagsFr) ? string.Empty : tagsFr + " ") +
                        (string.IsNullOrEmpty(fileFr) ? string.Empty : fileFr)).Trim();

                    if (string.IsNullOrEmpty(addIdFr) && string.IsNullOrEmpty(titleFr) && string.IsNullOrEmpty(highlightedText)) break;

                    Document doc = searcher.Doc(sd.Doc);
                    SearchDoc item = RVLuceneDocument.toSearchDoc(applicationId, doc);
                    item.Description = highlightedText;
                    listDocs.Add(item);
                }

                return listDocs;
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "SearchIndexDocuments", ex, ModuleIdentifier.SRCH);
                return new List<SearchDoc>();
            }
        }

        public static List<SearchDoc> search(Guid applicationId, SearchOptions options)
        {
            if (options == null) return new List<SearchDoc>();

            if (options.CustomData == null) options.CustomData = new Dictionary<string, object>();

            string queryParam = "query", searcherParam = "searcher";

            Query query = options.CustomData.ContainsKey(queryParam) && 
                options.CustomData[queryParam].GetType() == typeof(Query) ? (Query)options.CustomData[queryParam] : null;

            IndexSearcher searcher = options.CustomData.ContainsKey(searcherParam) && 
                options.CustomData[searcherParam].GetType() == typeof(IndexSearcher) ? 
                (IndexSearcher)options.CustomData[searcherParam] : null;

            if (query == null || searcher == null)
            {
                create_lucene_searcher(applicationId, options, ref query, ref searcher);

                if (query == null || searcher == null) return new List<SearchDoc>();
                else {
                    options.CustomData[queryParam] = query;
                    options.CustomData[searcherParam] = searcher;
                }
            }

            return lucene_search(applicationId, options, ref query, ref searcher);
        }

        public static void _update_index(Guid applicationId, List<SearchDoc> docs)
        {
            _remove_docs(applicationId, docs);
            _create_index(applicationId, docs);
        }
    }
}
