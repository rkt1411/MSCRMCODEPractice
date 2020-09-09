using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Drawing;
using System.Net.Mail;

namespace WorkWithRelationships
{
    public class Contact
    {
        CRMHelper objCRMHelper;
        IOrganizationService iOrgService;
        Guid contactId = Guid.Empty;
        int age;
        DateTime dobdate;
        //  DateTime? dobdate = null;
        EntityCollection contactResults = new EntityCollection();
        Contact objContact;
        public bool LoadContact()
        {

            try
            {
                objContact = new Contact();

                objCRMHelper = new CRMHelper();
                iOrgService = objCRMHelper.setSrvice();
                // GetContactDetailFromCRM whos birthday id today.
                contactResults = GetContactDetailFromCRM(iOrgService);
                Console.WriteLine("Welcome {0} \t", contactResults.Entities.Count);
                //   Console.ReadKey();

                if (contactResults.Entities != null && contactResults.Entities.Count > 1)
                {

                    foreach (Entity contact in contactResults.Entities)
                    {
                       // DateTime dobdate = default(DateTime);
                        //DateTime defaultDate = default(DateTime);
                        string fullname = contact["fullname"].ToString();
                        Console.WriteLine("fullname \t {0}", fullname + "\n");

                        if (contact.Attributes.Contains("contactid") && contact["contactid"] != null)
                        {
                            contactId = new Guid(contact["contactid"].ToString());
                        }
                        // if (contact.Attributes.Contains("birthdate") && !string.IsNullOrEmpty(contact["birthdate"].ToString()) && contact["birthdate"].ToString()!= null)
                        if (contact.Attributes.Contains("birthdate") && contact["birthdate"].ToString() != null)
                        {
                            // dobdate = ((DateTime)contact["birthdate"]).ToLocalTime();
                            dobdate = (DateTime)contact["birthdate"];
                            Console.WriteLine("dobdate \t {0}", dobdate + "\n");
                        }
                        if (contact.Attributes.Contains("rah_age") && !string.IsNullOrEmpty(contact["rah_age"].ToString()))
                        {
                            age = (int)contact["rah_age"];
                        }


                        if (contact.Attributes.Contains("birthdate") && contact["birthdate"].ToString() != null)
                        {
                            // dobdate = ((DateTime)contact["birthdate"]).ToLocalTime();
                            dobdate = (DateTime)contact["birthdate"];
                            Console.WriteLine("dobdate \t {0}", dobdate + "\n");
                        }

                        // if (contactId != Guid.Empty && dobdate != null && defaultDate!= dobdate)
                        if (contactId != Guid.Empty && dobdate != null)
                        {
                            
                            //if (contact.Attributes.Contains("birthdate") && contact["birthdate"].ToString() != null)
                            //{
                                objContact.UpdateExistingContact(contactId, dobdate, age, iOrgService);
                            //}
                            
                        }

                    }
                }


            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private void UpdateExistingContact(Guid contactId, DateTime  birthday, int ageonCRM, IOrganizationService iOrgService)
        {
            objCRMHelper = new CRMHelper();
            iOrgService = objCRMHelper.setSrvice();

            objCRMHelper = new CRMHelper();

            DateTime nextBirthDate;
            try
            {
                Entity objContact = new Entity();
                objContact.LogicalName = "contact";
                objContact.Id = contactId;

                int age = DateTime.Now.Year - birthday.Year;
                if (DateTime.Now.Month < birthday.Month || DateTime.Now.Month == birthday.Month && DateTime.Now.Day < birthday.Day)
                    age--;


                objContact["rah_age"] = age;
                nextBirthDate = CalculateNextBirthday(birthday);
                objContact["rah_nextbirthday"] = nextBirthDate.ToLocalTime();
                iOrgService.Update(objContact);


            }
            catch (SmtpException ex)
            {
                Console.WriteLine("SmtpException {0} \t", ex.ToString());
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception {0} \t", exc.ToString());
            }

        }

        private DateTime CalculateNextBirthday(DateTime birthdate)
        {
            DateTime nextBirthday = new DateTime(birthdate.Year, birthdate.Month, birthdate.Day);
            //Check to see if this birthday occurred on a leap year
            bool leapYearAdjust = false;
            if (nextBirthday.Month == 2 && nextBirthday.Day == 29)
            {
                //Sanity check, was that year a leap year
                if (DateTime.IsLeapYear(nextBirthday.Year))
                {
                    //Check to see if the current year is a leap year
                    if (!DateTime.IsLeapYear(DateTime.Now.Year))
                    {
                        //Push the date to March 1st so that the date arithmetic will function correctly
                        nextBirthday = nextBirthday.AddDays(1);
                        leapYearAdjust = true;
                    }
                }
                else
                {
                    // throw new Exception("Invalid Birthdate specified", new ArgumentException("Birthdate"));
                }
            }

            //Calculate the year difference

            nextBirthday = nextBirthday.AddYears(DateTime.Now.Year - nextBirthday.Year);
            if (nextBirthday.Date <= DateTime.Now.Date)
            {
                nextBirthday = nextBirthday.AddYears(1);
            }
            //Check to see if the date was adjusted
            if (leapYearAdjust && DateTime.IsLeapYear(nextBirthday.Year))
            {
                nextBirthday = nextBirthday.AddDays(-1);
            }

            return nextBirthday;
        }

        private EntityCollection GetContactDetailFromCRM(IOrganizationService iOrgService)
        {
            //objCRMHelper = new CRMHelper();
            //iOrgService = objCRMHelper.setSrvice();
            try
            {
                string contactQuery = @"<fetch mapping='logical'>
                                            <entity name='contact'> 
                                            <attribute name='contactid' />
                                            <attribute name='fullname' />
                                            <attribute name='rah_nextbirthday' />
                                            <attribute name='birthdate' />
                                            <attribute name='rah_age' />
                                            <filter type='and'>
                                                <filter type='or'>
                                                   <condition attribute='rah_nextbirthday' operator='today'/>
                                                   <condition attribute='rah_nextbirthday' operator='null' />
                                                </filter>
                                           </filter>                                        
                                          </entity>
                                         </fetch>";
                contactResults = iOrgService.RetrieveMultiple(new FetchExpression(contactQuery));
                Console.WriteLine("Welcome" + contactResults.Entities.Count);

                Console.WriteLine("iOrgService");

            }
            catch (Exception)
            {

                throw;
            }
            return contactResults;
        }
    }
}
