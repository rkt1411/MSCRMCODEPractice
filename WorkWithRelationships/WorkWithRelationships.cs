using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Metadata;
namespace WorkWithRelationships
{
    public class WorkWithRelationships
    {
        private Guid _oneToManyRelationshipId;
        CRMHelper objCRMHelper;
        IOrganizationService _serviceProxy;
        private string _oneToManyRelationshipName;

        string fileName = "Contact_InterfaceLog_ " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".txt";

        public void Run()
        {
            objCRMHelper = new CRMHelper();
            _serviceProxy = objCRMHelper.setSrvice();

            try
            {
                //<snippetWorkWithRelationships1>
                bool eligibleCreateOneToManyRelationship = EligibleCreateOneToManyRelationship("rah_country", "rah_territory");
                Console.WriteLine("fileName" + fileName);

                if (eligibleCreateOneToManyRelationship)
                {


                    CreateOneToManyRequest createOneToManyRelationshipRequest = new CreateOneToManyRequest
                    {
                        OneToManyRelationship = new OneToManyRelationshipMetadata
                        {
                            ReferencedEntity = "rah_country",
                            ReferencingEntity = "rah_territory",
                            SchemaName = "rah_country_rah_territory",
                            AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                            {
                                // Behavior = AssociatedMenuBehavior.UseLabel,
                                Behavior = AssociatedMenuBehavior.UseLabel ,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label("MASTER Country", 1033),
                                Order = 10000
                            },
                            CascadeConfiguration = new CascadeConfiguration
                            {
                                Assign = CascadeType.NoCascade,
                                Delete = CascadeType.Restrict,
                                Merge = CascadeType.NoCascade,
                                Reparent = CascadeType.NoCascade,
                                Share = CascadeType.NoCascade,
                                Unshare = CascadeType.NoCascade
                            }
                        },
                        Lookup = new LookupAttributeMetadata
                        {
                            SchemaName = "rah_parent_rah_countryid",
                            DisplayName = new Label("Country Lookup", 1033),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            Description = new Label("MASTer Sample rah_country Lookup", 1033)
                        }
                    };
                    CreateOneToManyResponse createOneToManyRelationshipResponse =
                        (CreateOneToManyResponse)_serviceProxy.Execute(
                                createOneToManyRelationshipRequest);

                    _oneToManyRelationshipId =
                                createOneToManyRelationshipResponse.RelationshipId;
                    _oneToManyRelationshipName =
                        createOneToManyRelationshipRequest.OneToManyRelationship.SchemaName;

                    Console.WriteLine(
                                "The One-to-Many relationship has been created between {0} and {1}.",
                                "rah_country", "rah_territory");

                }


            }


            
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<OrganizationServiceFault> fe
                        = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }


        }
        //<snippetWorkWithRelationships4>
        /// <summary>
        /// Determines whether two entities are eligible to participate in a relationship
        /// </summary>
        /// <param name="referencedEntity">Primary Entity</param>
        /// <param name="referencingEntity">Referencing Entity</param>
        /// <returns></returns>
        public bool EligibleCreateOneToManyRelationship(string referencedEntity,
            string referencingEntity)
        {
            //Checks whether the specified entity can be the primary entity in one-to-many
            //relationship.
            CanBeReferencedRequest canBeReferencedRequest = new CanBeReferencedRequest
            {
                EntityName = referencedEntity
            };

            CanBeReferencedResponse canBeReferencedResponse =
                (CanBeReferencedResponse)_serviceProxy.Execute(canBeReferencedRequest);

            if (!canBeReferencedResponse.CanBeReferenced)
            {
                Console.WriteLine(
                    "Entity {0} can't be the primary entity in this one-to-many relationship",
                    referencedEntity);
            }

            //Checks whether the specified entity can be the referencing entity in one-to-many
            //relationship.
            CanBeReferencingRequest canBereferencingRequest = new CanBeReferencingRequest
            {
                EntityName = referencingEntity
            };

            CanBeReferencingResponse canBeReferencingResponse =
                (CanBeReferencingResponse)_serviceProxy.Execute(canBereferencingRequest);

            if (!canBeReferencingResponse.CanBeReferencing)
            {
                Console.WriteLine(
                    "Entity {0} can't be the referencing entity in this one-to-many relationship",
                    referencingEntity);
            }


            if (canBeReferencedResponse.CanBeReferenced == true && canBeReferencingResponse.CanBeReferencing == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //</snippetWorkWithRelationships4>
    }
}
