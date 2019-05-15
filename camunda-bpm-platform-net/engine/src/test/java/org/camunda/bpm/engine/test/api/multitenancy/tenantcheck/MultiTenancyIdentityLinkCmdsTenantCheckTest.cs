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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyIdentityLinkCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyIdentityLinkCmdsTenantCheckTest()
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

	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;

	  protected internal Task task;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		testRule.deployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		task = engineRule.TaskService.createTaskQuery().singleResult();

		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
	  }

	  // set Assignee
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAssigneeForTaskWithAuthenticatedTenant()
	  public virtual void setAssigneeForTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.setAssignee(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAssigneeForTaskWithNoAuthenticatedTenant()
	  public virtual void setAssigneeForTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.setAssignee(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setAssigneeForTaskWithDisabledTenantCheck()
	  public virtual void setAssigneeForTaskWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		taskService.setAssignee(task.Id, "demo");
		// then
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));
	  }

	  // set owner test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setOwnerForTaskWithAuthenticatedTenant()
	  public virtual void setOwnerForTaskWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.setOwner(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskOwner("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setOwnerForTaskWithNoAuthenticatedTenant()
	  public virtual void setOwnerForTaskWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.setOwner(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setOwnerForTaskWithDisabledTenantCheck()
	  public virtual void setOwnerForTaskWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		taskService.setOwner(task.Id, "demo");
		// then
		assertThat(taskService.createTaskQuery().taskOwner("demo").count(), @is(1L));
	  }

	  // get identity links
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getIdentityLinkWithAuthenticatedTenant()
	  public virtual void getIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		taskService.setOwner(task.Id, "demo");

		assertThat(taskService.getIdentityLinksForTask(task.Id)[0].Type, @is("owner"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getIdentityLinkWitNoAuthenticatedTenant()
	  public virtual void getIdentityLinkWitNoAuthenticatedTenant()
	  {

		taskService.setOwner(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.getIdentityLinksForTask(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getIdentityLinkWithDisabledTenantCheck()
	  public virtual void getIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.setOwner(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertThat(taskService.getIdentityLinksForTask(task.Id)[0].Type, @is("owner"));

	  }

	  // add candidate user
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateUserWithAuthenticatedTenant()
	  public virtual void addCandidateUserWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		taskService.addCandidateUser(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateUserWithNoAuthenticatedTenant()
	  public virtual void addCandidateUserWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.addCandidateUser(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateUserWithDisabledTenantCheck()
	  public virtual void addCandidateUserWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.addCandidateUser(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(1L));
	  }

	  // add candidate group
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateGroupWithAuthenticatedTenant()
	  public virtual void addCandidateGroupWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		taskService.addCandidateGroup(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateGroupWithNoAuthenticatedTenant()
	  public virtual void addCandidateGroupWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.addCandidateGroup(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addCandidateGroupWithDisabledTenantCheck()
	  public virtual void addCandidateGroupWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.addCandidateGroup(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));
	  }

	  // delete candidate users
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateUserWithAuthenticatedTenant()
	  public virtual void deleteCandidateUserWithAuthenticatedTenant()
	  {

		taskService.addCandidateUser(task.Id, "demo");
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.deleteCandidateUser(task.Id, "demo");
		// then
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateUserWithNoAuthenticatedTenant()
	  public virtual void deleteCandidateUserWithNoAuthenticatedTenant()
	  {

		taskService.addCandidateUser(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.deleteCandidateUser(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateUserWithDisabledTenantCheck()
	  public virtual void deleteCandidateUserWithDisabledTenantCheck()
	  {

		taskService.addCandidateUser(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.deleteCandidateUser(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(0L));
	  }

	  // delete candidate groups
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateGroupWithAuthenticatedTenant()
	  public virtual void deleteCandidateGroupWithAuthenticatedTenant()
	  {

		taskService.addCandidateGroup(task.Id, "demo");
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.deleteCandidateGroup(task.Id, "demo");
		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateGroupWithNoAuthenticatedTenant()
	  public virtual void deleteCandidateGroupWithNoAuthenticatedTenant()
	  {

		taskService.addCandidateGroup(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.deleteCandidateGroup(task.Id, "demo");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteCandidateGroupWithDisabledTenantCheck()
	  public virtual void deleteCandidateGroupWithDisabledTenantCheck()
	  {

		taskService.addCandidateGroup(task.Id, "demo");
		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.deleteCandidateGroup(task.Id, "demo");

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(0L));
	  }

	  // add user identity link
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addUserIdentityLinkWithAuthenticatedTenant()
	  public virtual void addUserIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

		// then
		assertThat(taskService.createTaskQuery().taskCandidateUser("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addUserIdentityLinkWithNoAuthenticatedTenant()
	  public virtual void addUserIdentityLinkWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addUserIdentityLinkWithDisabledTenantCheck()
	  public virtual void addUserIdentityLinkWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);

		// then
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));
	  }

	  // add group identity link
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addGroupIdentityLinkWithAuthenticatedTenant()
	  public virtual void addGroupIdentityLinkWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addGroupIdentityLinkWithNoAuthenticatedTenant()
	  public virtual void addGroupIdentityLinkWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addGroupIdentityLinkWithDisabledTenantCheck()
	  public virtual void addGroupIdentityLinkWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));
	  }

	  // delete user identity link
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteUserIdentityLinkWithAuthenticatedTenant()
	  public virtual void deleteUserIdentityLinkWithAuthenticatedTenant()
	  {

		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.deleteUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);
		// then
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteUserIdentityLinkWithNoAuthenticatedTenant()
	  public virtual void deleteUserIdentityLinkWithNoAuthenticatedTenant()
	  {

		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");
		taskService.deleteUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteUserIdentityLinkWithDisabledTenantCheck()
	  public virtual void deleteUserIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.addUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.deleteUserIdentityLink(task.Id, "demo", IdentityLinkType.ASSIGNEE);

		// then
		assertThat(taskService.createTaskQuery().taskAssignee("demo").count(), @is(0L));
	  }

	  // delete group identity link
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteGroupIdentityLinkWithAuthenticatedTenant()
	  public virtual void deleteGroupIdentityLinkWithAuthenticatedTenant()
	  {

		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		taskService.deleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);
		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteGroupIdentityLinkWithNoAuthenticatedTenant()
	  public virtual void deleteGroupIdentityLinkWithNoAuthenticatedTenant()
	  {

		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot assign the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		taskService.deleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteGroupIdentityLinkWithDisabledTenantCheck()
	  public virtual void deleteGroupIdentityLinkWithDisabledTenantCheck()
	  {

		taskService.addGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(1L));

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		taskService.deleteGroupIdentityLink(task.Id, "demo", IdentityLinkType.CANDIDATE);

		// then
		assertThat(taskService.createTaskQuery().taskCandidateGroup("demo").count(), @is(0L));
	  }
	}

}