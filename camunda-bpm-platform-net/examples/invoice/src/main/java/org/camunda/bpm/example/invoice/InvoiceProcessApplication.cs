using System;
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
//	import static org.camunda.bpm.engine.variable.Variables.createVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.fileValue;


	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// Process Application exposing this application's resources the process engine.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessApplication public class InvoiceProcessApplication extends org.camunda.bpm.application.impl.ServletProcessApplication
	public class InvoiceProcessApplication : ServletProcessApplication
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(InvoiceProcessApplication).FullName);

	  /// <summary>
	  /// In a @PostDeploy Hook you can interact with the process engine and access
	  /// the processes the application has deployed.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostDeploy public void startFirstProcess(org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual void startFirstProcess(ProcessEngine processEngine)
	  {
		createUsers(processEngine);

		//enable metric reporting
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		processEngineConfiguration.DbMetricsReporterActivate = true;
		processEngineConfiguration.DbMetricsReporter.ReporterId = "REPORTER";

		startProcessInstances(processEngine, "invoice", 1);
		startProcessInstances(processEngine, "invoice", null);

		//disable reporting
		processEngineConfiguration.DbMetricsReporterActivate = false;
	  }

	  public override void createDeployment(string processArchiveName, DeploymentBuilder deploymentBuilder)
	  {
		ProcessEngine processEngine = BpmPlatform.ProcessEngineService.getProcessEngine("default");

		// Hack: deploy the first version of the invoice process once before the process application
		//   is deployed the first time
		if (processEngine != null)
		{

		  RepositoryService repositoryService = processEngine.RepositoryService;

		  if (!isProcessDeployed(repositoryService, "invoice"))
		  {
			ClassLoader classLoader = ProcessApplicationClassloader;

			repositoryService.createDeployment(this.Reference).addInputStream("invoice.v1.bpmn", classLoader.getResourceAsStream("invoice.v1.bpmn")).addInputStream("invoiceBusinessDecisions.dmn", classLoader.getResourceAsStream("invoiceBusinessDecisions.dmn")).addInputStream("review-invoice.cmmn", classLoader.getResourceAsStream("review-invoice.cmmn")).deploy();
		  }
		}
	  }

	  protected internal virtual bool isProcessDeployed(RepositoryService repositoryService, string key)
	  {
		return repositoryService.createProcessDefinitionQuery().processDefinitionKey("invoice").count() > 0;
	  }

	  private void startProcessInstances(ProcessEngine processEngine, string processDefinitionKey, int? version)
	  {

		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		ProcessDefinitionQuery processDefinitionQuery = processEngine.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey);

		if (version != null)
		{
		  processDefinitionQuery.processDefinitionVersion(version);
		}
		else
		{
		  processDefinitionQuery.latestVersion();
		}

		ProcessDefinition processDefinition = processDefinitionQuery.singleResult();

		Stream invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");

		long numberOfRunningProcessInstances = processEngine.RuntimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).count();

		if (numberOfRunningProcessInstances == 0)
		{ // start three process instances

		  LOGGER.info("Start 3 instances of " + processDefinition.Name + ", version " + processDefinition.Version);
		  // process instance 1
		  processEngine.RuntimeService.startProcessInstanceById(processDefinition.Id, createVariables().putValue("creditor", "Great Pizza for Everyone Inc.").putValue("amount", 30.00d).putValue("invoiceCategory", "Travel Expenses").putValue("invoiceNumber", "GPFE-23232323").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create()));

		  IoUtil.closeSilently(invoiceInputStream);
		  invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");
		  processEngineConfiguration.DbMetricsReporter.reportNow();

		  // process instance 2
		  try
		  {
			DateTime calendar = new DateTime();
			calendar.AddDays(-14);
			ClockUtil.CurrentTime = calendar;

			ProcessInstance pi = processEngine.RuntimeService.startProcessInstanceById(processDefinition.Id, createVariables().putValue("creditor", "Bobby's Office Supplies").putValue("amount", 900.00d).putValue("invoiceCategory", "Misc").putValue("invoiceNumber", "BOS-43934").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create()));

			processEngineConfiguration.DbMetricsReporter.reportNow();
			calendar.AddDays(14);
			ClockUtil.CurrentTime = calendar;

			processEngine.IdentityService.setAuthentication("demo", Arrays.asList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));
			Task task = processEngine.TaskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
			processEngine.TaskService.claim(task.Id, "demo");
			processEngine.TaskService.complete(task.Id, createVariables().putValue("approved", true));
		  }
		  finally
		  {
			processEngineConfiguration.DbMetricsReporter.reportNow();
			ClockUtil.reset();
			processEngine.IdentityService.clearAuthentication();
		  }

		  IoUtil.closeSilently(invoiceInputStream);
		  invoiceInputStream = typeof(InvoiceProcessApplication).ClassLoader.getResourceAsStream("invoice.pdf");

		  // process instance 3
		  try
		  {
			DateTime calendar = new DateTime();
			calendar.AddDays(-5);
			ClockUtil.CurrentTime = calendar;

			ProcessInstance pi = processEngine.RuntimeService.startProcessInstanceById(processDefinition.Id, createVariables().putValue("creditor", "Papa Steve's all you can eat").putValue("amount", 10.99d).putValue("invoiceCategory", "Travel Expenses").putValue("invoiceNumber", "PSACE-5342").putValue("invoiceDocument", fileValue("invoice.pdf").file(invoiceInputStream).mimeType("application/pdf").create()));

			processEngineConfiguration.DbMetricsReporter.reportNow();
			calendar.AddDays(5);
			ClockUtil.CurrentTime = calendar;

			processEngine.IdentityService.AuthenticatedUserId = "mary";
			Task task = processEngine.TaskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
			processEngine.TaskService.createComment(null, pi.Id, "I cannot approve this invoice: the amount is missing.\n\n Could you please provide the amount?");
			processEngine.TaskService.complete(task.Id, createVariables().putValue("approved", false));
		  }
		  finally
		  {
			processEngineConfiguration.DbMetricsReporter.reportNow();
			ClockUtil.reset();
			processEngine.IdentityService.clearAuthentication();
		  }
		}
		else
		{
		  LOGGER.info("No new instances of " + processDefinition.Name + " version " + processDefinition.Version + " started, there are " + numberOfRunningProcessInstances + " instances running");
		}
	  }

	  private void createUsers(ProcessEngine processEngine)
	  {

		// create demo users
		(new DemoDataGenerator()).createUsers(processEngine);
	  }
	}

}