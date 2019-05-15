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
namespace org.camunda.bpm.engine.test.standalone.calendar
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using DurationHelper = org.camunda.bpm.engine.impl.calendar.DurationHelper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using After = org.junit.After;
	using Test = org.junit.Test;

	public class DurationHelperTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumber() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumber()
	  {
		ClockUtil.CurrentTime = new DateTime();
		DurationHelper dh = new DurationHelper("R2/PT10S");

		ClockUtil.CurrentTime = new DateTime(15000);
		assertEquals(20000, dh.DateAfter.Ticks);


		ClockUtil.CurrentTime = new DateTime(30000);
		assertNull(dh.DateAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumberPeriods() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumberPeriods()
	  {
		ClockUtil.CurrentTime = parse("19700101-00:00:00");
		DurationHelper dh = new DurationHelper("R2/1970-01-01T00:00:00/1970-01-01T00:00:10");

		ClockUtil.CurrentTime = parse("19700101-00:00:15");
		assertEquals(parse("19700101-00:00:20"), dh.DateAfter);


		ClockUtil.CurrentTime = parse("19700101-00:00:30");
		assertNull(dh.DateAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumberNegative() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumberNegative()
	  {
		ClockUtil.CurrentTime = parse("19700101-00:00:00");
		DurationHelper dh = new DurationHelper("R2/PT10S/1970-01-01T00:00:50");

		ClockUtil.CurrentTime = parse("19700101-00:00:20");
		assertEquals(parse("19700101-00:00:30"), dh.DateAfter);


		ClockUtil.CurrentTime = parse("19700101-00:00:35");

		assertEquals(parse("19700101-00:00:40"), dh.DateAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumberWithStartDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumberWithStartDate()
	  {
		DurationHelper dh = new DurationHelper("R2/PT10S", new DateTime());
		assertEquals(20000, dh.getDateAfter(new DateTime(15000)).Ticks);
		assertNull(dh.getDateAfter(new DateTime(30000)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumberPeriodsWithStartDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumberPeriodsWithStartDate()
	  {
		DurationHelper dh = new DurationHelper("R2/1970-01-01T00:00:00/1970-01-01T00:00:10", parse("19700101-00:00:00"));

		assertEquals(parse("19700101-00:00:20"), dh.getDateAfter(parse("19700101-00:00:15")));
		assertNull(dh.getDateAfter(parse("19700101-00:00:30")));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotExceedNumberNegativeWithStartDate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotExceedNumberNegativeWithStartDate()
	  {
		DurationHelper dh = new DurationHelper("R2/PT10S/1970-01-01T00:00:50", parse("19700101-00:00:00"));

		assertEquals(parse("19700101-00:00:30"), dh.getDateAfter(parse("19700101-00:00:20")));

		assertEquals(parse("19700101-00:00:40"), dh.getDateAfter(parse("19700101-00:00:35")));
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.Date parse(String str) throws Exception
	  private DateTime parse(string str)
	  {
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyyMMdd-HH:mm:ss");
		return simpleDateFormat.parse(str);
	  }


	}

}