using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{

	using DeploymentResourceNotFoundException = org.camunda.bpm.engine.exception.DeploymentResourceNotFoundException;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using FormData = org.camunda.bpm.engine.form.FormData;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public abstract class AbstractGetDeployedFormCmd : Command<Stream>
	{

	  protected internal static string EMBEDDED_KEY = "embedded:";
	  protected internal static int EMBEDDED_KEY_LENGTH = EMBEDDED_KEY.Length;

	  protected internal static string DEPLOYMENT_KEY = "deployment:";
	  protected internal static int DEPLOYMENT_KEY_LENGTH = DEPLOYMENT_KEY.Length;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Stream execute(CommandContext commandContext)
	  {
		checkAuthorization(commandContext);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.form.FormData formData = getFormData(commandContext);
		FormData formData = getFormData(commandContext);
		string formKey = formData.FormKey;

		if (string.ReferenceEquals(formKey, null))
		{
		  throw new BadUserRequestException("The form key is not set.");
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resourceName = getResourceName(formKey);
		string resourceName = getResourceName(formKey);

		try
		{
		  return commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, formData, resourceName));
		}
		catch (DeploymentResourceNotFoundException e)
		{
		  throw new NotFoundException("The form with the resource name '" + resourceName + "' cannot be found in deployment.", e);
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly AbstractGetDeployedFormCmd outerInstance;

		  private CommandContext commandContext;
		  private FormData formData;
		  private string resourceName;

		  public CallableAnonymousInnerClass(AbstractGetDeployedFormCmd outerInstance, CommandContext commandContext, FormData formData, string resourceName)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.formData = formData;
			  this.resourceName = resourceName;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.io.InputStream call() throws Exception
		  public override Stream call()
		  {
			return (new GetDeploymentResourceCmd(formData.DeploymentId, resourceName)).execute(commandContext);
		  }
	  }

	  protected internal virtual string getResourceName(string formKey)
	  {
		string resourceName = formKey;

		if (resourceName.StartsWith(EMBEDDED_KEY, StringComparison.Ordinal))
		{
		  resourceName = resourceName.Substring(EMBEDDED_KEY_LENGTH, resourceName.Length - EMBEDDED_KEY_LENGTH);
		}

		if (!resourceName.StartsWith(DEPLOYMENT_KEY, StringComparison.Ordinal))
		{
		  throw new BadUserRequestException("The form key '" + formKey + "' does not reference a deployed form.");
		}

		resourceName = resourceName.Substring(DEPLOYMENT_KEY_LENGTH, resourceName.Length - DEPLOYMENT_KEY_LENGTH);

		return resourceName;
	  }

	  protected internal abstract FormData getFormData(CommandContext commandContext);

	  protected internal abstract void checkAuthorization(CommandContext commandContext);

	}

}