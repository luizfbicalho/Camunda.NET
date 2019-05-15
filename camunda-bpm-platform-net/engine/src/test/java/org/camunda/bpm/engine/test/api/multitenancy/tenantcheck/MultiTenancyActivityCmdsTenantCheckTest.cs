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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class MultiTenancyActivityCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyActivityCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string processInstanceId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		testRule.deployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityInstanceWithAuthenticatedTenant()
	  public virtual void getActivityInstanceWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertNotNull(engineRule.RuntimeService.getActivityInstance(processInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityInstanceWithNoAuthenticatedTenant()
	  public virtual void getActivityInstanceWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.getActivityInstance(processInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityInstanceWithDisabledTenantCheck()
	  public virtual void getActivityInstanceWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertNotNull(engineRule.RuntimeService.getActivityInstance(processInstanceId));
	  }

	  // get active activity id
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityIdsWithAuthenticatedTenant()
	  public virtual void getActivityIdsWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		assertEquals(1, engineRule.RuntimeService.getActiveActivityIds(processInstanceId).Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityIdsWithNoAuthenticatedTenant()
	  public virtual void getActivityIdsWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		engineRule.RuntimeService.getActiveActivityIds(processInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getActivityIdsWithDisabledTenantCheck()
	  public virtual void getActivityIdsWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertEquals(1, engineRule.RuntimeService.getActiveActivityIds(processInstanceId).Count);

	  }

	}

}