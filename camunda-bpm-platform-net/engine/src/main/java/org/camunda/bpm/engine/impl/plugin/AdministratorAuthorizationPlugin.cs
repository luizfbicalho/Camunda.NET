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
namespace org.camunda.bpm.engine.impl.plugin
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;

	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using AbstractProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.AbstractProcessEnginePlugin;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AdministratorAuthorizationPlugin : AbstractProcessEnginePlugin
	{

	  private static readonly AdministratorAuthorizationPluginLogger LOG = ProcessEngineLogger.ADMIN_PLUGIN_LOGGER;

	  /// <summary>
	  /// The name of the administrator group.
	  /// 
	  /// If this name is set to a non-null and non-empty value,
	  /// the plugin will create group-level Administrator authorizations
	  /// on all built-in resources. 
	  /// </summary>
	  protected internal string administratorGroupName;

	  /// <summary>
	  /// The name of the administrator user.
	  /// 
	  /// If this name is set to a non-null and non-empty value,
	  /// the plugin will create group-level Administrator authorizations
	  /// on all built-in resources. 
	  /// </summary>
	  protected internal string administratorUserName;

	  protected internal bool authorizationEnabled;

	  public override void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		authorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
		if (!string.ReferenceEquals(administratorGroupName, null) && administratorGroupName.Length > 0)
		{
		  processEngineConfiguration.AdminGroups.Add(administratorGroupName);
		}
		if (!string.ReferenceEquals(administratorUserName, null) && administratorUserName.Length > 0)
		{
		  processEngineConfiguration.AdminUsers.Add(administratorUserName);
		}
	  }

	  public override void postProcessEngineBuild(ProcessEngine processEngine)
	  {
		if (!authorizationEnabled)
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.AuthorizationService authorizationService = processEngine.getAuthorizationService();
		AuthorizationService authorizationService = processEngine.AuthorizationService;

		if (!string.ReferenceEquals(administratorGroupName, null) && administratorGroupName.Length > 0)
		{
		  // create ADMIN authorizations on all built-in resources for configured group
		  foreach (Resource resource in Resources.values())
		  {
			if (authorizationService.createAuthorizationQuery().groupIdIn(administratorGroupName).resourceType(resource).resourceId(ANY).count() == 0)
			{
			  AuthorizationEntity adminGroupAuth = new AuthorizationEntity(AUTH_TYPE_GRANT);
			  adminGroupAuth.GroupId = administratorGroupName;
			  adminGroupAuth.setResource(resource);
			  adminGroupAuth.ResourceId = ANY;
			  adminGroupAuth.addPermission(ALL);
			  authorizationService.saveAuthorization(adminGroupAuth);
			  LOG.grantGroupPermissions(administratorGroupName, resource.resourceName());

			}
		  }
		}

		if (!string.ReferenceEquals(administratorUserName, null) && administratorUserName.Length > 0)
		{
		  // create ADMIN authorizations on all built-in resources for configured user
		  foreach (Resource resource in Resources.values())
		  {
			if (authorizationService.createAuthorizationQuery().userIdIn(administratorUserName).resourceType(resource).resourceId(ANY).count() == 0)
			{
			  AuthorizationEntity adminUserAuth = new AuthorizationEntity(AUTH_TYPE_GRANT);
			  adminUserAuth.UserId = administratorUserName;
			  adminUserAuth.setResource(resource);
			  adminUserAuth.ResourceId = ANY;
			  adminUserAuth.addPermission(ALL);
			  authorizationService.saveAuthorization(adminUserAuth);
			  LOG.grantUserPermissions(administratorUserName, resource.resourceName());
			}
		  }
		}

	  }



	  // getter / setters ////////////////////////////////////

	  public virtual string AdministratorGroupName
	  {
		  get
		  {
			return administratorGroupName;
		  }
		  set
		  {
			this.administratorGroupName = value;
		  }
	  }


	  public virtual string AdministratorUserName
	  {
		  get
		  {
			return administratorUserName;
		  }
		  set
		  {
			this.administratorUserName = value;
		  }
	  }


	}

}