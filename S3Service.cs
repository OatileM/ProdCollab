using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProdCollab
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            string awsProfile = configuration["AWS:Profile"];
            string awsRegion = configuration["AWS:Region"];

            var credentials = new CredentialProfileStoreChain().TryGetAWSCredentials(awsProfile, out AWSCredentials awsCredentials)
                ? awsCredentials
                : throw new InvalidOperationException($"AWS profile '{awsProfile}' not found.");

            _s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(awsRegion));
        }

        public async Task UploadFileAsync(string bucketName, string key, Stream fileStream)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(fileStream, bucketName, key);
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string key)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            var response = await fileTransferUtility.OpenStreamAsync(bucketName, key);
            return response;
        }
    }
}
