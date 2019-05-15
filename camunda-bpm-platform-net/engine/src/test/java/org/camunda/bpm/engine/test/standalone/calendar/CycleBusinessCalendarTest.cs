﻿using System;

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
namespace org.camunda.bpm.engine.test.standalone.calendar
{

	using CycleBusinessCalendar = org.camunda.bpm.engine.impl.calendar.CycleBusinessCalendar;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using After = org.junit.After;

	public class CycleBusinessCalendarTest : PvmTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSimpleCron() throws Exception
	  public virtual void testSimpleCron()
	  {
		CycleBusinessCalendar businessCalendar = new CycleBusinessCalendar();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
		DateTime now = simpleDateFormat.parse("2011 03 11 - 17:23");
		ClockUtil.CurrentTime = now;

		DateTime duedate = businessCalendar.resolveDuedate("0 0 0 1 * ?");

		DateTime expectedDuedate = simpleDateFormat.parse("2011 04 1 - 00:00");

		assertEquals(expectedDuedate, duedate);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSimpleDuration() throws Exception
	  public virtual void testSimpleDuration()
	  {
		CycleBusinessCalendar businessCalendar = new CycleBusinessCalendar();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
		DateTime now = simpleDateFormat.parse("2010 06 11 - 17:23");
		ClockUtil.CurrentTime = now;

		DateTime duedate = businessCalendar.resolveDuedate("R/P2DT5H70M");

		DateTime expectedDuedate = simpleDateFormat.parse("2010 06 13 - 23:33");

		assertEquals(expectedDuedate, duedate);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSimpleCronWithStartDate() throws Exception
	  public virtual void testSimpleCronWithStartDate()
	  {
		CycleBusinessCalendar businessCalendar = new CycleBusinessCalendar();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
		DateTime now = simpleDateFormat.parse("2011 03 11 - 17:23");

		DateTime duedate = businessCalendar.resolveDuedate("0 0 0 1 * ?", now);

		DateTime expectedDuedate = simpleDateFormat.parse("2011 04 1 - 00:00");

		assertEquals(expectedDuedate, duedate);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testSimpleDurationWithStartDate() throws Exception
	  public virtual void testSimpleDurationWithStartDate()
	  {
		CycleBusinessCalendar businessCalendar = new CycleBusinessCalendar();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy MM dd - HH:mm");
		DateTime now = simpleDateFormat.parse("2010 06 11 - 17:23");

		DateTime duedate = businessCalendar.resolveDuedate("R/P2DT5H70M", now);

		DateTime expectedDuedate = simpleDateFormat.parse("2010 06 13 - 23:33");

		assertEquals(expectedDuedate, duedate);
	  }

	}

}