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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using Email = org.apache.commons.mail.Email;
	using EmailException = org.apache.commons.mail.EmailException;
	using HtmlEmail = org.apache.commons.mail.HtmlEmail;
	using SimpleEmail = org.apache.commons.mail.SimpleEmail;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public class MailActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal Expression to;
	  protected internal Expression from;
	  protected internal Expression cc;
	  protected internal Expression bcc;
	  protected internal Expression subject;
	  protected internal Expression text;
	  protected internal Expression html;
	  protected internal Expression charset;

	  public override void execute(ActivityExecution execution)
	  {
		string toStr = getStringFromField(to, execution);
		string fromStr = getStringFromField(from, execution);
		string ccStr = getStringFromField(cc, execution);
		string bccStr = getStringFromField(bcc, execution);
		string subjectStr = getStringFromField(subject, execution);
		string textStr = getStringFromField(text, execution);
		string htmlStr = getStringFromField(html, execution);
		string charSetStr = getStringFromField(charset, execution);

		Email email = createEmail(textStr, htmlStr);

		addTo(email, toStr);
		setFrom(email, fromStr);
		addCc(email, ccStr);
		addBcc(email, bccStr);
		setSubject(email, subjectStr);
		MailServerProperties = email;
		setCharset(email, charSetStr);

		try
		{
		  email.send();
		}
		catch (EmailException e)
		{
		  throw LOG.sendingEmailException(toStr, e);
		}
		leave(execution);
	  }

	  protected internal virtual Email createEmail(string text, string html)
	  {
		if (!string.ReferenceEquals(html, null))
		{
		  return createHtmlEmail(text, html);
		}
		else if (!string.ReferenceEquals(text, null))
		{
		  return createTextOnlyEmail(text);
		}
		else
		{
		  throw LOG.emailFormatException();
		}
	  }

	  protected internal virtual HtmlEmail createHtmlEmail(string text, string html)
	  {
		HtmlEmail email = new HtmlEmail();
		try
		{
		  email.HtmlMsg = html;
		  if (!string.ReferenceEquals(text, null))
		  { // for email clients that don't support html
			email.TextMsg = text;
		  }
		  return email;
		}
		catch (EmailException e)
		{
		  throw LOG.emailCreationException("HTML", e);
		}
	  }

	  protected internal virtual SimpleEmail createTextOnlyEmail(string text)
	  {
		SimpleEmail email = new SimpleEmail();
		try
		{
		  email.Msg = text;
		  return email;
		}
		catch (EmailException e)
		{
		  throw LOG.emailCreationException("text-only", e);
		}
	  }

	  protected internal virtual void addTo(Email email, string to)
	  {
		string[] tos = splitAndTrim(to);
		if (tos != null)
		{
		  foreach (string t in tos)
		  {
			try
			{
			  email.addTo(t);
			}
			catch (EmailException e)
			{
			  throw LOG.addRecipientException(t, e);
			}
		  }
		}
		else
		{
		  throw LOG.missingRecipientsException();
		}
	  }

	  protected internal virtual void setFrom(Email email, string from)
	  {
		string fromAddress = null;

		if (!string.ReferenceEquals(from, null))
		{
		  fromAddress = from;
		}
		else
		{ // use default configured from address in process engine config
		  fromAddress = Context.ProcessEngineConfiguration.MailServerDefaultFrom;
		}

		try
		{
		  email.From = fromAddress;
		}
		catch (EmailException e)
		{
		  throw LOG.addSenderException(from, e);
		}
	  }

	  protected internal virtual void addCc(Email email, string cc)
	  {
		string[] ccs = splitAndTrim(cc);
		if (ccs != null)
		{
		  foreach (string c in ccs)
		  {
			try
			{
			  email.addCc(c);
			}
			catch (EmailException e)
			{
			  throw LOG.addCcException(c, e);
			}
		  }
		}
	  }

	  protected internal virtual void addBcc(Email email, string bcc)
	  {
		string[] bccs = splitAndTrim(bcc);
		if (bccs != null)
		{
		  foreach (string b in bccs)
		  {
			try
			{
			  email.addBcc(b);
			}
			catch (EmailException e)
			{
			  throw LOG.addBccException(b, e);
			}
		  }
		}
	  }

	  protected internal virtual void setSubject(Email email, string subject)
	  {
		email.Subject = !string.ReferenceEquals(subject, null) ? subject : "";
	  }

	  protected internal virtual Email MailServerProperties
	  {
		  set
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
    
			string host = processEngineConfiguration.MailServerHost;
			ensureNotNull("Could not send email: no SMTP host is configured", "host", host);
			value.HostName = host;
    
			int port = processEngineConfiguration.MailServerPort;
			value.SmtpPort = port;
    
			value.TLS = processEngineConfiguration.MailServerUseTLS;
    
			string user = processEngineConfiguration.MailServerUsername;
			string password = processEngineConfiguration.MailServerPassword;
			if (!string.ReferenceEquals(user, null) && !string.ReferenceEquals(password, null))
			{
			  value.setAuthentication(user, password);
			}
		  }
	  }

	  protected internal virtual void setCharset(Email email, string charSetStr)
	  {
		if (charset != null)
		{
		  email.Charset = charSetStr;
		}
	  }

	  protected internal virtual string[] splitAndTrim(string str)
	  {
		if (!string.ReferenceEquals(str, null))
		{
		  string[] splittedStrings = str.Split(",", true);
		  for (int i = 0; i < splittedStrings.Length; i++)
		  {
			splittedStrings[i] = splittedStrings[i].Trim();
		  }
		  return splittedStrings;
		}
		return null;
	  }

	  protected internal virtual string getStringFromField(Expression expression, DelegateExecution execution)
	  {
		if (expression != null)
		{
		  object value = expression.getValue(execution);
		  if (value != null)
		  {
			return value.ToString();
		  }
		}
		return null;
	  }

	}

}