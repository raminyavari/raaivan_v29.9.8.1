using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Documents
{
    public class ExtractFileContents
    {
        /// <summary>
        /// 2. Shoro e estekhraje matn e te'dadi file dar sa'at haye moshakhasi az rouz.
        ///    va anjame ravande estekhraj ta payan e zaman e moshakhas shode.
        /// </summary>
        public static void start_extract(object rvThread)
        {
            RVJob trd = (RVJob)rvThread;

            if (!trd.TenantID.HasValue || !RaaiVanSettings.FileContentExtraction.FileContents(trd.TenantID.Value)) return;
            
            if (!trd.TenantID.HasValue) return;
            
            if (!trd.StartTime.HasValue) trd.StartTime = RaaiVanSettings.FileContentExtraction.StartTime(trd.TenantID.Value);
            if (!trd.EndTime.HasValue) trd.EndTime = RaaiVanSettings.FileContentExtraction.EndTime(trd.TenantID.Value);

            // Estekhraje matn e te'dadi file dar sa'at haye moshakhasi az rouz.
            // va anjame ravande estekhraj ta payan e zaman e moshakhas shode
            // dar sourate vojoode file
            while (true)
            {
                if (!trd.Interval.HasValue) trd.Interval = RaaiVanSettings.FileContentExtraction.ExtractionInterval(trd.TenantID.Value);
                else Thread.Sleep(trd.Interval.Value);

                if (!trd.check_time()) continue;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();

                bool notExist = false;

                // taiin tedade file ha baraye estekhraj, dar har doure zamani
                for (int i = 0; i < RaaiVanSettings.FileContentExtraction.BatchSize(trd.TenantID.Value); i++)
                {
                    ExtractOneDocument(trd.TenantID.Value, ref notExist);
                    if (notExist) break;
                }

                trd.LastActivityDate = DateTime.Now;

                sw.Stop();
                trd.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }

        /// 3. Taiin e yek file az DB ke mantnash estekhraj nashode ast.
        /// 4. Estekhraj e matn e file.
        /// 5. Zakhire ye matn e estekhraj shode dar DB (movaffagh ya Na movaffagh)
        public static void ExtractOneDocument(Guid applicationId, ref bool notExist)
        {
            //Taiin e yek file az DB ke mantnash estekhraj nashode ast.
            DocFileInfo file = DocumentsController.get_not_extracted_files(applicationId,
                "pdf,doc,docx,xlsx,ppt,pptx,txt,xml,htm,html", ',', 1).FirstOrDefault();

            if (file == null || !file.FileID.HasValue)
            {
                notExist = true;
                return;
            }
            
            // vojoode file dar folder ha
            file.refresh_folder_name();

            bool find = file.exists();

            //estekhraje mant e file ba farakhani method e "ExtractFileContent" 
            string content = string.Empty;
            bool NotExtractable = false;
            double duration = 0;

            string errorText = string.Empty;

            if (find)
            {
                DateTime dtBegin = DateTime.Now;
                content = FileContentExtractor.ExtractFileContent(file, ref errorText);
                duration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds;
            }

            if (!find) find = false;
            else if (string.IsNullOrEmpty(content)) NotExtractable = true;

            // Zakhire ye matn e estekhraj shode dar DB dar halate movaffagh boodan.
            DocumentsController.save_file_content(applicationId,
                file.FileID.Value, content, NotExtractable, !find, duration, errorText);
        }
    }
}
