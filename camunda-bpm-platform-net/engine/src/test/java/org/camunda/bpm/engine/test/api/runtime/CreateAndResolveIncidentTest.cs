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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using IncidentHandler = org.camunda.bpm.engine.impl.incident.IncidentHandler;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;

	public class CreateAndResolveIncidentTest
	{
		private bool InstanceFieldsInitialized = false;

		public CreateAndResolveIncidentTest()
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
			ruleChain = RuleChain.outerRule(processEngineBootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule processEngineBootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.CustomIncidentHandlers = Arrays.asList((IncidentHandler) new CustomIncidentHandler());
			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(processEngineBootstrapRule);
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineBootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createIncident()
	  public virtual void createIncident()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		// when
		Incident incident = runtimeService.createIncident("foo", processInstance.Id, "aa", "bar");

		// then
		Incident incident2 = runtimeService.createIncidentQuery().executionId(processInstance.Id).singleResult();
		assertEquals(incident2.Id, incident.Id);
		assertEquals("foo", incident2.IncidentType);
		assertEquals("aa", incident2.Configuration);
		assertEquals("bar", incident2.IncidentMessage);
		assertEquals(processInstance.Id, incident2.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createIncidentWithNullExecution()
	  public virtual void createIncidentWithNullExecution()
	  {

		try
		{
		  runtimeService.createIncident("foo", null, "userTask1", "bar");
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Execution id cannot be null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createIncidentWithNullIncidentType()
	  public virtual void createIncidentWithNullIncidentType()
	  {
		try
		{
		  runtimeService.createIncident(null, "processInstanceId", "foo", "bar");
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("incidentType is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createIncidentWithNonExistingExecution()
	  public virtual void createIncidentWithNonExistingExecution()
	  {

		try
		{
		  runtimeService.createIncident("foo", "aaa", "bbb", "bar");
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot find an execution with executionId 'aaa'"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveIncident()
	  public virtual void resolveIncident()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		Incident incident = runtimeService.createIncident("foo", processInstance.Id, "userTask1", "bar");

		// when
		runtimeService.resolveIncident(incident.Id);

		// then
		Incident incident2 = runtimeService.createIncidentQuery().executionId(processInstance.Id).singleResult();
		assertNull(incident2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveUnexistingIncident()
	  public virtual void resolveUnexistingIncident()
	  {
		try
		{
		  runtimeService.resolveIncident("foo");
		  fail("Exception expected");
		}
		catch (NotFoundException e)
		{
		  assertThat(e.Message, containsString("Cannot find an incident with id 'foo'"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveNullIncident()
	  public virtual void resolveNullIncident()
	  {
		try
		{
		  runtimeService.resolveIncident(null);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("incidentId is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveIncidentOfTypeFailedJob()
	  public virtual void resolveIncidentOfTypeFailedJob()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		// when
		IList<Job> jobs = engineRule.ManagementService.createJobQuery().withRetriesLeft().list();

		foreach (Job job in jobs)
		{
		  engineRule.ManagementService.setJobRetries(job.Id, 1);
		  try
		  {
			engineRule.ManagementService.executeJob(job.Id);
		  }
		  catch (Exception)
		  {
		  }
		}

		// then
		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();
		try
		{
		  runtimeService.resolveIncident(incident.Id);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot resolve an incident of type failedJob"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createIncidentWithIncidentHandler()
	  public virtual void createIncidentWithIncidentHandler()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		// when
		Incident incident = runtimeService.createIncident("custom", processInstance.Id, "configuration");

		// then
		assertNotNull(incident);

		Incident incident2 = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident2);
		assertEquals(incident, incident2);
		assertEquals("custom", incident.IncidentType);
		assertEquals("configuration", incident.Configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resolveIncidentWithIncidentHandler()
	  public virtual void resolveIncidentWithIncidentHandler()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.createIncident("custom", processInstance.Id, "configuration");
		Incident incident = runtimeService.createIncidentQuery().singleResult();

		// when
		runtimeService.resolveIncident(incident.Id);

		// then
		incident = runtimeService.createIncidentQuery().singleResult();
		assertNull(incident);
	  }

	  public class CustomIncidentHandler : IncidentHandler
	  {

		internal string incidentType = "custom";

		public virtual string IncidentHandlerType
		{
			get
			{
			  return incidentType;
			}
		}

		public virtual Incident handleIncident(IncidentContext context, string message)
		{
		  return IncidentEntity.createAndInsertIncident(incidentType, context, message);
		}

		public virtual void resolveIncident(IncidentContext context)
		{
		  deleteIncident(context);
		}

		public virtual void deleteIncident(IncidentContext context)
		{
		  IList<Incident> incidents = Context.CommandContext.IncidentManager.findIncidentByConfigurationAndIncidentType(context.Configuration, incidentType);
		  ((IncidentEntity) incidents[0]).delete();
		}

	  }
	}

}