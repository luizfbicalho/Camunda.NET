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
namespace org.camunda.bpm.engine.test.bpmn.@event.timer
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	/// <summary>
	/// Test timer expression according to act-865
	/// 
	/// @author Saeid Mirzaei
	/// </summary>

	public class TimeExpressionTest : PluggableProcessEngineTestCase
	{


		  private DateTime testExpression(string timeExpression)
		  {
				// Set the clock fixed
				Dictionary<string, object> variables1 = new Dictionary<string, object>();
				variables1["dueDate"] = timeExpression;

				// After process start, there should be timer created    
				ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables1);
				assertEquals(1, managementService.createJobQuery().processInstanceId(pi1.Id).count());


				IList<Job> jobs = managementService.createJobQuery().executable().list();
				assertEquals(1, jobs.Count);
				return jobs[0].Duedate;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionComplete() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionComplete()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dt));
				assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dt),(new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dueDate));
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionWithoutSeconds() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionWithoutSeconds()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dt));
				assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dt),(new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dueDate));
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionWithoutMinutes() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionWithoutMinutes()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(DateTime.Now));
				assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(dt),(new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(dueDate));
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionWithoutTime() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionWithoutTime()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd")).format(DateTime.Now));
				assertEquals((new SimpleDateFormat("yyyy-MM-dd")).format(dt),(new SimpleDateFormat("yyyy-MM-dd")).format(dueDate));
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionWithoutDay() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionWithoutDay()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM")).format(DateTime.Now));
				assertEquals((new SimpleDateFormat("yyyy-MM")).format(dt),(new SimpleDateFormat("yyyy-MM")).format(dueDate));
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"}) public void testTimeExpressionWithoutMonth() throws Exception
		  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml"})]
		  public virtual void testTimeExpressionWithoutMonth()
		  {
				DateTime dt = DateTime.Now;

				DateTime dueDate = testExpression((new SimpleDateFormat("yyyy")).format(DateTime.Now));
				assertEquals((new SimpleDateFormat("yyyy")).format(dt),(new SimpleDateFormat("yyyy")).format(dueDate));
		  }
	}

}