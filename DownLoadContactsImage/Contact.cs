using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Drawing;

namespace DownLoadContactsImage
{
    public class Contact
    {
        CRMHelper objCRMHelper;
        IOrganizationService iOrgService;
        EntityCollection binaryContactImageresult;
        // //Retrieve and download the binary images
        public void DownloadContactImages()
        {
            objCRMHelper = new CRMHelper();

            iOrgService = objCRMHelper.setSrvice();
            string binaryImageQuery = String.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                  <entity name='contact'>
                                                    <attribute name='contactid' />
                                                        <attribute name='lastname' />
                                                            <attribute name='entityimage' />
                                                            <attribute name='entityimage_url' />
                                                    <filter type='and'>
                                                      <condition attribute='entityimage' operator='not-null' />
                                                    </filter>
                                                  </entity>
                                                </fetch>", "contact");
            binaryContactImageresult = iOrgService.RetrieveMultiple(new FetchExpression(binaryImageQuery));
            Console.WriteLine("Records retrieved and image files saved to: {0}", Directory.GetCurrentDirectory());

            foreach (Entity record in binaryContactImageresult.Entities)
            {
                String recordName = record["lastname"] as String;
                String downloadedFileName = String.Format("Downloaded_{0}", recordName);


                if (record.Contains("entityimage") && record["entityimage"] != null)
                {
                    String path = @"D:\ContactImages\" + recordName + ".jpg";
                    byte[] imageBytes = record["entityimage"] as byte[];
                    ImageConverter ic = new ImageConverter();
                    Image img = ic.ConvertFrom(imageBytes) as Image;

                    var fs = new BinaryWriter(new FileStream(downloadedFileName, FileMode.Append, FileAccess.Write));
                    img.Save(path);
                    fs.Write("written Successfully");
                    fs.Close();
                }
            }
            Console.WriteLine("Records retrieved and image files saved to: ");
        }
    }
}
