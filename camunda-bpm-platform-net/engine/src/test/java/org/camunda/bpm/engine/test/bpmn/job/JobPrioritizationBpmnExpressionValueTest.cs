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
namespace org.camunda.bpm.engine.test.bpmn.job
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobPrioritizationBpmnExpressionValueTest : PluggableProcessEngineTestCase
	{

	  protected internal const long EXPECTED_DEFAULT_PRIORITY = 123;
	  protected internal const long EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = 296;

	  protected internal long originalDefaultPriority;
	  protected internal long originalDefaultPriorityOnFailure;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		originalDefaultPriority = DefaultJobPriorityProvider.DEFAULT_PRIORITY;
		originalDefaultPriorityOnFailure = DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE;

		DefaultJobPriorityProvider.DEFAULT_PRIORITY = EXPECTED_DEFAULT_PRIORITY;
		DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		// reset default priorities
		DefaultJobPriorityProvider.DEFAULT_PRIORITY = originalDefaultPriority;
		DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = originalDefaultPriorityOnFailure;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testConstantValueExpressionPrioritization()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task2").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(15, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testConstantValueHashExpressionPrioritization()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task4").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(16, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testVariableValueExpressionPrioritization()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task1").setVariable("priority", 22).execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(22, job.Priority);
	  }

	  /// <summary>
	  /// Can't distinguish this case from the cases we have to tolerate due to CAM-4207
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void FAILING_testVariableValueExpressionPrioritizationFailsWhenVariableMisses()
	  {
		// when
		try
		{
		  runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task1").execute();
		  fail("this should not succeed since the priority variable is not defined");
		}
		catch (ProcessEngineException e)
		{

		  assertTextPresentIgnoreCase("Unknown property used in expression: ${priority}. " + "Cause: Cannot resolve identifier 'priority'", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testExecutionExpressionPrioritization()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task1").setVariable("priority", 25).execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(25, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testExpressionEvaluatesToNull()
	  {
		// when
		try
		{
		  runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task3").setVariable("priority", null).execute();
		  fail("this should not succeed since the priority variable is not defined");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("Priority value is not an Integer", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testExpressionEvaluatesToNonNumericalValue()
	  {
		// when
		try
		{
		  runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task3").setVariable("priority", "aNonNumericalVariableValue").execute();
		  fail("this should not succeed since the priority must be integer");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("Priority value is not an Integer", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testExpressionEvaluatesToNonIntegerValue()
	  {
		// when
		try
		{
		  runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task3").setVariable("priority", 4.2d).execute();
		  fail("this should not succeed since the priority must be integer");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("Priority value must be either Short, Integer, or Long", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testConcurrentLocalVariablesAreAccessible()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task2").startBeforeActivity("task1").setVariableLocal("priority", 14).execute();

		// then
		Job job = managementService.createJobQuery().activityId("task1").singleResult();
		assertNotNull(job);
		assertEquals(14, job.Priority);
	  }

	  /// <summary>
	  /// This test case asserts that a non-resolving expression does not fail job creation;
	  /// This is a unit test scenario, where simply the variable misses (in general a human-made error), but
	  /// the actual case covered by the behavior are missing beans (e.g. in the case the engine can't perform a
	  /// context switch)
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testDefaultPriorityWhenBeanMisses()
	  {
		// creating a job with a priority that can't be resolved does not fail entirely but uses a default priority
		runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task1").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertEquals(EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioExpressionProcess.bpmn20.xml")]
	  public virtual void testDisableGracefulDegradation()
	  {
		try
		{
		  processEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure = false;

		  try
		  {
			runtimeService.createProcessInstanceByKey("jobPrioExpressionProcess").startBeforeActivity("task1").execute();
			fail("should not succeed due to missing variable");
		  }
		  catch (ProcessEngineException e)
		  {
			assertTextPresentIgnoreCase("unknown property used in expression", e.Message);
		  }

		}
		finally
		{
		  processEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure = true;
		}
	  }

	  public virtual void testDefaultEngineConfigurationSetting()
	  {
		ProcessEngineConfigurationImpl config = new StandaloneInMemProcessEngineConfiguration();

		assertTrue(config.EnableGracefulDegradationOnContextSwitchFailure);
	  }

	}

}