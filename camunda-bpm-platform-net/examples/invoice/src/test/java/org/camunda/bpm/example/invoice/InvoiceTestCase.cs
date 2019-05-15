using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.example.invoice
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.fileValue;


	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ProcessEngineTestCase = org.camunda.bpm.engine.test.ProcessEngineTestCase;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	public class InvoiceTestCase : ProcessEngineTestCase
	{

	  [Deployment(resources: {"invoice.v1.bpmn", "invoiceBusinessDecisions.dmn"})]
	  public virtual void testHappyPathV1()
	  {
		Stream invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");
		VariableMap variables = Variables.createVariables().putValue("creditor", "Great Pizza for Everyone Inc.").putValue("amount", 300.0d).putValue("invoiceCategory", "Travel Expenses").putValue("invoiceNumber", "GPFE-23232323").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create());

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("invoice", variables);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("approveInvoice", task.TaskDefinitionKey);

		IList<IdentityLink> links = taskService.getIdentityLinksForTask(task.Id);
		ISet<string> approverGroups = new HashSet<string>();
		foreach (IdentityLink link in links)
		{
		  approverGroups.Add(link.GroupId);
		}
		assertEquals(2, approverGroups.Count);
		assertTrue(approverGroups.Contains("accounting"));
		assertTrue(approverGroups.Contains("sales"));

		variables.clear();
		variables.put("approved", true);
		taskService.complete(task.Id, variables);

		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();

		assertEquals("prepareBankTransfer", task.TaskDefinitionKey);
		taskService.complete(task.Id);

		Job archiveInvoiceJob = managementService.createJobQuery().singleResult();
		assertNotNull(archiveInvoiceJob);
		managementService.executeJob(archiveInvoiceJob.Id);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources: {"invoice.v2.bpmn", "invoiceBusinessDecisions.dmn"})]
	  public virtual void testHappyPathV2()
	  {
		Stream invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");
		VariableMap variables = Variables.createVariables().putValue("creditor", "Great Pizza for Everyone Inc.").putValue("amount", 300.0d).putValue("invoiceCategory", "Travel Expenses").putValue("invoiceNumber", "GPFE-23232323").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create());

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("invoice", variables);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("approveInvoice", task.TaskDefinitionKey);

		IList<IdentityLink> links = taskService.getIdentityLinksForTask(task.Id);
		ISet<string> approverGroups = new HashSet<string>();
		foreach (IdentityLink link in links)
		{
		  approverGroups.Add(link.GroupId);
		}
		assertEquals(2, approverGroups.Count);
		assertTrue(approverGroups.Contains("accounting"));
		assertTrue(approverGroups.Contains("sales"));

		variables.clear();
		variables.put("approved", true);
		taskService.complete(task.Id, variables);

		task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();

		assertEquals("prepareBankTransfer", task.TaskDefinitionKey);
		taskService.complete(task.Id);

		Job archiveInvoiceJob = managementService.createJobQuery().singleResult();
		assertNotNull(archiveInvoiceJob);
		managementService.executeJob(archiveInvoiceJob.Id);

		assertProcessEnded(pi.Id);
	  }

	  [Deployment(resources: {"invoice.v2.bpmn", "invoiceBusinessDecisions.dmn"})]
	  public virtual void testApproveInvoiceAssignment()
	  {
		Stream invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");

		VariableMap variables = Variables.createVariables().putValue("creditor", "Great Pizza for Everyone Inc.").putValue("amount", 300.0d).putValue("invoiceCategory", "Travel Expenses").putValue("invoiceNumber", "GPFE-23232323").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create()).putValue("approverGroups", Arrays.asList("sales", "accounting"));

		ProcessInstance pi = runtimeService.createProcessInstanceByKey("invoice").setVariables(variables).startBeforeActivity("approveInvoice").execute();

		// givent that the process instance is waiting at task "approveInvoice"
		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		assertEquals("approveInvoice", task.TaskDefinitionKey);

		// and task has candidate groups
		IList<IdentityLink> links = taskService.getIdentityLinksForTask(task.Id);
		ISet<string> approverGroups = new HashSet<string>();
		foreach (IdentityLink link in links)
		{
		  approverGroups.Add(link.GroupId);
		}
		assertEquals(2, approverGroups.Count);
		assertTrue(approverGroups.Contains("accounting"));
		assertTrue(approverGroups.Contains("sales"));

		// and variable approver is null
		assertNull(taskService.getVariable(task.Id, "approver"));

		// if mary claims the task
		taskService.claim(task.Id, "mary");

		// then the variable "approver" exists and is set to mary
		assertEquals("mary", taskService.getVariable(task.Id, "approver"));

	  }

	}

}