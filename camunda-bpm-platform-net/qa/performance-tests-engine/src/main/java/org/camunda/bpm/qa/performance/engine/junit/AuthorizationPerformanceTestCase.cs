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
namespace org.camunda.bpm.qa.performance.engine.junit
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;

	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using PerfTestBuilder = org.camunda.bpm.qa.performance.engine.framework.PerfTestBuilder;
	using PerfTestConfiguration = org.camunda.bpm.qa.performance.engine.framework.PerfTestConfiguration;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AuthorizationPerformanceTestCase
	{
		private bool InstanceFieldsInitialized = false;

		public AuthorizationPerformanceTestCase()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(testConfigurationRule).around(resultRecorderRule);
		}


	  public PerfTestConfigurationRule testConfigurationRule = new PerfTestConfigurationRule();

	  public PerfTestResultRecorderRule resultRecorderRule = new PerfTestResultRecorderRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(testConfigurationRule).around(resultRecorderRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngine engine;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		engine = PerfTestProcessEngine.Instance;
		taskService = engine.TaskService;
		historyService = engine.HistoryService;
		runtimeService = engine.RuntimeService;
		repositoryService = engine.RepositoryService;
	  }

	  public virtual PerfTestBuilder performanceTest()
	  {
		PerfTestConfiguration configuration = testConfigurationRule.PerformanceTestConfiguration;
		configuration.Platform = "camunda BPM";
		return new PerfTestBuilder(configuration, resultRecorderRule);
	  }


	  protected internal virtual void grouptGrant(string groupId, Resource resource, params Permission[] perms)
	  {

		AuthorizationService authorizationService = engine.AuthorizationService;
		Authorization groupGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		groupGrant.Resource = resource;
		groupGrant.ResourceId = ANY;
		foreach (Permission permission in perms)
		{
		  groupGrant.addPermission(permission);
		}
		groupGrant.GroupId = groupId;
		authorizationService.saveAuthorization(groupGrant);
	  }

	  protected internal virtual void userGrant(string userId, Resource resource, params Permission[] perms)
	  {

		AuthorizationService authorizationService = engine.AuthorizationService;
		Authorization groupGrant = authorizationService.createNewAuthorization(AUTH_TYPE_GRANT);
		groupGrant.Resource = resource;
		groupGrant.ResourceId = ANY;
		foreach (Permission permission in perms)
		{
		  groupGrant.addPermission(permission);
		}
		groupGrant.UserId = userId;
		authorizationService.saveAuthorization(groupGrant);
	  }

	}

}