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
namespace org.camunda.bpm.integrationtest.functional.jodatime
{

	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JodaTimeClassloadingTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JodaTimeClassloadingTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		return initWebArchiveDeployment().addAsResource("org/camunda/bpm/integrationtest/functional/jodatime/JodaTimeClassloadingTest.bpmn20.xml");
		}


	  private DateTime testExpression(string timeExpression)
	  {
		// Set the clock fixed
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["dueDate"] = timeExpression;

		// After process start, there should be timer created
		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables1);
		Assert.assertEquals(1, managementService.createJobQuery().processInstanceId(pi1.Id).count());

		IList<Job> jobs = managementService.createJobQuery().executable().list();
		Assert.assertEquals(1, jobs.Count);
		runtimeService.deleteProcessInstance(pi1.Id, "test");

		return jobs[0].Duedate;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionComplete() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionComplete()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dt));
		Assert.assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dt), (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).format(dueDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionWithoutSeconds() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionWithoutSeconds()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dt));
		Assert.assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dt), (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm")).format(dueDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionWithoutMinutes() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionWithoutMinutes()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(DateTime.Now));
		Assert.assertEquals((new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(dt), (new SimpleDateFormat("yyyy-MM-dd'T'HH")).format(dueDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionWithoutTime() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionWithoutTime()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM-dd")).format(DateTime.Now));
		Assert.assertEquals((new SimpleDateFormat("yyyy-MM-dd")).format(dt), (new SimpleDateFormat("yyyy-MM-dd")).format(dueDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionWithoutDay() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionWithoutDay()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy-MM")).format(DateTime.Now));
		Assert.assertEquals((new SimpleDateFormat("yyyy-MM")).format(dt), (new SimpleDateFormat("yyyy-MM")).format(dueDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimeExpressionWithoutMonth() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTimeExpressionWithoutMonth()
	  {
		DateTime dt = DateTime.Now;

		DateTime dueDate = testExpression((new SimpleDateFormat("yyyy")).format(DateTime.Now));
		Assert.assertEquals((new SimpleDateFormat("yyyy")).format(dt), (new SimpleDateFormat("yyyy")).format(dueDate));
	  }

	}

}