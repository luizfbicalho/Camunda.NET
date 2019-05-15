using System;

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
namespace org.camunda.bpm.engine.test.bpmn.async
{
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class JobRetryCmdWithDefaultPropertyTest : ResourceProcessEngineTestCase
	{

	  public JobRetryCmdWithDefaultPropertyTest() : base("org/camunda/bpm/engine/test/bpmn/async/default.job.retry.property.camunda.cfg.xml")
	  {
	  }

	  /// <summary>
	  /// Check if property "DefaultNumberOfRetries" will be used
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedTask.bpmn20.xml" })]
	  public virtual void testDefaultNumberOfRetryProperty()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedTask");
		assertNotNull(pi);

		Job job = managementService.createJobQuery().processInstanceId(pi.ProcessInstanceId).singleResult();
		assertNotNull(job);
		assertEquals(pi.ProcessInstanceId, job.ProcessInstanceId);
		assertEquals(2, job.Retries);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" })]
	  public virtual void testOverwritingPropertyWithBpmnExtension()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedServiceTask");
		assertNotNull(pi);

		Job job = managementService.createJobQuery().processInstanceId(pi.ProcessInstanceId).singleResult();
		assertNotNull(job);
		assertEquals(pi.ProcessInstanceId, job.ProcessInstanceId);

		try
		{
		  managementService.executeJob(job.Id);
		  fail("Exception expected!");
		}
		catch (Exception)
		{
		  // expected
		}

		job = managementService.createJobQuery().jobId(job.Id).singleResult();
		assertEquals(4, job.Retries);

	  }
	}

}