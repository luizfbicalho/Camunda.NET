﻿/*
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
namespace org.camunda.bpm.container.impl.deployment.jobexecutor
{
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;

	/// <summary>
	/// <para>Deployment operation step responsible for starting the JobExecutor</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartJobExecutorStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Starting the Managed Job Executor";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml jobExecutorXml = getJobExecutorXml(operationContext);
		JobExecutorXml jobExecutorXml = getJobExecutorXml(operationContext);

		// add a deployment operation step for each job acquisition
		foreach (JobAcquisitionXml jobAcquisitionXml in jobExecutorXml.JobAcquisitions)
		{
		  operationContext.addStep(new StartJobAcquisitionStep(jobAcquisitionXml));
		}

	  }

	  private JobExecutorXml getJobExecutorXml(DeploymentOperation operationContext)
	  {
		BpmPlatformXml bpmPlatformXml = operationContext.getAttachment(Attachments.BPM_PLATFORM_XML);
		JobExecutorXml jobExecutorXml = bpmPlatformXml.JobExecutor;
		return jobExecutorXml;
	  }



	}

}