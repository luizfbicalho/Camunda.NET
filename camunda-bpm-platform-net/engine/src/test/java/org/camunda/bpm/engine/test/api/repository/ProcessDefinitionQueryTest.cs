using System;
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
namespace org.camunda.bpm.engine.test.api.repository
{
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ProcessDefinitionQueryTest : AbstractDefinitionQueryTest
	{

	  private string deploymentThreeId;

	  protected internal override string ResourceOnePath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/repository/one.bpmn20.xml";
		  }
	  }

	  protected internal override string ResourceTwoPath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/repository/two.bpmn20.xml";
		  }
	  }

	  protected internal virtual string ResourceThreePath
	  {
		  get
		  {
			return "org/camunda/bpm/engine/test/repository/three_.bpmn20.xml";
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deploymentThreeId = repositoryService.createDeployment().name("thirdDeployment").addClasspathResource(ResourceThreePath).deploy().Id;
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		repositoryService.deleteDeployment(deploymentThreeId, true);
	  }

	  public virtual void testProcessDefinitionProperties()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionName().asc().orderByProcessDefinitionVersion().asc().orderByProcessDefinitionCategory().asc().list();

		ProcessDefinition processDefinition = processDefinitions[0];
		assertEquals("one", processDefinition.Key);
		assertEquals("One", processDefinition.Name);
		assertEquals("Desc one", processDefinition.Description);
		assertTrue(processDefinition.Id.StartsWith("one:1", StringComparison.Ordinal));
		assertEquals("Examples", processDefinition.Category);

		processDefinition = processDefinitions[1];
		assertEquals("one", processDefinition.Key);
		assertEquals("One", processDefinition.Name);
		assertEquals("Desc one", processDefinition.Description);
		assertTrue(processDefinition.Id.StartsWith("one:2", StringComparison.Ordinal));
		assertEquals("Examples", processDefinition.Category);

		processDefinition = processDefinitions[2];
		assertEquals("two", processDefinition.Key);
		assertEquals("Two", processDefinition.Name);
		assertNull(processDefinition.Description);
		assertTrue(processDefinition.Id.StartsWith("two:1", StringComparison.Ordinal));
		assertEquals("Examples2", processDefinition.Category);
	  }

	  public virtual void testQueryByDeploymentId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().deploymentId(deploymentOneId);
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().deploymentId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().deploymentId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByName()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionName("Two");
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionName("One");
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidName()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionName("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionName(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByNameLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionNameLike("%w%");
		verifyQueryResults(query, 1);
		query = query.processDefinitionNameLike("%z\\_%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidNameLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionNameLike("%invalid%");
		verifyQueryResults(query, 0);
	  }

	  /// <summary>
	  /// CAM-8014
	  /// 
	  /// Verify that search by name like returns results with case-insensitive
	  /// </summary>
	  public virtual void testQueryByNameLikeCaseInsensitive()
	  {
		ProcessDefinitionQuery queryCaseInsensitive = repositoryService.createProcessDefinitionQuery().processDefinitionNameLike("%OnE%");
		verifyQueryResults(queryCaseInsensitive, 2);
	  }

	  public virtual void testQueryByKey()
	  {
		// process one
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one");
		verifyQueryResults(query, 2);

		// process two
		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("two");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByKeys()
	  {

		// empty list
		assertTrue(repositoryService.createProcessDefinitionQuery().processDefinitionKeysIn("a", "b").list().Empty);


		// collect all definition keys
		IList<ProcessDefinition> list = repositoryService.createProcessDefinitionQuery().list();
		string[] processDefinitionKeys = new string[list.Count];
		for (int i = 0; i < processDefinitionKeys.Length; i++)
		{
		  processDefinitionKeys[i] = list[i].Key;
		}

		IList<ProcessDefinition> keyInList = repositoryService.createProcessDefinitionQuery().processDefinitionKeysIn(processDefinitionKeys).list();
		foreach (ProcessDefinition processDefinition in keyInList)
		{
		  bool found = false;
		  foreach (ProcessDefinition otherProcessDefinition in list)
		  {
			if (otherProcessDefinition.Key.Equals(processDefinition.Key))
			{
			  found = true;
			  break;
			}
		  }
		  if (!found)
		  {
			fail("Expected to find process definition " + processDefinition);
		  }
		}

		assertEquals(0, repositoryService.createProcessDefinitionQuery().processDefinitionKey("dummyKey").processDefinitionKeysIn(processDefinitionKeys).count());
	  }

	  public virtual void testQueryByInvalidKey()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionKey(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByKeyLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKeyLike("%o%");
		verifyQueryResults(query, 3);
		query = query.processDefinitionKeyLike("%z\\_%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidKeyLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKeyLike("%invalid%");
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionKeyLike(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByResourceNameLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionResourceNameLike("%ee\\_%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidResourceNameLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionResourceNameLike("%invalid%");
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionResourceNameLike(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByCategory()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionCategory("Examples");
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByCategoryLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionCategoryLike("%Example%");
		verifyQueryResults(query, 3);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionCategoryLike("%amples2");
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionCategoryLike("%z\\_%");
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByVersion()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(2);
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1);
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryByInvalidVersion()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionVersion(3);
		verifyQueryResults(query, 0);

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionVersion(-1).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionVersion(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByKeyAndVersion()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").processDefinitionVersion(1);
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").processDefinitionVersion(2);
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").processDefinitionVersion(3);
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryByLatest()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().latestVersion();
		verifyQueryResults(query, 3);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").latestVersion();
		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().processDefinitionKey("two").latestVersion();
		verifyQueryResults(query, 1);
	  }

	  public virtual void testInvalidUsageOfLatest()
	  {
		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionId("test").latestVersion().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}

		try
		{
		  repositoryService.createProcessDefinitionQuery().processDefinitionVersion(1).latestVersion().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}

		try
		{
		  repositoryService.createProcessDefinitionQuery().deploymentId("test").latestVersion().list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQuerySorting()
	  {

		// asc

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByDeploymentId().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionKey().asc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().asc();
		verifyQueryResults(query, 4);

		// desc

		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByDeploymentId().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionKey().desc();
		verifyQueryResults(query, 4);

		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionVersion().desc();
		verifyQueryResults(query, 4);

		// Typical use case
		query = repositoryService.createProcessDefinitionQuery().orderByProcessDefinitionKey().asc().orderByProcessDefinitionVersion().desc();
		IList<ProcessDefinition> processDefinitions = query.list();
		assertEquals(4, processDefinitions.Count);

		assertEquals("one", processDefinitions[0].Key);
		assertEquals(2, processDefinitions[0].Version);
		assertEquals("one", processDefinitions[1].Key);
		assertEquals(1, processDefinitions[1].Version);
		assertEquals("two", processDefinitions[2].Key);
		assertEquals(1, processDefinitions[2].Version);
	  }

	  public virtual void testQueryByMessageSubscription()
	  {
		Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/processWithNewBookingMessage.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/test/api/repository/processWithNewInvoiceMessage.bpmn20.xml").deploy();

		assertEquals(1,repositoryService.createProcessDefinitionQuery().messageEventSubscriptionName("newInvoiceMessage").count());

		assertEquals(1,repositoryService.createProcessDefinitionQuery().messageEventSubscriptionName("newBookingMessage").count());

		assertEquals(0,repositoryService.createProcessDefinitionQuery().messageEventSubscriptionName("bogus").count());

		repositoryService.deleteDeployment(deployment.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentId()
	  public virtual void testQueryByIncidentId()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("failingProcess").count());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().incidentId(incident.Id);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidIncidentId()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		verifyQueryResults(query.incidentId("invalid"), 0);

		try
		{
		  query.incidentId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentType()
	  public virtual void testQueryByIncidentType()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("failingProcess").count());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().incidentType(incident.IncidentType);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidIncidentType()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		verifyQueryResults(query.incidentType("invalid"), 0);

		try
		{
		  query.incidentType(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessage()
	  public virtual void testQueryByIncidentMessage()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("failingProcess").count());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().incidentMessage(incident.IncidentMessage);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidIncidentMessage()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		verifyQueryResults(query.incidentMessage("invalid"), 0);

		try
		{
		  query.incidentMessage(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByIncidentMessageLike()
	  public virtual void testQueryByIncidentMessageLike()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("failingProcess").count());

		runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidentList = runtimeService.createIncidentQuery().list();
		assertEquals(1, incidentList.Count);

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery().incidentMessageLike("%expected%");

		verifyQueryResults(query, 1);

		query = repositoryService.createProcessDefinitionQuery().incidentMessageLike("%\\_expected%");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidIncidentMessageLike()
	  {
		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		verifyQueryResults(query.incidentMessageLike("invalid"), 0);

		try
		{
		  query.incidentMessageLike(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

	  public virtual void testQueryByProcessDefinitionIds()
	  {

		// empty list
		assertTrue(repositoryService.createProcessDefinitionQuery().processDefinitionIdIn("a", "b").list().Empty);


		// collect all ids
		IList<ProcessDefinition> list = repositoryService.createProcessDefinitionQuery().list();
		string[] ids = new string[list.Count];
		for (int i = 0; i < ids.Length; i++)
		{
		  ids[i] = list[i].Id;
		}

		IList<ProcessDefinition> idInList = repositoryService.createProcessDefinitionQuery().processDefinitionIdIn(ids).list();
		foreach (ProcessDefinition processDefinition in idInList)
		{
		  bool found = false;
		  foreach (ProcessDefinition otherProcessDefinition in list)
		  {
			if (otherProcessDefinition.Id.Equals(processDefinition.Id))
			{
			  found = true;
			  break;
			}
		  }
		  if (!found)
		  {
			fail("Expected to find process definition " + processDefinition);
		  }
		}

		assertEquals(0, repositoryService.createProcessDefinitionQuery().processDefinitionId("dummyId").processDefinitionIdIn(ids).count());
	  }

	  public virtual void testQueryByLatestAndName()
	  {
		string firstDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/first-process.bpmn20.xml").deploy().Id;

		string secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/first-process.bpmn20.xml").deploy().Id;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		query.processDefinitionName("First Test Process").latestVersion();

		verifyQueryResults(query, 1);

		ProcessDefinition result = query.singleResult();

		assertEquals("First Test Process", result.Name);
		assertEquals(2, result.Version);

		repositoryService.deleteDeployment(firstDeployment, true);
		repositoryService.deleteDeployment(secondDeployment, true);

	  }

	  public virtual void testQueryByLatestAndName_NotFound()
	  {
		string firstDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/first-process.bpmn20.xml").deploy().Id;

		string secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/second-process.bpmn20.xml").deploy().Id;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		query.processDefinitionName("First Test Process").latestVersion();

		verifyQueryResults(query, 0);

		repositoryService.deleteDeployment(firstDeployment, true);
		repositoryService.deleteDeployment(secondDeployment, true);

	  }

	  public virtual void testQueryByLatestAndNameLike()
	  {
		string firstDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/first-process.bpmn20.xml").deploy().Id;

		string secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/second-process.bpmn20.xml").deploy().Id;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		query.processDefinitionNameLike("%Test Process").latestVersion();

		verifyQueryResults(query, 1);

		ProcessDefinition result = query.singleResult();

		assertEquals("Second Test Process", result.Name);
		assertEquals(2, result.Version);

		query.processDefinitionNameLike("%Test%").latestVersion();

		verifyQueryResults(query, 1);

		result = query.singleResult();

		assertEquals("Second Test Process", result.Name);
		assertEquals(2, result.Version);

		query.processDefinitionNameLike("Second%").latestVersion();

		result = query.singleResult();

		assertEquals("Second Test Process", result.Name);
		assertEquals(2, result.Version);

		repositoryService.deleteDeployment(firstDeployment, true);
		repositoryService.deleteDeployment(secondDeployment, true);
	  }

	  public virtual void testQueryByLatestAndNameLike_NotFound()
	  {
		string firstDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/first-process.bpmn20.xml").deploy().Id;

		string secondDeployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/repository/second-process.bpmn20.xml").deploy().Id;

		ProcessDefinitionQuery query = repositoryService.createProcessDefinitionQuery();

		query.processDefinitionNameLike("First%").latestVersion();

		verifyQueryResults(query, 0);

		repositoryService.deleteDeployment(firstDeployment, true);
		repositoryService.deleteDeployment(secondDeployment, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByVersionTag()
	  public virtual void testQueryByVersionTag()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().versionTag("ver_tag_2").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={"org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml"}) public void testQueryByVersionTagLike()
	  public virtual void testQueryByVersionTagLike()
	  {
		assertEquals(1, repositoryService.createProcessDefinitionQuery().versionTagLike("ver\\_tag\\_%").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment(resources={ "org/camunda/bpm/engine/test/api/repository/failingProcessCreateOneIncident.bpmn20.xml", "org/camunda/bpm/engine/test/api/repository/VersionTagTest.testParsingVersionTag.bpmn20.xml" }) public void testQueryOrderByVersionTag()
	  public virtual void testQueryOrderByVersionTag()
	  {
		IList<ProcessDefinition> processDefinitionList = repositoryService.createProcessDefinitionQuery().versionTagLike("ver%tag%").orderByVersionTag().asc().list();

		assertEquals("ver_tag_2", processDefinitionList[1].VersionTag);
	  }

	  public virtual void testQueryByStartableInTasklist()
	  {
		assertEquals(4, repositoryService.createProcessDefinitionQuery().startableInTasklist().count());
	  }

	  public virtual void testQueryByStartableInTasklistNestedProcess()
	  {
		// given
		// startable super process
		// non-startable subprocess
		BpmnModelInstance[] nestedProcess = setupNestedProcess(false);
		string dplmntId = deployment(nestedProcess);

		// when
		ProcessDefinition actualStartable = repositoryService.createProcessDefinitionQuery().deploymentId(dplmntId).startableInTasklist().singleResult();

		ProcessDefinition actualNotStartable = repositoryService.createProcessDefinitionQuery().deploymentId(dplmntId).notStartableInTasklist().singleResult();

		// then

		assertEquals("calling", actualStartable.Key);
		assertEquals("called", actualNotStartable.Key);

		// cleanup
		repositoryService.deleteDeployment(dplmntId);
	  }

	  public virtual void testQueryByStartableInTasklistNestedProcessDeployedSecondTime()
	  {
		// given
		// startable super process & subprocess
		BpmnModelInstance[] nestedProcess = setupNestedProcess(true);
		string dplmntId1 = deployment(nestedProcess);

		// assume
		long processes = repositoryService.createProcessDefinitionQuery().deploymentId(dplmntId1).notStartableInTasklist().count();
		assertEquals(0, processes);

		// deploy second version
		// startable super process
		// non-startable subprocess
		nestedProcess = setupNestedProcess(false);
		string dplmntId2 = deployment(nestedProcess);

		// when
		ProcessDefinition startable = repositoryService.createProcessDefinitionQuery().deploymentId(dplmntId2).startableInTasklist().singleResult();
		ProcessDefinition notStartable = repositoryService.createProcessDefinitionQuery().deploymentId(dplmntId2).notStartableInTasklist().singleResult();

		// then
		assertEquals("calling", startable.Key);
		assertEquals("called", notStartable.Key);

		// cleanup
		repositoryService.deleteDeployment(dplmntId1);
		repositoryService.deleteDeployment(dplmntId2);
	  }

	  protected internal virtual BpmnModelInstance[] setupNestedProcess(bool isStartableSubprocess)
	  {
		BpmnModelInstance[] result = new BpmnModelInstance[2];
		result[0] = Bpmn.createExecutableProcess("calling").startEvent().callActivity().calledElement("called").endEvent().done();

		result[1] = Bpmn.createExecutableProcess("called").camundaStartableInTasklist(isStartableSubprocess).startEvent().userTask().endEvent().done();

		return result;
	  }
	}

}