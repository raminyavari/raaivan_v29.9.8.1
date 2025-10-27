using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.QA;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Documents;

namespace RaaiVan.Modules.Search
{
    public static class SearchUtilities
    {
        private static void update_index(Guid applicationId, List<SearchDoc> docs)
        {
            if (RaaiVanSettings.Solr.Enabled)
                SolrAPI.add(applicationId, docs);
            else LuceneAPI._update_index(applicationId, docs);
        }

        private static void remove_docs(Guid applicationId, List<SearchDoc> docs)
        {
            if (RaaiVanSettings.Solr.Enabled)
                SolrAPI.delete(applicationId, docs);
            else LuceneAPI._remove_docs(applicationId, docs);
        }

        public static void start_update(object rvThread)
        {
            RVJob trd = (RVJob)rvThread;

            if (!trd.TenantID.HasValue || !RaaiVanSettings.IndexUpdate.Index(trd.TenantID.Value)) return;

            if (!trd.TenantID.HasValue) return;

            if (!trd.StartTime.HasValue) trd.StartTime = RaaiVanSettings.IndexUpdate.StartTime(trd.TenantID.Value);
            if (!trd.EndTime.HasValue) trd.EndTime = RaaiVanSettings.IndexUpdate.EndTime(trd.TenantID.Value);

            while (true)
            {
                //sleep thread be madate Interval saniye
                if (!trd.Interval.HasValue) trd.Interval = RaaiVanSettings.IndexUpdate.Interval(trd.TenantID.Value);
                else Thread.Sleep(trd.Interval.Value);

                //agar dar saati hastim ke bayad update shavad edame midahim
                if (!trd.check_time()) continue;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();

                try
                {
                    //Aya Index ha dar RAM ham zakhire shavand
                    bool inRam = RaaiVanSettings.IndexUpdate.Ram(trd.TenantID.Value);

                    //tartibe index kardane Search Doc ha cheghoone bashad
                    for (int i = 0; i < RaaiVanSettings.IndexUpdate.Priorities(trd.TenantID.Value).Length; i++)
                    {
                        SearchDocType type = SearchDocType.All;
                        if (!Enum.TryParse(RaaiVanSettings.IndexUpdate.Priorities(trd.TenantID.Value)[i], out type))
                            type = SearchDocType.All;

                        update_index(trd.TenantID.Value, type, RaaiVanSettings.IndexUpdate.BatchSize(trd.TenantID.Value), 20000);

                        Thread.Sleep(20000);
                    }
                }
                catch (Exception ex)
                {
                    LogController.save_error_log(trd.TenantID.Value, null, "IndexUpdateJob", ex, ModuleIdentifier.SRCH, LogLevel.Fatal);
                }

                trd.LastActivityDate = DateTime.Now;

                sw.Stop();
                trd.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }

        public static void update_index(Guid applicationId, SearchDocType type, int batchSize, int sleepInterval = 20000)
        {
            //Update Tags Before Index Update: Because tagextraction uses IndexLastUpdateDate field
            if (type == SearchDocType.Node) CNController.update_form_and_wiki_tags(applicationId, batchSize);

            List<SearchDoc> updateSdList = SearchController.get_index_queue_items(applicationId, batchSize, type);

            List<SearchDoc> deletedSdList = updateSdList.Where(u => u.Deleted == true).ToList();
            List<Guid> IDs = updateSdList.Select(u => u.ID).ToList();

            deletedSdList.ForEach(sd => updateSdList.Remove(sd));

            remove_docs(applicationId, deletedSdList);

            if (!RaaiVanSettings.Solr.Enabled) Thread.Sleep(sleepInterval);

            update_index(applicationId, updateSdList);

            SearchController.set_index_last_update_date(applicationId, type, IDs);
        }

        private static Dictionary<SearchDocType, List<Guid>> get_existing_ids(Guid applicationId,
            List<SearchDoc> docs, ref List<DocFileInfo> files)
        {
            Dictionary<SearchDocType, List<Guid>> ids = new Dictionary<SearchDocType, List<Guid>>();

            ids[SearchDocType.Node] = CNController.get_existing_node_ids(applicationId,
                docs.Where(u => u.SearchDocType == SearchDocType.Node).Select(v => v.ID).ToList(), true, false);

            ids[SearchDocType.NodeType] = CNController.get_existing_node_type_ids(applicationId,
                docs.Where(u => u.SearchDocType == SearchDocType.NodeType).Select(v => v.ID).ToList(), false);

            ids[SearchDocType.Question] = QAController.get_existing_question_ids(applicationId,
                docs.Where(u => u.SearchDocType == SearchDocType.Question).Select(v => v.ID).ToList());

            ids[SearchDocType.User] = UsersController.get_approved_user_ids(applicationId,
                docs.Where(u => u.SearchDocType == SearchDocType.User).Select(v => v.ID).ToList());

            //Files
            List<DocFileInfo> newFiles = DocumentsController.get_file_owner_nodes(applicationId,
                docs.Where(u => u.SearchDocType == SearchDocType.File).Select(v => v.ID).ToList())
                .Where(u => u.FileID.HasValue).ToList();
            ids[SearchDocType.File] = newFiles.Select(v => v.FileID.Value).ToList();

            foreach (DocFileInfo f in newFiles)
                if (!files.Any(u => u.FileID == f.FileID)) files.Add(f);
            //end of Files

            return ids;
        }

        private static List<SearchDoc> process_search_results(Guid applicationId,
            List<SearchDoc> listDocs, Guid? currentUserId, ref List<SearchDoc> toBeRemoved, int count)
        {
            List<DocFileInfo> files = new List<DocFileInfo>();

            Dictionary<SearchDocType, List<Guid>> existingObjs = get_existing_ids(applicationId, listDocs, ref files);

            listDocs.Where(doc => files.Any(u => u.FileID == doc.ID)).ToList()
                .ForEach(doc => doc.FileInfo = files.Where(u => u.FileID == doc.ID).FirstOrDefault());

            List<Guid> existingIds = new List<Guid>();

            //Remove not existing docs
            foreach (SearchDoc sd in listDocs)
            {
                if (!existingObjs.Any(x => x.Value.Any(z => z == sd.ID)) && !toBeRemoved.Any(x => x.ID == sd.ID))
                    toBeRemoved.Add(sd);
            }
            //end of Remove not existing docs

            List<Guid> granted = new List<Guid>();

            //Check access to nodes
            List<Guid> nodeIdsToCheckAccess = new List<Guid>();
            List<Guid> idsToCheckAccess = new List<Guid>();

            if (existingObjs.ContainsKey(SearchDocType.Node)) nodeIdsToCheckAccess.AddRange(existingObjs[SearchDocType.Node]);
            if (existingObjs.ContainsKey(SearchDocType.File))
            {
                existingObjs[SearchDocType.File].ForEach(f =>
                {
                    SearchDoc fl = listDocs.Where(x => x.ID == f && x.FileInfo != null &&
                        x.FileInfo.OwnerNodeID.HasValue).FirstOrDefault();
                    if (fl == null) return;

                    if (!nodeIdsToCheckAccess.Any(a => a == fl.FileInfo.OwnerNodeID))
                        nodeIdsToCheckAccess.Add(fl.FileInfo.OwnerNodeID.Value);

                    if (fl.FileInfo.OwnerID.HasValue && fl.FileInfo.OwnerID != fl.FileInfo.OwnerNodeID)
                    {
                        if (!idsToCheckAccess.Any(a => a == fl.FileInfo.OwnerID))
                            idsToCheckAccess.Add(fl.FileInfo.OwnerID.Value);
                    }

                    if (fl.FileInfo.OwnerID.HasValue && fl.FileInfo.OwnerID != fl.FileInfo.OwnerNodeID &&
                        !idsToCheckAccess.Any(a => a == fl.FileInfo.OwnerID))
                        idsToCheckAccess.Add(fl.FileInfo.OwnerID.Value);
                });
            }

            List<PermissionType> pts = new List<PermissionType>();
            pts.Add(PermissionType.View);
            pts.Add(PermissionType.ViewAbstract);
            pts.Add(PermissionType.Download);

            Dictionary<Guid, List<PermissionType>> ps = PrivacyController.check_access(applicationId,
                currentUserId, nodeIdsToCheckAccess, PrivacyObjectType.Node, pts);

            granted.AddRange(ps.Keys.Where(
                k => ps[k].Any(p => p == PermissionType.ViewAbstract || p == PermissionType.View)));

            List<Guid> grantedFileOwners = PrivacyController.check_access(applicationId,
                currentUserId, idsToCheckAccess, PrivacyObjectType.None, PermissionType.View);

            listDocs.Where(d => d.SearchDocType == SearchDocType.File && d.FileInfo != null &&
                d.FileInfo.OwnerNodeID.HasValue).ToList().ForEach(doc =>
                {
                    Guid ndId = doc.FileInfo.OwnerNodeID.Value;

                    bool isGranted = ps.ContainsKey(ndId) && ps[ndId].Any(u => u == PermissionType.View) &&
                        ps[ndId].Any(u => u == PermissionType.Download);

                    if (isGranted && doc.FileInfo.OwnerID.HasValue && doc.FileInfo.OwnerID != ndId &&
                        !grantedFileOwners.Any(o => o == doc.FileInfo.OwnerID))
                        isGranted = false;

                    doc.AccessIsDenied = !isGranted;
                });
            //end of Check access to nodes

            //Check access to other objects
            List<Guid> ids = new List<Guid>();

            existingObjs.Keys.Where(x => x != SearchDocType.Node).ToList()
                .ForEach(u => ids.AddRange(existingObjs[u]));

            granted.AddRange(PrivacyController.check_access(applicationId,
                currentUserId, ids, PrivacyObjectType.None, PermissionType.View));
            //end of Check access to other objects

            //Check permissions
            bool forceCheckPermission = RaaiVanSettings.IndexUpdate.CheckPermissions(applicationId);

            existingObjs.Keys.ToList().ForEach(k =>
            {
                existingObjs[k].ForEach(id =>
                {
                    SearchDoc doc = listDocs.Where(d => d.ID == id).FirstOrDefault();
                    if (doc == null) return;

                    bool isGranted = doc.AccessIsDenied.HasValue && doc.AccessIsDenied.Value ?
                        false : granted.Any(x => x == id);

                    if (!isGranted) doc.AccessIsDenied = true;

                    if (isGranted || !forceCheckPermission) existingIds.Add(id);
                });
            });
            //end of Check permissions

            return listDocs.Where(doc => existingIds.Any(x => x == doc.ID))
                .Take(Math.Min(count, listDocs.Count)).ToList();
        }

        private static void search(Guid applicationId, Guid? currentUserId,
            ref List<SearchDoc> retDocs, ref List<SearchDoc> toBeRemoved, SearchOptions options)
        {
            int newBoundary = options.LowerBoundary;

            List<SearchDoc> listDocs = RaaiVanSettings.Solr.Enabled ?
                SolrAPI.search(applicationId, options) : LuceneAPI.search(applicationId, options);

            List<SearchDoc> processedResults = 
                process_search_results(applicationId, listDocs, currentUserId, ref toBeRemoved, options.Count);

            retDocs.AddRange(processedResults);

            int processedCount = processedResults.Count == 0 ? listDocs.Count :
                listDocs.FindIndex(d => d.ID == processedResults.Last().ID) + 1;

            newBoundary += processedCount;

            //for consistency we are not allowed to remove items after the last valid search result
            List<SearchDoc> invalidRemoveRange = processedCount >= listDocs.Count ? 
                new List<SearchDoc>() : listDocs.GetRange(processedCount, listDocs.Count - processedCount);

            toBeRemoved = toBeRemoved.Where(r => !invalidRemoveRange.Any(i => i.ID == r.ID)).ToList();
            //end of for consistency ...

            if (options.LowerBoundary != newBoundary)
            {
                options.LowerBoundary = newBoundary;
                options.Count -= retDocs.Count;

                if (retDocs.Count < options.Count) search(applicationId, currentUserId, ref retDocs, ref toBeRemoved, options);
            }
        }

        public static List<SearchDoc> search(Guid applicationId, Guid? currentUserId, SearchOptions options)
        {
            if (options == null) return new List<SearchDoc>();

            options.AdditionalID = options.AdditionalID &&
                (new[] { SearchDocType.Node, SearchDocType.User, SearchDocType.NodeType })
                .Any(t => options.DocTypes.Any(d => d == t));

            options.Description = options.Description && options.DocTypes.Any(u => u != SearchDocType.File);

            options.Content = options.Content && (new[] { SearchDocType.Node, SearchDocType.Question })
                .Any(t => options.DocTypes.Any(d => d == t));

            options.Tags = options.Tags && options.DocTypes.Any(u => u == SearchDocType.Node);

            options.FileContent = options.FileContent && (new[] { SearchDocType.Node, SearchDocType.File })
                .Any(t => options.DocTypes.Any(d => d == t));

            try
            {
                List<SearchDoc> retDocs = new List<SearchDoc>();
                List<SearchDoc> toBeRemoved = new List<SearchDoc>();

                search(applicationId, currentUserId, ref retDocs, ref toBeRemoved, options);

                //in case of fetching more items, removed items do not exists in the index. so, we have to modify the lower boundary
                options.LowerBoundary -= toBeRemoved.Count;

                if (toBeRemoved.Count > 0) remove_docs(applicationId, toBeRemoved);

                return retDocs;
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "InitSearchIndexDocuments", ex, ModuleIdentifier.SRCH, LogLevel.Fatal);
                return new List<SearchDoc>();
            }
        }
    }
}