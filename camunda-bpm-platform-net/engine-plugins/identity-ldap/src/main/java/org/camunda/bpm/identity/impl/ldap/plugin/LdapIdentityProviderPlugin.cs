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
namespace org.camunda.bpm.identity.impl.ldap.plugin
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using CertificateHelper = org.camunda.bpm.identity.impl.ldap.util.CertificateHelper;
	using LdapPluginLogger = org.camunda.bpm.identity.impl.ldap.util.LdapPluginLogger;

	/// <summary>
	/// <para><seealso cref="ProcessEnginePlugin"/> providing Ldap Identity Provider support</para>
	/// 
	/// <para>This class extends <seealso cref="LdapConfiguration"/> such that the configuration properties
	/// can be set directly on this class vie the <code>&lt;properties .../&gt;</code> element
	/// in bpm-platform.xml / processes.xml</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapIdentityProviderPlugin : LdapConfiguration, ProcessEnginePlugin
	{

	  protected internal bool acceptUntrustedCertificates = false;

	  public virtual void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {

		LdapPluginLogger.INSTANCE.pluginActivated(this.GetType().Name, processEngineConfiguration.ProcessEngineName);

		if (acceptUntrustedCertificates)
		{
		  CertificateHelper.acceptUntrusted();
		  LdapPluginLogger.INSTANCE.acceptingUntrustedCertificates();
		}

		LdapIdentityProviderFactory ldapIdentityProviderFactory = new LdapIdentityProviderFactory();
		ldapIdentityProviderFactory.LdapConfiguration = this;
		processEngineConfiguration.IdentityProviderSessionFactory = ldapIdentityProviderFactory;

	  }

	  public virtual void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		// nothing to do
	  }

	  public virtual void postProcessEngineBuild(ProcessEngine processEngine)
	  {
		// nothing to do
	  }

	  public virtual bool AcceptUntrustedCertificates
	  {
		  set
		  {
			this.acceptUntrustedCertificates = value;
		  }
		  get
		  {
			return acceptUntrustedCertificates;
		  }
	  }


	}

}