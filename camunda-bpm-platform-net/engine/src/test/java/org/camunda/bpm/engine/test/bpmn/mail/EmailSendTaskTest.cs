using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.bpmn.mail
{


	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using WiserMessage = org.subethamail.wiser.WiserMessage;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public class EmailSendTaskTest : EmailTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleTextMail() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSimpleTextMail()
	  {
		runtimeService.startProcessInstanceByKey("simpleTextOnly");

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(1, messages.Count);

		WiserMessage message = messages[0];
		assertEmailSend(message, false, "Hello Kermit!", "This a text only e-mail.", "camunda@localhost", Arrays.asList("kermit@camunda.org"), null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleTextMailMultipleRecipients()
	  public virtual void testSimpleTextMailMultipleRecipients()
	  {
		runtimeService.startProcessInstanceByKey("simpleTextOnlyMultipleRecipients");

		// 3 recipients == 3 emails in wiser with different receivers
		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(3, messages.Count);

		// sort recipients for easy assertion
		IList<string> recipients = new List<string>();
		foreach (WiserMessage message in messages)
		{
		  recipients.Add(message.EnvelopeReceiver);
		}
		recipients.Sort();

		assertEquals("fozzie@camunda.org", recipients[0]);
		assertEquals("kermit@camunda.org", recipients[1]);
		assertEquals("mispiggy@camunda.org", recipients[2]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTextMailExpressions() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTextMailExpressions()
	  {

		string sender = "mispiggy@activiti.org";
		string recipient = "fozziebear@activiti.org";
		string recipientName = "Mr. Fozzie";
		string subject = "Fozzie, you should see this!";

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sender"] = sender;
		vars["recipient"] = recipient;
		vars["recipientName"] = recipientName;
		vars["subject"] = subject;

		runtimeService.startProcessInstanceByKey("textMailExpressions", vars);

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(1, messages.Count);

		WiserMessage message = messages[0];
		assertEmailSend(message, false, subject, "Hello " + recipientName + ", this is an e-mail", sender, Arrays.asList(recipient), null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCcAndBcc() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCcAndBcc()
	  {
		runtimeService.startProcessInstanceByKey("ccAndBcc");

		IList<WiserMessage> messages = wiser.Messages;
		assertEmailSend(messages[0], false, "Hello world", "This is the content", "camunda@localhost", Arrays.asList("kermit@camunda.org"), Arrays.asList("fozzie@camunda.org"));

		// Bcc is not stored in the header (obviously)
		// so the only way to verify the bcc, is that there are three messages send.
		assertEquals(3, messages.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testHtmlMail() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHtmlMail()
	  {
		runtimeService.startProcessInstanceByKey("htmlMail", CollectionUtil.singletonMap("gender", "male"));

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(1, messages.Count);
		assertEmailSend(messages[0], true, "Test", "Mr. <b>Kermit</b>", "camunda@localhost", Arrays.asList("kermit@camunda.org"), null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSendEmail() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSendEmail()
	  {

		string from = "ordershipping@activiti.org";
		bool male = true;
		string recipientName = "John Doe";
		string recipient = "johndoe@alfresco.com";
		DateTime now = DateTime.Now;
		string orderId = "123456";

		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["sender"] = from;
		vars["recipient"] = recipient;
		vars["recipientName"] = recipientName;
		vars["male"] = male;
		vars["now"] = now;
		vars["orderId"] = orderId;

		runtimeService.startProcessInstanceByKey("sendMailExample", vars);

		IList<WiserMessage> messages = wiser.Messages;
		assertEquals(1, messages.Count);

		WiserMessage message = messages[0];
		MimeMessage mimeMessage = message.MimeMessage;

		assertEquals("Your order " + orderId + " has been shipped", mimeMessage.getHeader("Subject", null));
		assertEquals(from, mimeMessage.getHeader("From", null));
		assertTrue(mimeMessage.getHeader("To", null).contains(recipient));
	  }

	  // Helper

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assertEmailSend(org.subethamail.wiser.WiserMessage emailMessage, boolean htmlMail, String subject, String message, String from, java.util.List<String> to, java.util.List<String> cc) throws java.io.IOException
	  private void assertEmailSend(WiserMessage emailMessage, bool htmlMail, string subject, string message, string from, IList<string> to, IList<string> cc)
	  {
		try
		{
		  MimeMessage mimeMessage = emailMessage.MimeMessage;

		  if (htmlMail)
		  {
			assertTrue(mimeMessage.ContentType.contains("multipart/mixed"));
		  }
		  else
		  {
			assertTrue(mimeMessage.ContentType.contains("text/plain"));
		  }

		  assertEquals(subject, mimeMessage.getHeader("Subject", null));
		  assertEquals(from, mimeMessage.getHeader("From", null));
		  assertTrue(getMessage(mimeMessage).Contains(message));

		  foreach (string t in to)
		  {
			assertTrue(mimeMessage.getHeader("To", null).contains(t));
		  }

		  if (cc != null)
		  {
			foreach (string c in cc)
			{
			  assertTrue(mimeMessage.getHeader("Cc", null).contains(c));
			}
		  }

		}
		catch (MessagingException e)
		{
		  fail(e.Message);
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String getMessage(javax.mail.internet.MimeMessage mimeMessage) throws javax.mail.MessagingException, java.io.IOException
	  protected internal virtual string getMessage(MimeMessage mimeMessage)
	  {
		DataHandler dataHandler = mimeMessage.DataHandler;
		MemoryStream baos = new MemoryStream();
		dataHandler.writeTo(baos);
		baos.Flush();
		return baos.ToString();
	  }

	}

}