using System;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.identity.impl.ldap.util
{

	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;
	using BaseLogger = org.camunda.commons.logging.BaseLogger;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class LdapPluginLogger : BaseLogger
	{

	  public const string PROJECT_CODE = "LDAP";

	  public static readonly LdapPluginLogger INSTANCE = BaseLogger.createLogger(typeof(LdapPluginLogger), PROJECT_CODE, "org.camunda.bpm.identity.impl.ldap", "00");

	  public virtual void pluginActivated(string pluginClassName, string engineName)
	  {
		logInfo("001", "PLUGIN {} activated on process engine {}", pluginClassName, engineName);
	  }

	  public virtual void acceptingUntrustedCertificates()
	  {
		logWarn("002", "Enabling accept of untrusted certificates. Use at own risk.");
	  }

	  public virtual void exceptionWhenClosingLdapCOntext(Exception e)
	  {
		logDebug("003", "exception while closing LDAP DIR CTX", e);
	  }

	  public virtual void invalidLdapGroupReturned(GroupEntity group, SearchResult searchResult)
	  {
		logError("004", "LDAP group query returned a group with id null. This group will be ignored. " + "This indicates a misconfiguration of the LDAP plugin or a problem with the LDAP service." + " Enable DEBUG/FINE logging for details.");
		// log sensitive data only on FINE
		logDebug("004", "Invalid group: {} based on search result {}", group, searchResult);
	  }

	  public virtual void invalidLdapUserReturned(UserEntity user, SearchResult searchResult)
	  {
		logError("004", "LDAP user query returned a user with id null. This user will be ignored. " + "This indicates a misconfiguration of the LDAP plugin or a problem with the LDAP service." + " Enable DEBUG/FINE logging for details.");
		// log sensitive data only on FINE
		logDebug("004", "Invalid user: {} based on search result {}", user, searchResult);
	  }

	  public virtual void groupQueryResult(string summary)
	  {
		// log sensitive data only on FINE
		logDebug("005", summary);
	  }

	  public virtual void userQueryResult(string summary)
	  {
		// log sensitive data only on FINE
		logDebug("006", summary);
	  }
	}

}