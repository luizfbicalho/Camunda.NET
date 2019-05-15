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
namespace org.camunda.bpm.engine.test.standalone.authentication
{

	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using User = org.camunda.bpm.engine.identity.User;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Test = org.junit.Test;

	public class LoginAttemptsTest : ResourceProcessEngineTestCase
	{

	  private static SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

	  public LoginAttemptsTest() : base("org/camunda/bpm/engine/test/standalone/authentication/camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUsuccessfulAttemptsResultInException() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testUsuccessfulAttemptsResultInException()
	  {
		User user = identityService.newUser("johndoe");
		user.Password = "xxx";
		identityService.saveUser(user);

		DateTime now = sdf.parse("2000-01-24T13:00:00");
		ClockUtil.CurrentTime = now;
		try
		{
		  for (int i = 0; i <= 6; i++)
		  {
			assertFalse(identityService.checkPassword("johndoe", "invalid pwd"));
			now = DateUtils.addSeconds(now, 5);
			ClockUtil.CurrentTime = now;
		  }
		  fail("expected exception");
		}
		catch (AuthenticationException e)
		{
		  assertTrue(e.Message.contains("The user with id 'johndoe' is locked."));
		}

		identityService.deleteUser(user.Id);
	  }
	}

}