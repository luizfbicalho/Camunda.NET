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
namespace org.camunda.bpm.engine.test.cmmn.humantask
{

	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using Period = org.joda.time.Period;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HumanTaskFollowUpDateTest : CmmnProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/cmmn/humantask/HumanTaskFollowUpDateTest.testHumanTaskFollowUpDate.cmmn"}) public void testHumanTaskFollowUpDateExtension() throws Exception
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/humantask/HumanTaskFollowUpDateTest.testHumanTaskFollowUpDate.cmmn"})]
	  public virtual void testHumanTaskFollowUpDateExtension()
	  {

		DateTime date = (new SimpleDateFormat("dd-MM-yyyy hh:mm:ss")).parse("01-01-2015 12:10:00");
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = date;

		string caseInstanceId = caseService.createCaseInstanceByKey("case", variables).Id;

		Task task = taskService.createTaskQuery().caseInstanceId(caseInstanceId).singleResult();

		assertNotNull(task.FollowUpDate);
		assertEquals(date, task.FollowUpDate);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/cmmn/humantask/HumanTaskFollowUpDateTest.testHumanTaskFollowUpDate.cmmn"}) public void testHumanTaskFollowUpDateStringExtension() throws Exception
	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/humantask/HumanTaskFollowUpDateTest.testHumanTaskFollowUpDate.cmmn"})]
	  public virtual void testHumanTaskFollowUpDateStringExtension()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = "2015-01-01T12:10:00";

		string caseInstanceId = caseService.createCaseInstanceByKey("case", variables).Id;

		Task task = taskService.createTaskQuery().caseInstanceId(caseInstanceId).singleResult();

		assertNotNull(task.FollowUpDate);
		DateTime date = (new SimpleDateFormat("dd-MM-yyyy HH:mm:ss")).parse("01-01-2015 12:10:00");
		assertEquals(date, task.FollowUpDate);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/humantask/HumanTaskFollowUpDateTest.testHumanTaskFollowUpDate.cmmn"})]
	  public virtual void testHumanTaskRelativeFollowUpDate()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = "P2DT2H30M";

		string caseInstanceId = caseService.createCaseInstanceByKey("case", variables).Id;

		Task task = taskService.createTaskQuery().caseInstanceId(caseInstanceId).singleResult();

		DateTime followUpDate = task.FollowUpDate;
		assertNotNull(followUpDate);

		Period period = new Period(task.CreateTime.Ticks, followUpDate.Ticks);
		assertEquals(period.Days, 2);
		assertEquals(period.Hours, 2);
		assertEquals(period.Minutes, 30);
	  }

	}

}