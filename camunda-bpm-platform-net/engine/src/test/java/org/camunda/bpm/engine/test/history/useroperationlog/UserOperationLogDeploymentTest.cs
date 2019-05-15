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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{

	using EmbeddedProcessApplication = org.camunda.bpm.application.impl.EmbeddedProcessApplication;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class UserOperationLogDeploymentTest : AbstractUserOperationLogTest
	{

	  protected internal const string DEPLOYMENT_NAME = "my-deployment";
	  protected internal const string RESOURCE_NAME = "path/to/my/process.bpmn";
	  protected internal const string PROCESS_KEY = "process";


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();

		IList<Deployment> deployments = repositoryService.createDeploymentQuery().list();
		foreach (Deployment deployment in deployments)
		{
		  repositoryService.deleteDeployment(deployment.Id, true, true);
		}
	  }

	  public virtual void testCreateDeployment()
	  {
		// when
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		// then
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.DEPLOYMENT, userOperationLogEntry.EntityType);
		assertEquals(deployment.Id, userOperationLogEntry.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, userOperationLogEntry.OperationType);

		assertEquals("duplicateFilterEnabled", userOperationLogEntry.Property);
		assertNull(userOperationLogEntry.OrgValue);
		assertFalse(Convert.ToBoolean(userOperationLogEntry.NewValue));

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		assertNull(userOperationLogEntry.JobDefinitionId);
		assertNull(userOperationLogEntry.ProcessInstanceId);
		assertNull(userOperationLogEntry.ProcessDefinitionId);
		assertNull(userOperationLogEntry.ProcessDefinitionKey);
		assertNull(userOperationLogEntry.CaseInstanceId);
		assertNull(userOperationLogEntry.CaseDefinitionId);
	  }

	  public virtual void testCreateDeploymentPa()
	  {
		// given
		EmbeddedProcessApplication application = new EmbeddedProcessApplication();

		// when
		Deployment deployment = repositoryService.createDeployment(application.Reference).name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		// then
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.DEPLOYMENT, userOperationLogEntry.EntityType);
		assertEquals(deployment.Id, userOperationLogEntry.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, userOperationLogEntry.OperationType);

		assertEquals("duplicateFilterEnabled", userOperationLogEntry.Property);
		assertNull(userOperationLogEntry.OrgValue);
		assertFalse(Convert.ToBoolean(userOperationLogEntry.NewValue));

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		assertNull(userOperationLogEntry.JobDefinitionId);
		assertNull(userOperationLogEntry.ProcessInstanceId);
		assertNull(userOperationLogEntry.ProcessDefinitionId);
		assertNull(userOperationLogEntry.ProcessDefinitionKey);
		assertNull(userOperationLogEntry.CaseInstanceId);
		assertNull(userOperationLogEntry.CaseDefinitionId);
	  }

	  public virtual void testPropertyDuplicateFiltering()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		// when
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(false).deploy();

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(2, query.count());

		// (1): duplicate filter enabled property
		UserOperationLogEntry logDuplicateFilterEnabledProperty = query.property("duplicateFilterEnabled").singleResult();
		assertNotNull(logDuplicateFilterEnabledProperty);

		assertEquals(EntityTypes.DEPLOYMENT, logDuplicateFilterEnabledProperty.EntityType);
		assertEquals(deployment.Id, logDuplicateFilterEnabledProperty.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, logDuplicateFilterEnabledProperty.OperationType);

		assertEquals(USER_ID, logDuplicateFilterEnabledProperty.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logDuplicateFilterEnabledProperty.Category);

		assertEquals("duplicateFilterEnabled", logDuplicateFilterEnabledProperty.Property);
		assertNull(logDuplicateFilterEnabledProperty.OrgValue);
		assertTrue(Convert.ToBoolean(logDuplicateFilterEnabledProperty.NewValue));

		// (2): deploy changed only
		UserOperationLogEntry logDeployChangedOnlyProperty = query.property("deployChangedOnly").singleResult();
		assertNotNull(logDeployChangedOnlyProperty);

		assertEquals(EntityTypes.DEPLOYMENT, logDeployChangedOnlyProperty.EntityType);
		assertEquals(deployment.Id, logDeployChangedOnlyProperty.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, logDeployChangedOnlyProperty.OperationType);
		assertEquals(USER_ID, logDeployChangedOnlyProperty.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logDeployChangedOnlyProperty.Category);

		assertEquals("deployChangedOnly", logDeployChangedOnlyProperty.Property);
		assertNull(logDeployChangedOnlyProperty.OrgValue);
		assertFalse(Convert.ToBoolean(logDeployChangedOnlyProperty.NewValue));

		// (3): operation id
		assertEquals(logDuplicateFilterEnabledProperty.OperationId, logDeployChangedOnlyProperty.OperationId);
	  }

	  public virtual void testPropertiesDuplicateFilteringAndDeployChangedOnly()
	  {
		// given
		BpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		// when
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, model).enableDuplicateFiltering(true).deploy();

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(2, query.count());

		// (1): duplicate filter enabled property
		UserOperationLogEntry logDuplicateFilterEnabledProperty = query.property("duplicateFilterEnabled").singleResult();
		assertNotNull(logDuplicateFilterEnabledProperty);
		assertEquals(EntityTypes.DEPLOYMENT, logDuplicateFilterEnabledProperty.EntityType);
		assertEquals(deployment.Id, logDuplicateFilterEnabledProperty.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, logDuplicateFilterEnabledProperty.OperationType);
		assertEquals(USER_ID, logDuplicateFilterEnabledProperty.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logDuplicateFilterEnabledProperty.Category);

		assertEquals("duplicateFilterEnabled", logDuplicateFilterEnabledProperty.Property);
		assertNull(logDuplicateFilterEnabledProperty.OrgValue);
		assertTrue(Convert.ToBoolean(logDuplicateFilterEnabledProperty.NewValue));

		// (2): deploy changed only
		UserOperationLogEntry logDeployChangedOnlyProperty = query.property("deployChangedOnly").singleResult();
		assertNotNull(logDeployChangedOnlyProperty);

		assertEquals(EntityTypes.DEPLOYMENT, logDeployChangedOnlyProperty.EntityType);
		assertEquals(deployment.Id, logDeployChangedOnlyProperty.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, logDeployChangedOnlyProperty.OperationType);
		assertEquals(USER_ID, logDeployChangedOnlyProperty.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logDeployChangedOnlyProperty.Category);

		assertEquals("deployChangedOnly", logDeployChangedOnlyProperty.Property);
		assertNull(logDeployChangedOnlyProperty.OrgValue);
		assertTrue(Convert.ToBoolean(logDeployChangedOnlyProperty.NewValue));

		// (3): operation id
		assertEquals(logDuplicateFilterEnabledProperty.OperationId, logDeployChangedOnlyProperty.OperationId);
	  }

	  public virtual void testDeleteDeploymentCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE);

		assertEquals(1, query.count());

		// when
		repositoryService.deleteDeployment(deployment.Id, true);

		// then
		assertEquals(1, query.count());
	  }

	  public virtual void testDeleteDeploymentWithoutCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE);

		assertEquals(1, query.count());

		// when
		repositoryService.deleteDeployment(deployment.Id, false);

		// then
		assertEquals(1, query.count());
	  }

	  public virtual void testDeleteDeployment()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);

		// when
		repositoryService.deleteDeployment(deployment.Id, false);

		// then
		assertEquals(1, query.count());

		UserOperationLogEntry log = query.singleResult();
		assertNotNull(log);

		assertEquals(EntityTypes.DEPLOYMENT, log.EntityType);
		assertEquals(deployment.Id, log.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, log.OperationType);

		assertEquals("cascade", log.Property);
		assertNull(log.OrgValue);
		assertFalse(Convert.ToBoolean(log.NewValue));

		assertEquals(USER_ID, log.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, log.Category);

		assertNull(log.JobDefinitionId);
		assertNull(log.ProcessInstanceId);
		assertNull(log.ProcessDefinitionId);
		assertNull(log.ProcessDefinitionKey);
		assertNull(log.CaseInstanceId);
		assertNull(log.CaseDefinitionId);
	  }

	  public virtual void testDeleteDeploymentCascading()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);

		// when
		repositoryService.deleteDeployment(deployment.Id, true);

		// then
		assertEquals(1, query.count());

		UserOperationLogEntry log = query.singleResult();
		assertNotNull(log);

		assertEquals(EntityTypes.DEPLOYMENT, log.EntityType);
		assertEquals(deployment.Id, log.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, log.OperationType);

		assertEquals("cascade", log.Property);
		assertNull(log.OrgValue);
		assertTrue(Convert.ToBoolean(log.NewValue));

		assertEquals(USER_ID, log.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, log.Category);

		assertNull(log.JobDefinitionId);
		assertNull(log.ProcessInstanceId);
		assertNull(log.ProcessDefinitionId);
		assertNull(log.ProcessDefinitionKey);
		assertNull(log.CaseInstanceId);
		assertNull(log.CaseDefinitionId);
	  }


	  public virtual void testDeleteProcessDefinitionCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE);

		assertEquals(1, query.count());

		// when
		repositoryService.deleteProcessDefinition(procDef.Id, true);

		// then
		assertEquals(1, query.count());
	  }

	  public virtual void testDeleteProcessDefinitiontWithoutCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE);

		assertEquals(1, query.count());

		// when
		repositoryService.deleteProcessDefinition(procDef.Id);

		// then
		assertEquals(1, query.count());
	  }

	  public virtual void testDeleteProcessDefinition()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);

		// when
		repositoryService.deleteProcessDefinition(procDef.Id, false);

		// then
		assertEquals(1, query.count());

		UserOperationLogEntry log = query.singleResult();
		assertNotNull(log);

		assertEquals(EntityTypes.PROCESS_DEFINITION, log.EntityType);
		assertEquals(procDef.Id, log.ProcessDefinitionId);
		assertEquals(procDef.Key, log.ProcessDefinitionKey);
		assertEquals(deployment.Id, log.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, log.OperationType);

		assertEquals("cascade", log.Property);
		assertFalse(Convert.ToBoolean(log.OrgValue));
		assertFalse(Convert.ToBoolean(log.NewValue));

		assertEquals(USER_ID, log.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, log.Category);

		assertNull(log.JobDefinitionId);
		assertNull(log.ProcessInstanceId);
		assertNull(log.CaseInstanceId);
		assertNull(log.CaseDefinitionId);
	  }

	  public virtual void testDeleteProcessDefinitionCascading()
	  {
		// given
		Deployment deployment = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).deploy();

		ProcessDefinition procDef = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);

		// when
		repositoryService.deleteProcessDefinition(procDef.Id, true);

		// then
		assertEquals(1, query.count());

		UserOperationLogEntry log = query.singleResult();
		assertNotNull(log);

		assertEquals(EntityTypes.PROCESS_DEFINITION, log.EntityType);
		assertEquals(procDef.Id, log.ProcessDefinitionId);
		assertEquals(procDef.Key, log.ProcessDefinitionKey);
		assertEquals(deployment.Id, log.DeploymentId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, log.OperationType);

		assertEquals("cascade", log.Property);
		assertFalse(Convert.ToBoolean(log.OrgValue));
		assertTrue(Convert.ToBoolean(log.NewValue));

		assertEquals(USER_ID, log.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, log.Category);

		assertNull(log.JobDefinitionId);
		assertNull(log.ProcessInstanceId);
		assertNull(log.CaseInstanceId);
		assertNull(log.CaseDefinitionId);
	  }

	  protected internal virtual BpmnModelInstance createProcessWithServiceTask(string key)
	  {
		return Bpmn.createExecutableProcess(key).startEvent().serviceTask().camundaExpression("${true}").endEvent().done();
	  }

	}

}