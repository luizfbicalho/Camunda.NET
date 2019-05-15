using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.history
{

	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIdentityLinkLogTestByXml : PluggableProcessEngineTestCase
	{

	  private static string PROCESS_DEFINITION_KEY_CANDIDATE_USER = "oneTaskProcessForHistoricIdentityLinkWithCanidateUser";
	  private static string PROCESS_DEFINITION_KEY_CANDIDATE_GROUP = "oneTaskProcessForHistoricIdentityLinkWithCanidateGroup";
	  private static string PROCESS_DEFINITION_KEY_ASSIGNEE = "oneTaskProcessForHistoricIdentityLinkWithAssignee";
	  private static string PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_USER = "oneTaskProcessForHistoricIdentityLinkWithCanidateStarterUsers";
	  private static string PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_GROUP = "oneTaskProcessForHistoricIdentityLinkWithCanidateStarterGroups";
	  private const string XML_USER = "demo";
	  private const string XML_GROUP = "demoGroups";
	  private const string XML_ASSIGNEE = "assignee";

	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string CANDIDATE_STARTER_USER = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterUser.bpmn20.xml";
	  protected internal const string CANDIDATE_STARTER_USERS = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterUsers.bpmn20.xml";

	  protected internal const string CANDIDATE_STARTER_GROUP = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterGroup.bpmn20.xml";
	  protected internal const string CANDIDATE_STARTER_GROUPS = "org/camunda/bpm/engine/test/api/repository/ProcessDefinitionCandidateTest.testCandidateStarterGroups.bpmn20.xml";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithCandidateUser.bpmn20.xml" })]
	  public virtual void testShouldAddTaskCandidateforAddIdentityLinkUsingXml()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY_CANDIDATE_USER);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// query Test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(XML_USER).count(), 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithTaskAssignee.bpmn20.xml" })]
	  public virtual void testShouldAddTaskAssigneeforAddIdentityLinkUsingXml()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY_ASSIGNEE);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// query Test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(XML_ASSIGNEE).count(), 1);


	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithCandidateGroups.bpmn20.xml" })]
	  public virtual void testShouldAddTaskCandidateGroupforAddIdentityLinkUsingXml()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY_CANDIDATE_GROUP);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// query Test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.groupId(XML_GROUP).count(), 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithCandidateStarterUsers.bpmn20.xml" })]
	  public virtual void testShouldAddProcessCandidateStarterUserforAddIdentityLinkUsingXml()
	  {

		// Pre test - Historical identity link is added as part of deployment
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// given
		ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_USER).singleResult();
		assertNotNull(latestProcessDef);

		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		assertEquals(1, links.Count);

		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// query Test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(XML_USER).count(), 1);
	  }
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithCandidateStarterGroups.bpmn20.xml" })]
	  public virtual void testShouldAddProcessCandidateStarterGroupforAddIdentityLinkUsingXml()
	  {

		// Pre test - Historical identity link is added as part of deployment
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// given
		ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY_CANDIDATE_STARTER_GROUP).singleResult();
		assertNotNull(latestProcessDef);

		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		assertEquals(1, links.Count);

		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// query Test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.groupId(XML_GROUP).count(), 1);
	  }

	  public virtual void testPropagateTenantIdToCandidateStarterUser()
	  {
		// when
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_USER).tenantId(TENANT_ONE).deploy();

		// then
		IList<HistoricIdentityLinkLog> historicLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicLinks.Count, 1);

		HistoricIdentityLinkLog historicLink = historicLinks[0];
		assertNotNull(historicLink.TenantId);
		assertEquals(TENANT_ONE, historicLink.TenantId);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testPropagateTenantIdToCandidateStarterUsers()
	  {
		// when
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_USERS).tenantId(TENANT_ONE).deploy();

		  // then
		  IList<HistoricIdentityLinkLog> historicLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		  assertEquals(3, historicLinks.Count);

		foreach (HistoricIdentityLinkLog historicLink in historicLinks)
		{
		  assertNotNull(historicLink.TenantId);
		  assertEquals(TENANT_ONE, historicLink.TenantId);
		}

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testPropagateTenantIdToCandidateStarterGroup()
	  {
		// when
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_GROUP).tenantId(TENANT_ONE).deploy();

		  // then
		  IList<HistoricIdentityLinkLog> historicLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		  assertEquals(historicLinks.Count, 1);

		  HistoricIdentityLinkLog historicLink = historicLinks[0];
		  assertNotNull(historicLink.TenantId);
		  assertEquals(TENANT_ONE, historicLink.TenantId);

		  repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testPropagateTenantIdToCandidateStarterGroups()
	  {
		// when
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource(CANDIDATE_STARTER_GROUPS).tenantId(TENANT_ONE).deploy();

		  // then
		  IList<HistoricIdentityLinkLog> historicLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		  assertEquals(3, historicLinks.Count);

		foreach (HistoricIdentityLinkLog historicLink in historicLinks)
		{
		  assertNotNull(historicLink.TenantId);
		  assertEquals(TENANT_ONE, historicLink.TenantId);
		}

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  protected internal virtual ProcessInstance startProcessInstance(string key)
	  {
		return runtimeService.startProcessInstanceByKey(key);
	  }
	}

}