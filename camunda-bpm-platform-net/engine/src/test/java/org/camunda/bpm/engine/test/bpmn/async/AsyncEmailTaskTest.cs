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
namespace org.camunda.bpm.engine.test.bpmn.async
{

	using EmailServiceTaskTest = org.camunda.bpm.engine.test.bpmn.mail.EmailServiceTaskTest;
	using EmailTestCase = org.camunda.bpm.engine.test.bpmn.mail.EmailTestCase;
	using WiserMessage = org.subethamail.wiser.WiserMessage;

	/// 
	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class AsyncEmailTaskTest : EmailTestCase
	{

	  // copied from org.camunda.bpm.engine.test.bpmn.mail.EmailServiceTaskTest
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleTextMail() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSimpleTextMail()
	  {
		string procId = runtimeService.startProcessInstanceByKey("simpleTextOnly").Id;

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(0, messages.Count);

		waitForJobExecutorToProcessAllJobs(5000L);

		messages = wiser.Messages;
		assertEquals(1, messages.Count);

		WiserMessage message = messages[0];
		EmailServiceTaskTest.assertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", Arrays.asList("kermit@camunda.org"), null);
		assertProcessEnded(procId);
	  }

	  // copied from org.camunda.bpm.engine.test.bpmn.mail.EmailSendTaskTest
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleTextMailSendTask() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSimpleTextMailSendTask()
	  {
		runtimeService.startProcessInstanceByKey("simpleTextOnly");

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(0, messages.Count);

		waitForJobExecutorToProcessAllJobs(5000L);

		messages = wiser.Messages;
		assertEquals(1, messages.Count);

		WiserMessage message = messages[0];
		EmailServiceTaskTest.assertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", Arrays.asList("kermit@camunda.org"), null);
	  }

	}

}