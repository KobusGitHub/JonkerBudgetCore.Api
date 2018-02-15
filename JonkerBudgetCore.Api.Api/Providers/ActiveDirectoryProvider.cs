using JonkerBudgetCore.Api.Auth.ActiveDirectory;
using System.Collections.Generic;
using JonkerBudgetCore.Api.Auth;
using Novell.Directory.Ldap;
using JonkerBudgetCore.Api.Api.Providers;
using Microsoft.Extensions.Options;

namespace JonkerBudgetCore.Api.Api.Providers
{
    public class ActiveDirectoryProvider : IActiveDirectoryProvider
    {
        private readonly ActiveDirectoryOptions options;

        public ActiveDirectoryProvider(IOptions<ActiveDirectoryOptions> options)
        {
            this.options = options.Value;
        }

        public IEnumerable<ActiveDirectoryUser> QueryActiveDirectory(string username)
        {
            List<ActiveDirectoryUser> listToReturn = new List<ActiveDirectoryUser>();               

            try
            {
                using (var cn = new LdapConnection())
                {
                    cn.Connect(options.Host, options.Port); //connect                            
                    cn.Bind(options.Username, options.Password); // bind with credentials
                    LdapSearchResults lsc = cn.Search("dc=supergrp,dc=net",
                           LdapConnection.SCOPE_SUB,
                           "(&(objectClass=person)(sAMAccountName=*" + username + "*))",
                           null,
                           false);

                    while (lsc.hasMore())
                    {
                        LdapEntry nextEntry = null;
                        try
                        {
                            nextEntry = lsc.next();
                            LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
                            var newActiveDirectoryUser = new ActiveDirectoryUser();

                            // addition check to be in place because our Ad has computers also setup as Users and Persons
                            if (attributeSet.getAttribute("givenName") == null)
                            {
                                continue;
                            }

                            newActiveDirectoryUser.Firstname = attributeSet.getAttribute("givenName").StringValue;
                            
                            if (attributeSet.getAttribute("sn") != null)
                            {
                                newActiveDirectoryUser.Lastname = attributeSet.getAttribute("sn").StringValue;
                            }

                            if (attributeSet.getAttribute("sAMAccountName") != null)
                            {
                                newActiveDirectoryUser.UserName = attributeSet.getAttribute("sAMAccountName").StringValue;
                            }

                            if (attributeSet.getAttribute("mail") != null)
                            {
                                newActiveDirectoryUser.Email = attributeSet.getAttribute("mail").StringValue;
                            }

                            listToReturn.Add(newActiveDirectoryUser);

                        }
                        catch (LdapException)
                        {
                            // Exception is thrown, return the list
                            return listToReturn;                          
                        }                                           
                    }                  
                }
            
            }
            catch (System.Exception)
            {                
                return listToReturn;
            }

            return listToReturn;
        }
    }
}
