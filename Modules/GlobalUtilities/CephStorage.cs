using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class CephStorage
    {
        private static AmazonS3Client get_client()
        {
            try
            {
                return new AmazonS3Client(
                    RaaiVanSettings.CephStorage.AccessKey,
                    RaaiVanSettings.CephStorage.SecretKey,
                    new AmazonS3Config()
                    {
                        ServiceURL = RaaiVanSettings.CephStorage.URL
                    });
            }
            catch { return null; }
        }

        public static List<string> get_buckets()
        {
            AmazonS3Client client = get_client();

            return client == null ? new List<string>() :
                client.ListBuckets().Buckets.Select(b => b.BucketName).ToList();
        }

        public static bool add_file(string fileName, byte[] content, bool isPublic)
        {
            try
            {
                AmazonS3Client client = get_client();
                if (client == null) return false;

                using (MemoryStream stream = new MemoryStream(content)) {
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest()
                    {
                        BucketName = RaaiVanSettings.CephStorage.Bucket,
                        Key = fileName,
                        InputStream = stream,
                        StorageClass = S3StorageClass.Standard,
                        PartSize = 8388608 //8MB
                    };

                    if (isPublic) request.CannedACL = S3CannedACL.PublicRead;

                    TransferUtility utility = new TransferUtility(client);

                    utility.Upload(request);
                }
                
                return true;
            }
            catch(Exception ex) { return false; }
        }

        public static List<KeyValuePair<string, DateTime>> files(string folderName, int maxCount = 0)
        {
            try
            {
                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return new List<KeyValuePair<string, DateTime>>();

                    /*
                    if (!string.IsNullOrEmpty(folderName)) folderName = folderName.Replace("/", "\\");

                    S3DirectoryInfo dir = new S3DirectoryInfo(client, RaaiVanSettings.CephStorage.Bucket, folderName);

                    S3FileInfo[] files = dir.GetFiles();

                    return files == null ? new List<KeyValuePair<string, DateTime>>() :
                        files.Take(Math.Min(files.Length, maxCount <= 0 ? 2000 : maxCount))
                        .Select(f => new KeyValuePair<string, DateTime>(f.Name, f.LastWriteTime)).ToList();
                    */
                    
                    ListObjectsV2Response response = client.ListObjectsV2(new ListObjectsV2Request() {
                        BucketName = RaaiVanSettings.CephStorage.Bucket,
                        Prefix = folderName + "/",
                        MaxKeys = maxCount <= 0 ? 2000 : maxCount
                    });

                    return response?.S3Objects == null ? new List<KeyValuePair<string, DateTime>>() :
                        response.S3Objects.Select(o => new KeyValuePair<string, DateTime>(o.Key, o.LastModified)).ToList();
                }
            }
            catch (AmazonS3Exception ex)
            {
                bool notFound = ex.StatusCode == System.Net.HttpStatusCode.NotFound;
                return new List<KeyValuePair<string, DateTime>>();
            }
        }

        public static bool folder_exists(string folderName)
        {
            return files(folderName, maxCount: 1).Count > 0;
        }

        public static bool file_exists(string fileName)
        {
            Stopwatch watch = PublicMethods.is_dev() ? Stopwatch.StartNew() : null;

            try
            {
                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return false;

                    GetObjectMetadataRequest request = new GetObjectMetadataRequest();
                    request.BucketName = RaaiVanSettings.CephStorage.Bucket;
                    request.Key = fileName;

                    GetObjectMetadataResponse response = client.GetObjectMetadata(request);

                    watch?.Stop();
                    if (watch != null) Logger.info(LoggerName.Ceph, new
                    {
                        method = "GetObjectMetaData",
                        file_name = fileName,
                        duration = watch.ElapsedMilliseconds
                    });

                    return true;
                }
            }
            catch (AmazonS3Exception ex)
            {
                watch?.Stop();
                if (watch != null) Logger.info(LoggerName.Ceph, new {
                    method = "GetObjectMetaData",
                    file_name = fileName,
                    duration = watch.ElapsedMilliseconds
                }, ex);

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
                return false;
            }
        }

        public static byte[] get_file(string fileName)
        {
            Stopwatch watch = PublicMethods.is_dev() ? Stopwatch.StartNew() : null;

            try
            {
                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return new byte[0];

                    GetObjectRequest request = new GetObjectRequest();
                    request.BucketName = RaaiVanSettings.CephStorage.Bucket;
                    request.Key = fileName;

                    using (GetObjectResponse response = client.GetObject(request))
                    using (MemoryStream stream = new MemoryStream())
                    {
                        response.ResponseStream.CopyTo(stream);

                        watch?.Stop();
                        if (watch != null) Logger.info(LoggerName.Ceph, new {
                            method = "GetObject",
                            file_name = fileName,
                            size = stream.Length,
                            duration = watch.ElapsedMilliseconds
                        });

                        byte[] ret = stream.ToArray();

                        return ret;
                    }
                }
            }
            catch(Exception ex) {
                watch?.Stop();
                if (watch != null) Logger.info(LoggerName.Ceph, new {
                    method = "GetObject",
                    file_name = fileName,
                    duration = watch.ElapsedMilliseconds
                }, ex);

                return new byte[0];
            }
        }

        public static bool rename_file(string oldFileName, string newFileName, bool isPublic)
        {
            try
            {
                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return false;

                    CopyObjectRequest request = new CopyObjectRequest();
                    request.SourceBucket = request.DestinationBucket = RaaiVanSettings.CephStorage.Bucket;
                    request.SourceKey = oldFileName;
                    request.DestinationKey = newFileName;
                    if (isPublic) request.CannedACL = S3CannedACL.PublicRead;

                    CopyObjectResponse response = client.CopyObject(request);
                    bool result = response.HttpStatusCode == System.Net.HttpStatusCode.OK;

                    if (result) delete_file(oldFileName);

                    return result;
                }
            }
            catch { return false; }
        }

        public static bool delete_file(string fileName)
        {
            try
            {
                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return false;

                    DeleteObjectRequest request = new DeleteObjectRequest();
                    request.BucketName = RaaiVanSettings.CephStorage.Bucket;
                    request.Key = fileName;

                    DeleteObjectResponse response = client.DeleteObject(request);
                    return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch { return false; }
        }

        public static string get_download_url(string fileName, bool isPublic, int expiresInMinutes = 60 * 10)
        {
            Stopwatch watch = PublicMethods.is_dev() ? Stopwatch.StartNew() : null;

            try
            {
                if (string.IsNullOrEmpty(fileName)) return string.Empty;

                if (isPublic) return RaaiVanSettings.CephStorage.URL
                        .Replace("://", "://" + RaaiVanSettings.CephStorage.Bucket + ".") + "/" + fileName;

                using (AmazonS3Client client = get_client())
                {
                    if (client == null) return string.Empty;

                    if (expiresInMinutes <= 0) expiresInMinutes = 60 * 10;

                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                    request.BucketName = RaaiVanSettings.CephStorage.Bucket;
                    request.Key = fileName;
                    request.Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);
                    request.Protocol = RaaiVanSettings.CephStorage.URL.ToLower().StartsWith("https") ? Protocol.HTTPS : Protocol.HTTP;

                    string url = client.GetPreSignedURL(request);

                    watch?.Stop();
                    if (watch != null) Logger.info(LoggerName.Ceph, new {
                        method = "GetPreSignedURL",
                        file_name = fileName,
                        url,
                        duration = watch.ElapsedMilliseconds
                    });

                    return url;
                }
            }
            catch(Exception ex) {
                watch?.Stop();
                if (watch != null) Logger.info(LoggerName.Ceph, new {
                    method = "GetPreSignedURL",
                    file_name = fileName,
                    duration = watch.ElapsedMilliseconds
                }, ex);

                return string.Empty;
            }
        }
    }
}
